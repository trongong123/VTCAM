using EQX.Core.InOut;
using EQX.InOut;
using EQX.InOut.InOut.Analog;
using FrontCameraAssembleEquipment.Defines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddIODevicesExtension
    {
        public static IHostBuilder AddIODevices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
#if SIMULATION
                services.AddKeyedScoped<IDInputDevice>("InputDevice#1", (services, obj) =>
                {
                    return services.GetRequiredService<ProcessConfig>().MachineType == FrontCameraAssembleEquipment.Defines.EMachineType.OneConveyor
                        ? new SimulationInputDevice_ClientMMF<EInput1CV>() { Id = 1, Name = "InDevice1", MaxPin = 128 }
                        : new SimulationInputDevice_ClientMMF<EInput2CV>() { Id = 1, Name = "InDevice1", MaxPin = 128 };
                });
#else
                services.AddKeyedScoped<IDInputDevice>("InputDevice#1", (services, obj) =>
                {
                    return services.GetRequiredService<ProcessConfig>().MachineType == FrontCameraAssembleEquipment.Defines.EMachineType.OneConveyor
                        ? new AjinInputDevice<EInput1CV> { Id = 1, Name = "InDevice1", MaxPin = 128 }
                        : new AjinInputDevice<EInput2CV> { Id = 1, Name = "InDevice1", MaxPin = 128 };
                });
#endif

#if SIMULATION
                services.AddKeyedScoped<IDOutputDevice>("OutputDevice#1", (services, obj) =>
                {
                    return services.GetRequiredService<ProcessConfig>().MachineType == FrontCameraAssembleEquipment.Defines.EMachineType.OneConveyor
                        ? new SimulationOutputDevice<EOutput1CV>() { Id = 1, Name = "OutDevice1", MaxPin = 96 }
                        : new SimulationOutputDevice<EOutput2CV>() { Id = 1, Name = "OutDevice1", MaxPin = 96 };
                });
#else
                services.AddKeyedScoped<IDOutputDevice>("OutputDevice#1", (services, obj) =>
                {
                    return services.GetRequiredService<ProcessConfig>().MachineType == FrontCameraAssembleEquipment.Defines.EMachineType.OneConveyor
                        ? new AjinOutputDevice<EOutput1CV> { Id = 1, Name = "OutDevice1", MaxPin = 96 }
                        : new AjinOutputDevice<EOutput2CV> { Id = 1, Name = "OutDevice1", MaxPin = 96 };
                });

#endif

                services.AddSingleton<Inputs>();
                services.AddSingleton<Outputs>();
            });

            return hostBuilder;
        }
    }


}
