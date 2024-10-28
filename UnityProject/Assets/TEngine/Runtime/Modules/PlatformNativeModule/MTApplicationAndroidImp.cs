#if UNITY_ANDROID
using System;
using System.Collections.Generic;
using UnityEngine;

public class MTApplicationAndroidImp : MTApplication
{
    AndroidJavaClass mJavaClassUBridge = null;
    AndroidJavaClass mJavaClassAppActivity = null;

    protected override void Awake()
    {
        mJavaClassUBridge = new AndroidJavaClass("com.mtgame.UBridge");
        mJavaClassAppActivity = new AndroidJavaClass("com.mtgame.AppActivity");
        base.Awake();
    }

    #region 广告

    #region Banner

    public override void ShowBanner(bool isTop, string tag)
    {
        if (!AdsDisable && CanAdsByRemote && CanShowBannerRemote)
            mJavaClassUBridge.CallStatic("ShowBanner", isTop, tag);
    }

    public override void ShowBannerNoRemote(bool isTop)
    {
        if (!AdsDisable && CanAdsByRemote) mJavaClassUBridge.CallStatic("ShowBanner", isTop, "Banner");
    }

    public override void ShowBanner(bool isTop)
    {
        ShowBanner(isTop, "Banner");
    }

    public override void HideBanner()
    {
        mJavaClassUBridge.CallStatic("HideBanner");
    }

    #endregion

    #region 插屏

    public override void ShowInterstitialAd(Action completed)
    {
        ShowInterstitialAdForTag("Interstitial", completed);
    }

    public override void ShowInterstitialAd()
    {
        ShowInterstitialAdForTag("Interstitial");
    }

    public override bool IsInterstitialAdAvailable()
    {
        return IsInterstitialAdAvailableForTag("Interstitial");
    }

    System.Action _lastCompleteInterstitialAdCallback = null;

    void onInterstitialAdDidHideAd()
    {
        onDidHideAd -= onInterstitialAdDidHideAd;
        if (_lastCompleteInterstitialAdCallback != null)
            _lastCompleteInterstitialAdCallback();
    }

    public override void ShowInterstitialAdForTag(string tag, Action completed)
    {
        if (IsInterstitialAdAvailableForTag(tag))
        {
            onDidHideAd -= onInterstitialAdDidHideAd;
            onDidHideAd += onInterstitialAdDidHideAd;
            _lastCompleteInterstitialAdCallback = completed;
            mJavaClassUBridge.CallStatic("ShowInterstitialAdForTag", tag);
        }
        else
        {
            if (completed != null)
                completed();
        }
    }

    public override void ShowInterstitialAdForTag(string tag)
    {
        ShowInterstitialAdForTag(tag, null);
    }

    public override bool IsInterstitialAdAvailableForTag(string tag)
    {
        return !AdsDisable && CanAdsByRemote &&
               mJavaClassUBridge.CallStatic<bool>("IsInterstitialAdAvailableForTag", tag);
    }

    #endregion

    #region 静态插屏

    public override void ShowStaticInterstitialAd(Action completed)
    {
        ShowStaticInterstitialAdForTag("StaticInterstitial", completed);
    }

    public override void ShowStaticInterstitialAd()
    {
        ShowStaticInterstitialAdForTag("StaticInterstitial");
    }

    public override bool IsStaticInterstitialAdAvailable()
    {
        return IsStaticInterstitialAdAvailableForTag("StaticInterstitial");
    }

    System.Action _lastCompleteStaticInterstitialAdCallback = null;

    void onStaticInterstitialAdDidHideAd()
    {
        onDidHideAd -= onStaticInterstitialAdDidHideAd;
        if (_lastCompleteStaticInterstitialAdCallback != null)
            _lastCompleteStaticInterstitialAdCallback();
    }

    public override void ShowStaticInterstitialAdForTag(string tag, Action completed)
    {
        if (IsStaticInterstitialAdAvailableForTag(tag))
        {
            onDidHideAd -= onStaticInterstitialAdDidHideAd;
            onDidHideAd += onStaticInterstitialAdDidHideAd;
            _lastCompleteStaticInterstitialAdCallback = completed;
            mJavaClassUBridge.CallStatic("ShowStaticInterstitialAdForTag", tag);
        }
        else
        {
            if (completed != null)
                completed();
        }
    }

    public override void ShowStaticInterstitialAdForTag(string tag)
    {
        ShowStaticInterstitialAdForTag(tag, null);
    }

    public override bool IsStaticInterstitialAdAvailableForTag(string tag)
    {
        return !AdsDisable && CanAdsByRemote &&
               mJavaClassUBridge.CallStatic<bool>("IsStaticInterstitialAdAvailableForTag", tag);
    }

    #endregion

    #region 激励

    public override bool IsIncentivizedAdAvailable()
    {
        return IsIncentivizedAdAvailableForTag("");
    }

    public override bool IsIncentivizedAdAvailableForTag(string tag)
    {
        return CanAdsByRemote && mJavaClassUBridge.CallStatic<bool>("IsIncentivizedAdAvailableForTag", tag);
    }


    public override void ShowIncentivizedAd(System.Action<bool> completed, float maxWaitingtimeForRewards)
    {
        _DelayCompleted = maxWaitingtimeForRewards;
        _lastCompleteIncentivizedADCallback = completed;
        onDidHideAd -= onIncentivizedAdDidHideAd;
        onDidCompleteAdRewards -= onIncentivizedAdDidCompleted;
        onDidCompleteAdRewardsFailed -= onIncentivizedAdDidCompleted;
        onDidHideAd += onIncentivizedAdDidHideAd;
        onDidCompleteAdRewards += onIncentivizedAdDidCompleted;
        onDidCompleteAdRewardsFailed += onIncentivizedAdDidCompleted;
        CancelInvoke("onShowIncentivizedAdCompleted");
        mJavaClassUBridge.CallStatic("ShowIncentivizedAdForTag", "");
    }

    public override void ShowIncentivizedAd(System.Action<bool> completed)
    {
        ShowIncentivizedAd(completed, 2);
        //ShowIncentivizedAdForTag("Incentivized", completed);
    }

    float _DelayCompleted = 0;
    System.Action<bool> _lastCompleteIncentivizedADCallback = null;

    void onIncentivizedAdDidCompleted()
    {
        if (IsInvoking("onShowIncentivizedAdCompleted"))
        {
            CancelInvoke("onShowIncentivizedAdCompleted");
            onShowIncentivizedAdCompleted();
        }
        else
        {
            _DelayCompleted = 0;
        }
    }

    void onIncentivizedAdDidHideAd()
    {
        if (_DelayCompleted > 0)
        {
            Invoke("onShowIncentivizedAdCompleted", _DelayCompleted);
        }
        else
        {
            onShowIncentivizedAdCompleted();
        }
    }

    void onShowIncentivizedAdCompleted()
    {
        onDidHideAd -= onIncentivizedAdDidHideAd;
        onDidCompleteAdRewards -= onIncentivizedAdDidCompleted;
        onDidCompleteAdRewardsFailed -= onIncentivizedAdDidCompleted;
        if (_lastCompleteIncentivizedADCallback != null)
            _lastCompleteIncentivizedADCallback(HasGettedADRewards);
    }

    public override void ShowIncentivizedAdForTag(string tag, Action<bool> completed, float maxWaitingtimeForRewards)
    {
        _DelayCompleted = maxWaitingtimeForRewards;
        _lastCompleteIncentivizedADCallback = completed;
        onDidHideAd -= onIncentivizedAdDidHideAd;
        onDidCompleteAdRewards -= onIncentivizedAdDidCompleted;
        onDidCompleteAdRewardsFailed -= onIncentivizedAdDidCompleted;
        onDidHideAd += onIncentivizedAdDidHideAd;
        onDidCompleteAdRewards += onIncentivizedAdDidCompleted;
        onDidCompleteAdRewardsFailed += onIncentivizedAdDidCompleted;
        CancelInvoke("onShowIncentivizedAdCompleted");
        mJavaClassUBridge.CallStatic("ShowIncentivizedAdForTag", tag);
    }

    public override void ShowIncentivizedAdForTag(string tag, Action<bool> completed)
    {
        ShowIncentivizedAdForTag(tag, completed, 2);
    }

    #endregion

    #region GIF Ad

    // 默认位置在720/1280分辨率中,靠左,中间偏上
    public override void ShowGifAds(int x = 0, int y = 420, int rotate = 0, float scale = 0.5f)
    {
        mJavaClassUBridge.CallStatic("ShowGifAds", x, y, rotate, scale);
    }

    public override void HideGifAds()
    {
        mJavaClassUBridge.CallStatic("HideGifAds");
    }

    #endregion

    #endregion

    /// <summary>
    /// 退出应用
    /// 一般在Android平台 ，游戏首页时点击返回按钮时候发送的通知.
    /// </summary>
    public override void QuitApp()
    {
        mJavaClassUBridge.CallStatic("QuitApp");
    }

    #region 评价

    public override void RateByUrl()
    {
        mJavaClassUBridge.CallStatic("RateByUrl");
    }

    public override void RateInGame()
    {
        Debug.Log("未提供内评价");
    }

    public override bool CanRateInGame()
    {
        return false;
    }

    #endregion

    public override void TapticImpact(MTApplication.TapticImpactType tapticImpactType)
    {
        if (CanTapticImpact())
        {
            mJavaClassUBridge.CallStatic("TapticImpact", (int)tapticImpactType);
        }
    }

    #region 分享

    public override void Share(string text, string imagePath)
    {
        mJavaClassUBridge.CallStatic("Share", imagePath);
    }

    public override void FBShareImage(string hashTag, string imagePath, Action<string> shareCallback = null)
    {
        mShareCallback = shareCallback;
        mJavaClassUBridge.CallStatic("FBShareImage", hashTag, imagePath);
    }

    public override void FBShareText(string text, string hashTag, Action<string> shareCallback = null)
    {
        mShareCallback = shareCallback;
        mJavaClassUBridge.CallStatic("FBShareText", text, hashTag);
    }

    #endregion

    #region 内购

    Action<bool /*result*/, string /*productIdentifier*/, string /*receipt*/> _purchasedCallback = null;

    public override void IapBuy(string productIdentifier,
        Action<bool /*result*/, string /*productIdentifier*/, string /*receipt*/> callback)
    {
        if (CanIapByRemote)
        {
            LastIapProductIdentifier = productIdentifier;
            _purchasedCallback = callback;
            onIapPurchased -= onIapPurchasedCallback;
            onIapPurchased += onIapPurchasedCallback;
            mJavaClassUBridge.CallStatic("RequestBuy", productIdentifier);
        }
    }

    void onIapPurchasedCallback(bool b, string productIdentifier, string receipt)
    {
        onIapPurchased -= onIapPurchasedCallback;
        if (_purchasedCallback != null)
            _purchasedCallback(b, productIdentifier, receipt);
        _purchasedCallback = null;
    }

    public override void IapRestore(Action<System.Collections.Generic.List<string>> callback)
    {
        if (CanIapByRemote)
        {
            if (callback != null)
                callback(null);
        }
    }

    public override void SyncNetIapProducts()
    {
        if (!IapCanBuy())
        {
            return;
        }

        string ss = "";
        foreach (KeyValuePair<string, IapProduct> pair in IapProdcutsMap)
            ss += pair.Key + "#%#";
        if (ss != "") mJavaClassUBridge.CallStatic("SyncNetIapProducts", ss);
    }

    public override bool IapCanBuy()
    {
        return CanIapByRemote && mJavaClassUBridge.CallStatic<bool>("IsIapSupported");
    }


    public override void CheckSubscriptionStatus()
    {
        if (IapCanBuy())
        {
            mJavaClassUBridge.CallStatic("CheckSubscriptionStatus");
        }
    }

    #endregion

    #region 通用统计

    public override void AnalyticsCustom(string eventName,
        System.Collections.Generic.Dictionary<string, object> parameters)
    {
        string ss = null;
        if (parameters != null)
        {
            ss = "";
            foreach (KeyValuePair<string, object> pair in parameters)
                ss += string.Format("{0},{1},", pair.Key, pair.Value);
            if (ss != "")
                ss = ss.Remove(ss.Length - 1);
        }

        mJavaClassUBridge.CallStatic("AnalyticsCustom", eventName, ss);
    }

    public override void AnalyticsCustom(string eventName, string key, object value)
    {
        Dictionary<string, object> dic = new Dictionary<string, object>(1);
        dic[key] = value;
        AnalyticsCustom(eventName, dic);
    }

    public override void AnalyticsCustom(string eventName)
    {
        AnalyticsCustom(eventName, null);
    }

    public override void AnalyticsCustomSingleParam(string eventName, object parameter)
    {
        mJavaClassUBridge.CallStatic("AnalyticsCustomSingleParam", eventName, parameter.ToString());
    }

    public override void UmengLevelStart(string level)
    {
        mJavaClassUBridge.CallStatic("UmengLevelStart", level);
    }

    public override void UmengLevelFail(string level)
    {
        mJavaClassUBridge.CallStatic("UmengLevelFail", level);
    }

    public override void UmengLevelFail(string level, int score)
    {
        mJavaClassUBridge.CallStatic("UmengLevelFail", level, score);
    }

    public override void UmengLevelFinish(string level)
    {
        mJavaClassUBridge.CallStatic("UmengLevelFinish", level);
    }

    public override void UmengLevelFinish(string level, int score)
    {
        mJavaClassUBridge.CallStatic("UmengLevelFinish", level, score);
    }

    public override void UmengSetUserLevel(int levelNo)
    {
        mJavaClassUBridge.CallStatic("UmengSetUserLevel", levelNo);
    }

    /// <summary>
    /// 热云自定义事件
    /// </summary>
    /// <param name="eventName">只能是event_1 ~ event12</param>
    public override void ReYunTrackEvent(string eventName)
    {
        mJavaClassUBridge.CallStatic("ReYunTrackEvent", eventName);
    }

    #endregion

    #region 通知

    public override void RegisterLocalNotification()
    {
        //Debug.Log("RegisterLocalNotification");
        mJavaClassUBridge.CallStatic("RegisterLocalNotification");
    }

    public override void AddLocalNotification(string notifiText, float waitSecond)
    {
        //Debug.Log(string.Format("AddLocalNotification:{0}  wait:{1}s", notifiText, waitSecond));
        mJavaClassUBridge.CallStatic("AddLocalNotification", notifiText, waitSecond);
    }

    public override void ClearAllLocalNotifications()
    {
        //Debug.Log("ClearAllLocalNotifications");
        mJavaClassUBridge.CallStatic("ClearAllLocalNotifications");
    }

    #endregion

    public override void SendEmail(string subject, string recipients, string emailBody)
    {
        mJavaClassUBridge.CallStatic("SendEmail", subject, recipients, emailBody);
    }

    public override MTApplication.Channel channel
    {
        get
        {
            Channel tempChannel;
            if (Enum.TryParse(mJavaClassUBridge.CallStatic<string>("GetChannel"), true, out tempChannel))
            {
                return tempChannel;
            }
            else
            {
                return Channel.unknown;
            }
            //return (MTApplication.Channel)Enum.Parse(typeof(MTApplication.Channel), mJavaClassUBridge.CallStatic<string>("GetChannel"), true);
        }
    }

    public override string udid
    {
        get { return mJavaClassUBridge.CallStatic<string>("GetUDID"); }
    }

    #region 登录

    #region APP发布平台登录(例如:  appStore / googlePlay 等等)

    public override void Login()
    {
        mJavaClassUBridge.CallStatic("Login");
    }

    #endregion

    #endregion

    #region 排行榜

    /// <summary>
    /// 是否支持排行榜
    /// </summary>
    public override bool leaderboardSupported
    {
        get { return mJavaClassUBridge.CallStatic<bool>("IsLeaderboardSupported"); }
    }

    /// <summary>
    /// 向排行榜提交排行数据
    /// </summary>
    public override void ReportScoreToLeaderboard(long score, string boardId)
    {
        mJavaClassUBridge.CallStatic("ReportScoreToLeaderboard", score, boardId);
    }

    /// <summary>
    /// 弹出排行榜界面
    /// </summary>
    public override bool ShowLeaderboard()
    {
        mJavaClassUBridge.CallStatic("ShowLeaderboard");
        return leaderboardSupported;
    }

    public override bool ShowAchievements()
    {
        mJavaClassUBridge.CallStatic("ShowAchievement");
        return leaderboardSupported;
    }

    public override bool ReportAchievement(long score, string boardId)
    {
        mJavaClassUBridge.CallStatic("ReportAchievement", score, boardId);
        return leaderboardSupported;
    }

    #endregion

    #region MoreGame

    public override bool IsNeedShowMoreGame()
    {
        return mJavaClassUBridge.CallStatic<bool>("IsNeedShowMoreGame");
    }

    public override void MoreGameClick()
    {
        mJavaClassUBridge.CallStatic("MoreGameClick");
    }

    #endregion

    #region 隐私策略

    public override bool IsNeedShowPrivacy()
    {
        return mJavaClassUBridge.CallStatic<bool>("IsNeedShowPrivacy");
    }

    public override bool IsNeedShowFeedBack()
    {
        return mJavaClassUBridge.CallStatic<bool>("IsNeedShowFeedBack");
    }

    public override void ShowPrivacy(PrivacyType privacyType)
    {
        mJavaClassUBridge.CallStatic("ShowPrivacy", (int)privacyType);
    }

    public override bool IsNeedShowLogout()
    {
        return mJavaClassUBridge.CallStatic<bool>("IsNeedShowLogout");
    }

    public override void ShowLogout()
    {
        mJavaClassUBridge.CallStatic("ShowLogout");
    }

    public override bool IsNeedShowAntiAddiction()
    {
        return mJavaClassUBridge.CallStatic<bool>("IsNeedShowAntiAddiction");
    }

    public override void ShowAntiAddction()
    {
        mJavaClassUBridge.CallStatic("ShowAntiAddction");
    }

    #endregion

    #region 系统版本号

    public override int GetSystemVersion()
    {
        return mJavaClassUBridge.CallStatic<int>("GetAndroidSDKVersion");
    }

    #endregion

    #region Remote

    public override string GetRemoteValueString(string key, string defaultValue = "default")
    {
        string result = mJavaClassUBridge.CallStatic<string>("GetRemoteValueString", key);
        if (string.IsNullOrEmpty(result))
        {
            return defaultValue;
        }
        else
        {
            return result;
        }
    }

    #endregion

    #region 游戏存档导入导出

    public override bool CanSaveLoadGame()
    {
        return false;
    }

    public override void ExportGameProfile()
    {
        
    }

    public override void ImportGameProfile(string saveText)
    {
        
    }

    public override bool EnableCloudGameSave()
    {
        return mJavaClassUBridge.CallStatic<bool>("EnableCloudGameSave");
    }
    
    public override void UploadGameFileToCloud(string json)
    {
        mJavaClassUBridge.CallStatic("UploadGameFileToCloud",json);
    }

    public override void LoadCloudGameFile()
    {
        mJavaClassUBridge.CallStatic("LoadCloudGameFile");
    }

    public override void DownloadCloudGameFile()
    {
        mJavaClassUBridge.CallStatic("DownloadCloudGameFile");
    }

    #endregion

    #region 兑换码

    public override bool EnableGiftCode()
    {
        return mJavaClassUBridge.CallStatic<bool>("EnableGiftCode");
    }
    
    public override void SendGiftCode(string giftCode)
    {
        //Debug.Log("SendGiftCode:" + giftCode);

        mJavaClassUBridge.CallStatic("SendGiftCode",giftCode);
        //callback_gift_code_result(
        //    @"{""activity_id"":""TDS202407191731212FZ"",""c_sign"":""b1281a0e1bf871f0a1965b9dd9e15e21d8705249"",""content"":""[{\""name\"": \""Skin101\"", \""number\"": 1}, {\""name\"": \""Screw\"", \""number\"": 100}]"",""content_obj"":[{""name"":""Skin101"",""number"":1},{""name"":""Screw"",""number"":100}],""custom"":{},""error"":0,""nonce_str"":""KEP"",""sign"":""e8477bec508ba4f2554984eb2654d7aa87aedd34"",""success"":true,""timestamp"":1721381587}");
    }

    #endregion
    
    #region 排行榜
    

    public override bool EnableLeaderBoard()
    {
        return mJavaClassUBridge.CallStatic<bool>("EnableLeaderBoard");
    }

    public override void ShowLeaderBoard(string leaderboardName)
    {
        mJavaClassUBridge.CallStatic("ShowLeaderBoard",leaderboardName);
    }

    public override void ShowMyScore(string leaderboardName)
    {
        mJavaClassUBridge.CallStatic("ShowMyScore",leaderboardName);
    }

    public override void SubmitScore(string leaderboardName, int score)
    {
        mJavaClassUBridge.CallStatic("SubmitScore",leaderboardName,score);
    }

    

    #endregion

    #region 成就

    public override bool EnableAchievement()
    {
        return mJavaClassUBridge.CallStatic<bool>("EnableAchievement");
    }

    public override void ReachAchievement(string displayID)
    {
        mJavaClassUBridge.CallStatic("ReachAchievement",displayID);
    }

    public override void SetSteps(string displayID, int steps)
    {
        mJavaClassUBridge.CallStatic("SetSteps",displayID,steps);
    }
    public override void AddSteps(string displayID, int steps)
    {
        mJavaClassUBridge.CallStatic("AddSteps",displayID,steps);
    }
    #endregion
}
#endif