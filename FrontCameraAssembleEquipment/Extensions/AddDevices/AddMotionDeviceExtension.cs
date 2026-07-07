using EQX.Core.Motion;
using EQX.Motion;
using EQX.Motion.ByVendor.Ajinextek;
using FrontCameraAssembleEquipment.Defines;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.IO;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddMotionDeviceExtension
    {
        public static IHostBuilder AddMotionDevices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<List<IMotionParameter>>((ser) =>
                {
                    var configuration = ser.GetRequiredService<IConfiguration>();

                    var motionParameters = JsonConvert.DeserializeObject<List<MotionAjinParameter>>(
                        File.ReadAllText(configuration["Files:MotionParaConfigFile"] ?? "")
                    );
                    if (motionParameters == null)
                    {
                        throw new FormatException("MotionParaConfigFile format error");
                    }

                    List<IMotionParameter> result = new List<IMotionParameter>();
                    foreach (var parameter in motionParameters)
                    {
                        result.Add(parameter);
                    }
                    return result;
                });

#if SIMULATION
                services.AddKeyedScoped<IMotionMaster, MotionMasterAjin>("AjinMaster#1", (ser, obj) =>
                {
                    return new MotionMasterAjin() { NumberOfDevices = Enum.GetNames(typeof(EMotion)).Length };
                });
                services.AddKeyedScoped<IMotionFactory<IMotion>, SimulationMotionFactory>("AjinMotionFactory");
#else
                services.AddKeyedScoped<IMotionMaster, MotionMasterAjin>("AjinMaster#1", (ser, obj) =>
                {
                    return new MotionMasterAjin() { NumberOfDevices = Enum.GetNames(typeof(EMotion)).Length };
                });
                services.AddKeyedScoped<IMotionFactory<IMotion>>("AjinMotionFactory", (ser, obj) =>
                {
                    return new MotionAjinFactory(ser.GetRequiredKeyedService<IMotionMaster>("AjinMaster#1"));
                });
#endif

                services.AddSingleton<Motions>();

                services.AddSingleton<Devices>();
            });

            return hostBuilder;
        }
    }
}
