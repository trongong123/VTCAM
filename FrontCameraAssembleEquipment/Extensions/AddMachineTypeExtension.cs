using System.IO;
using FrontCameraAssembleEquipment.Defines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddMachineTypeExtension
    {
        public static IHostBuilder AddMachineType(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton(serviceProvider =>
                {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    string processConfigFilePath = configuration["Files:ProcessConfigFile"] ?? string.Empty;
                    string? configFolder = Path.GetDirectoryName(processConfigFilePath);

                    if (string.IsNullOrWhiteSpace(processConfigFilePath) == false && File.Exists(processConfigFilePath) == false)
                    {
                        if (string.IsNullOrWhiteSpace(configFolder) == false)
                        {
                            Directory.CreateDirectory(configFolder);
                        }

                        File.WriteAllText(processConfigFilePath, JsonConvert.SerializeObject(new ProcessConfig(), Formatting.Indented));
                    }

                    if (string.IsNullOrWhiteSpace(processConfigFilePath) || File.Exists(processConfigFilePath) == false)
                    {
                        return new ProcessConfig();
                    }

                    ProcessConfig? processConfig = JsonConvert.DeserializeObject<ProcessConfig>(File.ReadAllText(processConfigFilePath));
                    return processConfig ?? new ProcessConfig();
                });
            });

            return hostBuilder;
        }
    }
}
