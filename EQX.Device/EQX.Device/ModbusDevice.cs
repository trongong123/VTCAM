using EQX.Core.Communication.Modbus;
using EQX.Core.Device.Indicator;
using System.Threading;

namespace EQX.Device
{
    public class ModbusDevice<EKeys> : IModbusDevice<EKeys> where EKeys : Enum
    {
        IModbusCommunication modbusCommunication { get; }

        private object lockObject = new object();
        bool lockTaken;

        object? oldValue = null;

        public object this[EKeys key]
        {
            get
            {
                object newValue = GetValue(key);
                if (newValue == null) return oldValue;

                oldValue = newValue;
                return newValue;
            }
            set => SetValue(key, value);
        }

        public ModbusDevice(int id, string name, IModbusCommunication modbusCommunication)
        {
            Id = id;
            Name = name;

            lockTaken = false;
            this.modbusCommunication = modbusCommunication;
        }

        public bool IsConnected => modbusCommunication.IsConnected;

        public int Id { get; }

        public string Name { get; }

        public bool Connect()
        {
            return modbusCommunication.Connect();
        }

        public bool Disconnect()
        {
            return modbusCommunication.Disconnect();
        }

        protected virtual object GetValue(EKeys key)
        {
            lockTaken = false;
            try
            {
                Monitor.TryEnter(lockObject, 500, ref lockTaken);
                if (lockTaken)
                {
                    if (IsConnected == false) return 0;

                    if (KeyNumberOfBits[key] != 1 &&
                        KeyNumberOfBits[key] % 16 != 0)
                    {
                        throw new ArgumentException("keyNumberOfBits 1, 16, 32, 48... support only");
                    }

                    if (KeyNumberOfBits[key] == 16)
                    {
                        return modbusCommunication.ModbusMaster.ReadHoldingRegisters((byte)Id, KeyAddressPairs[key], 1)[0];
                    }
                    if (KeyNumberOfBits[key] % 16 == 0)
                    {
                        return modbusCommunication.ModbusMaster.ReadHoldingRegisters((byte)Id, KeyAddressPairs[key], (ushort)(KeyNumberOfBits[key] / 16));
                    }
                    if (KeyNumberOfBits[key] == 1)
                    {
                        return modbusCommunication.ModbusMaster.ReadCoils((byte)Id, KeyAddressPairs[key], 1)[0];
                    }

                    return null; // This should not happen
                }
                else
                {
                    // The lock was not acquired.
                    return null; // This should not happen
                }
            }
            finally
            {
                // Ensure that the lock is released.
                if (lockTaken)
                {
                    Monitor.Exit(lockObject);
                }
            }
        }

        protected virtual void SetValue(EKeys key, object value)
        {
            lock (lockObject)
            {
                try
                {
                    if (IsConnected == false) return;

                    if (KeyNumberOfBits[key] != 1 &&
                        KeyNumberOfBits[key] % 16 != 0)
                    {
                        throw new ArgumentException("keyNumberOfBits 1, 16, 32, 48... support only");
                    }

                    if (KeyNumberOfBits[key] == 16)
                    {
                        modbusCommunication.ModbusMaster.WriteSingleRegister((byte)Id, KeyAddressPairs[key], Convert.ToUInt16(value));
                        return;
                    }
                    if (KeyNumberOfBits[key] % 16 == 0)
                    {
                        modbusCommunication.ModbusMaster.WriteMultipleRegisters((byte)Id, KeyAddressPairs[key], (ushort[])value);
                        return;
                    }
                    if (KeyNumberOfBits[key] == 1)
                    {
                        modbusCommunication.ModbusMaster.WriteSingleCoil((byte)Id, KeyAddressPairs[key], (bool)value);
                        return;
                    }
                }
                catch { }
            }
        }

        protected Dictionary<EKeys, ushort> KeyAddressPairs { get; set; }
        protected Dictionary<EKeys, uint> KeyNumberOfBits { get; set; }
    }
}
