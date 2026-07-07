using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.InOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FrontCameraAssembleEquipment.Defines
{
    public class Vaccum : ObservableObject
    {
        public Vaccum(IDInput dInput)
        {
            VacOnInput = dInput;
            if (VacOnInput != null)
            {
                VacOnInput.ValueUpdated += VacOnInput_ValueUpdated;
            }
        }

        private void VacOnInput_ValueUpdated(object? sender, EventArgs e)
        {
            OnPropertyChanged(nameof(IsVaccumOn));
            OnPropertyChanged(nameof(IsVaccumOff));
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public IDInput VacOnInput { get; set; }

        public IDOutput VacOnOutput { get; set; }
        public IDOutput VacOffOutput { get; set; }

        public bool IsVaccumOn 
        {
            get => VacOnInput.Value;
        }
        public bool IsVaccumOff
        {
            get => !VacOnInput.Value;
        }

        public void VaccumOn()
        {
            VacOffOutput.Value = false;
            VacOnOutput.Value = true;
        }

        public async void VaccumOff()
        {
            VacOnOutput.Value = false;
            VacOffOutput.Value = true;
            await Task.Delay(500);
            VacOffOutput.Value = false;
        }
    }
}
