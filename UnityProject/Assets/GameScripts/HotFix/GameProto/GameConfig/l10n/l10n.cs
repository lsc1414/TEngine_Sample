using TEngine.Localization;
using UnityEngine.Device;

namespace GameConfig.l10n
{
    public sealed partial class textinfo : Luban.BeanBase
    {
        public string GetText()
        {
            switch (Application.systemLanguage)
            {
                case UnityEngine.SystemLanguage.English:
                    return En;
                case UnityEngine.SystemLanguage.Chinese:
                case UnityEngine.SystemLanguage.ChineseSimplified:
                case UnityEngine.SystemLanguage.ChineseTraditional:
                    return Zh;
                case UnityEngine.SystemLanguage.Japanese:
                    return Jp;
                default:
                    return En;
            }
        }
    }
}