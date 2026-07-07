using FrontCameraAssembleEquipment.Defines.Recipes;
using Microsoft.Extensions.DependencyInjection;

namespace FrontCameraAssembleEquipment.Defines.Recipes
{
    public class RecipeList
    {
        public RecipeList(GlobalRecipe globalRecipe, TraySuplierRecipe traySuplierRecipe, TrayHeadRecipe trayHeadRecipe, 
            FlipperTapeDetachRecipe flipperTapeDetachRecipe, FilmDetachHeadRecipe filmDetachHeadRecipe, 
            CameraHeadRecipe cameraHeadRecipe, SetConveyorRecipe setConveyorRecipe)
        {
            GlobalRecipe = globalRecipe;
            TraySuplierRecipe = traySuplierRecipe;
            TrayHeadRecipe = trayHeadRecipe;
            FlipperTapeDetachRecipe = flipperTapeDetachRecipe;
            FilmDetachHeadRecipe = filmDetachHeadRecipe;
            CameraHeadRecipe = cameraHeadRecipe;
            SetConveyorRecipe = setConveyorRecipe;

            GlobalRecipe.Name = "Common Recipe";
            TraySuplierRecipe.Name = "Tray Suplier Recipe";
            TrayHeadRecipe.Name = "Tray Head Recipe";
            FlipperTapeDetachRecipe.Name = "Rotator Sponge Detach Recipe";
            FilmDetachHeadRecipe.Name = "Film Detach Head Recipe";
            CameraHeadRecipe.Name = "Camera Head Recipe";
            SetConveyorRecipe.Name = " Set Conveyor Recipe";
        }

        public GlobalRecipe GlobalRecipe { get; }
        public TraySuplierRecipe TraySuplierRecipe { get; }
        public TrayHeadRecipe TrayHeadRecipe { get; }
        public FlipperTapeDetachRecipe FlipperTapeDetachRecipe { get; }
        public FilmDetachHeadRecipe FilmDetachHeadRecipe { get; }
        public CameraHeadRecipe CameraHeadRecipe { get; }
        public SetConveyorRecipe SetConveyorRecipe { get; }
    }
}
