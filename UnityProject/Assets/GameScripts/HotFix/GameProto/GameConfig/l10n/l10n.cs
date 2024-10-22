
namespace GameConfig.l10n
{
    public sealed partial class localization : Luban.BeanBase
    {
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