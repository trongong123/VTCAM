using EQX.Core.Units;
using FrontCameraAssembleEquipment.Defines;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace FrontCameraAssembleEquipment.Converters
{
    public class TrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ITray<ETrayCellStatus>).IsAssignableFrom(objectType);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            Tray<ETrayCellStatus> tray = new Tray<ETrayCellStatus>("");
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
            try
            {
                tray.Cells = new ObservableCollection<ITrayCell<ETrayCellStatus>>();

                JToken token = JToken.Load(reader);
                if (token == null) return tray;

                ObservableCollection<TrayCell<ETrayCellStatus>> cells = token.ToObject<ObservableCollection<TrayCell<ETrayCellStatus>>>(JsonSerializer.Create(settings));

                if (cells == null) return tray;

                foreach (var cell in cells)
                {
                    tray.Cells.Add(cell);
                }
            }
            catch (Exception ex)
            {
                tray.Cells = new ObservableCollection<ITrayCell<ETrayCellStatus>>();
            }
            return tray;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is Tray<ETrayCellStatus> tray)
            {
                var settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                };
                serializer.Formatting = Formatting.Indented;
                serializer.TypeNameHandling = TypeNameHandling.Auto;
                serializer.Serialize(writer, tray.Cells);
            }
        }
    }
}
