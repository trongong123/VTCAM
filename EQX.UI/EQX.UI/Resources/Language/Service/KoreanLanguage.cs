namespace EQX.UI.Language
{
    public class KoreanLanguage : ExtendResourcePathClass, ILanguageDefinition
    {
        public SupportedLanguage Id => SupportedLanguage.Korean;
        public string DisplayName => "한국어";
        public string ResourcePath => "/EQX.UI;component/Resources/Language/KoreanLanguage.xaml";
        public string CultureCode => "ko-KR";
        public string FlagResourceKey => "South_Korea_Flag";

        public KoreanLanguage(string extendResourcePath) : base(extendResourcePath) { }
        public override string ToString() => DisplayName;
    }
}
