using EQX.Core.Common;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Converters;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing;
using EQX.UI.Language;

namespace FrontCameraAssembleEquipment.Services
{
    public class AlarmService : IAlertService
    {
        #region Contructor(s)
        public AlarmService()
        {
            ChangeCulture("English");
        }
        #endregion

        #region Public Method(s)
        public void ChangeCulture(string culture)
        {
            string filePath = $@"C:\\FA\\FRONTCAMASSEMBLY_C_V1\\CONTROL_CONFIG\\FrontCameraAssembleEquipment\\Alert\\Alarm\\data_{culture}.json";
            if (!File.Exists(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            string content = File.ReadAllText(filePath);
            alertModels = JsonSerializer.Deserialize<List<AlertModel>>(content) ?? new List<AlertModel>();

            SyncWithEnum();
        }

        public AlertModel GetById(int id)
        {
            AlertModel alertModel = alertModels.FirstOrDefault(t => t.Id == id);
            return alertModel;
        }

        public void Update()
        {
        }
        #endregion

        #region Private(s)
        private void SyncWithEnum()
        {
            var enumValues = Enum.GetValues(typeof(EAlarm))
                             .Cast<EAlarm>()
                             .ToList();

            foreach (var culture in supportedCultures)
            {
                string filePath = $@"C:\\FA\\FRONTCAMASSEMBLY_C_V1\\CONTROL_CONFIG\\FrontCameraAssembleEquipment\\Alert\\Alarm\\data_{culture}.json";
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
                            AlertOverviewSource = "/FrontCameraAssembleEquipment;component/Resource/Image/PictureName.png",
                            AlertOverviewHighlightRectangle = new Rectangle(0, 0, 0, 0),
                            AlertDetailviewSource = "/FrontCameraAssembleEquipment;component/Resource/Image/PictureName.png",
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

        private List<AlertModel> alertModels;
        private readonly List<string> supportedCultures = new List<string> { "English", "Vietnamese" };
        #endregion
    }

    public class RectangleConverter : JsonConverter<Rectangle>
    {
        public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<Rectangle>(ref reader, options);
        }

        public override void Write(Utf8JsonWriter writer, Rectangle value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);
            writer.WriteNumber("Width", value.Width);
            writer.WriteNumber("Height", value.Height);
            writer.WriteEndObject();
        }
    }
}
