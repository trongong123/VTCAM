using EQX.Core.Recipe;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrontCameraAssembleEquipment.Defines.Recipes
{
    public class DevRecipe : RecipeBase
    {
        private readonly IConfiguration _configuration;
        private bool isRaiseWarningRemoveSpongeFail;
        private bool useSpongeSensorCheck;
        private bool useRetryPickFail;
        private bool useSpongeVacCheck;
        private bool useAGV;
        private bool useOriginalSpongeRemove;
        private bool useVIPRunMode;
        private bool useRetryBarcodeScan;
        private bool useBarcodeScanAll;
        private bool useCamPrealignCheckAfterRemoveSponge;
        private bool useRetryRemoveSponge = true;
        private bool useVinylDetachCheck = true;
        private bool isUseRollerIOControl;

        private string recipeFolder => _configuration.GetValue<string>("Folders:RecipeFolder") ?? "";

        public DevRecipe(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [SingleRecipeDescription(Description = "Raise Warning Remove Sponge Fail", Detail = "Check to enable Raise Warning Remove Sponge Fail")]
        public bool IsRaiseWarningRemoveSpongeFail
        {
            get { return isRaiseWarningRemoveSpongeFail; }
            set { isRaiseWarningRemoveSpongeFail = value; OnPropertyChanged(); }
        }

        [SingleRecipeDescription(Description = "Use Sponge Sensor Check", Detail = "Check to enable Sponge Remove Check")]
        public bool UseSpongeSensorCheck
        {
            get { return useSpongeSensorCheck; }
            set { useSpongeSensorCheck = value; OnPropertyChanged(); }
        }

        [SingleRecipeDescription(Description = "Use Retry pick Fail in Tray", Detail = "Use Retry pick Fail in Tray")]
        public bool UseRetryPickFail
        {
            get { return useRetryPickFail; }
            set { useRetryPickFail = value; OnPropertyChanged(); }
        }

        [SingleRecipeDescription(Description = "Use Sponge Vac Check", Detail = "Use Sponge Vac Check")]
        public bool UseSpongeVacCheck
        {
            get { return useSpongeVacCheck; }
            set { useSpongeVacCheck = value; OnPropertyChanged(); }
        }


        [SingleRecipeDescription(Description = "Use AGV", Detail = "Use AGV to Load/Unload Material")]
        public bool UseAGV
        {
            get { return useAGV; }
            set { useAGV = value; OnPropertyChanged(); }
        }

        [SingleRecipeDescription(Description = "* Use Original Sponge Remove", Detail = "Use Original Sponge Remove")]
        public bool UseOriginalSpongeRemove
        {
            get { return useOriginalSpongeRemove; }
            set { useOriginalSpongeRemove = value; OnPropertyChanged(); }
        }

        [SingleRecipeDescription(Description = "Run VIP Mode", Detail = "Enable to Run VIP Mode")]
        public bool UseVIPRunMode
        {
            get { return useVIPRunMode; }
            set { useVIPRunMode = value; OnPropertyChanged(); }
        }

        [SingleRecipeDescription(Description = "Retry Barcode Scan", Detail = "Enable to Retry Barcode Scan")]
        public bool UseRetryBarcodeScan
        {
            get { return useRetryBarcodeScan; }
            set { useRetryBarcodeScan = value; OnPropertyChanged(); }
        }
        [SingleRecipeDescription(Description = "Barcode Scan All", Detail = "Enable to Barcode Scan All")]
        public bool UseBarcodeScanAll
        {
            get { return useBarcodeScanAll; }
            set { useBarcodeScanAll = value; OnPropertyChanged(); }
        }
        [SingleRecipeDescription(Description = "Use Cam prealign Check", Detail = "Enable to use Cam prealign Check after remove sponge")]
        public bool UseCamPrealignCheckAfterRemoveSponge
        {
            get { return useCamPrealignCheckAfterRemoveSponge; }
            set { useCamPrealignCheckAfterRemoveSponge = value; OnPropertyChanged(); }
        }

        [SingleRecipeDescription(Description = "Use Retry Remove Sponge", Detail = "Enable to Use Retry Remove Sponge")]
        public bool UseRetryRemoveSponge
        {
            get { return useRetryRemoveSponge; }
            set { useRetryRemoveSponge = value; OnPropertyChanged(); }
        }

        [SingleRecipeDescription(Description = "Use Vinyl Detach Check", Detail = "Enable to Use Vinyl Detach Check")]
        public bool UseVinylDetachCheck
        {
            get { return useVinylDetachCheck; }
            set { useVinylDetachCheck = value; OnPropertyChanged(); }
        }

        [SingleRecipeDescription(Description = "Roller Use IO Control" , Detail ="Check to Use IO Control Roller")]
        public bool IsUseRollerIOControl
        {
            get { return isUseRollerIOControl; }
            set { isUseRollerIOControl = value; OnPropertyChanged(); }
        }

        public bool Load()
        {
            string devRecipeFile = Path.Combine(recipeFolder, "DevRecipe.json");
            if (File.Exists(devRecipeFile) == false)
            {
                var devRecipe = new DevRecipe(_configuration);
                PropertyInfo[] properties = this.GetType().GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        object sourceValue = property.GetValue(devRecipe, null);
                        property.SetValue(this, sourceValue);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                }
                return false;
            }

            string devRecipeFileContent = File.ReadAllText(devRecipeFile);

            try
            {
                var devRecipe = JsonConvert.DeserializeObject<DevRecipe>(devRecipeFileContent);

                PropertyInfo[] properties = this.GetType().GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        object sourceValue = property.GetValue(devRecipe, null);
                        property.SetValue(this, sourceValue);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void Save()
        {
            string currentRecipeFile = Path.Combine(recipeFolder, "DevRecipe.json");
            if (Directory.Exists(recipeFolder) == false)
                MessageBox.Show($" Recipe folder \"{recipeFolder}\" not found");

            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            string serializeStr = JsonConvert.SerializeObject(this, Formatting.Indented, settings);
            File.WriteAllText(currentRecipeFile, serializeStr);
        }
    }
}
