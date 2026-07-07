using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.InOut;
using EQX.Core.InOut.Conveyor;
using EQX.Core.InOut.Conveyor;
using EQX.InOut;
using FrontCameraAssembleEquipment.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace FrontCameraAssembleEquipment.Defines
{
    public class CVs: ObservableObject
    {
        // Tray Conveyor
        public IConveyor TrayIn_ExternalCv { get; }
        public IConveyor TrayOut_ExternalCv { get; }

        // SetWork Front
        public IConveyor FrontSetWorkCV_PreLoadCV { get; }
        public IConveyor FrontSetWorkCV_SetLoadInput { get; }
        public IConveyor FrontSetWorkCV_SetFilmDetach { get; }
        public IConveyor FrontSetWorkCV_SetCamAssemble { get; }
        public IConveyor FrontSetWorkCV_SetUnloadOutput { get; }

        // SetWork Rear
        public IConveyor RearSetWorkCV_PreLoadCV { get; }
        public IConveyor RearSetWorkCV_SetLoadInput { get; }
        public IConveyor RearSetWorkCV_SetFilmDetach { get; }
        public IConveyor RearSetWorkCV_SetCamAssemble { get; }
        public IConveyor RearSetWorkCV_SetUnloadOutput { get; }

        public IConveyor TrayInConveyor { get; }
        public IConveyor TrayInLiftConveyor { get; }
        public IConveyor TrayOutConveyor { get; }
        public IConveyor TrayOutLiftConveyor { get; }

        public CVs(IConveyorFactory conveyorFactory, Inputs inputs, Outputs outputs)
        {
            _conveyorFactory = conveyorFactory;
            _inputs = inputs;
            _outputs = outputs;

            TrayIn_ExternalCv = _conveyorFactory
                .Create(null, _outputs.TrayInExtCvRun, null, null)
                .SetIdentity((int)ECV.TraySup_TrayInExternal, ECV.TraySup_TrayInExternal.GetDescription());;

            TrayIn_ExternalCv = _conveyorFactory
                .Create(null, _outputs.TrayInExtCvRun, null, null)
                .SetIdentity((int)ECV.TraySup_TrayInExternal, ECV.TraySup_TrayInExternal.GetDescription());

            TrayOut_ExternalCv = _conveyorFactory
                .Create(null, _outputs.TrayOutExtCvRun, null, null)
                .SetIdentity((int)ECV.TraySup_TrayOutExternal, ECV.TraySup_TrayOutExternal.GetDescription());

            FrontSetWorkCV_PreLoadCV = _conveyorFactory
                .Create(null, _outputs.PreFrontCVRun, null, null)
                .SetIdentity((int)ECV.SetWork_FrontPreLoadCV, ECV.SetWork_FrontPreLoadCV.GetDescription());

            FrontSetWorkCV_SetLoadInput = _conveyorFactory
                .Create(null, _outputs.FrontLoadCvOn, null, null)
                .SetIdentity((int)ECV.SetWork_FrontSetLoadInput, ECV.SetWork_FrontSetLoadInput.GetDescription());
            FrontSetWorkCV_SetFilmDetach = _conveyorFactory
                .Create(null, _outputs.FrontDetachCvOn, null, null)
                .SetIdentity((int)ECV.SetWork_FrontSetFilmDetach, ECV.SetWork_FrontSetFilmDetach.GetDescription());
            FrontSetWorkCV_SetCamAssemble = _conveyorFactory
                .Create(null, _outputs.FrontAssembleCvOn, null, null)
                .SetIdentity((int)ECV.SetWork_FrontSetCamAssemble, ECV.SetWork_FrontSetCamAssemble.GetDescription());
            FrontSetWorkCV_SetUnloadOutput = _conveyorFactory
                .Create(null, _outputs.FrontUnloadCvOn, null, null)
                .SetIdentity((int)ECV.SetWork_FrontSetUnloadOutput, ECV.SetWork_FrontSetUnloadOutput.GetDescription());

            RearSetWorkCV_PreLoadCV = _conveyorFactory
                .Create(null, _outputs.PreRearCVRun, null, null)
                .SetIdentity((int)ECV.SetWork_RearPreLoadCV, ECV.SetWork_RearPreLoadCV.GetDescription());
            RearSetWorkCV_SetLoadInput = _conveyorFactory
                .Create(null, _outputs.RearLoadCvOn, null, null)
                .SetIdentity((int)ECV.SetWork_RearSetLoadInput, ECV.SetWork_RearSetLoadInput.GetDescription());
            RearSetWorkCV_SetFilmDetach = _conveyorFactory
                .Create(null, _outputs.RearDetachCvOn, null, null)
                .SetIdentity((int)ECV.SetWork_RearSetFilmDetach, ECV.SetWork_RearSetFilmDetach.GetDescription());
            RearSetWorkCV_SetCamAssemble = _conveyorFactory
                .Create(null, _outputs.RearAssembleCvOn, null, null)
                .SetIdentity((int)ECV.SetWork_RearSetCamAssemble, ECV.SetWork_RearSetCamAssemble.GetDescription());
            RearSetWorkCV_SetUnloadOutput = _conveyorFactory
                .Create(null, _outputs.RearUnloadCvOn, null, null)
                .SetIdentity((int)ECV.SetWork_RearSetUnloadOutput, ECV.SetWork_RearSetUnloadOutput.GetDescription());

            TrayInConveyor = conveyorFactory
                .Create(null,_outputs.TrayInCVRollerRun , null, null)
                .SetIdentity((int)ECV.TraySup_TrayInput , ECV.TraySup_TrayInput.GetDescription());
            TrayInLiftConveyor = conveyorFactory
                .Create(null, _outputs.TrayInLiftRollerRun, null, null)
                .SetIdentity((int)ECV.TraySup_TrayInElevator, ECV.TraySup_TrayInElevator.GetDescription());

            TrayOutConveyor = conveyorFactory
                .Create(null, _outputs.TrayOutCVRollerRun, null, null)
                .SetIdentity((int)ECV.TraySup_TrayOutput, ECV.TraySup_TrayOutput.GetDescription());

            TrayOutLiftConveyor = conveyorFactory
                .Create(null, _outputs.TrayOutLiftRollerRun, null, null)
                .SetIdentity((int)ECV.TraySup_TrayOutElevator, ECV.TraySup_TrayOutElevator.GetDescription());
        }

        private readonly IConveyorFactory _conveyorFactory;
        private readonly Inputs _inputs;
        private readonly Outputs _outputs;
    }
}
