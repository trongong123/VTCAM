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
                services.AddKeyedScoped<IDInputDevice>("InputDevice#1", (services, obj) => { return new SimulationInputDevice_ClientMMF<EInput>() { Id = 1, Name = "InDevice1", MaxPin = 128 }; });
#else
                services.AddKeyedScoped<IDInputDevice>("InputDevice#1", (services, obj) => { return new AjinInputDevice<EInput> { Id = 1, Name = "InDevice1", MaxPin = 128 }; });
#endif

#if SIMULATION
                services.AddKeyedScoped<IDOutputDevice>("OutputDevice#1", (services, obj) => { return new SimulationOutputDevice<EOutput>() { Id = 1, Name = "OutDevice1", MaxPin = 96 }; });
#else
                services.AddKeyedScoped<IDOutputDevice>("OutputDevice#1", (services, obj) => { return new AjinOutputDevice<EOutput> { Id = 1, Name = "OutDevice1", MaxPin = 96 }; });

#endif

                services.AddSingleton<Inputs>();
                services.AddSingleton<Outputs>();
            });

            return hostBuilder;
        }
    }


}
