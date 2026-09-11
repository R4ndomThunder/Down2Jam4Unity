using Newtonsoft.Json;
using Down2Jam4Unity;
using RTDK.Logger;
using UnityEngine;

public class JamcoreServiceExample : MonoBehaviour
{
    public string username, password;

    public string token;

    public int testAchievementId;
    public int testLeaderboardId;
    public int testScore;

    public string testImgPath;

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
        var imgResp = await JamcoreAPI.UploadImage(testImgPath, token);
        if (imgResp != null && imgResp.success)
        {
            var scoreResp = await JamcoreAPI.UploadScoreOnLeaderboard(testLeaderboardId, testScore, imgResp.data, token);

            if (scoreResp != null)
            {
                RTDKLogger.Log($"Leaderboard: {scoreResp.message}");
            }
            else
            {
                RTDKLogger.Log($"Leaderboard error");
            }
        }
        else
        {
            RTDKLogger.Log($"Image upload error");
        }
    }
}