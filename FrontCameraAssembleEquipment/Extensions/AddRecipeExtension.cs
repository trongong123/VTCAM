using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Defines.Units;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddRecipeExtension
    {
        public static IHostBuilder AddRecipes(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<GlobalRecipe>();
                services.AddSingleton<TraySuplierRecipe>();
                services.AddSingleton<TrayHeadRecipe>();
                services.AddSingleton<FlipperTapeDetachRecipe>();
                services.AddSingleton<FilmDetachHeadRecipe>();
                services.AddSingleton<CameraHeadRecipe>();
                services.AddSingleton<SetConveyorRecipe>();
                services.AddSingleton<DevRecipe>();

                services.AddSingleton<RecipeList>();
                services.AddSingleton<RecipeSelector>();

                services.AddSingleton<PositionList>();
                services.AddSingleton<AxisUnitList>();
            });
            return hostBuilder;
        }
    }
}
