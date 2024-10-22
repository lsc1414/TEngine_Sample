using UnityEngine;
using Cysharp.Threading.Tasks;

namespace TEngine
{
    /// <summary>
    /// 本地化组件。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class LocalizationModule : Module
    {
        private string m_DefaultLanguage = "Chinese";
        private const string SettingLanguageKey = "language";

        private string m_CurrentLanguage;

        /// <summary>
        /// 获取或设置本地化语言。
        /// </summary>
        public Language Language
        {
            get { return DefaultLocalizationHelper.GetLanguage(m_CurrentLanguage); }
            set { SetLanguage(DefaultLocalizationHelper.GetLanguageStr(value)); }
        }

        /// <summary>
        /// 获取系统语言。
        /// </summary>
        public Language SystemLanguage => DefaultLocalizationHelper.SystemLanguage;

        private void Start()
        {
            RootModule rootModule = ModuleSystem.GetModule<RootModule>();
            if (rootModule == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }

            m_DefaultLanguage = DefaultLocalizationHelper.GetLanguageStr(
                rootModule.EditorLanguage != Language.Unspecified ? rootModule.EditorLanguage : SystemLanguage);

            // 如果已经设置语言，就用设置的语言。否则用系统语言
            m_DefaultLanguage = GameModule.Setting.GetString(SettingLanguageKey, m_DefaultLanguage);
            AsyncInit();
        }

        private void AsyncInit()
        {
            if (string.IsNullOrEmpty(m_DefaultLanguage))
            {
                Log.Fatal($"Must set defaultLanguage.");
                return;
            }

            SetLanguage(m_DefaultLanguage);
        }

        /// <summary>
        /// 设置当前语言。
        /// </summary>
        /// <param name="language">语言名称。</param>
        /// <returns></returns>
        public bool SetLanguage(string language)
        {
            if (m_CurrentLanguage == language)
            {
                return true;
            }

            Log.Info($"设置当前语言 = {language}");
            GameModule.Setting.SetString(SettingLanguageKey, language);
            m_CurrentLanguage = language;
            return true;
        }

        /// <summary>
        /// 设置当前语言
        /// </summary>
        /// <param name="language">语言枚举</param>
        /// <returns></returns>
        public bool SetLanguage(Language language)
        {
            var newLanguage = DefaultLocalizationHelper.GetLanguageStr(language);
            return SetLanguage(newLanguage);
        }
    }
}