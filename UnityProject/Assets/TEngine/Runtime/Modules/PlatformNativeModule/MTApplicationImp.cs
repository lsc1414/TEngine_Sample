using UnityEngine;
public class MTApplicationImp :
#if UNITY_EDITOR || UNITY_STANDALONE
        MTApplicationStandaloneImp
#elif UNITY_WEBGL
        MTApplicationWebGLImp
#elif UNITY_ANDROID && !LionStudios && !VOODOO && !Supersonic && !Tapnation && !Smillage && !supersonicLite && !DBT
        MTApplicationAndroidImp
#elif UNITY_ANDROID && (LionStudios || VOODOO || Smillage || supersonicLite)
        MTApplicationLionAndroidImp
#elif UNITY_ANDROID && DBT
        MTApplicationDBTAndroidImp
#elif UNITY_ANDROID && Supersonic
        MTApplicationSuperSonicAndroidImp
#elif UNITY_IPHONE && !Supersonic && !Tapnation
        MTApplicationiOSImp
#elif UNITY_IPHONE && Supersonic
        MTApplicationSuperSoniciOSImp
#elif UNITY_ANDROID && Tapnation
        MTApplicationTapnationAndroidImp
#elif UNITY_IPHONE && Tapnation
        MTApplicationTapnationiOSImp
#endif
{

}
