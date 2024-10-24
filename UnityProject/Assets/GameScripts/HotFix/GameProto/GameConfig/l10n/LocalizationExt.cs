namespace GameConfig.l10n
{
    public sealed partial class Localization : Luban.BeanBase
    {
        public static string GetString(string key)
        {
            TEngine.TProfiler.BeginSample("GetString");
            var loc = ConfigSystem.Instance.Tables.TbLocalization.GetOrDefault(key);
            if (loc == null)
            {
                TEngine.Log.Warning("没有找到key：{0}", key);
                return key;
            }
            else
            {
                return loc.GetText();
            }
            TEngine.TProfiler.EndSample();
        }
        
        public string GetText()
        {
            switch (TEngine.GameModule.Localization.Language)
            {
                case TEngine.Language.English:
                    return En;
                case TEngine.Language.ChineseSimplified:
                case TEngine.Language.ChineseTraditional:
                    return Zh;
                case TEngine.Language.Japanese:
                    return Jp;
                default:
                    return En;
            }
        }
    }
}