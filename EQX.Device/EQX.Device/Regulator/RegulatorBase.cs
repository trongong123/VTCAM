using EQX.Core.Device.Regulator;

namespace EQX.Device.Regulator
{
    public class RegulatorBase : IRegulator
    {
        public int Id { get; init; }
        public string Name { get; set; }

        public virtual bool IsConnected { get; protected set; }

        public RegulatorBase(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public virtual bool Connect()
        {
            return true;
        }

        public virtual bool DecreasePressure()
        {
            return true;
        }

        public virtual bool Disconnect()
        {
            return true;
        }

        public virtual double GetPressure()
        {
            return 0;
        }

        public virtual bool IncreasePressure()
        {
            return true;
        }

        public virtual bool SetPressure(double value)
        {
            return true;
        }
    }
}
