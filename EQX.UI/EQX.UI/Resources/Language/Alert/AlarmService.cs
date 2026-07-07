using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EQX.UI.Language
{
    public class AlarmService<TEnum> : AlertService<TEnum> where TEnum : Enum
    {
        internal override string AlertLevel => "Alarm";

        public AlarmService(IConfiguration configuration, ILanguageService languageService)
            : base(configuration, languageService)
        {
            Update();
        }
    }
}
