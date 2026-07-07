using System.Net;
using EQX.Core.Communication;

namespace EQX.ThirdParty.Fastech
{
    public class EziPlusEEventCommunicator : TCPEventCommunicator
    {
        public EziPlusEEventCommunicator(int index, string name, IPAddress iPAddress, int port)
            : base(index, name, iPAddress, port)
        {
            syncNumber = (byte)new Random().Next(0, 255);
            transmitFrameData = new byte[260];
            responseFrameData = new byte[260];
        }

        /// <summary>
        /// The Sync number of the packet, which is used to check whether the command is executed in the drive module
        /// </summary>
        protected byte syncNumber;
        /// <summary>
        /// Controller (PC, Rpi) to Ezi, transmit frame data
        /// </summary>
        protected byte[] transmitFrameData;
        /// <summary>
        /// Ezi to Controller (PC, Rpi), response frame data
        /// </summary>
        protected byte[] responseFrameData;

        protected byte CommunicationStatus => responseFrameData[5];

        /// <summary>
        /// Set transmit frame data, which the frame type is null data type (data length is 0 byte)
        /// </summary>
        /// <param name="frameType"></param>
        protected void SetTransmitFrameData(byte frameType)
        {
            SetTransmitFrameData(frameType, 0, null);
        }

        protected void SetTransmitFrameData(byte frameType, int dataLength, byte[]? data)
        {
            // Header : 0xAA
            transmitFrameData[0] = 0xAA;
            // Length : Length of Data after Length (SyncNumber + Reserved + Frame Type + Data)
            transmitFrameData[1] = (byte)(3 + dataLength);
            // Sync Number. The value should change every time when send a new command
            transmitFrameData[2] = ++syncNumber;
            // Reserved : 0x00
            transmitFrameData[3] = 0x00;
            // Frame type : Specify the command type of the Frame
            transmitFrameData[4] = frameType;
            // Data : The data structure and length of this clause are determinated by the Frame type
            if (data != null)
            {
                for (int i = 0; i < dataLength; i++)
                {
                    transmitFrameData[5 + i] = data[i];
                }
            }
        }

        protected void SendTransmitFrameData()
        {
            //if (tcpClient.Connected == false) return;
            //tcpClient.Send(transmitFrameData, transmitFrameData[1] + 2, SocketFlags.None);

            SendData(transmitFrameData, transmitFrameData[1] + 2);
        }

        protected int ReadResponseFrameData()
        {
            //if (tcpClient.Connected == false) return -1;
            //return tcpClient.Receive(responseFrameData);

            return ReadData(ref responseFrameData);
        }
    }

    public class EziPlusECommunicator : TCPBasicCommunicator
    {
        public EziPlusECommunicator(int index, string name, IPAddress iPAddress, int port)
            : base(index, name, iPAddress, port)
        {
            syncNumber = (byte)new Random().Next(0, 255);
            transmitFrameData = new byte[260];
            responseFrameData = new byte[260];
        }

        /// <summary>
        /// The Sync number of the packet, which is used to check whether the command is executed in the drive module
        /// </summary>
        protected byte syncNumber;
        /// <summary>
        /// Controller (PC, Rpi) to Ezi, transmit frame data
        /// </summary>
        protected byte[] transmitFrameData;
        /// <summary>
        /// Ezi to Controller (PC, Rpi), response frame data
        /// </summary>
        protected byte[] responseFrameData;

        protected byte CommunicationStatus => responseFrameData[5];

        /// <summary>
        /// Set transmit frame data, which the frame type is null data type (data length is 0 byte)
        /// </summary>
        /// <param name="frameType"></param>
        protected void SetTransmitFrameData(byte frameType)
        {
            SetTransmitFrameData(frameType, 0, null);
        }

        protected void SetTransmitFrameData(byte frameType, int dataLength, byte[]? data)
        {
            // Header : 0xAA
            transmitFrameData[0] = 0xAA;
            // Length : Length of Data after Length (SyncNumber + Reserved + Frame Type + Data)
            transmitFrameData[1] = (byte)(3 + dataLength);
            // Sync Number. The value should change every time when send a new command
            transmitFrameData[2] = ++syncNumber;
            // Reserved : 0x00
            transmitFrameData[3] = 0x00;
            // Frame type : Specify the command type of the Frame
            transmitFrameData[4] = frameType;
            // Data : The data structure and length of this clause are determinated by the Frame type
            if (data != null)
            {
                for (int i = 0; i < dataLength; i++)
                {
                    transmitFrameData[5 + i] = data[i];
                }
            }
        }

        protected void SendTransmitFrameData()
        {
            //if (tcpClient.Connected == false) return;
            //tcpClient.Send(transmitFrameData, transmitFrameData[1] + 2, SocketFlags.None);

            SendData(transmitFrameData, transmitFrameData[1] + 2);
        }

        protected int ReadResponseFrameData()
        {
            //if (tcpClient.Connected == false) return -1;
            //return tcpClient.Receive(responseFrameData);

            return ReadData(ref responseFrameData);
        }
    }

}
