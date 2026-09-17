using Cysharp.Threading.Tasks;
using Down2Jam4Unity.Models;
using Down2Jam4Unity.Utility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Down2Jam4Unity
{
    public static class JamcoreAPI
    {
        public const string ENDPOINT = "https://d2jam.com/api/v1";

        /// <summary>
        /// Login using Username and Password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static async UniTask<LoginData.Response> Login(string username, string password)
        {
            var body = new LoginData.Request()
            {
                username = username,
                password = password
            };

            return await CRUDUtility.Post<LoginData.Response>($"{ENDPOINT}/session", JsonConvert.SerializeObject(body));
        }

        /// <summary>
        /// Unlocks an achievement
        /// </summary>
        /// <param name="achievementId">Numeric ID of the Achievement</param>
        /// <param name="token">Token returned by login</param>
        /// <returns></returns>
        public static async UniTask<AchievementData.Response> UnlockAchievement(int achievementId, string token)
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

        /// <summary>
        /// Upload an image from local memory
        /// </summary>
        /// <param name="filePath">Path to image</param>
        /// <param name="token">Token returned by login</param>
        /// <returns></returns>
        public static async UniTask<ImageData.Response> UploadImage(string filePath, string token)
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
            var fileData = new MultipartFormFileSection("upload", byteData, fileName, "image/png");
            form.Add(fileData);

            byte[] boundary = System.Text.Encoding.UTF8.GetBytes(GenerateWebKitBoundary());

            return await CRUDUtility.Upload<ImageData.Response>($"{ENDPOINT}/image", form, boundary, contentType: "application/json; charset=utf-8", customHeaders: headers);
        }

        /// <summary>
        /// Upload an image from local memory
        /// </summary>
        /// <param name="filePath">Path to image</param>
        /// <param name="token">Token returned by login</param>
        /// <returns></returns>
        public static async UniTask<ImageData.Response> UploadImage(byte[] byteData, string filename, string token)
        {
            var headers = new List<RequestHeader>()
            {
                new(){
                    name = "Authorization", value = $"Bearer {token}"
                }
            };

            List<IMultipartFormSection> form = new();
            var fileData = new MultipartFormFileSection("upload", byteData, filename, "image/png");
            form.Add(fileData);

            byte[] boundary = System.Text.Encoding.UTF8.GetBytes(GenerateWebKitBoundary());

            return await CRUDUtility.Upload<ImageData.Response>($"{ENDPOINT}/image", form, boundary, contentType: "application/json; charset=utf-8", customHeaders: headers);
        }

        /// <summary>
        /// Upload a new score to a leaderboard
        /// </summary>
        /// <param name="leaderboardId">Numeric ID of the leaderboard</param>
        /// <param name="score">Score you want to submit</param>
        /// <param name="imgUrl">Url of an image to use as evidence (Use <b>UploadImage</b> method to upload one)</param>
        /// <param name="token">Token returned by login</param>
        /// <returns></returns>
        public static async UniTask<LeaderboardData.Response> UploadScoreOnLeaderboard(int leaderboardId, int score, string imgUrl, string token)
        {
            var headers = new List<RequestHeader>()
            {
                new(){
                    name = "Authorization", value = $"Bearer {token}"
                }
            };

            var body = new LeaderboardData.Request
            {
                evidence = imgUrl,
                leaderboardId = leaderboardId,
                score = score,
            };

            return await CRUDUtility.Post<LeaderboardData.Response>($"{ENDPOINT}/score", JsonConvert.SerializeObject(body), customHeaders: headers);
        }

        /// <summary>
        /// Create a new token request for device
        /// </summary>
        /// <param name="gameSlug">Your game slug</param>
        /// <returns></returns>
        public static async UniTask<LoginData.TokenResponse> LoginWithToken(string gameSlug)
        {
            var req = new LoginData.TokenRequest
            {
                clientName = $"{Application.productName}-{Application.platform}",
                gameSlug = gameSlug
            };

            return await CRUDUtility.Post<LoginData.TokenResponse>($"{ENDPOINT}/device/code", JsonConvert.SerializeObject(req));
        }

        /// <summary>
        /// Check if the token is approved or not by user (Check this every ~10 seconds)
        /// </summary>
        /// <param name="deviceCode">Code returned by <b>LoginWithToken</b> method</param>
        /// <returns></returns>
        public static async UniTask<LoginData.TokenPollResponse> CheckTokenStatus(string deviceCode)
        {
            var req = new LoginData.TokenPollRequest
            {
                deviceCode = deviceCode
            };

            return await CRUDUtility.Post<LoginData.TokenPollResponse>($"{ENDPOINT}/device/token", JsonConvert.SerializeObject(req));
        }

        /// <summary>
        /// Get available user's tokens. Used to get a token id for revoking.
        /// </summary>
        /// <param name="token">Token returned by login</param>
        /// <returns></returns>
        public static async UniTask<LoginData.TokenListResponse> GetUserTokens(string token)
        {
            var headers = new List<RequestHeader>()
            {
                new(){
                    name = "Authorization", value = $"Bearer {token}"
                }
            };
            return await CRUDUtility.Get<LoginData.TokenListResponse>($"{ENDPOINT}/self/game-tokens", customHeaders: headers);
        }

        /// <summary>
        /// Revoke an approved token for user.
        /// </summary>
        /// <param name="tokenId">Token ID taken from <b>GetUserToken</b> method</param>
        /// <param name="token">Token returned by login</param>
        /// <returns></returns>
        public static async UniTask<LoginData.TokenRevokeResponse> RevokeToken(string tokenId, string token)
        {
            var headers = new List<RequestHeader>()
            {
                new(){
                    name = "Authorization", value = $"Bearer {token}"
                }
            };

            var req = new LoginData.TokenRevokeRequest
            {
                id = tokenId
            };

            return await CRUDUtility.Delete<LoginData.TokenRevokeResponse>($"{ENDPOINT}/self/game-tokens", JsonConvert.SerializeObject(req), customHeaders: headers);
        }

        /// <summary>
        /// Create a WebkitBoundary for image uploading
        /// </summary>
        /// <returns></returns>
        private static string GenerateWebKitBoundary()
        {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] randomChars = new char[16];
            System.Random random = new System.Random();

            for (int i = 0; i < randomChars.Length; i++)
            {
                randomChars[i] = chars[random.Next(chars.Length)];
            }

            return "----WebKitFormBoundary" + new string(randomChars);
        }

        /// <summary>
        /// Get the current song that D2Jam radio is playing
        /// </summary>
        /// <param name="station"></param>
        /// <returns></returns>
        public static async UniTask<RadioData.ResponseData> GetCurrentRadio(string station)
        {
            return await CRUDUtility.Get<RadioData.ResponseData>($"{ENDPOINT}/radio?station={station}");
        }
    }
}