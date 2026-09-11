namespace Down2Jam4Unity.Models
{
    public class LeaderboardData
    {
        public class Request
        {
            public string evidence;
            public int score;
            public int leaderboardId;
        }

        public class Response
        {
            public bool success;
            public string message;
        }
    }
}
