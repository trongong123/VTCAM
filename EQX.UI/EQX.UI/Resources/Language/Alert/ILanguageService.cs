using System.ComponentModel;

namespace EQX.UI.Language
{
    public interface ILanguageService : INotifyPropertyChanged
    {
        ILanguageDefinition CurrentLanguage { get; set; }
        IEnumerable<ILanguageDefinition> AvailableLanguages { get; }
        void SwitchLanguage(SupportedLanguage languageId);
    }
}
