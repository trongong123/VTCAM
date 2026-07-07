using EQX.Core.Communication.Modbus;
using EQX.Core.Device.SpeedController;
using EQX.Core.InOut;
using EQX.Core.InOut.Conveyor;
using EQX.Core.Motion;
using EQX.Device.SpeedController;
using EQX.InOut;
using EQX.InOut.InOut;
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
