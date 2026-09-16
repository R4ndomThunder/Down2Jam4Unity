using System;
using UnityEngine;

namespace Down2Jam4Unity.Models
{
    public class RadioData
    {
        [Serializable]
        public class ResponseData
        {
            public bool success;
            public CurrentRadioData data;
        }

        [Serializable]
        public class CurrentRadioData
        {
            public bool enabled;
            public int listenerCount;
            public string serverTime;
            public CurrentSongData current;
            public RecentEmoteData[] recentEmotes;
            public VotingData[] voting;
        }

        [Serializable]
        public class CurrentSongData
        {
            public float durationSeconds;
            public float offsetSeconds;
            public TrackData track;
        }

        [Serializable]
        public class TrackData
        {
            public bool allowBackgroundUse;
            public bool allowDownload;
            public int id;
            public string name;
            public string slug;
            public string url;
            public ComposerData composer;
            public GamePageData gamePage;
        }

        [Serializable]
        public class ComposerData
        {
            public int id;
            public string name;
            public string slug;
        }

        [Serializable]
        public class GamePageData
        {
            public string name;
            public int gameId;
            public string soundtrackThumbnail;
            public string thumbnail;
            public GameInfo game;
        }

        [Serializable]
        public class GameInfo
        {
            public int id;
            public string slug;
        }

        [Serializable]
        public class RecentEmoteData
        {

        }

        [Serializable]
        public class VotingData { }
    }
}
