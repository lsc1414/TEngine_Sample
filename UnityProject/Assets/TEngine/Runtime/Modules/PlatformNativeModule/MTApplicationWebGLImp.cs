#if UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
public class MTApplicationWebGLImp : MTApplication
{

    [DllImport("__Internal")]
    private static extern void nav_init();

    [DllImport("__Internal")]
    private static extern void nav_showBanner(string tag, bool top);

    [DllImport("__Internal")]
    private static extern void nav_hideBanner();
    [DllImport("__Internal")]
    private static extern void nav_showIncentivizedAd();
    [DllImport("__Internal")]
    static extern bool nav_isIncentivizedAdAvailable();

    [DllImport("__Internal")]
    static extern void nav_showInterstitialAd();
    [DllImport("__Internal")]
    static extern bool nav_isInterstitialAdAvailable();
    [DllImport("__Internal")]
    static extern bool nav_isNativeAdAvailable();
    [DllImport("__Internal")]
    static extern string nav_loadNativeAdInfo();
    [DllImport("__Internal")]
    static extern void nav_clickNativeAd(string adId);
    //力反馈
    [DllImport("__Internal")]
    static extern void nav_impactLight();
    [DllImport("__Internal")]
    static extern void nav_impactMedium();
    [DllImport("__Internal")]
    static extern void nav_impactHeavy();
    [DllImport("__Internal")]
    static extern bool nav_isInstallShortcut();
    [DllImport("__Internal")]
    static extern void nav_installShortcut();
    [DllImport("__Internal")]
    static extern void nav_clickMoreGame(string url);
    //[DllImport("__Internal")]
    //static extern void nav_quitapp();
    protected override void Start()
    {
        nav_init();
    }

#region Banner

    static float lastShowBannerTime = -9999;
    static int showBannerInterval = 60;
    public override void ShowBanner(bool isTop, string tag)
    {
        if((Time.realtimeSinceStartup-lastShowBannerTime)>showBannerInterval)
        {
            nav_showBanner(tag, isTop);
            lastShowBannerTime = Time.realtimeSinceStartup;
        }
        // if (!AdsDisable && CanAdsByRemote && CanShowBannerRemote) 
        //     nav_showBanner(tag, isTop);
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
        nav_hideBanner();
    }
#endregion
#region Interstitial
    System.Action _lastCompleteInterstitialAdCallback = null;
    void onInterstitialAdDidHideAd()
    {
        onDidHideAd -= onInterstitialAdDidHideAd;
        if (_lastCompleteInterstitialAdCallback != null)
            _lastCompleteInterstitialAdCallback();
    }

    public override void ShowInterstitialAd()
    {
        ShowInterstitialAd(null);
    }

    public override void ShowInterstitialAd(Action completed)
    {
        if (IsInterstitialAdAvailable())
        {
            _lastCompleteInterstitialAdCallback = completed;
            onDidHideAd += onInterstitialAdDidHideAd;
            nav_showInterstitialAd();
        }
        else
        {
            if (completed != null)
                completed();
        }
    }

    public override bool IsInterstitialAdAvailable()
    {
        return !AdsDisable && CanAdsByRemote && nav_isInterstitialAdAvailable();
    }

    public override void ShowInterstitialAdForTag(string tag, Action completed)
    {
        if (IsInterstitialAdAvailableForTag(tag))
        {
            _lastCompleteInterstitialAdCallback = completed;
            onDidHideAd += onInterstitialAdDidHideAd;
            nav_showInterstitialAd();
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
        return !AdsDisable && CanAdsByRemote && nav_isInterstitialAdAvailable();
    }
#endregion
#region StaticInterstitial
    System.Action _lastCompleteStaticInterstitialAdCallback = null;
    void onStaticInterstitialAdDidHideAd()
    {
        onDidHideAd -= onStaticInterstitialAdDidHideAd;
        if (_lastCompleteStaticInterstitialAdCallback != null)
            _lastCompleteStaticInterstitialAdCallback();
    }

    public override void ShowStaticInterstitialAd()
    {
        ShowStaticInterstitialAd(null);
    }

    public override void ShowStaticInterstitialAd(Action completed)
    {
        if (IsStaticInterstitialAdAvailable())
        {
            _lastCompleteStaticInterstitialAdCallback = completed;
            onDidHideAd += onStaticInterstitialAdDidHideAd;
            nav_showInterstitialAd();
        }
        else
        {
            if (completed != null)
                completed();
        }
    }

    public override bool IsStaticInterstitialAdAvailable()
    {
        return !AdsDisable && CanAdsByRemote && nav_isInterstitialAdAvailable();
    }

    public override void ShowStaticInterstitialAdForTag(string tag, Action completed)
    {
        if (IsStaticInterstitialAdAvailableForTag(tag))
        {
            _lastCompleteStaticInterstitialAdCallback = completed;
            onDidHideAd += onStaticInterstitialAdDidHideAd;
            nav_showInterstitialAd();
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
        return !AdsDisable && CanAdsByRemote && nav_isInterstitialAdAvailable();
    }
#endregion
#region Reward Video
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
        _lastCompleteIncentivizedADCallback = null;
    }

    public override void ShowIncentivizedAd(System.Action<bool> completed, float maxWaitingtimeForRewards)
    {
        _DelayCompleted = maxWaitingtimeForRewards;
        _lastCompleteIncentivizedADCallback = completed;
        onDidHideAd += onIncentivizedAdDidHideAd;
        onDidCompleteAdRewards += onIncentivizedAdDidCompleted;
        onDidCompleteAdRewardsFailed += onIncentivizedAdDidCompleted;
        CancelInvoke("onShowIncentivizedAdCompleted");
        nav_showIncentivizedAd();
    }

    public override void ShowIncentivizedAd(System.Action<bool> completed)
    {
        ShowIncentivizedAd(completed, 2);
    }

    public override void ShowIncentivizedAdForTag(string tag, Action<bool> completed, float maxWaitingtimeForRewards)
    {
        _DelayCompleted = maxWaitingtimeForRewards;
        _lastCompleteIncentivizedADCallback = completed;
        onDidHideAd += onIncentivizedAdDidHideAd;
        onDidCompleteAdRewards += onIncentivizedAdDidCompleted;
        onDidCompleteAdRewardsFailed += onIncentivizedAdDidCompleted;
        CancelInvoke("onShowIncentivizedAdCompleted");
        nav_showIncentivizedAd();
    }

    public override void ShowIncentivizedAdForTag(string tag, Action<bool> completed)
    {
        ShowIncentivizedAdForTag(tag, completed, 2);
    }

    public override bool IsIncentivizedAdAvailable()
    {
        return CanAdsByRemote && nav_isIncentivizedAdAvailable();
    }

    public override bool IsIncentivizedAdAvailableForTag(string tag)
    {
        return CanAdsByRemote &&nav_isIncentivizedAdAvailable();
    }
#endregion

#region 原生广告

    static float lastShowNativeAdTime = -9999;
    static int showNativeAdInterval = 60;  
    public override bool IsNativeAdAvailable() {
         return (Time.realtimeSinceStartup-lastShowNativeAdTime>showNativeAdInterval) && nav_isNativeAdAvailable(); 
    }
    public override string LoadNativeAdInfo() {

        lastShowNativeAdTime = Time.realtimeSinceStartup;
        return nav_loadNativeAdInfo(); 
        }

    public override void ClickNativeAd(string adId) {
        nav_clickNativeAd(adId);
    }
#endregion
    /// <summary>
    /// 退出应用
    /// 一般在Android平台 ，游戏首页时点击返回按钮时候发送的通知.
    /// </summary>
    public override void QuitApp()
    {
        //nav_quitapp();
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
        if (CanTapticImpact())
        {
            switch (tapticImpactType)
            {
                case MTApplication.TapticImpactType.Light:
                    nav_impactLight();
                    break;
                case MTApplication.TapticImpactType.Medium:
                    nav_impactMedium();
                    break;
                case MTApplication.TapticImpactType.Heavy:
                    nav_impactHeavy();
                    break;
            }
        }
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

    public override void IapBuy(string productIdentifier, System.Action<bool/*result*/, string/*productIdentifier*/, string/*receipt*/> callback)
    {
        if (callback != null)
        {
            PlayerPrefs.SetInt(productIdentifier, 1);
            PlayerPrefs.Save();
            callback(true, productIdentifier, "");
            if(productIdentifier.Contains("subs"))
            {
                // 订阅,5分钟
                System.DateTime expireDate2 = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                System.DateTime now = System.DateTime.Now;
                now = now.AddMinutes(5);
                var time = now - expireDate2;
                var seconds =(long) time.TotalSeconds;
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

    public override void AnalyticsCustom(string eventName, System.Collections.Generic.Dictionary<string, object> parameters)
    {
        UnityEngine.Analytics.Analytics.CustomEvent(eventName, parameters);
        StringBuilder sb = new StringBuilder();
        if(parameters!=null)
        {
            foreach (var kv in parameters)
            {
                sb.AppendFormat("{0}:{1} ", kv.Key, kv.Value);
            }
        }
        string str = sb.ToString();
        Debug.Log("通用统计:" + eventName+" : "+ str);
    }

    public override void AnalyticsCustom(string eventName, string key, object value)
    {
        AnalyticsCustom(eventName, new Dictionary<string, object> { { key, value } });

    }

    public override void AnalyticsCustom(string eventName)
    {
        AnalyticsCustom(eventName, null);
    }

    public override void AnalyticsCustomSingleParam(string eventName, object parameter)
    {
        Debug.Log("通用统计:" + eventName);
    }

    public override void RegisterLocalNotification()
    {
        //Debug.Log("RegisterLocalNotification");
    }

    public override void AddLocalNotification(string notifiText, float waitSecond)
    {
        //Debug.Log(string.Format("AddLocalNotification:{0}  wait:{1}h", notifiText, waitSecond / 3600));
    }

    public override void ClearAllLocalNotifications()
    {
        //Debug.Log("ClearAllLocalNotifications");
    }

    public override void SendEmail(string subject, string recipients, string emailBody)
    {
        Debug.Log(string.Format("SendEmail: subject:{0}  recipients:{1} emailBody:{2}", subject, recipients, emailBody));
    }

    public override MTApplication.Channel channel
    {
        get
        {
            return Channel.oppo_mini;
        }
    }

    public override string udid
    {
        get 
        {
            return "udid";
        }
    }

    
#region installShortcut
    public override bool IsInstallShortcut()
    {
        return nav_isInstallShortcut();
    }
    public override void InstallShortcut()
    {
        nav_installShortcut();
        UnityAnalyticsCustom("InstallShortcutClick");
    }
#endregion

#region MiniMoreGame
    public override void OnMiniMoreGameClick(string url)
    {
        nav_clickMoreGame(url);
        UnityAnalyticsCustom("MiniMoreGameClick", new Dictionary<string, object>()
        {
            { "package",url}
        });
    }
#endregion
    protected override void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
    }
}
#endif