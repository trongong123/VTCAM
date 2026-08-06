using FrontCameraAssembleEquipment.Vision;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddVisionExtension
    {
        public static IHostBuilder AddVisionServices(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<CVision_FrontCamera>();
                services.AddSingleton<VisionProcess>();
                services.AddSingleton<VisionResultList>();
            });
            return hostBuilder;
        }
    }
}
