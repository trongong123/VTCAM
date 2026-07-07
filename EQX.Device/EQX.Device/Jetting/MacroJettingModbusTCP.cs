using EQX.Core.Common;
using EQX.Core.Communication.Modbus;
using NModbus.Extensions.Enron;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Device.Jetting
{
    public enum MacroJettingDispensingMode
    {
        Fobrid = 0,
        Trigger = 1,
        Permanent = 2
    }
    public class MacroJettingModbusTCP : INameable, IIdentifier, IHandleConnection
    {
        public const int MaxMsgBufSize = 1024;
        private readonly IModbusCommunication _modbusCommunication;

        public int Id { get; set; }
        public string Name { get; set; }

        public bool IsConnected => _modbusCommunication.IsConnected;

        public MacroJettingModbusTCP(int id, string name, IModbusCommunication modbusCommunication)
        {
            Id = id;
            Name = name;
            _modbusCommunication = modbusCommunication;
        }

        public bool Connect()
        {
            return _modbusCommunication.Connect();
        }

        public bool Disconnect()
        {
            return _modbusCommunication.Disconnect();
        }

        public void ReadTemperature(ref double readMaxTemp, ref double readSetTemp, ref double readCurrTemp)
        {
            var result = _modbusCommunication.ModbusMaster.ReadHoldingRegisters((byte)Id, 30, 4);

            if (result.Length >= 6)
            {
                readMaxTemp = ((result[0] << 8) | result[1]) * 0.1;
                readSetTemp = ((result[2] << 8) | result[3]) * 0.1;
                readCurrTemp = ((result[4] << 8) | result[5]) * 0.1;
            }
        }

        public void WriteTemperature(double setTemp)
        {
            ushort[] Buff = new ushort[2];

            Buff[0] = (ushort)(100.0 * 10);
            Buff[1] = (ushort)(setTemp * 10);

            _modbusCommunication.ModbusMaster.WriteMultipleRegisters((byte)Id, 30, Buff);
        }

        public void WritePulseOn(double WritePulseOn)
        {
            int v = (int)(WritePulseOn * 1000);

            ushort[] Buff = new ushort[2];
            Buff[0] = (ushort)(v >> 16);      // High word
            Buff[1] = (ushort)(v & 0xFFFF);   // Low word

            _modbusCommunication.ModbusMaster.WriteMultipleRegisters((byte)Id, 40, Buff);
        }

        public void WritePulseCycle(double WritePulseCycle)
        {
            int v = (int)(WritePulseCycle * 1000);

            ushort[] Buff = new ushort[2];
            Buff[0] = (ushort)(v >> 16);
            Buff[1] = (ushort)(v & 0xFFFF);

            _modbusCommunication.ModbusMaster.WriteMultipleRegisters((byte)Id, 42, Buff);
        }

        public void WritePulseCount(int WritePulseCount)
        {
            int v = WritePulseCount;

            ushort[] Buff = new ushort[2];
            Buff[0] = (ushort)(v >> 16);
            Buff[1] = (ushort)(v & 0xFFFF);

            _modbusCommunication.ModbusMaster.WriteMultipleRegisters((byte)Id, 44, Buff); // +4
        }

        public void WriteDispensingMode(MacroJettingDispensingMode mode)
        {
            ushort[] Buff = new ushort[1];
            Buff[0] = (ushort)mode;
            _modbusCommunication.ModbusMaster.WriteMultipleRegisters((byte)Id, 47, Buff);
        }
    }
}
