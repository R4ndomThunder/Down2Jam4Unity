using Down2Jam4Unity;
using Newtonsoft.Json;
using RTDK.Logger;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class JamcoreServiceExample : MonoBehaviour
{
    public string username, password;

    public string token;

    public string gameSlug;
    public int testAchievementId;
    public int testLeaderboardId;
    public int testScore;

    public string testImgPath;

    string deviceCode;

    [ContextMenu("LoginWithToken")]
    public async void LoginWithToke()
    {
        var resp = await JamcoreAPI.LoginWithToken(username, gameSlug);

        if (resp != null && resp.success)
        {
            Application.OpenURL(resp.data.verificationUri);
            bool tokenApproved = false;

            while (!tokenApproved)
            {
                var respToken = await JamcoreAPI.TokenPoll(resp.data.deviceCode);

                if (respToken != null && respToken.success)
                {
                    if (respToken.data.status == "approved")
                    {
                        token = respToken.data.token;
                        tokenApproved = true;
                    }
                    else if (respToken.data.status == "declined")
                    {

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

        RTDKLogger.Log($"Login: {JsonConvert.SerializeObject(resp)}");
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
            RTDKLogger.Log($"Achievement response: {JsonConvert.SerializeObject(resp)}");
        else
        {
            RTDKLogger.Log($"Achievement response: null");
        }
    }

    [ContextMenu("UploadScore")]
    public async void UploadScore()
    {
        var screenshotName = $"Leaderboard-{username}-{DateTime.UtcNow:dd-MM-yyyy-hh-mm-ss}";
        var screenshotPath = Application.temporaryCachePath + $"/{screenshotName}.png";
        Debug.Log($"Screenshot saved at: {screenshotPath}");
        ScreenCapture.CaptureScreenshot(screenshotPath);

        await Task.Delay(500);

        //IMAGE UPLOAD CURRENTLY NOT WORKING

        //var imgResp = await JamcoreAPI.UploadImage(screenshotPath, token);
        //if (imgResp != null && imgResp.success)
        //{
        var scoreResp = await JamcoreAPI.UploadScoreOnLeaderboard(testLeaderboardId, testScore, $"https://d2jam.com/api/v1/image/14c9dce4-31e9-4060-b721-ff275fbe33c8.png", token);

        if (scoreResp != null)
        {
            RTDKLogger.Log($"Leaderboard: {scoreResp.message}");
        }
        else
        {
            RTDKLogger.Log($"Leaderboard error");
        }
        //}
        //else
        //{
        //    RTDKLogger.Log($"Image upload error");
        //}
    }
}