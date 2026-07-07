using TOPENG_Device;

namespace EQX.Core.Communication.CIM
{
    public interface ICIMMapHelper
    {
        void GetBitAddress(CIMCommand cmd, out string cimAddress, out string plcAddress);
        void GetWordAddress(CIMCommand cmd, out string cimAddress, out string plcAddress, out int cimLength, out int plcLength);
        void GetAddress(CIMCommand cmd, out string cimBitAddress, out string plcBitAddress, out string cimWordAddress, out string plcWordAddress, out int cimWordLength, out int plcWordLength);

        void GetBitAddress(EquipEvent cmd, out string cimAddress, out string plcAddress);
        void GetWordAddress(EquipEvent cmd, out string cimAddress, out string plcAddress, out int cimLength, out int plcLength);
        void GetAddress(EquipEvent cmd, out string cimBitAddress, out string plcBitAddress, out string cimWordAddress, out string plcWordAddress, out int cimWordLength, out int plcWordLength);
    }
}
