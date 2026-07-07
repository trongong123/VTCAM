namespace EQX.UI.Language
{
    public class EnglishLanguage : ExtendResourcePathClass, ILanguageDefinition
    {
        public SupportedLanguage Id => SupportedLanguage.English;
        public string DisplayName => "English";
        public string ResourcePath => "/EQX.UI;component/Resources/Language/EnglishLanguage.xaml";
        public string CultureCode => "en-US";
        public string FlagResourceKey => "USA_Flag";

        public EnglishLanguage(string extendResourcePath) : base(extendResourcePath) { }
        public override string ToString() => DisplayName;
    }
}
