using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace EQX.UI.Language
{
    public class WarningService<TEnum> : AlertService<TEnum> where TEnum : Enum
    {
        internal override string AlertLevel => "Warning";

        public WarningService(IConfiguration configuration, ILanguageService languageService)
            : base(configuration, languageService)
        {
            Update();
        }
    }
}
