using System;

namespace Down2Jam4Unity.Models
{
    public class JamsData
    {
        [Serializable]
        public class Response
        {
            public bool success;
            public JamData[] data;
        }

        [Serializable]
        public class JamData
        {
            public int id;
            public string name;
            public string slug;
            public string startTime;
            public int suggestionHours;
            public int slaughterHours;
            public int votingHours;
            public int jammingHours;
            public int ratingHours;
            public int submissionHours;
            public int postJamRefinementHours;
            public int postJamRatingHours;
            public bool isActive;
            public string createdAt;
            public string updatedAt;
            public int themePerUser;
            public int themePerRound;
            public int noOfRounds;
            public string icon;
            public string color;
            public string tenantId;
            public string sourceUrl;
            public string sourcePlatform;
        }
    }
}