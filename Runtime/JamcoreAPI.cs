using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Down2Jam4Unity.Models;
using Down2Jam4Unity.Utility;

namespace Down2Jam4Unity
{
    public static class JamcoreAPI
    {
        public const string ENDPOINT = "https://d2jam.com/api/v1";

        public static string token;

        public static async Task<LoginData.Response> Login(string username, string password)
        {
            var body = new LoginData.Request()
            {
                username = username,
                password = password
            };

            return await CRUDUtility.Post<LoginData.Response>($"{ENDPOINT}/session", JsonConvert.SerializeObject(body));
        }

        public static async Task<AchievementData.Response> UnlockAchievement(int achievementId, string token)
        {
            var headers = new List<RequestHeader>()
            {
                new(){
                    name = "Authorization", value = $"Bearer {token}"
                }
            };

            var body = new AchievementData.Request()
            {
                achievementId = achievementId
            };

            return await CRUDUtility.Post<AchievementData.Response>($"{ENDPOINT}/achievement", JsonConvert.SerializeObject(body), customHeaders: headers);
        }

        public static async Task<ImageData.Response> UploadImage(string fileUrl, string token)
        {
            var headers = new List<RequestHeader>()
            {
                new(){
                    name = "Authorization", value = $"Bearer {token}"
                }
            };

            byte[] file = File.ReadAllBytes(fileUrl);
            WWWForm form = new();

            form.AddBinaryData("upload", file);
            return await CRUDUtility.Upload<ImageData.Response>($"{ENDPOINT}/image", form, contentType: "multipart/form-data; boundary=----WebKitFormBoundaryGaZZtKJb8URr1d7P", customHeaders: headers);
        }

        public static async Task<LeaderboardData.Response> UploadScoreOnLeaderboard(int leaderboardId, int score, string imgPath, string token)
        {
            var headers = new List<RequestHeader>()
            {
                new(){
                    name = "Authorization", value = $"Bearer {token}"
                }
            };

            var body = new LeaderboardData.Request
            {
                evidence = imgPath,
                leaderboardId = leaderboardId,
                score = score,
            };

            return await CRUDUtility.Post<LeaderboardData.Response>($"{ENDPOINT}/score", JsonConvert.SerializeObject(body), customHeaders: headers);
        }
    }
}