using EQX.Core.InOut;
using EQX.Core.InOut.Analog;

namespace EQX.InOut.InOut.Analog
{
    public class AOutput : IAOutput
    {
        private readonly IAOutputDevice _aOutputDevice;

        public double Volt
        {
            get => _aOutputDevice.GetVolt(Id);
            set
            {
                _aOutputDevice.SetVolt(Id, value);
            }
        }

        public int Id { get; init; }

        public string Name { get; init; }

        public AOutput(int id, string name, IAOutputDevice aOutputDevice)
        {
            Id = id;
            Name = name;
            _aOutputDevice = aOutputDevice;
        }
    }
}
