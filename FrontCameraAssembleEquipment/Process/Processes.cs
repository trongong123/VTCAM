using EQX.Core.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Defines.Process;

namespace FrontCameraAssembleEquipment.Process
{
    public class Processes
    {
        public EMachineType MachineType => _processConfig.MachineType;
        public bool IsTwoConveyor => MachineType == EMachineType.TwoConveyor;
        public IProcess<ESequence> RootProcess => _processes.First(p=>p.Name == EProcess.Root.ToString());
        public IProcess<ESequence> TrayInCVProcess => _processes.First(p => p.Name == EProcess.TrayInCV.ToString());
        public IProcess<ESequence> TrayInElevatorProcess => _processes.First(p => p.Name == EProcess.TrayInElevator.ToString());
        public IProcess<ESequence> TrayOutCVProcess => _processes.First(p => p.Name == EProcess.TrayOutCV.ToString());
        public IProcess<ESequence> TrayOutElevatorProcess => _processes.First(p => p.Name == EProcess.TrayOutElevator.ToString());
        public IProcess<ESequence> TransferHeadProcess=> _processes.First(p => p.Name == EProcess.TrayHead.ToString());
        public IProcess<ESequence> SpongeDetachProcess => _processes.First(p => p.Name == EProcess.SpongeDetach.ToString());
        public IProcess<ESequence> CameraFlipperProcess => _processes.First(p => p.Name == EProcess.CameraRotator.ToString());
        public IProcess<ESequence> FilmDetachProcess => _processes.First(p => p.Name == EProcess.FilmDetach.ToString());
        public IProcess<ESequence> CameraAssembleProcess => _processes.First(p => p.Name == EProcess.CameraAssemble.ToString());
        public IProcess<ESequence> FrontCVSetLoadProcess => _processes.First(p => p.Name == EProcess.FrontSetCVIn.ToString());
        public IProcess<ESequence> FrontCVSetFilmDetachProcess => _processes.First(p => p.Name == EProcess.FrontSetCVDetach.ToString());
        public IProcess<ESequence> FrontCVSetCamAssembleProcess => _processes.First(p => p.Name == EProcess.FrontSetCVAssemble.ToString());
        public IProcess<ESequence> FrontCVSetUnloadProcess => _processes.First(p => p.Name == EProcess.FrontSetCVOut.ToString());
        public IProcess<ESequence> RearCVSetLoadProcess => _processes.First(p => p.Name == EProcess.RearSetCVIn.ToString());
        public IProcess<ESequence> RearCVSetFilmDetachProcess => _processes.First(p => p.Name == EProcess.RearSetCVDetach.ToString());
        public IProcess<ESequence> RearCVSetCamAssembleProcess => _processes.First(p => p.Name == EProcess.RearSetCVAssemble.ToString());
        public IProcess<ESequence> RearCVSetUnloadProcess => _processes.First(p => p.Name == EProcess.RearSetCVOut.ToString());

        public Processes(List<IProcess<ESequence>> processes, ProcessConfig processConfig)
        {
            _processes = processes;
            _processConfig = processConfig;
        }

        public void Initialize()
        {
            // Initialize the processes

            // Set the process hierarchy
            RootProcess.AddChild(TrayInCVProcess);
            RootProcess.AddChild(TrayInElevatorProcess);
            RootProcess.AddChild(TrayOutCVProcess);
            RootProcess.AddChild(TrayOutElevatorProcess);
            RootProcess.AddChild(TransferHeadProcess);
            RootProcess.AddChild(SpongeDetachProcess);
            RootProcess.AddChild(CameraFlipperProcess);
            RootProcess.AddChild(FilmDetachProcess);
            RootProcess.AddChild(CameraAssembleProcess);
            RootProcess.AddChild(FrontCVSetLoadProcess);
            RootProcess.AddChild(FrontCVSetFilmDetachProcess);
            RootProcess.AddChild(FrontCVSetCamAssembleProcess);
            RootProcess.AddChild(FrontCVSetUnloadProcess);
            if (IsTwoConveyor)
            {
                RootProcess.AddChild(RearCVSetLoadProcess);
                RootProcess.AddChild(RearCVSetFilmDetachProcess);
                RootProcess.AddChild(RearCVSetCamAssembleProcess);
                RootProcess.AddChild(RearCVSetUnloadProcess);
            }

            foreach (var process in RootProcess.Childs)
            {
                process.AlarmRaised += ((alarmId, alarmSource) =>
                {
                    RootProcess.RaiseAlarm(alarmId, alarmSource);
                });
                process.WarningRaised += ((wawrningId, wawrningSource) =>
                {
                    RootProcess.RaiseWarning(wawrningId, wawrningSource);
                });
            }

            ProcessesStart();
        }

        private void ProcessesStart()
        {
            RootProcess.Start();
            RootProcess.Childs?.All(p => p.Start());
        }

        #region Privates
        private readonly List<IProcess<ESequence>> _processes;
        private readonly ProcessConfig _processConfig;
        #endregion
    }
}
