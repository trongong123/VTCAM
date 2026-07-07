using System.Text;

namespace EQX.Core.Communication.CIM.Custom.WordArea
{
    public static class PlcDataConverter
    {
        // Đọc String (ASCII) từ mảng short[]
        public static string GetString(short[] data, int wordOffset, int wordLength)
        {
            byte[] bytes = new byte[wordLength * 2];
            Buffer.BlockCopy(data, wordOffset * 2, bytes, 0, bytes.Length);
            // Cắt bỏ các ký tự null (\0) hoặc khoảng trắng thừa ở cuối chuỗi
            return Encoding.ASCII.GetString(bytes).TrimEnd('\0', ' ');
        }

        // Ghi String (ASCII) vào mảng short[]
        public static void SetString(short[] data, int wordOffset, int wordLength, string value)
        {
            byte[] bytes = new byte[wordLength * 2];
            if (!string.IsNullOrEmpty(value))
            {
                byte[] strBytes = Encoding.ASCII.GetBytes(value);
                Array.Copy(strBytes, bytes, Math.Min(strBytes.Length, bytes.Length));
            }
            Buffer.BlockCopy(bytes, 0, data, wordOffset * 2, bytes.Length);
        }

        // Đọc số nguyên 32-bit (DEC, Length = 2 Words)
        public static int GetInt32(short[] data, int wordOffset)
        {
            // Theo tài liệu Mitsubishi (mdReceiveEx), Word đầu là Lower, Word sau là Upper [3, 4]
            ushort lower = (ushort)data[wordOffset];
            ushort upper = (ushort)data[wordOffset + 1];
            return (upper << 16) | lower;
        }

        // Ghi số nguyên 32-bit (DEC, Length = 2 Words)
        public static void SetInt32(short[] data, int wordOffset, int value)
        {
            data[wordOffset] = (short)(value & 0xFFFF);           // Lower word
            data[wordOffset + 1] = (short)((value >> 16) & 0xFFFF); // Upper word
        }

        public static short GetInt16(short[] data, int wordOffset)
        {
            return data[wordOffset];
        }

        public static void SetInt16(short[] data, int wordOffset, short value)
        {
            data[wordOffset] = value;
        }
    }

}
