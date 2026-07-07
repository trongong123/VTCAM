using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.InOut;
using EQX.Core.Interlock;
using log4net;

namespace EQX.InOut
{
    public class DOutput : ObservableObject, IDOutput
    {
        public int Id { get; init; }
        public string Name { get; init; }

        public bool Value
        {
            get { return _dOutputDevice[Id]; }
            set
            {
                if (value)
                {
                    var blockedInterlock = OutputEnableInterlocks?.FirstOrDefault(i => !i.Value());

                    if (blockedInterlock?.Key != null)
                    {
                        InterlockMonitor.InterlockBlocked(this, "Set", blockedInterlock?.Key!);
                        return;
                    }
                }

                _dOutputDevice[Id] = value;
                RaiseValueUpdated();
            }
        }

        public void RaiseValueUpdated()
        {
            OnPropertyChanged(nameof(Value));
        }

        public Dictionary<string, Func<bool>>? OutputEnableInterlocks { get; set; }

        public DOutput(int id, string name, IDOutputDevice dOutputDevice)
        {
            Id = id;
            Name = name;
            _dOutputDevice = dOutputDevice;
        }

        internal IDOutputDevice GetOutputDevice() => _dOutputDevice;

        private readonly IDOutputDevice _dOutputDevice;
    }
}
