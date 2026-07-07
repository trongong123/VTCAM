using EQX.Core.Communication.CIM.Custom.WordArea;
using TOPENG_Device;

namespace EQX.Core.Communication.CIM
{
    public class CIMCommandArgs
    {
        public CIMCommand CIMCommand;
        public short[] Buffer;

        public CIMCommandArgs(CIMCommand cIMCommand, short[] buffer)
        {
            CIMCommand = cIMCommand;
            Buffer = buffer;
        }
    }
}
