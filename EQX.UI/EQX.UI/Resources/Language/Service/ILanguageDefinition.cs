using System;
using System.Collections.Generic;
namespace EQX.UI.Language
{
    public enum SupportedLanguage
    {
        English,
        Korean,
        Vietnamese,
        Chinese,
    }

    public interface ILanguageDefinition
    {
        SupportedLanguage Id { get; }
        string DisplayName { get; }
        string ResourcePath { get; }
        /// <summary>
        /// Application resource path extension for language resources.
        /// </summary>
        string ExtendResourcePath { get; set; }
        string CultureCode { get; }
        string FlagResourceKey { get; }
    }
}
