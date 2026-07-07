using EQX.Core.Common;
using EQX.UI.Converters;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.MVVM.ViewModels;
using FrontCameraAssembleEquipment.Process;
using FrontCameraAssembleEquipment.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FrontCameraAssembleEquipment.Defines.ProductDatas;
using EQX.UI.Language;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddStoreExtension
    {
        public static IHostBuilder AddStores(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                //services.AddSingleton<UserStore>();
                services.AddSingleton<ICellColorRepository, CellColorRepository>();
                services.AddSingleton<CellStatusToColorConverter>();
                services.AddSingleton<Processes>();
                services.AddSingleton<CCountData>();
                services.AddSingleton<CWorkData>();
                services.AddSingleton<ProductionData>();

                services.AddKeyedScoped<IAlertService, AlarmService>("AlarmService");
                services.AddKeyedScoped<IAlertService, WarningService>("WarningService");

                services.AddKeyedScoped("BlinkTimer", (s, o) => { return new ActionAssignableTimer(500); });
                services.AddKeyedScoped("GripperTimer", (s, o) => { return new ActionAssignableTimer(200); });
            });

            return hostBuilder;
        }
    }
}
