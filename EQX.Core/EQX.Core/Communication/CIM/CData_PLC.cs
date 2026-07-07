using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPENG_Device
{
    public struct strPLCData
    {
        public string Address;
        public string Name;
        public DATA_TYPE DataType;
        public byte[] Value;

        public int getLength()
        {
            return Value == null ? 0 : Value.Length;
        }
        public void setLength(int length)
        {
            Value = new byte[length];
        }
        public void clearValue()
        {
            if (Value == null) return;
            Array.Clear(Value, 0, Value.Length);
        }
        public string getValue()
        {
            return CPLC.getDataToString(DataType, Value);
        }
        public void setValue(string str)
        {
            Value = CPLC.getStringToData(DataType, str);
        }
        public void setValue(byte[] data)
        {
            Value = data;
        }
    }
    class CData_PLC
    {
    }
}
