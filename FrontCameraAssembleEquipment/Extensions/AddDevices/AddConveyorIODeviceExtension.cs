using EQX.Core.InOut.Conveyor;
using EQX.InOut;
using FrontCameraAssembleEquipment.Defines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddCVIODeviceExtension
    {
        public static IHostBuilder AddCVIODevices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<IConveyorFactory, ConveyorFactory>();
                services.AddSingleton<CVs>();
            });

            return hostBuilder;
        }
    }
}
