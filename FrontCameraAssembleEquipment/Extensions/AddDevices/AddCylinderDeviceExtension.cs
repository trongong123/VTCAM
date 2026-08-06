using EQX.Core.InOut;
using EQX.InOut;
using FrontCameraAssembleEquipment.Defines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddCylinderDevicesExtension
    {
        public static IHostBuilder AddCylinderDevices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
#if SIMULATION
                services.AddSingleton<ICylinderFactory, SimulationCylinderFactory>();
#else
                services.AddSingleton<ICylinderFactory, CylinderFactory>();
#endif
                services.AddSingleton<Cylinders>();
            });

            return hostBuilder;
        }
    }

}
