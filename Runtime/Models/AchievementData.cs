using UnityEngine;

namespace Down2Jam4Unity.Models
{
    public class AchievementData
    {
        public class Request
        {
            public int achievementId;
        }

        public class Response
        {
            public bool success;
            public string message;
        }
    }
}
