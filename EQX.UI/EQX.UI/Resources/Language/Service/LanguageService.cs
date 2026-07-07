﻿using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace EQX.UI.Language
{
    public class LanguageService : ObservableObject, ILanguageService
    {
        private readonly Dictionary<SupportedLanguage, ILanguageDefinition> _languages;
        private ILanguageDefinition _currentLanguage;

        private ResourceDictionary _currentMainLangDict;
        private ResourceDictionary _currentExtendLangDict;

        public IEnumerable<ILanguageDefinition> AvailableLanguages => _languages.Values;

        public ILanguageDefinition CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                if (SetProperty(ref _currentLanguage, value)) // Giả sử ObservableObject có SetProperty
                {
                    ApplyLanguage(value);
                }
            }
        }

        public LanguageService(IEnumerable<ILanguageDefinition> languages)
        {
            _languages = languages.ToDictionary(l => l.Id);
            _currentLanguage = _languages[SupportedLanguage.English];
        }

        public void SwitchLanguage(SupportedLanguage languageId)
        {
            if (_languages.TryGetValue(languageId, out var language))
            {
                CurrentLanguage = language;
            }
        }

        private void ApplyLanguage(ILanguageDefinition language)
        {
            try
            {
                var app = Application.Current;
                if (app?.Resources?.MergedDictionaries == null) return;

                // 1. Xóa các Resource Dictionary của ngôn ngữ hiện tại một cách an toàn
                if (_currentMainLangDict != null)
                {
                    app.Resources.MergedDictionaries.Remove(_currentMainLangDict);
                }
                if (_currentExtendLangDict != null)
                {
                    app.Resources.MergedDictionaries.Remove(_currentExtendLangDict);
                }

                // 2. Thêm Resource mới
                _currentMainLangDict = new ResourceDictionary
                {
                    Source = new Uri(language.ResourcePath, UriKind.RelativeOrAbsolute)
                };
                _currentExtendLangDict = new ResourceDictionary
                {
                    Source = new Uri(language.ExtendResourcePath, UriKind.RelativeOrAbsolute)
                };
                app.Resources.MergedDictionaries.Add(_currentMainLangDict);
                app.Resources.MergedDictionaries.Add(_currentExtendLangDict);

                // 3. Cập nhật Culture
                var culture = new CultureInfo(language.CultureCode);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;

                // Lưu ý: OverrideMetadata chỉ nên gọi 1 lần duy nhất khi khởi tạo App 
                // hoặc dùng cách tiếp cận khác cho FrameworkElement.LanguageProperty
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Language Switch Error: {ex.Message}");
            }
        }
    }
}
