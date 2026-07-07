using EQX.Core.Common;
using FrontCameraAssembleEquipment.Define;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Recipes;
using FrontCameraAssembleEquipment.Helpers;
//using Microsoft.Data.SqlClient;

namespace FrontCameraAssembleEquipment.MVVM.ViewModels
{
    public class ManualViewModel : ViewModelBase
    {
        #region Properties
        public IEnumerable<MaintenanceViewModel<ESemiSequence, RecipeList>> MaintenanceViewModels { get; }
        #endregion

        #region Methods
        #endregion

        #region Constructors
        public ManualViewModel(IEnumerable<MaintenanceViewModel<ESemiSequence, RecipeList>> maintenanceViewModels)
        {
            MaintenanceViewModels = maintenanceViewModels;

            for (int i = 0; i < MaintenanceViewModels.Count(); i++)
            {
                MaintenanceViewModels.ToList()[i].Name = (EUnit.TraySupplier + i).GetDescription();
            }

            foreach (var item in MaintenanceViewModels)
            {
                item.Init();
            }
        }


        public MaintenanceViewModel<ESemiSequence, RecipeList> TraySupplierManualViewModel => MaintenanceViewModels.First(m => m.Name == EUnit.TraySupplier.GetDescription());
        public MaintenanceViewModel<ESemiSequence, RecipeList> TrayCamLoaderManualViewModel => MaintenanceViewModels.First(m => m.Name == EUnit.TrayHead.GetDescription());
        public MaintenanceViewModel<ESemiSequence, RecipeList> CamSpongeDetachManualViewModel => MaintenanceViewModels.First(m => m.Name == EUnit.SpongeDetach.GetDescription());
        public MaintenanceViewModel<ESemiSequence, RecipeList> VinylDetachManualViewModel => MaintenanceViewModels.First(m => m.Name == EUnit.FilmDetach.GetDescription());
        public MaintenanceViewModel<ESemiSequence, RecipeList> CamAssembleManualViewModel => MaintenanceViewModels.First(m => m.Name == EUnit.CameraAssemble.GetDescription());
        public MaintenanceViewModel<ESemiSequence, RecipeList> ConveyorManualViewModel => MaintenanceViewModels.First(m => m.Name == EUnit.SetCV.GetDescription());
        public MaintenanceViewModel<ESemiSequence, RecipeList> VisionManualViewModel => MaintenanceViewModels.First(m => m.Name == EUnit.Vision.GetDescription());
        #endregion

        #region Privates
        #endregion
    }

}
