using EQX.Core.InOut;
using System.IO.MemoryMappedFiles;

namespace EQX.InOut
{
    public static class SimulationInputSetter
    {
        //static ModbusTcpCommunication ModbusTcpCommunication = new ModbusTcpCommunication();

        public static void SetSimInput(IDInput? input, bool value)
        {
            if (input == null) return;

            try
            {
                using (MemoryMappedViewStream stream = _mmf.CreateViewStream(input.Id + input.SimulationOffset, 0))
                {
                    BinaryWriter writer = new BinaryWriter(stream);
                    if (value)
                    {
                        writer.Write((char)1);
                    }
                    else
                    {
                        writer.Write((char)0);
                    }
                }
            }
            catch
            {
                return;
            }
        }

        private static object lockObj = new object();

        //public static void SetSimModbusInput(IDInput? input, bool value)
        //{
        //    if (input == null) return;
        //    try
        //    {
        //        lock (lockObj)
        //        {
        //            if (!ModbusTcpCommunication.IsConnected)
        //            {
        //                ModbusTcpCommunication.Connect();
        //            }
        //        }

        //        ModbusTcpCommunication.ModbusMaster.WriteSingleCoil(0, (ushort)input.Id, value);
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        static MemoryMappedFile _mmf = MemoryMappedFile.OpenExisting("SimInputData");
    }
}
