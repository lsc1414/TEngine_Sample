using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TEngine;
using UnityEngine;
using UnityEngine.Analytics;

abstract public class MTApplication : MonoBehaviour
{
    /// <summary>
    /// 力反馈力度
    /// </summary>
    public enum TapticImpactType
    {
        Light,
        Medium,
        Heavy
    }

    /// <summary>
    /// 当前渠道
    /// </summary>
    public enum Channel
    {
        ios_appstore,
        android_googleplay,
        oppo,
        vivo,
        m4399,
        megatouch,
        oppo_mini,
        unknown,
        taptap
    }
    #region Public Event.

    #region 广告回调

    /// <summary>
    /// banner点击事件回调
    /// </summary>
    public event Action onBannerWasClicked = null;

    /// <summary>
    /// 全屏广告点击事件回调
    /// </summary>
    public event Action onADWasClicked = null;

    /// <summary>
    /// 开始展示全屏广告
    /// </summary>
    public event Action onDidShowAd = null;

    /// <summary>
    /// 全屏广告关闭
    /// </summary>
    public event Action onDidHideAd = null;

    /// <summary>
    /// 获得广告奖励
    /// </summary>
    public event Action onDidCompleteAdRewards = null;

    /// <summary>
    ///  未获得广告奖励
    /// </summary>
    public event Action onDidCompleteAdRewardsFailed = null;

    #endregion

    /// <summary>
    /// 内购回调
    /// </summary>
    public event Action<bool /*result*/, string /*productIdentifier*/, string /*receipt*/> onIapPurchased = null;

    public event Action<string> onIapRestored = null;
    public event Action onIapRestoreFailed = null;
    public event Action onIapRestoreFinished = null;
    public event Action onIapProductUpdated = null;
    public event Action onIapSubscriptionUpdated = null;

    #region 登录回调

    public event Action<bool, string> onFacebookLogin = null;

    #endregion

    #region 图标安装回调

    public event Action onInstallShortcutSuccess = null;

    #endregion

    #region Properties.

    //最近的视频广告是否获取到了奖励
    public bool HasGettedADRewards { get; protected set; }

    //广告失效
    public bool AdsDisable
    {
        get { return PlayerPrefs.GetInt("[AdsDisable]", 0) != 0; }
        set
        {
            PlayerPrefs.SetInt("[AdsDisable]", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    //远程控制是否可以内购
    public bool CanIapByRemote = true;

    public bool CanIapByLanguage = true;

    //远程控制是否可以广告
    public bool CanAdsByRemote = true;

    public bool CanShowBannerRemote = false;

    //渠道配置磁盘路径
    public string ChannelProfilePath
    {
        get
        {
            string path = Application.persistentDataPath + "/ChannelProfile";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }

    public void InitRemoteSetting()
    {
        CanShowBannerRemote = RemoteSettings.GetBool("EnableBanner", false);
        CanIapByRemote = RemoteSettings.GetBool("CanIapByRemote", true);
        CanAdsByRemote = RemoteSettings.GetBool("CanAdsByRemote", true);
    }

    public virtual string GetRemoteValueString(string key, string defaultValue = "default")
    {
        return defaultValue;
    }

    //服务器获取的Iap商品信息文件存放路径
    public string NetIapProductInfoFilePath
    {
        get { return ChannelProfilePath + "/productInfo.json"; }
    }

    //最近的内购商品ID
    public string LastIapProductIdentifier { get; protected set; }

    #endregion

    // 初始化结束后,再进入游戏
    [HideInInspector] public bool isInitFinish = false;

    protected virtual void Awake()
    {
        name = "__mtapplication__";
        SyncNetIapProducts();
        InitRemoteSetting();
        //DontDestroyOnLoad(gameObject);
        isInitFinish = true;
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    #region native.

    virtual public void DisableAds()
    {
        AdsDisable = true;
        HideBanner();
    }

    /// <summary>
    /// 显示banner广告
    /// </summary>
    /// <param name="isTop">是否在上面显示</param>
    abstract public void ShowBanner(bool isTop, string tag);

    abstract public void ShowBanner(bool isTop);
    abstract public void ShowBannerNoRemote(bool isTop);

    /// <summary>
    /// 隐藏banner广告
    /// </summary>
    abstract public void HideBanner();

    /// <summary>
    /// 播放插屏广告
    /// </summary>
    abstract public void ShowInterstitialAd(Action completed);

    abstract public void ShowInterstitialAd();
    abstract public void ShowInterstitialAdForTag(string tag, Action completed);
    abstract public void ShowInterstitialAdForTag(string tag);

    // 插屏冷却20s
    public int InterstitialMinimumInterval = 20;

    // 插屏开始关卡
    public int InterstitialMinimumLevel = 1;

    /// <summary>
    /// 插屏广告是否有效
    /// </summary>
    abstract public bool IsInterstitialAdAvailable();

    abstract public bool IsInterstitialAdAvailableForTag(string tag);

    /// <summary>
    /// 播放静态插屏广告
    /// </summary>
    abstract public void ShowStaticInterstitialAd(Action completed);

    abstract public void ShowStaticInterstitialAd();
    abstract public void ShowStaticInterstitialAdForTag(string tag, Action completed);
    abstract public void ShowStaticInterstitialAdForTag(string tag);

    /// <summary>
    /// 静态插屏广告是否有效
    /// </summary>
    abstract public bool IsStaticInterstitialAdAvailable();

    abstract public bool IsStaticInterstitialAdAvailableForTag(string tag);

    /// <summary>
    /// 播放激励广告
    /// </summary>
    /// <param name="completed">完成回调，参数获取是否成功获取奖励</param>
    /// <param name="maxWaitingtimeForRewards">最大奖励回调的等待时长</param>
    abstract public void ShowIncentivizedAd(Action<bool> completed, float maxWaitingtimeForRewards);

    abstract public void ShowIncentivizedAd(Action<bool> completed);
    abstract public void ShowIncentivizedAdForTag(string tag, Action<bool> completed, float maxWaitingtimeForRewards);
    abstract public void ShowIncentivizedAdForTag(string tag, Action<bool> completed);

    /// <summary>
    /// 激励广告是否有效
    /// </summary>
    abstract public bool IsIncentivizedAdAvailable();

    abstract public bool IsIncentivizedAdAvailableForTag(string tag);


    public bool IsRewardedVideoAvailable { get; set; } = true;

    #region 原生广告

    public virtual bool IsNativeAdAvailable()
    {
        return true;
    }

    /// <summary>
    /// 获取NativeAd信息
    /// </summary>
    /// <returns>imageurl#adId</returns>
    public virtual string LoadNativeAdInfo()
    {
        return "";
    }

    public virtual void ClickNativeAd(string adId)
    {
    }

    #endregion

    #region GIF Ad

    public virtual void ShowGifAds(int x = 0, int y = 420, int rotate = 0, float scale = 0.5f)
    {
    }

    public virtual void HideGifAds()
    {
    }

    #endregion

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="subject">主题</param>
    /// <param name="recipients">收信人</param>
    /// <param name="emailBody">内容</param>
    abstract public void SendEmail(string subject, string recipients, string emailBody);

    /// <summary>
    /// 退出应用
    /// 一般在Android平台 ，游戏首页时点击返回按钮时候发送的通知.
    /// </summary>
    abstract public void QuitApp();

    /// <summary>
    /// URL评价
    /// </summary>
    abstract public void RateByUrl();

    /// <summary>
    /// 游戏内评价
    /// </summary>
    abstract public void RateInGame();

    /// <summary>
    /// 是否可以游戏内评价
    /// </summary>
    abstract public bool CanRateInGame();

    /// <summary>
    /// IOS触感引擎力反馈
    /// </summary>
    abstract public void TapticImpact(TapticImpactType tapticImpactType);

    static int tapticImapct = -1;

    public bool CanTapticImpact()
    {
        if (tapticImapct == -1)
        {
            tapticImapct = PlayerPrefs.GetInt("is_vibrate_on", 1);
        }

        return tapticImapct == 1;
    }

    public void ChangeTapticImpact(bool enable)
    {
        PlayerPrefs.SetInt("is_vibrate_on", enable ? 1 : 0);
        tapticImapct = enable ? 1 : 0;
    }

    #region 分享

    abstract public void Share(string text, string imagePath);

    public void Share(string imagePath)
    {
        Share(null, imagePath);
    }

    public void Share(Texture2D image)
    {
        Share(null, image);
    }

    public void Share(string text, Texture2D image)
    {
        string path = Application.persistentDataPath + "/temp_sharedImage.png";
        System.IO.File.WriteAllBytes(path, image.EncodeToPNG());
        Share(text, path);
    }

    /// <summary>
    /// FB分享文本+hashTag+商店连接
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="hashTag">hashTag</param>
    abstract public void FBShareText(string text, string hashTag, Action<string> shareCallback = null);

    /// <summary>
    /// FB分享图片+hashTag
    /// </summary>
    /// <param name="hashTag">hashTag</param>
    /// <param name="imagePath">图片</param>
    /// <param name="shareCallback">回调:1:success 2:fail 3:cancel</param>
    abstract public void FBShareImage(string hashTag, string imagePath, Action<string> shareCallback = null);

    /// <summary>
    /// FB分享图片+hashTag
    /// </summary>
    /// <param name="hashTag">hashTag</param>
    /// <param name="image">图片</param>
    /// <param name="shareCallback">回调:1:success 2:fail 3:cancel</param>
    public void FBShareImage(string hashTag, Texture2D image, Action<string> shareCallback = null)
    {
        string path = Application.persistentDataPath + "/temp_sharedImage.png";
        System.IO.File.WriteAllBytes(path, image.EncodeToPNG());
        FBShareImage(hashTag, path);
    }

    protected Action<string> mShareCallback = null;

    /// <summary>
    /// facebook分享回调
    /// </summary>
    /// <param name="result">1:success 2:fail 3:cancel</param>
    protected void callback_fbShareResult(string result)
    {
        Debug.Log("callback_fbShare_Result :" + result);
        mShareCallback?.Invoke(result);
    }

    #endregion

    #region 内购

    public class IapProduct
    {
        public string price;
        public string title;
        public string productId;
        public string description;
    }

    /// <summary>
    /// 同步后台的IAP商品列表
    /// </summary>
    abstract public void SyncNetIapProducts();

    /// <summary>
    /// 内购
    /// </summary>
    abstract public void IapBuy(string productIdentifier,
        Action<bool /*result*/, string /*productIdentifier*/, string /*receipt*/> callback);

    /// <summary>
    /// 恢复购买
    /// </summary>
    abstract public void IapRestore(Action<List<string>> callback);

    /// <summary>
    /// 是否已经购买过
    /// </summary>
    public bool IapHasPurchased(string productIdentifier)
    {
        return PlayerPrefs.GetInt(productIdentifier, 0) == 1;
    }

    /// <summary>
    /// 订阅是否生效中
    /// </summary>
    public bool IsSubscriptionEffectivity()
    {
        string expireDateStr = PlayerPrefs.GetString("subscriptionExpiredTime", "");
        if (String.IsNullOrEmpty(expireDateStr))
        {
            return false;
        }

        long seconds = long.Parse(expireDateStr);
        DateTime expireDate = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        expireDate = expireDate.AddSeconds(seconds).ToLocalTime();
        DateTime nowDate = DateTime.Now.ToLocalTime();
        if (expireDate < nowDate)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    string localIapProductInfoJson
    {
        get { return ReadStreamingAssets("ChannelProfile/" + channel.ToString() + "/productInfo.json"); }
    }

    void verifyNetIapProductInfo()
    {
        if (File.Exists(NetIapProductInfoFilePath))
        {
            try
            {
                LitJson.JsonData netJsonData = LitJson.JsonMapper.ToObject(File.ReadAllText(NetIapProductInfoFilePath));
                LitJson.JsonData localJsonData = LitJson.JsonMapper.ToObject(localIapProductInfoJson);
                if (netJsonData.Count == localJsonData.Count)
                {
                    for (int i = 0; i < netJsonData.Count; ++i)
                    {
                        if (netJsonData[i]["productId"].ToString() != localJsonData[i]["productId"].ToString())
                        {
                            File.Delete(NetIapProductInfoFilePath);
                            break;
                        }
                    }
                }
                else
                {
                    File.Delete(NetIapProductInfoFilePath);
                }
            }
            catch
            {
                File.Delete(NetIapProductInfoFilePath);
            }
        }
    }

    bool mIapProdcutDirty = true;
    bool mIapProdcutMapDirty = true;
    List<IapProduct> mIapProdcuts = new List<IapProduct>();
    Dictionary<string, IapProduct> mIapProdcutsMap = new Dictionary<string, IapProduct>();

    /// <summary>
    /// 内购商品List
    /// </summary>
    public List<IapProduct> IapProdcuts
    {
        get
        {
            if (mIapProdcutDirty)
            {
                mIapProdcuts.Clear();
                mIapProdcutDirty = false;
                verifyNetIapProductInfo();
                string json = null;
                if (File.Exists(NetIapProductInfoFilePath))
                {
                    json = File.ReadAllText(NetIapProductInfoFilePath);
                }
                else
                {
                    json = localIapProductInfoJson;
                }

                if (json != null)
                {
                    LitJson.JsonData jsonData = LitJson.JsonMapper.ToObject(json);
                    foreach (LitJson.JsonData jd in jsonData)
                    {
                        IapProduct ip = new IapProduct();
                        ip.price = jd["price"].ToString();
                        ip.title = jd["title"].ToString();
                        ip.productId = jd["productId"].ToString();
                        ip.description = jd["description"].ToString();
                        mIapProdcuts.Add(ip);
                    }
                }
                else
                {
                    Debug.LogError(string.Format("{0}No config！Please Check", channel.ToString()));
                }
            }

            return mIapProdcuts;
        }
    }

    /// <summary>
    /// 内购商品Map
    /// </summary>
    public Dictionary<string, IapProduct> IapProdcutsMap
    {
        get
        {
            if (mIapProdcutMapDirty)
            {
                mIapProdcutsMap.Clear();
                mIapProdcutMapDirty = false;
                foreach (IapProduct ip in IapProdcuts)
                    mIapProdcutsMap.Add(ip.productId, ip);
            }

            return mIapProdcutsMap;
        }
    }

    /// <summary>
    /// 是否可以购买
    /// </summary>
    abstract public bool IapCanBuy();

    /// <summary>
    /// 检测订阅状态
    /// </summary>
    abstract public void CheckSubscriptionStatus();

    #endregion

    /// <summary>
    /// unity 统计
    /// </summary>
    public void UnityAnalyticsCustom(string eventName, Dictionary<string, object> parameters)
    {
        Analytics.CustomEvent(eventName, parameters);
        AnalyticsCustom(eventName, parameters);
    }

    public void UnityAnalyticsCustom(string eventName, string key, object value)
    {
        UnityAnalyticsCustom(eventName, new Dictionary<string, object> { { key, value } });
    }

    public void UnityAnalyticsCustom(string eventName)
    {
        UnityAnalyticsCustom(eventName, null);
    }

    /// <summary>
    /// 通用统计
    /// </summary>
    abstract public void AnalyticsCustom(string eventName, Dictionary<string, object> parameters);

    abstract public void AnalyticsCustom(string eventName, string key, object value);
    abstract public void AnalyticsCustom(string eventName);
    abstract public void AnalyticsCustomSingleParam(string eventName, object parameter);

    public virtual void UmengLevelStart(string level)
    {
    }

    public virtual void UmengLevelFail(string level)
    {
    }

    public virtual void UmengLevelFail(string level, int score)
    {
    }

    public virtual void UmengLevelFinish(string level)
    {
    }

    public virtual void UmengLevelFinish(string level, int score)
    {
    }

    public virtual void UmengSetUserLevel(int levelNo)
    {
    }

    /// <summary>
    /// 热云自定义事件
    /// </summary>
    /// <param name="eventName">只能是event_1 ~ event12</param>
    public virtual void ReYunTrackEvent(string eventName)
    {
    }

    #region 通知

    //通知的发送规则设计为：
    //1.第2日的通知：如果玩家在6-22点之间登陆游戏，在第2日的相同登陆时间触发通知。如果玩家在22-6点之间登陆游戏，在第2日的22点触发通知。
    //2.第3、5、7日的通知：中午12点触发通知
    //3.第4、6、8日的通知：晚上6点触发通知
    //
    //通知内容配置在MessageTemplate中。
    //第2日的通知使用的Key为呢notifi:nextday, 
    //第3、5、7日的通知使用的Key为notifi:thirday, 
    //第4、6、8日的通知使用的Key为notifi:fourthday
    //如果需要配置多条不同的消息随机，那么请使用"|||"来分隔不同的消息
    public void AddCommonLocalNotifications(string morrowMsg,
        string day357Msg,
        string day468Msg)
    {
        DateTime now;
        int delay;
        delay = 0;
        now = DateTime.Now;
        //当前是早上6点到晚上10点
        if (now.Hour >= 6 && now.Hour <= 22)
        {
            //次日同点触发
            AddLocalNotification(getRandomString(morrowMsg), 24 * 3600);
        }
        else
        {
            //次日晚上10点触发
            if (now.Hour > 22)
            {
                delay = (int)(((new DateTime(now.Year, now.Month, now.Day, 22, 0, 0)).AddDays(1) - now).TotalSeconds);
                AddLocalNotification(getRandomString(morrowMsg), delay);
            }
            else
            {
                delay = (int)((new DateTime(now.Year, now.Month, now.Day, 22, 0, 0) - now).TotalSeconds);
                AddLocalNotification(getRandomString(morrowMsg), delay);
            }
        }

        //第3 ,5, 7天 中午12点触发通知
        for (int i = 2; i <= 6; i += 2)
        {
            if (now.Hour < 6)
            {
                delay = (int)(((new DateTime(now.Year, now.Month, now.Day, 12, 15, 0)).AddDays(i - 1) - now)
                    .TotalSeconds);
                AddLocalNotification(getRandomString(day357Msg), delay);
            }
            else
            {
                delay = (int)(((new DateTime(now.Year, now.Month, now.Day, 12, 15, 0)).AddDays(i) - now).TotalSeconds);
                AddLocalNotification(getRandomString(day357Msg), delay);
            }
        }

        //第 4，6,8 天 下午18点触发通知
        for (int i = 3; i <= 7; i += 2)
        {
            if (now.Hour < 6)
            {
                delay = (int)(((new DateTime(now.Year, now.Month, now.Day, 18, 15, 0)).AddDays(i - 1) - now)
                    .TotalSeconds);
                AddLocalNotification(getRandomString(day468Msg), delay);
            }
            else
            {
                delay = (int)(((new DateTime(now.Year, now.Month, now.Day, 18, 15, 0)).AddDays(i) - now).TotalSeconds);
                AddLocalNotification(getRandomString(day468Msg), delay);
            }
        }
    }

    string getRandomString(string str)
    {
        string[] s = str.Split(new string[] { "|||" }, StringSplitOptions.None);
        return s[UnityEngine.Random.Range(0, s.Length)];
    }

    abstract public void RegisterLocalNotification();
    abstract public void AddLocalNotification(string notifiText, float waitSecond);
    abstract public void ClearAllLocalNotifications();

    #endregion

    #region 通知回调

    protected void callback_registerNotificationResult(string result)
    {
        if (PlayerPrefs.HasKey("regRealNotifyResult"))
        {
            return;
        }

        if (result == "1")
        {
            UnityAnalyticsCustom("regRealNotifyResult", new Dictionary<string, object> { { "result", true } });
            PlayerPrefs.SetInt("regRealNotifyResult", 1);
        }
        else if (result == "0")
        {
            UnityAnalyticsCustom("regRealNotifyResult", new Dictionary<string, object> { { "result", false } });
            PlayerPrefs.SetInt("regRealNotifyResult", 0);
        }
    }

    protected void callback_registerNotification_launch()
    {
        UnityAnalyticsCustom("regNotifyLanuch");
    }

    #endregion

    #region 排行榜

    /// <summary>
    /// 是否支持排行榜
    /// </summary>
    public virtual bool leaderboardSupported
    {
        get
        {
#if UNITY_IPHONE || UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    /// <summary>
    /// 向排行榜提交排行数据
    /// </summary>
    public virtual void ReportScoreToLeaderboard(long score, string boardId)
    {
        if (leaderboardSupported && Social.localUser.authenticated)
            Social.ReportScore(score, boardId, null);
    }

    /// <summary>
    /// 弹出排行榜界面
    /// </summary>
    public virtual bool ShowLeaderboard()
    {
        bool ret = leaderboardSupported && Social.localUser.authenticated;
        if (ret) Social.ShowLeaderboardUI();
        return ret;
    }

    public virtual bool ShowAchievements()
    {
        bool ret = leaderboardSupported && Social.localUser.authenticated;
        if (ret) Social.ShowAchievementsUI();
        return ret;
    }

    public virtual bool ReportAchievement(long score, string boardId)
    {
        if (leaderboardSupported && Social.localUser.authenticated)
        {
            Social.ReportProgress(boardId, score, (bool success) =>
            {
                if (success)
                {
                }
                else
                {
                }
            });
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    /// <summary>
    /// 当前渠道
    /// </summary>
    abstract public Channel channel { get; }

    /// <summary>
    /// 设备id
    /// </summary>
    abstract public string udid { get; }

    #region 登录

    #region APP发布平台登录(例如:  appStore / googlePlay 等等)

    virtual public void Login()
    {
        Social.localUser.Authenticate((b) => { });
    }

    #endregion

    #endregion

    #region MoreGame

    public virtual bool IsNeedShowMoreGame()
    {
        return false;
    }

    public virtual void MoreGameClick()
    {
    }

    #endregion

    #region installShortcut

    public virtual bool IsInstallShortcut()
    {
        return true;
    }

    public virtual void InstallShortcut()
    {
    }

    #endregion

    #region MiniMoreGame

    public virtual void OnMiniMoreGameClick(string url)
    {
        Debug.Log("OnMiniMoreGameClick:" + url);
    }

    #endregion

    #region 隐私策略

    public enum PrivacyType
    {
        Privacy = 1,
        Rules,
        Feedback
    }

    public virtual bool IsNeedShowPrivacy()
    {
        return false;
    }

    public virtual bool IsNeedShowFeedBack()
    {
        return false;
    }

    public virtual void ShowPrivacy(PrivacyType privacyType)
    {
    }

    public virtual bool IsNeedShowLogout()
    {
        return false;
    }

    public virtual void ShowLogout()
    {
    }

    public virtual bool IsNeedShowAntiAddiction()
    {
        return false;
    }

    public virtual void ShowAntiAddction()
    {
    }

    #endregion

    #region 系统版本号

    public virtual int GetSystemVersion()
    {
        return -1;
    }

    #endregion

    #region 游戏存档导入导出

    public virtual bool CanSaveLoadGame()
    {
        return false;
    }

    public virtual void ExportGameProfile()
    {
    }

    public virtual void ImportGameProfile(string saveText)
    {
    }

    protected void callback_export_game_Result(string result)
    {
        //onInstallShortcutSuccess?.Invoke();
    }

    protected void callback_import_game_result(string result)
    {
        //onInstallShortcutSuccess?.Invoke();
    }

    public virtual bool EnableCloudGameSave()
    {
        return false;
    }

    public virtual void UploadGameFileToCloud(string json)
    {
    }

    public virtual void LoadCloudGameFile()
    {
    }

    public virtual void DownloadCloudGameFile()
    {
    }

    public event Action<string> onCloudFileLoaded = null;
    public event Action<string> onCloudFileDownloaded = null;
    public event Action<string> onUploadFileFinished = null;

    /// <summary>
    /// json
    /// 
    /// </summary>
    /// <param name="result"></param>
    protected void callback_load_cloud_file_Result(string result)
    {
        Debug.Log(result);
        onCloudFileLoaded?.Invoke(result);
    }

    protected void callback_download_cloud_file_Result(string result)
    {
        Debug.Log(result);
        onCloudFileDownloaded?.Invoke(result);
    }

    protected void callback_upload_cloud_file_Result(string result)
    {
        Debug.Log(result);
        onUploadFileFinished?.Invoke(result);
    }

    #endregion

    #region 兑换码

    public event Action<string> onGiftCodeCallback = null;

    public virtual bool EnableGiftCode()
    {
        return false;
    }

    public virtual void SendGiftCode(string giftCode)
    {
        Debug.Log("SendGiftCode:" + giftCode);

        callback_gift_code_result(
            @"{""activity_id"":""TDS202407191731212FZ"",""c_sign"":""b1281a0e1bf871f0a1965b9dd9e15e21d8705249"",""content"":""[{\""name\"": \""Skin101\"", \""number\"": 1}, {\""name\"": \""Screw\"", \""number\"": 100}]"",""content_obj"":[{""name"":""Skin101"",""number"":1},{""name"":""Screw"",""number"":100}],""custom"":{},""error"":0,""nonce_str"":""KEP"",""sign"":""e8477bec508ba4f2554984eb2654d7aa87aedd34"",""success"":true,""timestamp"":1721381587}");
    }

    // {"activity_id":"TDS20240719134110664","c_sign":"f083e7ad183880c8cc934a1b791f1f4851a734db","content":"[{\"name\": \"Sugar\", \"number\": 10}]","content_obj":[{"name":"Sugar","number":10}],"custom":{},"error":0,"nonce_str":"KE7","sign":"40666f86354e7e285a003d5527aa41b8818d95d0","success":true,"timestamp":1721377967}
    // {"error":100025,"message":"礼包码已经兑换过了","info":{"dev_message":"max_num_limit","hint":"兑换码兑换次数已达上限"}}
    // {"activity_id":"TDS202407191731212FZ","c_sign":"b1281a0e1bf871f0a1965b9dd9e15e21d8705249","content":"[{\"name\": \"Skin101\", \"number\": 1}, {\"name\": \"Screw\", \"number\": 100}]","content_obj":[{"name":"Skin101","number":1},{"name":"Screw","number":100}],"custom":{},"error":0,"nonce_str":"KEP","sign":"e8477bec508ba4f2554984eb2654d7aa87aedd34","success":true,"timestamp":1721381587}
    protected void callback_gift_code_result(string result)
    {
        onGiftCodeCallback?.Invoke(result);
    }

    #endregion

    #region 排行榜

    public event Action<string> onLeaderBoardCallback = null;
    public event Action<string> onMyScoreCallback = null;
    public event Action<string> onSubmitScoreCallback = null;

    public virtual bool EnableLeaderBoard()
    {
        return false;
    }

    public virtual void ShowLeaderBoard(string leaderboardName)
    {
        Debug.Log("ShowLeaderBoard");
    }

    public virtual void ShowMyScore(string leaderboardName)
    {
        Debug.Log("ShowMyScore");
    }

    public virtual void SubmitScore(string leaderboardName, int score)
    {
    }

    public void callback_leader_board_result(string result)
    {
        onLeaderBoardCallback?.Invoke(result);
    }

    public void callback_my_score_result(string result)
    {
        onMyScoreCallback?.Invoke(result);
    }

    public void callback_submit_score_result(string result)
    {
        onSubmitScoreCallback?.Invoke(result);
    }

    #endregion

    #region 成就

    public virtual bool EnableAchievement()
    {
        return false;
    }

    public virtual void ReachAchievement(string displayID)
    {
        
    }

    public virtual void SetSteps(string displayID, int steps)
    {
        
    }
    public virtual void AddSteps(string displayID, int steps)
    {
        
    }
    #endregion

    #endregion

    #region nav-callback

    #region 广告回调

    /// <summary>
    /// banner点击回调
    /// </summary>
    protected void callback_bannerWasClicked(string tag)
    {
        Debug.Log("NAV-callback_bannerWasClicked" + tag);
        if (onBannerWasClicked != null)
            onBannerWasClicked();
    }

    /// <summary>
    /// 获得激励视频奖励-成功
    /// </summary>
    protected void callback_didCompleteAdWithTag(string tag)
    {
        Debug.Log("NAV-callback_didCompleteAdWithTag" + tag);
        HasGettedADRewards = true;
        if (onDidCompleteAdRewards != null)
            onDidCompleteAdRewards();
    }

    /// <summary>
    /// 获得激励视频奖励-失败
    /// </summary>
    protected void callback_didFailToCompleteAdWithTag(string tag)
    {
        Debug.Log("NAV-callback_didFailToCompleteAdWithTag" + tag);
        HasGettedADRewards = false;
        if (onDidCompleteAdRewardsFailed != null)
            onDidCompleteAdRewardsFailed();
    }


    float saveTimeScale = 1.0f;

    /// <summary>
    /// 显示了全屏广告
    /// </summary>
    protected void callback_didShowAdWithTag(string tag)
    {
        Debug.Log("NAV-callback_didShowAdWithTag" + tag);
        HasGettedADRewards = false;
        AudioListener.volume = 0;
        saveTimeScale = Time.timeScale;
        Time.timeScale = 0;
        if (onDidShowAd != null)
            onDidShowAd();
    }

    /// <summary>
    /// 关闭了全屏广告
    /// </summary>
    protected void callback_didHideAdWithTag(string tag)
    {
        Debug.Log("NAV-callback_didHideAdWithTag" + tag);
        AudioListener.volume = 1;
        Time.timeScale = saveTimeScale;
        if (onDidHideAd != null)
            onDidHideAd();
    }

    /// <summary>
    /// 点击了全屏广告
    /// </summary>
    protected void callback_didClickAdWithTag(string tag)
    {
        Debug.Log("NAV-callback_didClickAdWithTag" + tag);
        if (onADWasClicked != null)
            onADWasClicked();
    }

    #endregion

    #region 内购回调

    //恢复购买失败
    protected void callback_restoreFailed()
    {
        Debug.Log("NAV-callback_restoreFailed");
        onIapRestoreFailed?.Invoke();
    }

    //完成恢复购买
    protected void callback_restoreFinished()
    {
        Debug.Log("NAV-callback_restoreFinished");
        onIapRestoreFinished?.Invoke();
    }

    //完成一个恢复购买
    protected void callback_restoreTransaction(string productIdentifier)
    {
        Debug.Log("NAV-callback_restoreTransaction:" + productIdentifier);
        PlayerPrefs.SetInt(productIdentifier, 1);
        PlayerPrefs.Save();
        onIapRestored?.Invoke(productIdentifier);
    }

    //成功购买
    protected void callback_completeTransaction(string receipt)
    {
        Debug.Log("NAV-callback_completeTransaction: [" + LastIapProductIdentifier + "] " + receipt);
        PlayerPrefs.SetInt(LastIapProductIdentifier, 1);
        PlayerPrefs.Save();
        onIapPurchased?.Invoke(true, LastIapProductIdentifier, receipt);
    }

    //购买失败
    protected void callback_failedTransaction()
    {
        Debug.Log("NAV-callback_failedTransaction");
        onIapPurchased?.Invoke(false, LastIapProductIdentifier, "");
    }

    //更新商品列表
    protected void callback_updateProductIdentifiers(string productIdentifiers)
    {
        Debug.Log("NAV-callback_updateProductIdentifiers");
        string[] ss = productIdentifiers.Split(new string[] { "#%#" }, StringSplitOptions.None);
        
         LitJson.JsonData root = LitJson.JsonMapper.ToObject(localIapProductInfoJson);
         for (int i = 0; i < ss.Length; i += 4)
         {
             foreach (LitJson.JsonData node in root)
             {
                 if (node["productId"].ToString() == ss[i])
                 {
                     node["price"] = ss[i + 1];
                     node["title"] = ss[i + 2];
                     node["description"] = ss[i + 3];
                 }
             }
         }

        File.WriteAllText(NetIapProductInfoFilePath, root.ToJson(), System.Text.UTF8Encoding.UTF8);
        mIapProdcutDirty = true;
        mIapProdcutMapDirty = true;
        onIapProductUpdated?.Invoke();
    }

    protected void callback_subscription(string expireDate)
    {
        DateTime expireDate2 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        expireDate2 = expireDate2.AddSeconds(long.Parse(expireDate)).ToLocalTime();
        Debug.Log("expireDate:" + expireDate + " " + expireDate2);
        PlayerPrefs.SetString("subscriptionExpiredTime", expireDate);
        onIapSubscriptionUpdated?.Invoke();
    }

    #endregion

    #region 登录回调

    #region Facebook登录回调

    //登录Facebook成功
    protected void callback_completeFacebookLogin(string userId)
    {
        Debug.Log("NAV-callback_completeFacebookLogin");
        if (onFacebookLogin != null)
            onFacebookLogin(true, userId);
    }

    //登录Facebook失败
    protected void callback_failedFacebookLogin()
    {
        Debug.Log("NAV-callback_failedFacebookLogin");
        if (onFacebookLogin != null)
            onFacebookLogin(false, "");
    }

    #endregion

    #endregion

    #region InstallShortcut回调

    protected void callback_didInstallShortcut()
    {
        onInstallShortcutSuccess?.Invoke();
    }

    #endregion

    #endregion

    /// <summary>
    /// ͬ����ȡStreamingAssets��Դ
    /// </summary>
    /// <param name="path">StreamingAssets�����·��</param>
    /// <returns>�ɹ����ض�Ӧ���ַ������ݣ����򷵻�null</returns>
    static string ReadStreamingAssets(string path)
    {
#if (UNITY_ANDROID && !UNITY_EDITOR)
        path = System.IO.Path.Combine(Application.streamingAssetsPath, Uri.EscapeUriString(path));
#else
        path = System.IO.Path.Combine("file://" + Application.streamingAssetsPath, Uri.EscapeUriString(path));
#endif
        string result = null;
        using (var www = new WWW(path))
        {
            while (true)
            {
                if (www.error != null)
                {
                    break;
                }
                else
                {
                    if (www.isDone)
                    {
                        result = www.text;
                        if (result == "") result = null;
                        break;
                    }
                }
            }
        }

        return result;
    }
    #endregion
}