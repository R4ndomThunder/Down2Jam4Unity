using Cysharp.Threading.Tasks;
using Down2Jam4Unity.Models;
using Down2Jam4Unity.Utility;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace Down2Jam4Unity.Editor.D2JamRadio
{
    public class D2JamRadioPlayer : EditorWindow
    {
        private VisualElement visualTree;

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        private AudioSource m_AudioSource;

        private bool useStreamSafe = true;
        private string lastAudioClipUrl;
        private string lastThumbnailUrl;

        private string currentGameSlug;
        private Sprite lastThumbnail;
        private AudioClip lastAudioClip;

        Label titleLabel, authorLabel, gameLabel;
        ProgressBar songProgress;
        Toggle safeStreamToggle;
        Slider volumeSlider;
        Image thumbnail;

        RadioData.CurrentSongData currentSong;

        CancellationTokenSource cToken;

        bool isOpen = false;

        [MenuItem("D2Jam/Radio Player")]
        public static void ShowWindow()
        {
            D2JamRadioPlayer wnd = GetWindow<D2JamRadioPlayer>();
            wnd.minSize = new Vector2(450, 120);
            wnd.maxSize = new Vector2(450.1f, 120.1f);
            wnd.titleContent = new GUIContent("D2Jam Radio Player", Resources.Load("D2JamRadio") as Texture2D);
        }

        [MainToolbarElement("D2Jam/D2Jam Radio", defaultDockPosition = MainToolbarDockPosition.Right)]
        public static MainToolbarElement RadioButton()
        {
            Texture2D assetIcon = Resources.Load("D2JamRadio") as Texture2D;
            return new MainToolbarButton(new MainToolbarContent("D2Jam Radio", assetIcon, "Opens the D2Jam radio"), ShowWindow);
        }

        public void CreateGUI()
        {
            EditorUtility.audioMasterMute = false;
            Application.runInBackground = true;

            EditorApplication.quitting += Quitting;
            EditorApplication.playModeStateChanged += OnPlaymodeChange;
            EditorApplication.focusChanged += OnChangeFocus;

            VisualElement root = rootVisualElement;
            root.dataSource = this;

            visualTree = m_VisualTreeAsset.Instantiate();
            root.Add(visualTree);

            titleLabel = root.Q<Label>("Title");
            titleLabel.RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == 0)
                {
                    OpenSongPage();
                }
            });

            authorLabel = root.Q<Label>("Author");
            authorLabel.RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == 0)
                {
                    OpenAuthorPage();
                }
            });
            gameLabel = root.Q<Label>("GameName");
            gameLabel.RegisterCallback<ClickEvent>((evt) =>
            {
                if (evt.button == 0)
                {
                    OpenGamePage();
                }
            });

            songProgress = root.Q<ProgressBar>("TimeProgress");
            thumbnail = root.Q<Image>("AlbumImage");

            safeStreamToggle = root.Q<Toggle>("UseStreamerSafe");
            safeStreamToggle.RegisterCallback<ChangeEvent<bool>>((evt) =>
            {
                EditorPrefs.SetBool("D2JamRadio.UseSafeStation", useStreamSafe);
                useStreamSafe = evt.newValue;
                GetRadioData();
            });

            safeStreamToggle.value = EditorPrefs.GetBool("D2JamRadio.UseSafeStation");

            volumeSlider = root.Q<Slider>("VolumeSlider");
            volumeSlider.RegisterCallback<ChangeEvent<float>>((evt) =>
            {
                EditorPrefs.SetFloat("D2JamRadio.Volume", volumeSlider.value);
                if (m_AudioSource != null)
                    m_AudioSource.volume = evt.newValue;
            });

            volumeSlider.value = EditorPrefs.GetFloat("D2JamRadio.Volume");
            isOpen = true;
        }

        private void Quitting()
        {
            CloseWindow();
        }

        private void OpenGamePage()
        {
            if (currentSong != null)
                Application.OpenURL($"https://d2jam.com/g/{currentSong.track.gamePage.game.slug}");
        }

        private void OpenSongPage()
        {
            if (currentSong != null)
                Application.OpenURL($"https://d2jam.com/m/{currentSong.track.slug}");
        }

        private void OpenAuthorPage()
        {
            if (currentSong != null)
                Application.OpenURL($"https://d2jam.com/u/{currentSong.track.composer.slug}");
        }

        private void OnPlaymodeChange(PlayModeStateChange change)
        {
            if (change == PlayModeStateChange.ExitingEditMode)
            {
                CloseWindow();
            }
        }

        void CloseWindow()
        {
            D2JamRadioPlayer wnd = GetWindow<D2JamRadioPlayer>();
            wnd.Close();
        }

        private async void OnChangeFocus(bool obj)
        {
            SetDataInUI();
            UpdateUI();

            do
            {
                OnInspectorUpdate();
                await Task.Delay(100);
            }
            while (!obj && isOpen);
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying) return;

            if (m_AudioSource == null)
                CreateAudioSource();
            else
            {
                if (m_AudioSource.isPlaying && m_AudioSource.time >= m_AudioSource.clip.length - 1)
                {
                    GetRadioData();
                }

                UpdateUI();
            }
        }

        void UpdateUI()
        {
            if (m_AudioSource == null || m_AudioSource.clip == null) return;

            TimeSpan play = TimeSpan.FromSeconds(m_AudioSource.time);
            TimeSpan length = TimeSpan.FromSeconds(m_AudioSource.clip.length);

            songProgress.value = m_AudioSource.time / m_AudioSource.clip.length;
            songProgress.title = $"{play.Minutes:00}:{play.Seconds:00}/{length.Minutes:00}:{length.Seconds:00}";
        }

        private void OnDestroy()
        {
            EditorUtility.audioMasterMute = EditorPrefs.GetBool("AudioMasterMute");

            if (cToken != null)
                cToken.Cancel(false);

            DestroyAudioSource();
            EditorApplication.quitting -= Quitting;
            EditorApplication.playModeStateChanged -= OnPlaymodeChange;
            EditorApplication.focusChanged -= OnChangeFocus;
        }

        //private void OnProjectChange()
        //{
        //    DestroyAudioSource(false);
        //}

        void CreateAudioSource()
        {
            m_AudioSource = new GameObject("[D2Jam Radio]").AddComponent<AudioSource>();
            m_AudioSource.volume = volumeSlider.value;
            GetRadioData();
        }

        void DestroyAudioSource(bool closing = true)
        {
            if (closing)
                isOpen = false;

            if (m_AudioSource != null)
                DestroyImmediate(m_AudioSource.gameObject);

        }

        async void SetDataInUI()
        {
            if (currentSong != null)
            {
                titleLabel.text = currentSong.track.name;
                authorLabel.text = $"Author: <u>{currentSong.track.composer.name}</u>";
                gameLabel.text = $"Game: <u>{currentSong.track.gamePage.name}</u>";
                var thumbnailUrl = currentSong.track.gamePage.soundtrackThumbnail;
                if (string.IsNullOrEmpty(thumbnailUrl))
                    thumbnailUrl = currentSong.track.gamePage.thumbnail;

                if (lastThumbnailUrl != thumbnailUrl)
                {
                    var texture = await GetThumbnail(thumbnailUrl);
                    if (texture != null)
                    {
                        lastThumbnail = Sprite.Create(texture, new Rect(Vector2.zero, new Vector2(texture.width, texture.height)), Vector2.one / 2);
                        lastThumbnail.name = currentSong.track.name;
                    }
                }

                if (lastThumbnail != null)
                    thumbnail.sprite = lastThumbnail;
            }
        }

        bool isLoading = false;
        async void GetRadioData()
        {
            if (isLoading) return;

            currentSong = null;
            isLoading = true;
            titleLabel.text = "Loading...";
            authorLabel.text = string.Empty;
            gameLabel.text = string.Empty;

            var resp = await JamcoreAPI.GetCurrentRadio(useStreamSafe ? "safe" : "all");
            if (resp != null && resp.success)
            {
                var currTrack = resp.data.current;

                currentGameSlug = currTrack.track.gamePage.game.slug;

                var url = currTrack.track.url;

                if (url != lastAudioClipUrl)
                {
                    var audioClip = await GetAudioClip(url);

                    if (m_AudioSource == null)
                        return;

                    audioClip.name = currTrack.track.name;

                    m_AudioSource.clip = audioClip;
                    m_AudioSource.Play();

                    if (currTrack.offsetSeconds <= m_AudioSource.clip.length)
                    {
                        m_AudioSource.time = currTrack.offsetSeconds;
                    }
                    else
                    {
                        m_AudioSource.Pause();
                        titleLabel.text = $"Waiting for the next song...";
                        cToken = new();
                        await UniTask.Delay(180000 - ((int)currTrack.offsetSeconds * 1000), cancellationToken: cToken.Token);
                        m_AudioSource.time = m_AudioSource.clip.length - 2;
                    }

                    lastAudioClip = audioClip;
                }
                else
                {
                    m_AudioSource.clip = lastAudioClip;
                    m_AudioSource.Play();

                    if (currTrack.offsetSeconds <= m_AudioSource.clip.length)
                    {
                        m_AudioSource.time = currTrack.offsetSeconds;
                    }
                    else
                    {
                        m_AudioSource.Pause();
                        titleLabel.text = $"Waiting for the next song...";
                        await UniTask.Delay(180000 - ((int)currTrack.offsetSeconds * 1000), cancellationToken: cToken.Token);
                        m_AudioSource.time = m_AudioSource.clip.length - 2;
                    }
                }

                currentSong = currTrack;
                SetDataInUI();
            }
            isLoading = false;
        }

        async UniTask<Texture2D> GetThumbnail(string url)
        {
            return await CRUDUtility.GetTexture2D($"https://d2jam.com/{url}") as Texture2D;
        }

        async UniTask<AudioClip> GetAudioClip(string url)
        {
            AudioType ext = AudioType.UNKNOWN;
            if (url.EndsWith("mp3"))
                ext = AudioType.MPEG;
            else if (url.EndsWith("wav"))
                ext = AudioType.WAV;
            else if (url.EndsWith("ogg"))
                ext = AudioType.OGGVORBIS;

            return await CRUDUtility.GetAudioClipStreaming($"https://d2jam.com/{url}", ext);
        }
    }
}