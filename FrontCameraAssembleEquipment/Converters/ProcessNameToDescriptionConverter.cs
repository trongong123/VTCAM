using FrontCameraAssembleEquipment.Defines.Process;
using FrontCameraAssembleEquipment.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FrontCameraAssembleEquipment.Converters
{
    public class ProcessNameToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string name)
            {
                switch (name)
                {
                    case "TrayInCV":
                        return EProcess.TrayInCV.GetDescription();
                    case "TrayInElevator":
                        return EProcess.TrayInElevator.GetDescription();
                    case "TrayOutCV":
                        return EProcess.TrayOutCV.GetDescription();
                    case "TrayOutElevator":
                        return EProcess.TrayOutElevator.GetDescription();
                    case "TrayHead":
                        return EProcess.TrayHead.GetDescription();
                    case "SpongeDetach":
                        return EProcess.SpongeDetach.GetDescription();
                    case "CameraRotator":
                        return EProcess.CameraRotator.GetDescription();
                    case "RearSetCVIn":
                        return EProcess.RearSetCVIn.GetDescription();
                    case "RearSetCVOut":
                        return EProcess.RearSetCVOut.GetDescription();
                    case "RearSetCVDetach":
                        return EProcess.RearSetCVDetach.GetDescription();
                    case "RearSetCVAssemble":
                        return EProcess.RearSetCVAssemble.GetDescription();
                    case "FrontSetCVIn":
                        return EProcess.FrontSetCVIn.GetDescription();
                    case "FrontSetCVOut":
                        return EProcess.FrontSetCVOut.GetDescription();
                    case "FrontSetCVDetach":
                        return EProcess.FrontSetCVDetach.GetDescription();
                    case "FrontSetCVAssemble":
                        return EProcess.FrontSetCVAssemble.GetDescription();
                    case "FilmDetach":
                        return EProcess.FilmDetach.GetDescription();
                    case "CameraAssemble":
                        return EProcess.CameraAssemble.GetDescription();
                }
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
