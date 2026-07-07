using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EQX.UI.Language
{
    public abstract class AlertService<TEnum> : IAlertService where TEnum : Enum
    {
        internal virtual string AlertLevel { get; }
        internal string FilePath => AlertInfoFilePath + $"\\{AlertLevel}.{_languageService.CurrentLanguage.CultureCode}.json";

        protected AlertService(IConfiguration configuration, ILanguageService languageService)
        {
            _configuration = configuration;
            _languageService = languageService;
        }

        public AlertModel GetById(int id)
        {
            AlertModel alertModel = alertModels.FirstOrDefault(t => t.Id == id);
            return alertModel;
        }

        public void Update()
        {
            if (!File.Exists(FilePath))
            {
                File.WriteAllText(FilePath, string.Empty);
            }
            string content = File.ReadAllText(FilePath);
            try
            {
                alertModels = JsonSerializer.Deserialize<List<AlertModel>?>(content);
            }
            catch
            {
                alertModels = new List<AlertModel>();
            }

            SyncWithEnum();
        }

        private void SyncWithEnum()
        {
            var enumValues = Enum.GetValues(typeof(TEnum))
                                 .Cast<TEnum>()
                                 .ToList();

            foreach (var language in _languageService.AvailableLanguages)
            {
                string filePath = AlertInfoFilePath + $"\\{AlertLevel}.{language.CultureCode}.json";
                List<AlertModel> cultureSpecificAlerts;

                try
                {
                    if (File.Exists(filePath))
                    {
                        string content = File.ReadAllText(filePath);
                        cultureSpecificAlerts = JsonSerializer.Deserialize<List<AlertModel>>(content) ?? new List<AlertModel>();
                    }
                    else
                    {
                        cultureSpecificAlerts = new List<AlertModel>();
                    }
                }
                catch
                {
                    cultureSpecificAlerts = new List<AlertModel>();
                }
                
                var updatedModels = new List<AlertModel>();

                for (int i = 0; i < enumValues.Count; i++)
                {
                    var enumValue = enumValues[i];
                    var enumId = Convert.ToInt32(enumValue);
                    var enumDescription = GetEnumDescription(enumValue);

                    var existingModel = cultureSpecificAlerts.FirstOrDefault(t => t.Id == enumId);

                    if (existingModel != null)
                    {
                        if (!string.IsNullOrWhiteSpace(enumDescription))
                        {
                            existingModel.IOName = enumDescription;
                        }

                        //existingModel.Message = enumValue.ToString();
                        updatedModels.Add(existingModel);
                    }
                    else
                    {
                        updatedModels.Add(new AlertModel
                        {
                            Id = enumId,
                            Message = enumValue.ToString(),
                            AlertOverviewSource = $"/EQX.UI;component/Resources/Images/TopPng.png",
                            AlertOverviewHighlightRectangle = new Rectangle(0, 0, 0, 0),
                            AlertDetailviewSource = $"/EQX.UI;component/Resources/Images/TopPng.png",
                            AlertDetailviewHighlightRectangle = new Rectangle(0, 0, 0, 0),
                            TroubleshootingSteps = new List<string> { "Fix problem" },
                            IOName = enumDescription
                        });
                    }
                }

                cultureSpecificAlerts = updatedModels;

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Converters = { new RectangleConverter() }
                };

                string updatedContent = JsonSerializer.Serialize(cultureSpecificAlerts, options);
                File.WriteAllText(filePath, updatedContent);
            }
        }

        private string GetEnumDescription(TEnum enumValue)
        {
            string enumName = enumValue.ToString();
            MemberInfo[] memberInfo = typeof(TEnum).GetMember(enumName);
            if (memberInfo.Length == 0)
            {
                return string.Empty;
            }

            DescriptionAttribute? descriptionAttribute = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();
            return descriptionAttribute?.Description ?? string.Empty;
        }

        internal readonly IConfiguration _configuration;
        internal readonly ILanguageService _languageService;
        internal string AlertInfoFilePath => _configuration["Folders:AlertInfoFolder"];
        internal List<AlertModel> alertModels;
    }
}
