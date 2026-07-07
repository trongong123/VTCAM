namespace EQX.UI.Language
{
    public class ChineseLanguage : ExtendResourcePathClass, ILanguageDefinition
    {
        public SupportedLanguage Id => SupportedLanguage.Chinese;
        public string DisplayName => "中文";
        public string ResourcePath => "/EQX.UI;component/Resources/Language/ChineseLanguage.xaml";
        public string CultureCode => "zh-CN";
        public string FlagResourceKey => "China_Flag";

        public ChineseLanguage(string extendResourcePath) : base(extendResourcePath) { }
        public override string ToString() => DisplayName;
    }
}
