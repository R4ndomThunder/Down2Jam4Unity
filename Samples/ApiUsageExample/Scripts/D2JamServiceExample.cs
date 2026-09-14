using Down2Jam4Unity;
using Down2Jam4Unity.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class D2JamServiceExample : MonoBehaviour
{
    public string username, password;
    public string gameSlug;
    public int testAchievementId;
    public int testLeaderboardId;
    public int testScore;
    public string testImgPath;

    LoginData.UserDataTokenResponse user;
    string token;

    public bool IsLogged => user != null;
    public LoginData.UserDataTokenResponse GetUser() => user;

    [ContextMenu("LoginWithToken")]
    public async void LoginWithToken()
    {
        var resp = await JamcoreAPI.LoginWithToken(gameSlug);

        if (resp != null && resp.success)
        {
            Application.OpenURL(resp.data.verificationUri);
            bool tokenApproved = false;

            while (!tokenApproved)
            {
                var respToken = await JamcoreAPI.CheckTokenStatus(resp.data.deviceCode);

                if (respToken != null && respToken.success)
                {
                    if (respToken.data.status == "approved")
                    {
                        token = respToken.data.token;
                        user = respToken.data.user;
                        tokenApproved = true;
                    }
                    else if (respToken.data.status == "declined")
                    {
                        tokenApproved = true;
                        user = null;
                    }
                }
                await Task.Delay(10000);
            }
        }
    }

    [ContextMenu("Login")]
    public async void Login()
    {
        var resp = await JamcoreAPI.Login(username, password);

        if (resp != null && resp.success)
        {
            token = resp.data.token;
        }

        Debug.Log($"Login: {JsonConvert.SerializeObject(resp)}");
    }

    [ContextMenu("Test Achivement")]
    public void TestAchievement()
    {
        UnlockAchievement(testAchievementId);
    }

    public async void UnlockAchievement(int achievementId)
    {
        var resp = await JamcoreAPI.UnlockAchievement(achievementId, token);

        if (resp != null)
            Debug.Log($"Achievement response: {JsonConvert.SerializeObject(resp)}");
        else
        {
            Debug.Log($"Achievement response: null");
        }
    }

    [ContextMenu("UploadScore")]
    public void UploadScore()
    {
        if (IsLogged)
            UploadScoreAsync(testLeaderboardId, testScore);
    }

    async void UploadScoreAsync(int leaderboardId, int score)
    {
        if (!IsLogged) return;

        var evidenceUrl = "https://d2jam.com/api/v1/image/d83bf32f-d406-410e-8798-b64d7212a1fe.png";

        var screenshotName = $"Leaderboard-{user.name ?? username}-{DateTime.UtcNow:dd-MM-yyyy-hh-mm-ss}";
        var screenshotPath = Application.temporaryCachePath + $"/{screenshotName}.png";
        Debug.Log($"Screenshot saved at: {screenshotPath}");
        ScreenCapture.CaptureScreenshot(screenshotPath);

        await Task.Delay(500);

        var resp = await JamcoreAPI.UploadImage(screenshotPath, token);
        if (resp != null && resp.success)
        {
            evidenceUrl = "https://d2jam.com" + resp.data;
        }

        var scoreResp = await JamcoreAPI.UploadScoreOnLeaderboard(leaderboardId, score, evidenceUrl, token);

        if (scoreResp != null)
        {
            Debug.Log($"Leaderboard: {scoreResp.message}");
        }
        else
        {
            Debug.Log($"Leaderboard error");
        }
    }

    async Task RevokeCurrentToken()
    {
        var tokensListResponse = await JamcoreAPI.GetUserTokens(token);
        if (tokensListResponse != null && tokensListResponse.success)
        {
            var currToken = tokensListResponse.data.FirstOrDefault(x => token.StartsWith(x.keyPrefix));

            var tokenRevokeResponse = await JamcoreAPI.RevokeToken(currToken.id, token);
            if (tokenRevokeResponse != null)
            {
                Debug.Log($"Token revoking {tokenRevokeResponse.success} ~ {tokenRevokeResponse.message}");
            }
        }
    }

    private async void OnApplicationQuit()
    {
        if (IsLogged)
            await RevokeCurrentToken();
    }

    public async void OnDestroy()
    {
        if (IsLogged)
            await RevokeCurrentToken();
    }
}