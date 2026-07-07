using EQX.Core.Communication.Modbus;
using EQX.Device.SpeedController;
using EQX.Motion;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.IO;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddSpeedControllerDeviceExtension
    {
        public static IHostBuilder AddSpeedControllerDevices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<SerialCOMConfig>((services) =>
                {
                    var configuration = services.GetRequiredService<IConfiguration>();
                    var serialCOMConfigPath = configuration["Files:SerialCommunicationConfig"] ?? "";

                    if (File.Exists(serialCOMConfigPath) == false)
                    {
                        SerialCOMConfig serialCOMConfig = new SerialCOMConfig() { COMPort = "COM1", Baudrate = 38400 };
                        File.WriteAllText(serialCOMConfigPath, JsonConvert.SerializeObject(serialCOMConfig));
                    }

                    SerialCOMConfig serialComParameter = JsonConvert.DeserializeObject<SerialCOMConfig>(File.ReadAllText(serialCOMConfigPath));
                    if (serialComParameter == null)
                    {
                        throw new FormatException("SerialCommunicationConfig format error");
                    }

                    return serialComParameter;
                });

                services.AddKeyedScoped<IModbusCommunication>("RollerModbusCommunication", (services, obj) =>
                {
                    var serialCOMconfig = services.GetRequiredService<SerialCOMConfig>();

                    if (serialCOMconfig == null)
                    {
                        return new ModbusRTUCommunication("COM1", 38400);
                        throw new FormatException("MotionParaConfigFile format error");
                    }

                    return new ModbusRTUCommunication(serialCOMconfig.COMPort, serialCOMconfig.Baudrate);
                });

                services.AddSingleton<RolllerList>((ser) =>
                {
                    IModbusCommunication modbusCommunication = ser.GetRequiredKeyedService<IModbusCommunication>("RollerModbusCommunication");

                    var speedCtlList = Enum.GetNames(typeof(ESpeedController)).ToList();
                    var speedCtlIndex = (int[])Enum.GetValues(typeof(ESpeedController));

                    var speedcontrollerList = new List<BD201SRollerController>();

                    for (int i = 0; i < speedCtlList.Count; i++)
                    {
                        speedcontrollerList.Add(new BD201SRollerController(speedCtlIndex[i], speedCtlList[i], modbusCommunication));
                    }
                    return new RolllerList(speedcontrollerList, ser.GetRequiredService<RecipeList>(), ser.GetRequiredService<DevRecipe>());
                });
            });

            return hostBuilder;
        }
    }
}
