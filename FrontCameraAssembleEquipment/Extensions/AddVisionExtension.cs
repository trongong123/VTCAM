using FrontCameraAssembleEquipment.Vision;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
