using FrontCameraAssembleEquipment.Defines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddTrayListExtension
    {
        public static IHostBuilder AddTrayList(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<TrayList>();
            });

            return hostBuilder;
        }
    }
}
