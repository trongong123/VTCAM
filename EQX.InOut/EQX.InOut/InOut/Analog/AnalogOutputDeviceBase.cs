using EQX.Core.InOut.Analog;

namespace EQX.InOut.InOut.Analog
{
    public class AnalogOutputDeviceBase<TEnum> : IAOutputDevice where TEnum : Enum
    {
        public List<IAOutput> AnalogOutputs { get; }

        public virtual bool IsConnected { get; protected set; }

        public int Id { get; init; }

        public string Name { get; init; }

        public AnalogOutputDeviceBase()
        {
            Name ??= GetType().Name;
            AnalogOutputs = new List<IAOutput>();
        }
        public virtual bool Connect()
        {
            return true;
        }

        public virtual bool Disconnect()
        {
            return true;
        }

        public virtual double GetVolt(int channel)
        {
            return 0.0;
        }

        public bool Initialize()
        {
            var outputList = Enum.GetNames(typeof(TEnum)).ToList();
            var outputIndex = (int[])Enum.GetValues(typeof(TEnum));
            for (int i = 0; i < outputList.Count; i++)
            {
                AnalogOutputs.Add(new AOutput(outputIndex[i], outputList[i], this));
            }

            ExtendInit();

            return true;
        }

        protected virtual void ExtendInit()
        {
        }

        public virtual void SetVolt(int channel, double voltage)
        {
        }
    }
}
