using EQX.Core.Communication.Modbus;

namespace EQX.Device.Torque
{
    public enum EDX3000Props
    {
        Torque,
        Run,
        Stop,
        Speed,
        ResetAlarm,
        Jog
    }
    public class DX3000TorqueController : ModbusDevice<EDX3000Props>
    {
        public DX3000TorqueController(int id, string name, IModbusCommunication modbusCommunication) 
            : base(id, name, modbusCommunication)
        {
            KeyAddressPairs = new Dictionary<EDX3000Props, ushort>
            {
                { EDX3000Props.Torque, 0x0001 },
                { EDX3000Props.Run, 0x0005 },
                { EDX3000Props.Stop, 0x0005 },
                { EDX3000Props.Speed, 0x0001 },
                { EDX3000Props.ResetAlarm, 0x0005 },
                { EDX3000Props.Jog, 0x0005 },
            };
            KeyNumberOfBits = new Dictionary<EDX3000Props, uint>
            {
                { EDX3000Props.Torque, 16 },
                { EDX3000Props.Run, 16 },
                { EDX3000Props.Stop, 16 },
                { EDX3000Props.Speed, 16 },
                { EDX3000Props.ResetAlarm, 16 },
                { EDX3000Props.Jog, 16 },
            };
        }

        public void SetTorque(int torque)
        {
            this[EDX3000Props.Torque] = torque;
        }

        public int GetValue()
        {
            return Convert.ToInt32(this[EDX3000Props.Torque]);
        }

        public void Stop()
        {
            this[EDX3000Props.Stop] = 0x0001;
        }

        public void SetSpeed(int speed)
        {
            this[EDX3000Props.Speed] = speed;
        }

        public void ResetAlarm()
        {
            this[EDX3000Props.ResetAlarm] = 0x0008;
        }

        public void Run(bool isForward = true)
        {
            this[EDX3000Props.Run] = (isForward ? 0x0002 : 0x0004);
        }

        public void Jog(int speed, bool isForward)
        {
            this[EDX3000Props.Jog] = (isForward ? 0x0002 : 0x0004);

            SetSpeed(speed);
        }

        #region Privates
        #endregion
    }
}
