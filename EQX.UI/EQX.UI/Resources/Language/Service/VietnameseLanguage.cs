namespace EQX.UI.Language
{
    public class VietnameseLanguage : ExtendResourcePathClass, ILanguageDefinition
    {
        public SupportedLanguage Id => SupportedLanguage.Vietnamese;
        public string DisplayName => "Tiếng Việt";
        public string ResourcePath => "/EQX.UI;component/Resources/Language/VietnameseLanguage.xaml";
        public string CultureCode => "vi-VN";
        public string FlagResourceKey => "Vietnam_Flag";

        public VietnameseLanguage(string extendResourcePath) : base(extendResourcePath) { }
        public override string ToString() => DisplayName;
    }
}
