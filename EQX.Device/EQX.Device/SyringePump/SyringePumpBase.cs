using EQX.Core.Device.SyringePump;

namespace EQX.Device.SyringePump
{
    public class SyringePumpBase : ISyringePump
    {
        public int Id { get; init; }
        public string Name { get; set; }

        public virtual bool IsConnected { get; protected set; }
        public SyringePumpBase(string name, int id)
        {
            Name = name;
            Id = id;
        }

        public virtual bool IsReady()
        {
            return true;
        }

        public virtual bool Connect()
        {
            return true;
        }

        public virtual bool Disconnect()
        {
            return true;
        }

        public virtual void Initialize()
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void Stop()
        {
        }

        public virtual void Dispense(double volume, int port)
        {
        }

        public virtual void Fill(double volume)
        {
        }

        public virtual void SetSpeed(int speed)
        {

        }

        public virtual void SetAcceleration(int accCode)
        {

        }

        public virtual void SetDeccelation(int decCode)
        {
        }

        public virtual void Dispense(double volume, int[] ports)
        {
        }

        public virtual void Fill(double volume, int speed)
        {
        }

        public virtual void Dispense(double volume, int[] ports, int speed)
        {
        }

        public virtual void DispenseAndFill(double volumeDispense, int[] dispensePorts, double volumeFill, int speed)
        {

        }
    }
}
