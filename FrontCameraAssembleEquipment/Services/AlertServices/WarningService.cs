using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing;
using System.IO;
using EQX.UI.Language;

namespace FrontCameraAssembleEquipment.Services
{
    public class WarningService : IAlertService
    {
        #region Constructor(s)
        public WarningService()
        {
            ChangeCulture("English");
        }
        #endregion

        #region Public Method(s)
        public AlertModel GetById(int id)
        {
            AlertModel alertModel = alertModels.FirstOrDefault(t => t.Id == id);
            return alertModel;
        }

        public void ChangeCulture(string culture)
        {
            string filePath = $@"C:\\FA\\FRONTCAMASSEMBLY_C_V1\\CONTROL_CONFIG\\FrontCameraAssembleEquipment\\Alert\\Warning\\data_{culture}.json";
            if (!File.Exists(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }
            string content = File.ReadAllText(filePath);
            alertModels = JsonSerializer.Deserialize<List<AlertModel>>(content) ?? new List<AlertModel>();

            SyncWithEnum();
        }

        public void Update()
        {

        }
        #endregion

        #region Private(s)
        private List<AlertModel> alertModels;
        private readonly List<string> supportedCultures = new List<string> { "English", "Vietnamese"};

        private void SyncWithEnum()
        {
            var enumValues = Enum.GetValues(typeof(EWarning))
                                 .Cast<EWarning>()
                                 .ToList();

            foreach (var culture in supportedCultures)
            {
                string filePath = $@"C:\\FA\\FRONTCAMASSEMBLY_C_V1\\CONTROL_CONFIG\\FrontCameraAssembleEquipment\\Alert\\Warning\\data_{culture}.json";
                List<AlertModel> cultureSpecificAlerts;

                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    cultureSpecificAlerts = JsonSerializer.Deserialize<List<AlertModel>>(content) ?? new List<AlertModel>();
                }
                else
                {
                    cultureSpecificAlerts = new List<AlertModel>();
                }

                var updatedModels = new List<AlertModel>();

                for (int i = 0; i < enumValues.Count; i++)
                {
                    var enumValue = enumValues[i];
                    var enumId = (int)enumValue;

                    var existingModel = cultureSpecificAlerts.FirstOrDefault(t => t.Id == enumId);

                    if (existingModel != null)
                    {
                        existingModel.Message = enumValue.ToString();
                        updatedModels.Add(existingModel);
                    }
                    else
                    {
                        updatedModels.Add(new AlertModel
                        {
                            Id = enumId,
                            Message = enumValue.ToString(),
                            AlertOverviewSource = "/FrontCameraAssembleEquipment;component/Resources/Images/PictureName.jpg",
                            AlertOverviewHighlightRectangle = new Rectangle(0, 0, 0, 0),
                            AlertDetailviewSource = "/FrontCameraAssembleEquipment;component/Resources/Images/PictureName.jpg",
                            AlertDetailviewHighlightRectangle = new Rectangle(0, 0, 0, 0),
                            TroubleshootingSteps = new List<string> { "Fix problem" }
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

        
        #endregion
    }
}
