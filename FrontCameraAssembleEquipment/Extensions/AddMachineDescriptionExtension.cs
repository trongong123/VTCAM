using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.ProductDatas;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Resources.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.IO;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddMachineDescriptionExtension
    {
        public static IHostBuilder AddMachineDescriptions(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<Information>();
                services.AddSingleton<MachineStatus>();
                services.AddSingleton<MaterialStatusList>();
                services.AddSingleton<EDMLogger>();
                services.AddSingleton<TotalTackTime>();

                services.AddSingleton<SystemConfig>((ser) =>
                {
                    var configuration = ser.GetRequiredService<IConfiguration>();

                    string systemConfigFilePath = configuration["Files:SystemConfigFile"] ?? "";
                    string configFolder = Path.GetDirectoryName(systemConfigFilePath);
                    if (File.Exists(systemConfigFilePath) == false && configFolder != null)
                    {
                        Directory.CreateDirectory(configFolder);
                        File.Create(systemConfigFilePath).Dispose();
                    }

                    var systemConfig = JsonConvert.DeserializeObject<SystemConfig>(File.ReadAllText(systemConfigFilePath));

                    if (systemConfig == null)
                    {
                        var settings = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        };
                        systemConfig = new SystemConfig();
                        File.WriteAllText(systemConfigFilePath,JsonConvert.SerializeObject(systemConfig, Formatting.Indented, settings));
                    }
                    
                    return systemConfig;
                });
            });

            return hostBuilder;
        }
    }
}
