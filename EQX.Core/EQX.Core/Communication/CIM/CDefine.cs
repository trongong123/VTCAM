using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPENG_Device
{
    public enum DEVICE_ID
    {
        Main_PLC = 0,
        Line_PLC,
        _MAX
    }
    public enum DATA_TYPE
    {
        BOOL,// = 1,      // 1 bit
        BYTE,// = 1,       // 1 byte

        USINT,// = 1,     // 1 byte
        UINT,// = 2,       // 1 word
        UDINT,// = 4,     // 1 double word
        ULINT,// = 8,      // 1 long word

        SINT,// = 1,       // 1 byte
        INT,// = 2,        // 1 word
        DINT,// = 4,       // 1 double word
        LINT,// = 8,       // 1 long word

        REAL,// = 4,       // 1 double word
        LREAL,// = 8,      // 1 long word

        STRING
    }

    public static class CPLC
    {
        public delegate void ErrorReceived(Exception ex);
        public static event ErrorReceived OnErrorReceived;

        public static int getBytesLength(DATA_TYPE dataType)
        {
            switch (dataType)
            {
                case DATA_TYPE.BOOL:
                case DATA_TYPE.BYTE:
                case DATA_TYPE.USINT:
                case DATA_TYPE.SINT:
                    return 1;

                case DATA_TYPE.UINT:
                case DATA_TYPE.INT:
                    return 2;

                case DATA_TYPE.UDINT:
                case DATA_TYPE.DINT:
                case DATA_TYPE.REAL:
                    return 4;

                case DATA_TYPE.ULINT:
                case DATA_TYPE.LINT:
                case DATA_TYPE.LREAL:
                    return 8;

                default:
                    return 0;
            }
        }
        public static string getDataToString(DATA_TYPE dataType, byte[] data)
        {
            if (data == null || data.Length == 0)
                return "";
            string sRet = "";
            switch (dataType)
            {
                case DATA_TYPE.BOOL:  // 1bit
                    sRet = data[0] == 0 ? "False" : "True";
                    break;
                case DATA_TYPE.BYTE:  // 1byte
                    sRet = BitConverter.ToString(data).Replace("-", " ");
                    break;

                case DATA_TYPE.USINT:  // 1byte
                    sRet = Convert.ToUInt16(data[0]).ToString();
                    break;
                case DATA_TYPE.UINT:  // 2bytes
                    sRet = BitConverter.ToUInt16(data, 0).ToString();
                    break;
                case DATA_TYPE.UDINT:  // 4bytes
                    sRet = BitConverter.ToUInt32(data, 0).ToString();
                    break;
                case DATA_TYPE.ULINT:  // 8bytes
                    sRet = BitConverter.ToUInt64(data, 0).ToString();
                    break;

                case DATA_TYPE.SINT:  // 1byte
                    sRet = Convert.ToInt16(data[0]).ToString();
                    break;
                case DATA_TYPE.INT:  // 2bytes
                    sRet = BitConverter.ToInt16(data, 0).ToString();
                    break;
                case DATA_TYPE.DINT:  // 4bytes
                    sRet = BitConverter.ToInt32(data, 0).ToString();
                    break;
                case DATA_TYPE.LINT:  // 8bytes
                    sRet = BitConverter.ToInt64(data, 0).ToString();
                    break;

                case DATA_TYPE.REAL:  // 4bytes
                    sRet = BitConverter.ToSingle(data, 0).ToString("F3");
                    break;
                case DATA_TYPE.LREAL:  // 8bytes
                    sRet = BitConverter.ToDouble(data, 0).ToString("F3");
                    break;

                case DATA_TYPE.STRING:
                    sRet = Encoding.Default.GetString(data);
                    break;
            }
            return sRet;
        }
        public static byte[] getStringToData(DATA_TYPE dataType, string str)
        {
            byte[] data = null;
            try
            {
                switch (dataType)
                {
                    case DATA_TYPE.BOOL:  // 1bit
                        int temp1 = 0;
                        data = new byte[1];
                        if (!int.TryParse(str, out temp1))  // true / false
                        {
                            if (str.ToLower().Trim() == "false")
                                data[0] = 0x00;
                            else
                                data[0] = 0x01;
                        }
                        else
                        {
                            if (temp1 == 0)
                                data[0] = 0x00;
                            else
                                data[0] = 0x01;
                        }
                        break;
                    case DATA_TYPE.BYTE:  // 1byte
                        data = new byte[1];
                        data[0] = Convert.ToByte(str.Substring(0, 2), 16);
                        break;

                    case DATA_TYPE.USINT:  // 1byte
                        data = new byte[1];
                        data[0] = Convert.ToByte(str);
                        break;
                    case DATA_TYPE.UINT:  // 2bytes
                        UInt16 temp2 = Convert.ToUInt16(str);
                        data = BitConverter.GetBytes(temp2);
                        break;
                    case DATA_TYPE.UDINT:  // 4bytes
                        UInt32 temp3 = Convert.ToUInt32(str);
                        data = BitConverter.GetBytes(temp3);
                        break;
                    case DATA_TYPE.ULINT:  // 8bytes
                        UInt64 temp4 = Convert.ToUInt64(str);
                        data = BitConverter.GetBytes(temp4);
                        break;

                    case DATA_TYPE.SINT:  // 1byte
                        data = new byte[1];
                        sbyte temp5 = Convert.ToSByte(str);
                        data[0] = (byte)temp5;
                        break;
                    case DATA_TYPE.INT:  // 2bytes
                        Int16 temp6 = Convert.ToInt16(str);
                        data = BitConverter.GetBytes(temp6);
                        break;
                    case DATA_TYPE.DINT:  // 4bytes
                        Int32 temp7 = Convert.ToInt32(str);
                        data = BitConverter.GetBytes(temp7);
                        break;
                    case DATA_TYPE.LINT:  // 8bytes
                        Int64 temp8 = Convert.ToInt32(str);
                        data = BitConverter.GetBytes(temp8);
                        break;

                    case DATA_TYPE.REAL:  // 4bytes
                        float temp9 = Convert.ToSingle(str);
                        data = BitConverter.GetBytes(temp9);
                        break;
                    case DATA_TYPE.LREAL:  // 8bytes
                        double temp10 = Convert.ToDouble(str);
                        data = BitConverter.GetBytes(temp10);
                        break;

                    case DATA_TYPE.STRING:
                        if (str.Length > 31)
                            throw new Exception("문자열 길이는 31자 이하여야 합니다.");
                        data = new byte[str.Length + 1];
                        Array.Copy(Encoding.Default.GetBytes(str), data, str.Length);
                        data[str.Length] = 0x00;
                        break;
                }
                return data;
            }
            catch (Exception ex)
            {
                OnErrorReceived?.Invoke(ex);
                return null;
            }
        }
    }
}
