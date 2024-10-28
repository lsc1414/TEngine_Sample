#if UNITY_EDITOR||UNITY_STANDALONE
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MTApplicationStandaloneImp : MTApplication
{
    protected override void Awake()
    {
        base.Awake();

#if UNITY_EDITOR && DBT
        isInitFinish = false;
       DBTSDK.DBTSDKManager.I.InitSDK(DBTSDK.ELangType.ZH_CN, "mtgame");
       DBTSDK.DBTSDKManager.I.Sdk.U3DGameStart();
       isInitFinish = true;
#endif
    }

    #region Banner

    public override void ShowBanner(bool isTop, string tag)
    {
        if (!AdsDisable)
            Debug.Log("ShowBanner");
    }

    public override void ShowBanner(bool isTop)
    {
        ShowBanner(isTop, "banner");
    }

    public override void ShowBannerNoRemote(bool isTop)
    {
        ShowBanner(isTop, "banner");
    }

    public override void HideBanner()
    {
        Debug.Log("HideBanner");
    }

    #endregion

    #region Interstitial

    public override void ShowInterstitialAd(System.Action completed)
    {
        if (completed != null)
            completed();
        Debug.Log("ShowInterstitialAd");
    }

    public override void ShowInterstitialAd()
    {
        if (IsInterstitialAdAvailable())
            ShowInterstitialAd(null);
    }

    public override void ShowInterstitialAdForTag(string tag, System.Action completed)
    {
        if (completed != null)
            completed();
        Debug.Log("ShowInterstitialAd:" + tag);
    }

    public override void ShowInterstitialAdForTag(string tag)
    {
        if (IsInterstitialAdAvailableForTag(tag))
            ShowInterstitialAdForTag(tag, null);
    }

    public override bool IsInterstitialAdAvailable()
    {
        return !AdsDisable && Random.Range(0, 2) == 0;
    }

    public override bool IsInterstitialAdAvailableForTag(string tag)
    {
        return !AdsDisable && Random.Range(0, 2) == 0;
    }

    #endregion

    #region StaticInterstitial

    public override void ShowStaticInterstitialAd(System.Action completed)
    {
        if (completed != null)
            completed();
        Debug.Log("ShowStaticInterstitialAd");
    }

    public override void ShowStaticInterstitialAd()
    {
        if (IsStaticInterstitialAdAvailable())
            ShowStaticInterstitialAd(null);
    }

    public override void ShowStaticInterstitialAdForTag(string tag, System.Action completed)
    {
        if (completed != null)
            completed();
        Debug.Log("ShowStaticInterstitialAd:" + tag);
    }

    public override void ShowStaticInterstitialAdForTag(string tag)
    {
        if (IsStaticInterstitialAdAvailableForTag(tag))
            ShowStaticInterstitialAdForTag(tag, null);
    }

    public override bool IsStaticInterstitialAdAvailable()
    {
        return !AdsDisable && Random.Range(0, 2) == 0;
    }

    public override bool IsStaticInterstitialAdAvailableForTag(string tag)
    {
        return !AdsDisable && Random.Range(0, 2) == 0;
    }

    #endregion

    #region Reward Video

    public override bool IsIncentivizedAdAvailable()
    {
        return IsRewardedVideoAvailable;
    }

    public override bool IsIncentivizedAdAvailableForTag(string tag)
    {
        return IsRewardedVideoAvailable;
    }

    public override void ShowIncentivizedAd(System.Action<bool> completed, float maxWaitingtimeForRewards)
    {
        if (completed != null)
            completed(true);
        Debug.Log("ShowIncentivizedAd");
    }

    public override void ShowIncentivizedAd(System.Action<bool> completed)
    {
        if (completed != null)
            completed(true);
        Debug.Log("ShowIncentivizedAd");
    }

    public override void ShowIncentivizedAdForTag(string tag, System.Action<bool> completed,
        float maxWaitingtimeForRewards)
    {
        if (completed != null)
            completed(true);
        Debug.Log("ShowIncentivizedAd");
    }

    public override void ShowIncentivizedAdForTag(string tag, System.Action<bool> completed)
    {
        if (completed != null)
            completed(true);
        Debug.Log("ShowIncentivizedAd");
    }

    #endregion

    #region 原生广告

    public override bool IsNativeAdAvailable()
    {
        return false;
    }

    public override string LoadNativeAdInfo()
    {
        return
            "https://static.global.durianclicks.com/res/ad/api/20200226/02261655342372214610.jpg#^#030a7a7f-8665-446d-94b1-967d24f5b32a";
    }

    public override void ClickNativeAd(string adId)
    {
        Debug.Log("ClickNativeAd:" + adId);
    }

    #endregion

    /// <summary>
    /// 退出应用
    /// 一般在Android平台 ，游戏首页时点击返回按钮时候发送的通知.
    /// </summary>
    public override void QuitApp()
    {
        Debug.Log("QuitApp");
    }

    public override void RateByUrl()
    {
    }

    public override void RateInGame()
    {
    }

    public override bool CanRateInGame()
    {
        return false;
    }

    public override void TapticImpact(MTApplication.TapticImpactType tapticImpactType)
    {
        //Debug.Log("vibire:" + tapticImpactType);
    }

    public override void Share(string text, string imagePath)
    {
        Debug.Log(string.Format("{0} -  {1}", text, imagePath));
    }

    public override void FBShareImage(string hashTag, string imagePath, System.Action<string> shareCallback = null)
    {
        Debug.Log(string.Format("FBShareImage: {0} -  {1}", hashTag, imagePath));
        shareCallback?.Invoke("1");
    }

    public override void FBShareText(string text, string hashTag, System.Action<string> shareCallback = null)
    {
        Debug.Log(string.Format("FBShareText: {0} -  {1}", hashTag, text));
        shareCallback?.Invoke("1");
    }

    public override void IapBuy(string productIdentifier,
        System.Action<bool /*result*/, string /*productIdentifier*/, string /*receipt*/> callback)
    {
        if (callback != null)
        {
            PlayerPrefs.SetInt(productIdentifier, 1);
            PlayerPrefs.Save();
            callback(true, productIdentifier, "");
            if (productIdentifier.Contains("subs"))
            {
                // 订阅,5分钟
                System.DateTime expireDate2 = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                System.DateTime now = System.DateTime.Now;
                now = now.AddMinutes(5);
                var time = now - expireDate2;
                var seconds = (long)time.TotalSeconds;
                callback_subscription(seconds.ToString());
            }
        }
    }

    public override void IapRestore(System.Action<System.Collections.Generic.List<string>> callback)
    {
        if (callback != null)
            callback(null);
    }

    public override bool IapCanBuy()
    {
#if CanBuyTest_CanBuy
        return true;
#else
        return false;
#endif
    }

    public override void SyncNetIapProducts()
    {
    }

    public override void CheckSubscriptionStatus()
    {
    }

    public override void AnalyticsCustom(string eventName,
        System.Collections.Generic.Dictionary<string, object> parameters)
    {
        StringBuilder sb = new StringBuilder();
        if (parameters != null)
        {
            foreach (var kv in parameters)
            {
                sb.AppendFormat("{0}:{1} ", kv.Key, kv.Value);
            }
        }

        string str = sb.ToString();
        Debug.Log("通用统计:" + eventName + " : " + str);
        AddLog(eventName + " : " + str);
    }

    public override void AnalyticsCustom(string eventName, string key, object value)
    {
        Debug.Log("通用统计:" + eventName + " key:" + key + " value:" + value);
        AddLog(eventName + " key:" + key + " value:" + value);
    }

    public override void AnalyticsCustom(string eventName)
    {
        Debug.Log("通用统计:" + eventName);
        AddLog(eventName);
    }

    public override void AnalyticsCustomSingleParam(string eventName, object parameter)
    {
        Debug.Log("通用统计:" + eventName);
        AddLog(eventName);
    }

    public override void RegisterLocalNotification()
    {
        Debug.Log("RegisterLocalNotification");
    }

    public override void AddLocalNotification(string notifiText, float waitSecond)
    {
        //Debug.Log(string.Format("AddLocalNotification:{0}  wait:{1}h", notifiText, waitSecond / 3600));
    }

    public override void ClearAllLocalNotifications()
    {
        Debug.Log("ClearAllLocalNotifications");
    }

    public override void SendEmail(string subject, string recipients, string emailBody)
    {
        Debug.Log(string.Format("SendEmail: subject:{0}  recipients:{1} emailBody:{2}", subject, recipients,
            emailBody));
    }


    public override MTApplication.Channel channel
    {
        get { return Channel.android_googleplay; }
    }

    public override string udid
    {
        get { return "udid"; }
    }

    float time = 0;

    protected override void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F5))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene()
                .name);
        }

        time += Time.fixedDeltaTime;
        if (time >= 1)
        {
            IsRewardedVideoAvailable = !IsRewardedVideoAvailable;
            time = 0;
        }
#else
    IsRewardedVideoAvailable = true;
#endif
    }

    void AddLog(string content)
    {
        var folder = Application.persistentDataPath + "\\log\\";
        var filePath = folder + System.DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        if (!System.IO.Directory.Exists(folder))
        {
            System.IO.Directory.CreateDirectory(folder);
        }

        if (!System.IO.File.Exists(filePath))
        {
            System.IO.File.Create(filePath).Dispose();
        }

        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(filePath, true))
        {
            sw.Write(content);
            sw.Write(System.Environment.NewLine);
            sw.Flush();
            sw.Close();
        }
    }

    #region 排行榜

#if STEAMWORKS_NET
    public override bool EnableLeaderBoard()
    {
        return true;
    }

    public override void ShowLeaderBoard(string leaderboardName)
    {
        SteamLeaderboardManager.GetScores();
    }

    public override void ShowMyScore(string leaderboardName)
    {
        SteamLeaderboardManager.GetMyScore();
    }

    public override void SubmitScore(string leaderboardName, int score)
    {
        SteamLeaderboardManager.UpdateScore(score);
    }
#endif

    #endregion
}
#endif