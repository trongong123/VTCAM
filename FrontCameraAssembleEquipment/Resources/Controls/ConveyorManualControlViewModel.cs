using EQX.Core.Common;
using EQX.Core.InOut;
using EQX.Core.Process;
using EQX.Device.SpeedController;
using FrontCameraAssembleEquipment.Defines;
using FrontCameraAssembleEquipment.Process;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    public class ConveyorManualControlViewModel: ViewModelBase
    {
        public string Name { get; set; }

        public ObservableCollection<BD201SRollerController> Rollers
        {
            get => _rollers;
            set
            {
                _rollers = value;
                OnPropertyChanged(nameof(Rollers));
            }
        }
        public ObservableCollection<ICylinder> Cylinders
        {
            get => _cylinders;
            set
            {
                _cylinders = value;
                OnPropertyChanged(nameof(Cylinders));
            }
        }
        public ObservableCollection<IConveyor> CVs
        {
            get => _conveyors;
            set
            {
                _conveyors = value;
                OnPropertyChanged(nameof(CVs));
            }
        }
        public ObservableCollection<Vaccum> Vaccums
        {
            get => _vaccums;
            set
            {
                _vaccums = value;
                OnPropertyChanged(nameof(Vaccums));
            }
        }

        public ConveyorManualControlViewModel(Processes processes, Devices devices)
        {
            _processes = processes;
            _devices = devices;

            LoadProperties();
        }

        // Auto Get Properties(In/Out, Cylinder, Motion, ...) from Process
        public static ObservableCollection<TProp> GetProcessProperties<TProp>(object processInstance)
        {
            var props = processInstance.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(p => typeof(TProp).IsAssignableFrom(p.PropertyType));

            ObservableCollection<TProp> result = new();
            foreach (var prop in props)
            {
                var value = prop.GetValue(processInstance);
                if (value is TProp typedValue)
                {
                    result.Add(typedValue);
                }
            }

            return result;
        }

        private void LoadProperties()
        {
            CylindersInCv = GetProcessProperties<ICylinder>(_processes.FrontCVSetLoadProcess);
            CylindersDetachCv = GetProcessProperties<ICylinder>(_processes.FrontCVSetFilmDetachProcess);
            CylindersAssembleCv = GetProcessProperties<ICylinder>(_processes.FrontCVSetCamAssembleProcess);
            CylindersOutCv = GetProcessProperties<ICylinder>(_processes.FrontCVSetUnloadProcess);

            CVsInCv = GetProcessProperties<IConveyor>(_processes.FrontCVSetLoadProcess).ToList().Last();
            ManualInCv = GetProcessProperties<IConveyor>(_processes.FrontCVSetLoadProcess).ToList().First();
            CVsDetachCv = GetProcessProperties<IConveyor>(_processes.FrontCVSetFilmDetachProcess);
            CVsAssembleCv = GetProcessProperties<IConveyor>(_processes.FrontCVSetCamAssembleProcess);
            CVsOutCv = GetProcessProperties<IConveyor>(_processes.FrontCVSetUnloadProcess);

            RearCylindersInCv = GetProcessProperties<ICylinder>(_processes.RearCVSetLoadProcess);
            RearCylindersDetachCv = GetProcessProperties<ICylinder>(_processes.RearCVSetFilmDetachProcess);
            RearCylindersAssembleCv = GetProcessProperties<ICylinder>(_processes.RearCVSetCamAssembleProcess);
            RearCylindersOutCv = GetProcessProperties<ICylinder>(_processes.RearCVSetUnloadProcess);

            RearCVsInCv = GetProcessProperties<IConveyor>(_processes.RearCVSetLoadProcess).ToList().Last();
            RearManualInCv = GetProcessProperties<IConveyor>(_processes.RearCVSetLoadProcess).ToList().First();
            RearCVsDetachCv = GetProcessProperties<IConveyor>(_processes.RearCVSetFilmDetachProcess);
            RearCVsAssembleCv = GetProcessProperties<IConveyor>(_processes.RearCVSetCamAssembleProcess);
            RearCVsOutCv = GetProcessProperties<IConveyor>(_processes.RearCVSetUnloadProcess);
        }

        public ObservableCollection<ICylinder> CylindersInCv
        {
            get { return _cylindersInCv; }
            set { _cylindersInCv = value; OnPropertyChanged(); }
        }
        public IConveyor CVsInCv
        {
            get { return _conveyorsInCv; }
            set { _conveyorsInCv = value; OnPropertyChanged(); }
        }
        public IConveyor ManualInCv
        {
            get { return _manualInCv; }
            set { _manualInCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ICylinder> CylindersDetachCv
        {
            get { return _cylindersDetachCv; }
            set { _cylindersDetachCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<IConveyor> CVsDetachCv
        {
            get { return _conveyorsDetachCv; }
            set { _conveyorsDetachCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ICylinder> CylindersAssembleCv
        {
            get { return _cylindersAssembleCv; }
            set { _cylindersAssembleCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<IConveyor> CVsAssembleCv
        {
            get { return _conveyorsAssembleCv; }
            set { _conveyorsAssembleCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ICylinder> CylindersOutCv
        {
            get { return _cylindersOutCv; }
            set { _cylindersOutCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<IConveyor> CVsOutCv
        {
            get { return _conveyorsOutCv; }
            set { _conveyorsOutCv = value; OnPropertyChanged(); }
        }

        //Rear Set Cv
        public ObservableCollection<ICylinder> RearCylindersInCv
        {
            get { return _RearcylindersInCv; }
            set { _RearcylindersInCv = value; OnPropertyChanged(); }
        }
        public IConveyor RearCVsInCv
        {
            get { return _rearCVsInCv; }
            set { _rearCVsInCv = value; OnPropertyChanged(); }
        }
        public IConveyor RearManualInCv
        {
            get { return _rearManualInCv; }
            set { _rearManualInCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ICylinder> RearCylindersDetachCv
        {
            get { return _RearcylindersDetachCv; }
            set { _RearcylindersDetachCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<IConveyor> RearCVsDetachCv
        {
            get { return _RearconveyorsDetachCv; }
            set { _RearconveyorsDetachCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ICylinder> RearCylindersAssembleCv
        {
            get { return _RearcylindersAssembleCv; }
            set { _RearcylindersAssembleCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<IConveyor> RearCVsAssembleCv
        {
            get { return _RearconveyorsAssembleCv; }
            set { _RearconveyorsAssembleCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<ICylinder> RearCylindersOutCv
        {
            get { return _RearcylindersOutCv; }
            set { _RearcylindersOutCv = value; OnPropertyChanged(); }
        }
        public ObservableCollection<IConveyor> RearCVsOutCv
        {
            get { return _RearconveyorsOutCv; }
            set { _RearconveyorsOutCv = value; OnPropertyChanged(); }
        }

        //


        private readonly Processes _processes;
        private readonly Devices _devices;

        private ObservableCollection<ICylinder> _cylinders;
        private ObservableCollection<IConveyor> _conveyors;
        private ObservableCollection<Vaccum> _vaccums;
        private ObservableCollection<BD201SRollerController> _rollers;

        // Set CV prop
        private ObservableCollection<ICylinder> _cylindersInCv;
        private IConveyor _conveyorsInCv;
        private IConveyor _manualInCv;
        private ObservableCollection<ICylinder> _cylindersDetachCv;
        private ObservableCollection<IConveyor> _conveyorsDetachCv;
        private ObservableCollection<ICylinder> _cylindersAssembleCv;
        private ObservableCollection<IConveyor> _conveyorsAssembleCv;
        private ObservableCollection<ICylinder> _cylindersOutCv;
        private ObservableCollection<IConveyor> _conveyorsOutCv;

        private ObservableCollection<ICylinder> _RearcylindersInCv;
        private IConveyor _rearCVsInCv;
        private IConveyor _rearManualInCv;
        private ObservableCollection<ICylinder> _RearcylindersDetachCv;
        private ObservableCollection<IConveyor> _RearconveyorsDetachCv;
        private ObservableCollection<ICylinder> _RearcylindersAssembleCv;
        private ObservableCollection<IConveyor> _RearconveyorsAssembleCv;
        private ObservableCollection<ICylinder> _RearcylindersOutCv;
        private ObservableCollection<IConveyor> _RearconveyorsOutCv;
        //
    }
}
