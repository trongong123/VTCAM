using System.Text.RegularExpressions;

namespace EQX.Core.Communication.CIM
{
    public class CIMHelpers
    {
        /// <summary>
        /// Lấy địa chỉ Word Address của Parameter dưới dạng chuỗi (String / Hexadecimal)
        /// </summary>
        /// <param name="index">Vị trí của Parameter (Bắt đầu từ 1)</param>
        /// <param name="isCimToPlc">False: Chiều PLC->CIM (W 9224), True: Chiều CIM->PLC (W DF80)</param>
        /// <returns>Địa chỉ Word dạng chuỗi Hexadecimal</returns>
        public static string GetParameterWordAddressString(int index, bool isCimToPlc = false)
        {
            // Gọi hàm tính địa chỉ Decimal ở trên
            int addressDec = GetParameterWordAddressInt(index, isCimToPlc);

            // Chuyển đổi số nguyên sang chuỗi Hexadecimal (Viết hoa, đệm đủ 4 ký tự)
            return addressDec.ToString("X4");
        }

        /// <summary>
        /// Lấy địa chỉ Word Address của Parameter dưới dạng số nguyên (Int / Decimal)
        /// </summary>
        /// <param name="index">Vị trí của Parameter (Bắt đầu từ 1)</param>
        /// <param name="isCimToPlc">False: Chiều PLC->CIM (W 9224), True: Chiều CIM->PLC (W DF80)</param>
        /// <returns>Địa chỉ Word dạng Decimal</returns>
        public static int GetParameterWordAddressInt(int index, bool isCimToPlc = false)
        {
            if (index < 1)
                throw new ArgumentException("Index của Parameter phải bắt đầu từ 1.");

            // Tổng số lượng Parameter tối đa cho phép: (4000 tổng - 22 header) / 2 = 1989
            if (index > 1989)
                throw new ArgumentOutOfRangeException("Index vượt quá giới hạn tối đa cho phép (1989).");

            // Lấy địa chỉ gốc (Base Address) hệ Hex chuyển sang Dec
            // W 9224 (Hex) = 37412 (Dec)
            // W DF80 (Hex) = 57216 (Dec)
            int baseAddressDec = isCimToPlc ? 0xDF80 : 0x9224;

            // Kích thước Header là 22 Words (Mode: 2 + PPID: 20)
            int headerOffset = 22;

            // Mỗi Parameter dài 2 Words
            int parameterLength = 2;

            // Công thức tính địa chỉ = Địa chỉ gốc + Header + ((Index - 1) * 2)
            int targetAddressDec = baseAddressDec + headerOffset + ((index - 1) * parameterLength);

            return targetAddressDec;
        }

        /// <summary>
        /// Lấy địa chỉ Word Address của một PPID trong PPID List dưới dạng số nguyên (Decimal)
        /// </summary>
        /// <param name="index">Vị trí của PPID trong danh sách (Từ 1 đến 100)</param>
        /// <returns>Địa chỉ Word dạng Decimal</returns>
        public static int GetPpidListWordAddressInt(int index)
        {
            // Kiểm tra giới hạn Index (Từ 1 đến tối đa 100 theo MAP)
            if (index < 1 || index > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index của PPID trong PPID List phải nằm trong khoảng từ 1 đến 100.");
            }

            // Địa chỉ gốc (Base Address) của PPID#1 là W 8A54 (Hex)
            int baseAddressDec = 0x8A54;

            // Mỗi chuỗi PPID dài 20 Words
            int ppidLength = 20;

            // Công thức: Địa chỉ = Base Address + ((Index - 1) * 20)
            int targetAddressDec = baseAddressDec + ((index - 1) * ppidLength);

            return targetAddressDec;
        }

        /// <summary>
        /// Lấy địa chỉ Word Address của một PPID trong PPID List dưới dạng chuỗi (Hexadecimal)
        /// </summary>
        /// <param name="index">Vị trí của PPID trong danh sách (Từ 1 đến 100)</param>
        /// <returns>Địa chỉ Word dạng chuỗi Hexadecimal (VD: "8A54")</returns>
        public static string GetPpidListWordAddressString(int index)
        {
            // Gọi hàm tính địa chỉ thập phân ở trên
            int addressDec = GetPpidListWordAddressInt(index);

            // Chuyển đổi số nguyên sang chuỗi Hexadecimal, viết hoa, padding đủ 4 ký tự
            return addressDec.ToString("X4");
        }

        /// <summary>
        /// Kiểm tra chuỗi format và lấy ra mã Recipe.
        /// </summary>
        /// <param name="input">Chuỗi đầu vào (VD: 090_MODEL_ABC)</param>
        /// <param name="recipeNumber">Số recipe lấy được (trả về -1 nếu sai format)</param>
        /// <returns>True nếu đúng format, False nếu sai format</returns>
        public static bool TryParseRecipeNumber(string input, out int recipeNumber)
        {
            recipeNumber = -1;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Pattern giải thích:
            // ^        : Bắt đầu chuỗi
            // (TT_)?   : Group 1 - Có thể có "TT_" hoặc không
            // (\d{3})  : Group 2 - Chính xác 3 chữ số liên tiếp
            // _        : Bắt buộc có dấu gạch dưới sau số
            string pattern = @"^(TT_)?(\d{3})_";

            Match match = Regex.Match(input, pattern);

            if (!match.Success)
                return false;

            // Lấy thông tin từ Regex
            bool hasTTPrefix = match.Groups[1].Success; // Ktra xem có chữ TT_ không
            string numberString = match.Groups[2].Value; // Lấy chuỗi 3 chữ số

            if (int.TryParse(numberString, out int number))
            {
                // Rule 1: Từ 001 -> 090 (KHÔNG CÓ TT_)
                if (!hasTTPrefix && number >= 1 && number <= 90)
                {
                    recipeNumber = number;
                    return true;
                }

                // Rule 2: Từ 091 -> 100 (BẮT BUỘC CÓ TT_)
                if (hasTTPrefix && number >= 91 && number <= 100)
                {
                    recipeNumber = number;
                    return true;
                }
            }

            // Các trường hợp còn lại (VD: 101, hoặc có TT_ nhưng số lại là 050...)
            return false;
        }

        /// <summary>
        /// Lấy địa chỉ Word Address của một ECM dưới dạng số nguyên (Decimal)
        /// </summary>
        /// <param name="index">Vị trí của ECM (Bắt đầu từ 1)</param>
        /// <param name="ecmLength">Độ dài Word của 1 item ECM (Thay đổi theo Map thực tế, mặc định giả định là 2)</param>
        /// <returns>Địa chỉ Word dạng Decimal</returns>
        public static int GetEcmWordAddressInt(int index, int ecmLength = 2)
        {
            if (index < 1)
                throw new ArgumentOutOfRangeException(nameof(index), "Index của ECM phải bắt đầu từ 1.");

            // Địa chỉ gốc (Base Address) của dải ECMPARAMETER là W A1C4 (Hex)
            int baseAddressDec = 0xA1C4;

            // Tổng chiều dài dải nhớ ECM là 3600 Words
            int maxTotalWords = 3600;

            // Công thức tính: Địa chỉ = Địa chỉ gốc + ((Index - 1) * Độ dài 1 ECM)
            int targetAddressDec = baseAddressDec + ((index - 1) * ecmLength);

            // Kiểm tra an toàn: Đảm bảo địa chỉ tính ra không vượt quá vùng nhớ cho phép (A1C4 + 3600)
            if (targetAddressDec >= baseAddressDec + maxTotalWords)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index vượt quá giới hạn tổng dung lượng 3600 Words của dải nhớ ECM.");
            }

            return targetAddressDec;
        }

        /// <summary>
        /// Lấy địa chỉ Word Address của một ECM dưới dạng chuỗi (Hexadecimal)
        /// </summary>
        /// <param name="index">Vị trí của ECM (Bắt đầu từ 1)</param>
        /// <param name="ecmLength">Độ dài Word của 1 item ECM</param>
        /// <returns>Địa chỉ Word dạng chuỗi Hexadecimal (VD: "A1C4")</returns>
        public static string GetEcmWordAddressString(int index, int ecmLength = 2)
        {
            // Gọi hàm tính địa chỉ thập phân ở trên
            int addressDec = GetEcmWordAddressInt(index, ecmLength);

            // Chuyển đổi số nguyên sang chuỗi Hexadecimal, viết hoa, đệm đủ 4 ký tự
            return addressDec.ToString("X4");
        }
    }
}
