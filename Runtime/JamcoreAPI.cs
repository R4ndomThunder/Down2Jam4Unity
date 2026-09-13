using Down2Jam4Unity.Models;
using Down2Jam4Unity.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Down2Jam4Unity
{
    public static class JamcoreAPI
    {
        public const string ENDPOINT = "https://d2jam.com/api/v1";

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

        public static async Task<ImageData.Response> UploadImage(string filePath, string token)
        {
            var headers = new List<RequestHeader>()
            {
                new(){
                    name = "Authorization", value = $"Bearer {token}"
                }
            };

            byte[] byteData = File.ReadAllBytes(filePath);
            var fileName = Path.GetFileName(filePath);

            List<IMultipartFormSection> form = new();
            var aaa = new MultipartFormFileSection("upload", byteData, fileName, "image/png");
            form.Add(aaa);

            byte[] boundary = System.Text.Encoding.UTF8.GetBytes("----WebKitFormBoundarycB6W4LTiLz6oXMuB");

            return await CRUDUtility.Upload<ImageData.Response>($"{ENDPOINT}/image", form, boundary, contentType: "application/json; charset=utf-8", customHeaders: headers);
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

        public static async Task<LoginData.TokenResponse> LoginWithToken(string userName, string gameSlug)
        {
            var req = new LoginData.TokenRequest
            {
                clientName = userName,
                gameSlug = gameSlug
            };

            return await CRUDUtility.Post<LoginData.TokenResponse>($"{ENDPOINT}/device/code", JsonConvert.SerializeObject(req));
        }


        public static async Task<LoginData.TokenPollResponse> TokenPoll(string deviceCode)
        {
            var req = new LoginData.TokenPollRequest
            {
                deviceCode = deviceCode
            };

            return await CRUDUtility.Post<LoginData.TokenPollResponse>($"{ENDPOINT}/device/token", JsonConvert.SerializeObject(req));
        }
    }
}