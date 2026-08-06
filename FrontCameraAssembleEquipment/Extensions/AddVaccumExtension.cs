using FrontCameraAssembleEquipment.Defines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddVaccumExtension
    {
        public static IHostBuilder AddVaccum(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<VaccumList>();
            });

            return hostBuilder;
        }
    }
}
