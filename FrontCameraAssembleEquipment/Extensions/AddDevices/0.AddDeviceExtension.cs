using FrontCameraAssembleEquipment.Defines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddDeviceExtension
    {
        public static IHostBuilder AddDevices(this IHostBuilder hostBuilder)
        {
            hostBuilder.AddMotionDevices();
            hostBuilder.AddIODevices();
            hostBuilder.AddCylinderDevices();
            hostBuilder.AddSpeedControllerDevices();
            hostBuilder.AddCVIODevices();
            hostBuilder.AddBarcodeReader();

            hostBuilder.AddTrayList();

            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<AGV>();
                services.AddSingleton<ARM>();
                services.AddSingleton<InterlockService>();
            });

            return hostBuilder;
        }

    }
}
