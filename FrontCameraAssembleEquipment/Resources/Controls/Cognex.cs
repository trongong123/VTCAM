using FrontCameraAssembleEquipment.Defines.Recipes;
using System;
using System.IO.Ports;

namespace FrontCameraAssembleEquipment.Resources.Controls
{
    public class CognexTrigger
    {
        private readonly GlobalRecipe _globalRecipe;

        private SerialPort _port;
        public event Action<string>? DataReceivedEvent;
        //public CognexTrigger(GlobalRecipe globalRecipe)
        //{
        //    _globalRecipe = globalRecipe;
        //}

        public void OpenPort()
        {
            try
            {
                _port = new SerialPort("COM99", 115200, Parity.None, 8, StopBits.One);
                _port.Handshake = Handshake.None;
                _port.NewLine = "\r"; // Cognex dùng CR
                _port.DataReceived += _port_DataReceived;  // BẮT SỰ KIỆN
                _port.Open();
            }
            catch
            {
            }
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = _port.ReadLine();  // Cognex trả theo từng dòng
                DataReceivedEvent?.Invoke(data);
            }
            catch { }
        }

        public void Trigger()
        {
            if (_port != null && _port.IsOpen)
            {
                // Lệnh trigger đúng chuẩn Cognex
                _port.Write("BCR\r\n");
            }
        }

        public void Close()
        {
            if (_port != null && _port.IsOpen)
                _port.Close();
        }
    }
}
