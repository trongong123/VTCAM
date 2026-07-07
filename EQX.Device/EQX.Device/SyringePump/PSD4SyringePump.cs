using EQX.Core.Communication;
using log4net;
using System.Text;

namespace EQX.Device.SyringePump
{
    public class PSD4SyringePump : SyringePumpBase
    {
        #region Properties
        public override bool IsConnected => _serialCommunicator.IsConnected;

        public override bool IsReady()
        {
            if (!_serialCommunicator.IsConnected)
                throw new InvalidOperationException("Syringe pump is not connected.");

            string strCommand = $"/{comId}Q";
            _serialCommunicator.Write(BuildCommand(strCommand));

            string strResponse = _serialCommunicator.Read();

            byte[] response = Encoding.ASCII.GetBytes(strResponse);

            if (response.Length <= 0) return false;

            byte[] result = ExtractSegment(response);

            if (result == null) return false;
            if (result.Length < 3) return false;
            if ((result[1] != (0x30 + Id)) && (result[1] != 0x30)) return false;

            byte status = result[2];

            //_log.Info(string.Join(" ", result.Select(b => b.ToString("X2"))));

            return (0x01 << 5 & status) == (0x01 << 5);
        }

        //public override int ErrorCode
        //{
        //    get
        //    {
        //        if (!_serialCommunicator.IsConnected)
        //            throw new InvalidOperationException("Syringe pump is not connected.");

        //        string strCommand = $"/{comId}Q";

        //        _serialCommunicator.Write(BuildCommand(strCommand));

        //        string strResponse = _serialCommunicator.Read();
        //        byte[] response = Encoding.ASCII.GetBytes(strResponse);

        //        if (response.Length <= 0) return (int)EPSD4SyringPumpError.None;

        //        byte[] result = ExtractSegment(response);

        //        if (result == null) return (int)EPSD4SyringPumpError.None;
        //        if (result.Length < 3) return (int)EPSD4SyringPumpError.None;
        //        if (result[1] != (0x30 + Id)) return (int)EPSD4SyringPumpError.None;
        //        byte status = result[2];

        //        return (int)(status & 0x0F);
        //    }
        //}
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id">Same as physical switch setting</param>
        /// <param name="serialCommunicator"></param>
        /// <param name="syringeVolume"></param>
        public PSD4SyringePump(string name, int id, SerialCommunicator serialCommunicator, double syringeVolume = 1.0) : base(name, id)
        {
            _serialCommunicator = serialCommunicator;
            _syringeVolume = syringeVolume;
            _log = LogManager.GetLogger(Name);
        }
        #endregion

        #region Methods
        public override bool Connect()
        {
            if (IsConnected)
                return true;
            return _serialCommunicator.Connect();
        }

        public override bool Disconnect()
        {
            if (!IsConnected)
                return true;
            return _serialCommunicator.Disconnect();
        }

        public override void Initialize()
        {
            if (!_serialCommunicator.IsConnected)
                throw new InvalidOperationException("Syringe pump is not connected.");

            // SET standard resolution
            string strCommand = $"/{comId}N0R";
            _serialCommunicator.Write(BuildCommand(strCommand));


            // Set max speed 10000
            strCommand = $"/{comId}V10000R";
            _serialCommunicator.Write(BuildCommand(strCommand));

            strCommand = $"/{comId}ZR";
            _serialCommunicator.Write(BuildCommand(strCommand));
            _log.Debug("Initialize");

        }

        public override void Dispense(double volume, int port)
        {
            if (!_serialCommunicator.IsConnected)
                throw new InvalidOperationException("Syringe pump is not connected.");

            int step = (int)((volume * maxStep) / _syringeVolume);
            string strCommand = $"/{comId}O{port}D{step}R";
            _serialCommunicator.Write(BuildCommand(strCommand));

            _log.Debug($"Dispense {volume}[ml], port : {port}");
        }

        public override void Fill(double volume)
        {
            if (!_serialCommunicator.IsConnected)
                throw new InvalidOperationException("Syringe pump is not connected.");

            int step = (int)((volume * maxStep) / _syringeVolume);
            string strCommand = $"/{comId}I7A{step}R";
            _serialCommunicator.Write(BuildCommand(strCommand));

            _log.Debug($"Fill {volume}[ml]");
        }

        public override void Fill(double volume, int speed)
        {
            int step = (int)((volume * maxStep) / _syringeVolume);
            string strCommand = $"/{comId}S{speed}I7A{step}R";
            _serialCommunicator.Write(BuildCommand(strCommand));

            _log.Debug($"Fill {volume}[ml]");
        }

        public override void SetSpeed(int speed)
        {
            string strCommand = $"/{comId}S{speed}R";
            _serialCommunicator.Write(BuildCommand(strCommand));

            _log.Debug($"Set speed : {speed}");
        }

        public override void SetAcceleration(int accCode)
        {
            string strCommand = $"/{comId}L{accCode}R";
            _serialCommunicator.Write(BuildCommand(strCommand));

            _log.Debug($"Set acceleration : {accCode}");
        }

        public override void SetDeccelation(int decCode)
        {
            string strCommand = $"/{comId}C{decCode}R";
            _serialCommunicator.Write(BuildCommand(strCommand));

            _log.Debug($"Set decceleration : {decCode}");
        }
        public override void Stop()
        {
            string strCommand = $"/{comId}T";
            _serialCommunicator.Write(BuildCommand(strCommand));

            _log.Debug("Stop");
        }

        public override void Dispense(double volume, int[] ports)
        {
            int step = (int)((volume * maxStep) / _syringeVolume);
            string strCommand = $"/{comId}";
            foreach (int port in ports)
            {
                strCommand += $"O{port}D{step}";
            }

            strCommand += "R";
            _serialCommunicator.Write(BuildCommand(strCommand));
        }

        public override void Dispense(double volume, int[] ports, int speed)
        {
            int step = (int)((volume * maxStep) / _syringeVolume);
            string strCommand = $"/{comId}S{speed}";
            foreach (int port in ports)
            {
                strCommand += $"O{port}D{step}";
            }

            strCommand += "R";
            _serialCommunicator.Write(BuildCommand(strCommand));
        }

        public override void DispenseAndFill(double volumeDispense, int[] dispensePorts, double volumeFill, int speed)
        {
            int stepDispense = (int)((volumeDispense * maxStep) / _syringeVolume);
            int stepFill = (int)((volumeFill * maxStep) / _syringeVolume);

            string strCommand = $"/{comId}S{speed}";
            foreach (int port in dispensePorts)
            {
                strCommand += $"O{port}D{stepDispense}";
            }

            strCommand += $"I7A{stepFill}";
            strCommand += "R";
            _serialCommunicator.Write(BuildCommand(strCommand));
        }
        #endregion

        #region Privates
        private int comId => Id + 1;

        private SerialCommunicator _serialCommunicator;
        private double _syringeVolume;
        private int maxStep = 3000;
        private ILog _log;
        #endregion

        private byte[] BuildCommand(string command)
        {
            return Encoding.ASCII.GetBytes(command).Concat(new byte[] { 0x0D, 0x0A }).ToArray();
        }

        private byte[] ExtractSegment(byte[] data)
        {
            int startIndex = -1;
            int endIndex = -1;

            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0x2F)
                {
                    startIndex = i;
                    endIndex = -1;
                }

                if (startIndex != -1 && i < data.Length - 1 && data[i] == 0x0D && data[i + 1] == 0x0A)
                {
                    endIndex = i + 1;
                    i++;
                }
            }

            if (startIndex != -1 && endIndex != -1)
            {
                int length = endIndex - startIndex + 1;
                byte[] segment = new byte[length];
                Array.Copy(data, startIndex, segment, 0, length);
                return segment;
            }

            return null;
        }
    }
}
