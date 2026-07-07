namespace EQX.ThirdParty.Helpers
{
    internal static class DataHelper
    {
        public static byte[] Uint16toByteArray(this ushort source)
        {
            byte[] bytes = new byte[2];

            bytes[0] = (byte)(source & 0x00ff);
            bytes[1] = (byte)(source >> 8);

            return bytes;
        }
    }
}
