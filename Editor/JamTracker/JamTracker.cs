using Cysharp.Threading.Tasks;
using Down2Jam4Unity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace Down2Jam4Unity.Editor.JamTracker
{
    public class D2JamTracker : EditorWindow
    {
        [SerializeField]
        private static Texture icon;

        private VisualElement visualTree;

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        JamsData.JamData currentJam = null;

        Label jamName;
        ProgressBar jamProgress;

        bool isOpen = false;

        List<DateTime> dates = new List<DateTime>();
        Dictionary<long, string> stringPerDate = new();


        [MenuItem("D2Jam/JamTracker")]
        public static void ShowWindow()
        {
            D2JamTracker wnd = GetWindow<D2JamTracker>();
            wnd.minSize = new Vector2(450, 120);
            wnd.maxSize = new Vector2(450.1f, 120.1f);
            wnd.titleContent = new GUIContent("D2Jam Tracker", Resources.Load("D2JamLogo") as Texture2D);
        }

        [MainToolbarElement("D2Jam/D2Jam Tracker", defaultDockPosition = MainToolbarDockPosition.Right)]
        public static MainToolbarElement OpenWindowButton()
        {
            Texture2D assetIcon = Resources.Load("D2JamLogo") as Texture2D;
            return new MainToolbarButton(new MainToolbarContent("D2Jam Tracker", assetIcon, "Opens the D2Jam progress tracker"), ShowWindow);
        }

        public void CreateGUI()
        {
            EditorApplication.quitting += Quitting;
            EditorApplication.playModeStateChanged += OnPlaymodeChange;
            EditorApplication.focusChanged += OnChangeFocus;

            VisualElement root = rootVisualElement;
            root.dataSource = this;

            visualTree = m_VisualTreeAsset.Instantiate();
            root.Add(visualTree);

            isOpen = true;

            InitUIComponents(root);

            InitUI();
        }

        void InitUIComponents(VisualElement root)
        {
            jamName = root.Q<Label>("JamName");
            jamProgress = root.Q<ProgressBar>("JamProgress");
        }

        async void InitUI()
        {
            var jams = await JamcoreAPI.GetJams();
            if (jams != null && jams.success)
            {
                var activeMostRecent = jams.data.FirstOrDefault(x => x.isActive);
                if (activeMostRecent != null)
                {
                    currentJam = activeMostRecent;
                    jamName.text = $"Current: {activeMostRecent.name}";

                    dates = new();

                    var jamStart = DateTime.Parse(currentJam.startTime); // T0
                    dates.Add(jamStart);
                    stringPerDate.Add(jamStart.Ticks, "Theme voting ends in");

                    var startThemeVoting = jamStart.AddHours(-currentJam.votingHours);
                    dates.Add(startThemeVoting);
                    stringPerDate.Add(startThemeVoting.Ticks, "Theme Elimination ends in");

                    var startThemeElimination = startThemeVoting.AddHours(-currentJam.slaughterHours);
                    dates.Add(startThemeElimination);
                    stringPerDate.Add(startThemeElimination.Ticks, "Theme Submission ends in");

                    var startThemeSuggestion = startThemeElimination.AddHours(-currentJam.suggestionHours);
                    dates.Add(startThemeSuggestion);
                    stringPerDate.Add(startThemeSuggestion.Ticks, "Theme Submission starts in");

                    var startSubmissionHour = jamStart.AddHours(currentJam.jammingHours);
                    dates.Add(startSubmissionHour);
                    stringPerDate.Add(startSubmissionHour.Ticks, "Jam ends in");

                    var startRatingPeriod = startSubmissionHour.AddHours(currentJam.submissionHours);
                    dates.Add(startRatingPeriod);
                    stringPerDate.Add(startRatingPeriod.Ticks, "Submission Time ends in");

                    var startResults = startSubmissionHour.AddHours(currentJam.ratingHours);
                    dates.Add(startResults);
                    stringPerDate.Add(startResults.Ticks, "Ratings ends in");

                    var startPostJam = startResults.AddHours(currentJam.postJamRatingHours);
                    dates.Add(startPostJam);
                    stringPerDate.Add(startPostJam.Ticks, "Post Jam rating ends in");

                    dates = dates.OrderBy(x => x.Ticks).ToList();
                }
                else
                {
                    currentJam = null;
                    jamName.text = "No active jam found D:";
                }
            }
        }



        void UpdateDate()
        {
            var currDate = DateTime.UtcNow;

            var nextStart = dates.FirstOrDefault(x => x.Ticks > currDate.Ticks);

            if (stringPerDate.TryGetValue(nextStart.Ticks, out string label))
            {
                var timeRemaining = nextStart - currDate;
                jamProgress.title = $"{label}: {timeRemaining.Days}d  {timeRemaining.Hours}h {timeRemaining.Minutes}m";
            }
        }

        private void OnDestroy()
        {
            EditorApplication.quitting -= Quitting;
            EditorApplication.playModeStateChanged -= OnPlaymodeChange;
            EditorApplication.focusChanged -= OnChangeFocus;
            isOpen = false;
        }

        private void OnPlaymodeChange(PlayModeStateChange change)
        {

        }

        void CloseWindow()
        {
            D2JamTracker wnd = GetWindow<D2JamTracker>();
            wnd.Close();
        }

        private void Quitting()
        {
            CloseWindow();
        }

        private async void OnChangeFocus(bool focus)
        {
            do
            {
                OnInspectorUpdate();
                await UniTask.Delay(100);
            }
            while (!focus && isOpen);
        }

        private void OnInspectorUpdate()
        {
            if (Application.isPlaying) return;


            UpdateDate();
        }
    }
}