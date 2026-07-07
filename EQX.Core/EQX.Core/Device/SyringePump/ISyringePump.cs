using EQX.Core.Common;

namespace EQX.Core.Device.SyringePump
{
    public interface ISyringePump : IHandleConnection, IIdentifier
    {
        void Reset();
        void Initialize();
        void Stop();
        void Dispense(double volume, int port);
        void Fill(double volume);
        void Fill(double volume, int speed);
        void SetSpeed(int speed);
        void SetAcceleration(int accCode);
        void SetDeccelation(int decCode);

        void Dispense(double volume, int[] ports);
        void Dispense(double volume, int[] ports, int speed);
        void DispenseAndFill(double volumeDispense, int[] dispensePorts, double volumeFill, int speed);
        bool IsReady();
    }
}
