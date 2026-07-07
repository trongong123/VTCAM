using EQX.Core.Common;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace EQX.Core.Communication
{
    public class SerialCommunicator : IHandleConnection, IIdentifier
    {
        public EventHandler DataReceived;
        #region Constructor(s)
        public SerialCommunicator(int id, string name, string comPort, int baudRate = 115200, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            Id = id;
            Name = name;

            _comPort = comPort;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsConnected => serialPort == null ? false : serialPort.IsOpen;
        /// <summary>
        /// Serial Comport Communicator
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="name">Name</param>
        /// <param name="comPort">Comport (ex: com1, com2,...)</param>
        /// <param name="baudRate">baudRate</param>
        /// <param name="parity">parity</param>
        /// <param name="dataBits">dataBits</param>
        /// <param name="stopBits">stopBits</param>
        #endregion

        #region Method(s)
        public bool Connect()
        {
            try
            {
                if (serialPort == null)
                {
                    serialPort = new SerialPort(_comPort, _baudRate, _parity, _dataBits, _stopBits);

                    //serialPort.DataReceived += (s, e) =>
                    //{
                    //    // Handle data received event if needed
                    //    SerialPort sp = (SerialPort)s;

                    //    // Use ReadExisting() to get all the data currently in the buffer
                    //    //string indata = sp.ReadExisting();

                    //    //DataReceived?.Invoke(indata, EventArgs.Empty);

                    //    //byte[] bytes = Encoding.Default.GetBytes(indata);
                    //    //Debug.Write($"Data Received: [ ");
                    //    //foreach (byte b in bytes)
                    //    //{
                    //    //    Debug.Write($"{b:X2} ");
                    //    //}
                    //    //Debug.WriteLine(" ]\r\n");
                    //};
                }

                serialPort.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Disconnect()
        {
            if (serialPort == null) return true;
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }

            return true;
        }

        public void Write(string message)
        {
            Write(System.Text.Encoding.UTF8.GetBytes(message));
        }

        public void Write(byte[] message)
        {
            lock (writeLock)
            {
                serialPort.Write(message, 0, message.Length);
            }
        }
        public void WriteLine(string message)
        {
            Write(System.Text.Encoding.UTF8.GetBytes(message + "\r\n"));
        }

        public string ReadLine()
        {
            return serialPort.ReadLine();
        }

        public string Read()
        {
            int bytes = serialPort.BytesToRead;

            byte[] buffer = new byte[bytes];
            serialPort.Read(buffer, 0, bytes);

            return System.Text.Encoding.Default.GetString(buffer);
        }
        #endregion

        #region Privates
        private SerialPort serialPort;
        private readonly string _comPort;
        private readonly int _baudRate;
        private readonly Parity _parity;
        private readonly int _dataBits;
        private readonly StopBits _stopBits;

        private object writeLock = new object();
        #endregion
    }
}
