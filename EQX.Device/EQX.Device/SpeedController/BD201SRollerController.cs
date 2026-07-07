using EQX.Core.Communication.Modbus;
using EQX.Device.Indicator;

namespace EQX.Device.SpeedController
{
    public enum ESD201SProps
    {
        Run,
        Stop,
        Direction,
        Speed,
        Acceleration,
        Deceleration,
    }
    public class BD201SRollerController : ModbusDevice<ESD201SProps>
    {
        public BD201SRollerController(int id, string name, IModbusCommunication modbusCommunication)
            : base(id, name, modbusCommunication)
        {
            KeyAddressPairs = new Dictionary<ESD201SProps, ushort>
            {
                { ESD201SProps.Run, 100 },
                { ESD201SProps.Stop, 100 },
                { ESD201SProps.Direction, 101 },
                { ESD201SProps.Speed, 102 },
                { ESD201SProps.Acceleration, 103 },
                { ESD201SProps.Deceleration, 104 },
            };
            KeyNumberOfBits = new Dictionary<ESD201SProps, uint>
            {
                { ESD201SProps.Run, 16 },
                { ESD201SProps.Stop, 16 },
                { ESD201SProps.Direction, 16 },
                { ESD201SProps.Speed, 16 },
                { ESD201SProps.Acceleration, 16 },
                { ESD201SProps.Deceleration, 16 },
            };
        }

        public void Run()
        {
            this[ESD201SProps.Run] = 1;
        }

        public void Run(bool isCW)
        {
            SetDirection(isCW);
            Run();
        }

        public void Stop()
        {
            this[ESD201SProps.Stop] = 0;
        }

        public void SetDirection(bool isCW)
        {
            this[ESD201SProps.Direction] = isCW ? 0 : 1;
        }

        public void SetSpeed(int speed)
        {
            if (speed < 200) speed = 200;

            this[ESD201SProps.Speed] = speed;
        }

        public void SetAcceleration(int acceleration)
        {
            if (acceleration < 400) acceleration = 400;

            this[ESD201SProps.Acceleration] = acceleration;
        }

        public void SetDeceleration(int deceleration)
        {
            if (deceleration < 400) deceleration = 400;

            this[ESD201SProps.Deceleration] = deceleration;
        }
    }
}
