using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TOPENG_Device;

namespace EQX.Core.Communication.CIM.Custom.WordArea
{
    public interface ICIMArea
    {
        void FromCIMData(short[] data);
    }

    public class SystemTimeSetter
    {
        // Cấu trúc hệ thống yêu cầu bởi Windows API
        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        // Import hàm SetLocalTime (thiết lập theo giờ máy tính hiện tại)
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetLocalTime(ref SystemTime st);

        public static void SetTimeFromString(string timeStr)
        {
            // 1. Parse chuỗi với định dạng chính xác
            if (DateTime.TryParseExact(timeStr, "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime targetDate))
            {
                // 2. Chuyển đổi sang cấu trúc SystemTime
                SystemTime st = new SystemTime
                {
                    wYear = (ushort)targetDate.Year,
                    wMonth = (ushort)targetDate.Month,
                    wDay = (ushort)targetDate.Day,
                    wHour = (ushort)targetDate.Hour,
                    wMinute = (ushort)targetDate.Minute,
                    wSecond = (ushort)targetDate.Second,
                    wMilliseconds = 0 // Chuỗi không có ms, mặc định là 0
                };

                // 3. Gọi lệnh hệ thống
                if (SetLocalTime(ref st))
                {
                    Debug.WriteLine($"Thành công! Thời gian hệ thống đã đổi thành: {targetDate}");
                }
                else
                {
                    int error = Marshal.GetLastWin32Error();
                    Debug.WriteLine($"Thất bại. Mã lỗi Win32: {error}");
                    if (error == 5) Console.WriteLine("Lưu ý: Bạn cần chạy ứng dụng với quyền Administrator!");
                }
            }
            else
            {
                Debug.WriteLine("Định dạng chuỗi không hợp lệ. Vui lòng dùng: YYYYMMDDhhmmss");
            }
        }
    }

    // --- Chiều CIM gửi xuống C# (Start: D000, Length: 7) ---
    public class DatetimeSetArea
    {
        public const int TOTAL_WORDS = 7;

        // Định dạng: YYYYMMDDhhmmss
        public string Datetime { get; set; }

        // Đọc Data do CIM gửi xuống C#
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException("Độ dài dữ liệu không hợp lệ.");

            Datetime = PlcDataConverter.GetString(data, 0, TOTAL_WORDS);
        }

        // Đóng gói Data (dùng khi C# cần giả lập CIM gửi lệnh xuống)
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            PlcDataConverter.SetString(data, 0, TOTAL_WORDS, Datetime);
            return data;
        }
    }

    // --- Chiều C# (PLC) gửi thông tin Material lên CIM ---
    // Sử dụng chung cho:
    // - MaterialInfoSend1 (Start: D503, Length: 230)
    // - MaterialInfoSend2 (Start: D5F3, Length: 230)
    // - MaterialInfoSend3 (Start: D6E3, Length: 230)
    public class MaterialInfoSendCimtoPLCArea
    {
        public const int TOTAL_WORDS = 230;

        // --- Nhóm thông tin cơ bản (ASCII) ---
        public string MaterialEqpID { get; set; }           // Length 20 (D503)
        public string MaterialBatchID { get; set; }         // Length 20
        public string MaterialCode { get; set; }            // Length 10
        public string MaterialUseDate { get; set; }         // Length 7
        public string MaterialDiseaseDate { get; set; }     // Length 7
        public string MaterialMaker { get; set; }           // Length 10
        public string MaterialValidationFlag { get; set; }  // Length 2
        public string MaterialDefectCode { get; set; }      // Length 2
        public string MaterialComment { get; set; }         // Length 20
        public string MaterialID { get; set; }              // Length 40
        public string MaterialType { get; set; }            // Length 10
        public string MaterialST { get; set; }              // Length 1
        public string MaterialProtID { get; set; }          // Length 1
        public string MaterialState { get; set; }           // Length 1

        // --- Nhóm thông tin số lượng (DEC - 2 Words = 32bit Int) ---
        public int MaterialTotalQTY { get; set; }           // Length 2 
        public int MaterialUseQTY { get; set; }             // Length 2 
        public int MaterialAssemQTY { get; set; }           // Length 2 
        public int MaterialNGQTY { get; set; }              // Length 2 
        public int MaterialRemainQTY { get; set; }          // Length 2 
        public int MaterialProceUseQTY { get; set; }        // Length 2 

        // --- Nhóm thông tin phản hồi (ASCII) ---
        public string MaterialReplyStatus { get; set; }     // Length 2
        public string MaterialReplyCode { get; set; }       // Length 5
        public string MaterialReplyText { get; set; }       // Length 60

        // Hàm đọc Data (Dùng khi C# cần parse lại gói tin nội bộ hoặc debug)
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException("Độ dài mảng dữ liệu không hợp lệ.");

            int offset = 0;

            MaterialEqpID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            MaterialBatchID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            MaterialCode = PlcDataConverter.GetString(data, offset, 10); offset += 10;
            MaterialUseDate = PlcDataConverter.GetString(data, offset, 7); offset += 7;
            MaterialDiseaseDate = PlcDataConverter.GetString(data, offset, 7); offset += 7;
            MaterialMaker = PlcDataConverter.GetString(data, offset, 10); offset += 10;
            MaterialValidationFlag = PlcDataConverter.GetString(data, offset, 2); offset += 2;
            MaterialDefectCode = PlcDataConverter.GetString(data, offset, 2); offset += 2;
            MaterialComment = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            MaterialID = PlcDataConverter.GetString(data, offset, 40); offset += 40;
            MaterialType = PlcDataConverter.GetString(data, offset, 10); offset += 10;
            MaterialST = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            MaterialProtID = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            MaterialState = PlcDataConverter.GetString(data, offset, 1); offset += 1;

            // Đọc các giá trị DEC (Số lượng)
            MaterialTotalQTY = PlcDataConverter.GetInt32(data, offset); offset += 2;
            MaterialUseQTY = PlcDataConverter.GetInt32(data, offset); offset += 2;
            MaterialAssemQTY = PlcDataConverter.GetInt32(data, offset); offset += 2;
            MaterialNGQTY = PlcDataConverter.GetInt32(data, offset); offset += 2;
            MaterialRemainQTY = PlcDataConverter.GetInt32(data, offset); offset += 2;
            MaterialProceUseQTY = PlcDataConverter.GetInt32(data, offset); offset += 2;

            MaterialReplyStatus = PlcDataConverter.GetString(data, offset, 2); offset += 2;
            MaterialReplyCode = PlcDataConverter.GetString(data, offset, 5); offset += 5;
            MaterialReplyText = PlcDataConverter.GetString(data, offset, 60); offset += 60;
        }
    }

    // --- Chiều C# (PLC) gửi thông tin Material Kitting/Cancel lên CIM ---
    // Sự kiện: KittingorCancel1 (BIT: B107E)
    // Dải địa chỉ: 7B14 ~ 7B85 (Chiều dài: 114 Words)
    public class MaterialKittingPlcToCimArea : ObservableObject
    {
        public const int TOTAL_WORDS = 114;

        public short CEID { get; set; }                   // Length 1 

        // --- Nhóm thông tin cơ bản (ASCII) ---
        public string BatchID { get; set; }               // Length 20

        // Get-only property: Tự động lấy 10 ký tự đầu của BatchID
        public string BatchName
        {
            get
            {
                if (string.IsNullOrEmpty(BatchID)) return string.Empty;
                return BatchID.Length >= 10 ? BatchID.Substring(0, 10) : BatchID;
            }
        }

        public string MaterialID => BatchID;                    // Length 40
        public string Type { get; set; }                  // Length 10
        // 자재 상태 정보
        //(1: Mount, 2: Emptied, 3: Unmount, 4: Failed)
        public string MaterialState { get; set; }                    // Length 1 
        public string PortID { get; set; }                // Length 1
        // 자재 가용 상태 정보
        // (1: Enable, 2: Active, 3: Disable)
        public string State { get; set; }                 // Length 1 

        // --- Nhóm thông tin số lượng (DEC) ---
        public int TotalQty { get; set; }                 // Length 2 
        public int UseQty { get; set; }                   // Length 2 
        public int AssemQty { get; set; }                 // Length 2 
        public int NGQty { get; set; }                    // Length 2 
        public int RemainQty { get; set; }                // Length 2 
        public int ProductQty { get; set; }               // Length 2 
        public int ProcessUseQty { get; set; }            // Length 2 
        public int ProcessAssemQty { get; set; }          // Length 2 
        public int ProcessNGQty { get; set; }             // Length 2 
        public int SupplyReqQty { get; set; }             // Length 2 

        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException("Độ dài mảng dữ liệu không hợp lệ.");

            int offset = 0;

            CEID = PlcDataConverter.GetInt16(data, offset); offset += 1;

            BatchID = PlcDataConverter.GetString(data, offset, 20); offset += 20;

            // Không gán vào BatchName (vì là get-only), nhưng phải cộng dồn offset để bỏ qua dải 20 words này
            offset += 20;

            // Không gán vào ID (vì là get-only), nhưng phải cộng dồn offset để bỏ qua dải 40 words này
            offset += 40;
            Type = PlcDataConverter.GetString(data, offset, 10); offset += 10;
            MaterialState = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            PortID = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            State = PlcDataConverter.GetString(data, offset, 1); offset += 1;

            TotalQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
            UseQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
            AssemQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
            NGQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
            RemainQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
            ProductQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
            ProcessUseQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
            ProcessAssemQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
            ProcessNGQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
            SupplyReqQty = PlcDataConverter.GetInt32(data, offset); offset += 2;
        }

        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetInt16(data, offset, CEID); offset += 1;

            PlcDataConverter.SetString(data, offset, 20, BatchID); offset += 20;
            // BatchName (Property get) sẽ tự động sinh ra chuỗi 10 ký tự từ BatchID để ghi xuống CIM
            PlcDataConverter.SetString(data, offset, 20, BatchName); offset += 20;

            PlcDataConverter.SetString(data, offset, 40, MaterialID); offset += 40;
            PlcDataConverter.SetString(data, offset, 10, Type); offset += 10;
            PlcDataConverter.SetString(data, offset, 1, MaterialState); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, PortID); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, State); offset += 1;

            PlcDataConverter.SetInt32(data, offset, TotalQty); offset += 2;
            PlcDataConverter.SetInt32(data, offset, UseQty); offset += 2;
            PlcDataConverter.SetInt32(data, offset, AssemQty); offset += 2;
            PlcDataConverter.SetInt32(data, offset, NGQty); offset += 2;
            PlcDataConverter.SetInt32(data, offset, RemainQty); offset += 2;
            PlcDataConverter.SetInt32(data, offset, ProductQty); offset += 2;
            PlcDataConverter.SetInt32(data, offset, ProcessUseQty); offset += 2;
            PlcDataConverter.SetInt32(data, offset, ProcessAssemQty); offset += 2;
            PlcDataConverter.SetInt32(data, offset, ProcessNGQty); offset += 2;
            PlcDataConverter.SetInt32(data, offset, SupplyReqQty); offset += 2;

            return data;
        }
    }

    // --- Chiều CIM gửi nội dung hiển thị xuống C# (Start: D011, Length: 61) ---
    public class TerminalDisplayCimToPlcArea
    {
        public const int TOTAL_WORDS = 61;

        public string TerminalDisplayText { get; set; } // Length 60
        public string TerminalNumber { get; set; }      // Length 1

        // Đọc Data do CIM gửi xuống
        public void FromCIMData(short[] data)
        {
            TerminalDisplayText = PlcDataConverter.GetString(data, 0, 60);
            TerminalNumber = PlcDataConverter.GetString(data, 60, 1);
        }

        // Đóng gói Data (dùng khi C# cần giả lập CIM)
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            PlcDataConverter.SetString(data, 0, 60, TerminalDisplayText);
            PlcDataConverter.SetString(data, 60, 1, TerminalNumber);
            return data;
        }
    }

    // --- Chiều C# gửi nội dung nhập từ màn hình lên CIM (Start: 1086, Length: 60) ---
    public class TerminalDisplayPlcToCimArea
    {
        public const int TOTAL_WORDS = 60;

        public string TerminalDisplaySnd { get; set; } // Length 60

        // Đọc Data (dùng khi C# cần parse lại gói tin nội bộ)
        public void FromCIMData(short[] data)
        {
            TerminalDisplaySnd = PlcDataConverter.GetString(data, 0, 60);
        }

        // Đóng gói Data để C# gửi lên CIM
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            PlcDataConverter.SetString(data, 0, 60, TerminalDisplaySnd);
            return data;
        }
    }

    // --- Chiều CIM gửi xuống C# (Start: D058, Length: 70) ---
    public class OperatorCallCimToPlcArea
    {
        public const int TOTAL_WORDS = 70;

        public string OperatorCallID { get; set; }   // Length 10
        public string OperatorCallText { get; set; } // Length 60

        // Đọc Data do CIM gửi xuống
        public void FromCIMData(short[] data)
        {
            OperatorCallID = PlcDataConverter.GetString(data, 0, 10);
            OperatorCallText = PlcDataConverter.GetString(data, 10, 60);
        }

        // Đóng gói Data (dùng khi C# cần giả lập CIM hoặc phản hồi nội bộ)
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            PlcDataConverter.SetString(data, 0, 10, OperatorCallID);
            PlcDataConverter.SetString(data, 10, 60, OperatorCallText);
            return data;
        }
    }

    // --- Chiều C# phản hồi lên CIM (Start: F30, Length: 150) ---
    public class OperatorCallPlcToCimArea
    {
        public const int TOTAL_WORDS = 150;

        public string OPCallCellIDConfirm { get; set; }     // Length 40
        public string OPCallProductIDConfirm { get; set; }  // Length 20
        public string OPCallStepIDConfirm { get; set; }     // Length 20
        public string OPCallIDComfirm { get; set; }         // Length 10
        public string OPCallMessageConfirm { get; set; }    // Length 60

        public void FromCIMData(short[] data)
        {
            OPCallCellIDConfirm = PlcDataConverter.GetString(data, 0, 40);
            OPCallProductIDConfirm = PlcDataConverter.GetString(data, 40, 20);
            OPCallStepIDConfirm = PlcDataConverter.GetString(data, 60, 20);
            OPCallIDComfirm = PlcDataConverter.GetString(data, 80, 10);
            OPCallMessageConfirm = PlcDataConverter.GetString(data, 90, 60);
        }

        // Đóng gói Data để C# gửi lên CIM
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            PlcDataConverter.SetString(data, 0, 40, OPCallCellIDConfirm);
            PlcDataConverter.SetString(data, 40, 20, OPCallProductIDConfirm);
            PlcDataConverter.SetString(data, 60, 20, OPCallStepIDConfirm);
            PlcDataConverter.SetString(data, 80, 10, OPCallIDComfirm);
            PlcDataConverter.SetString(data, 90, 60, OPCallMessageConfirm);
            return data;
        }
    }

    // --- Chiều CIM gửi xuống C# (Start: D09E, Length: 71) ---
    public class InterlockCimToPlcArea
    {
        public const int TOTAL_WORDS = 71;

        public string InterlockID { get; set; }      // Length 10
        public string InterlockMessage { get; set; } // Length 60
        public string InterlockRCMD { get; set; }    // Length 1

        public void FromCIMData(short[] data)
        {
            InterlockID = PlcDataConverter.GetString(data, 0, 10);
            InterlockMessage = PlcDataConverter.GetString(data, 10, 60);
            InterlockRCMD = PlcDataConverter.GetString(data, 70, 1);
        }

        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            PlcDataConverter.SetString(data, 0, 10, InterlockID);
            PlcDataConverter.SetString(data, 10, 60, InterlockMessage);
            PlcDataConverter.SetString(data, 70, 1, InterlockRCMD);
            return data;
        }
    }

    // --- Chiều C# phản hồi lên CIM (Start: FF0, Length: 150) ---
    public class InterlockPlcToCimArea
    {
        public const int TOTAL_WORDS = 150;

        public string InterlockCellIDConfirm { get; set; }     // Length 40
        public string InterlockProductIDConfirm { get; set; }  // Length 20
        public string InterlockStepIDConfirm { get; set; }     // Length 20
        public string InterlockIDComfirm { get; set; }         // Length 10
        public string InterlockMessageConfirm { get; set; }    // Length 60

        public void FromCIMData(short[] data)
        {
            InterlockCellIDConfirm = PlcDataConverter.GetString(data, 0, 40);
            InterlockProductIDConfirm = PlcDataConverter.GetString(data, 40, 20);
            InterlockStepIDConfirm = PlcDataConverter.GetString(data, 60, 20);
            InterlockIDComfirm = PlcDataConverter.GetString(data, 80, 10);
            InterlockMessageConfirm = PlcDataConverter.GetString(data, 90, 60);
        }

        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            PlcDataConverter.SetString(data, 0, 40, InterlockCellIDConfirm);
            PlcDataConverter.SetString(data, 40, 20, InterlockProductIDConfirm);
            PlcDataConverter.SetString(data, 60, 20, InterlockStepIDConfirm);
            PlcDataConverter.SetString(data, 80, 10, InterlockIDComfirm);
            PlcDataConverter.SetString(data, 90, 60, InterlockMessageConfirm);
            return data;
        }
    }

    // Lưu ý: CimToPlc không có data, chỉ có C# đẩy data lên CIM

    // --- Chiều C# phản hồi danh sách lên CIM (Start: 8A54, Length: 2000) ---
    public class CurrentEquipPPIDListPlcToCimArea
    {
        public const int TOTAL_WORDS = 2000;
        public string PPIDLIST { get; set; } // Length 2000 

        public void FromCIMData(short[] data)
        {
            PPIDLIST = PlcDataConverter.GetString(data, 0, 2000);
        }

        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            PlcDataConverter.SetString(data, 0, 2000, PPIDLIST);
            return data;
        }
    }

    // Lưu ý: CimToPlc không có data, chỉ có C# đẩy data lên CIM

    // --- Chiều C# phản hồi danh sách lên CIM (Start: A1C4, Length: 3600) ---
    public class EquipConstantNameListPlcToCimArea
    {
        public const int TOTAL_WORDS = 3600;
        public string ECMPARAMETER { get; set; } // Length 3600

        public void FromCIMData(short[] data)
        {
            ECMPARAMETER = PlcDataConverter.GetString(data, 0, 3600);
        }

        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            PlcDataConverter.SetString(data, 0, 3600, ECMPARAMETER);
            return data;
        }
    }

    // --- Lớp đại diện cho vùng nhớ của 1 Port vật tư đơn lẻ (Chiều dài: 53 Words) ---
    public class MaterialPortStateItemArea
    {
        public const int TOTAL_WORDS = 53;

        // --- Cấu trúc dữ liệu ---
        public string Type { get; set; }       // Length 10 (ASCII) - Phân loại vật tư (CELL, POL, IC, ACF,...)
        public string LST { get; set; }        // Length 1 (ASCII) - Trạng thái: 1(Mount), 2(Emptied), 3(Unmount), 4(Filled)
        public string ID { get; set; }         // Length 40 (ASCII) - ID của vật tư
        public short LoaderNo { get; set; }    // Length 1 (DEC) - Số Loader (1,2,3,4...)
        public short Usage { get; set; }       // Length 1 (DEC) - Lượng vật tư còn lại hoặc số lần sử dụng

        // --- Hàm đọc Data từ mảng short[] PLC nhận được (Độ dài 53) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException("Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất 53 Words.");

            int offset = 0;

            Type = PlcDataConverter.GetString(data, offset, 10); offset += 10;
            LST = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            ID = PlcDataConverter.GetString(data, offset, 40); offset += 40;

            // Đọc dữ liệu DEC (1 Word = 16 bit integer)
            LoaderNo = PlcDataConverter.GetInt16(data, offset); offset += 1;
            Usage = PlcDataConverter.GetInt16(data, offset); offset += 1;
        }

        // --- Hàm đóng gói Data để C# ghi xuống PLC (Độ dài 53) ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetString(data, offset, 10, Type); offset += 10;
            PlcDataConverter.SetString(data, offset, 1, LST); offset += 1;
            PlcDataConverter.SetString(data, offset, 40, ID); offset += 40;

            // Ghi dữ liệu DEC
            PlcDataConverter.SetInt16(data, offset, LoaderNo); offset += 1;
            PlcDataConverter.SetInt16(data, offset, Usage); offset += 1;

            return data;
        }
    }

    // --- Vùng nhớ MaterialPortState (Start: 160, Length: 424 Words) ---
    public class MaterialPortStateArea
    {
        // 53 Words/Port * 8 Ports = 424 Words
        public const int TOTAL_WORDS = 424;

        // Mảng chứa thông tin 8 Port (Index 0 tương ứng Port 1, Index 7 tương ứng Port 8)
        public MaterialPortStateItemArea[] Ports { get; set; }

        // Khởi tạo mảng khi new class
        public MaterialPortStateArea()
        {
            Ports = new MaterialPortStateItemArea[8];
            for (int i = 0; i < 8; i++)
            {
                Ports[i] = new MaterialPortStateItemArea();
            }
        }

        // Hàm đọc Data do PLC C# nhận được hoặc parse lại từ dải nhớ
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException("Độ dài mảng dữ liệu không hợp lệ.");

            int offset = 0;

            // Quét vòng lặp tự động bóc tách data cho 8 Port
            for (int i = 0; i < 8; i++)
            {
                Ports[i].Type = PlcDataConverter.GetString(data, offset, 10); offset += 10;
                Ports[i].LST = PlcDataConverter.GetString(data, offset, 1); offset += 1;
                Ports[i].ID = PlcDataConverter.GetString(data, offset, 40); offset += 40;

                // Đọc dữ liệu DEC (1 Word = 16 bit integer)
                Ports[i].LoaderNo = PlcDataConverter.GetInt16(data, offset); offset += 1;
                Ports[i].Usage = PlcDataConverter.GetInt16(data, offset); offset += 1;
            }
        }

        // Hàm đóng gói Data để C# ghi xuống dải W 160
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            for (int i = 0; i < 8; i++)
            {
                PlcDataConverter.SetString(data, offset, 10, Ports[i].Type); offset += 10;
                PlcDataConverter.SetString(data, offset, 1, Ports[i].LST); offset += 1;
                PlcDataConverter.SetString(data, offset, 40, Ports[i].ID); offset += 40;

                // Ghi dữ liệu DEC
                PlcDataConverter.SetInt16(data, offset, Ports[i].LoaderNo); offset += 1;
                PlcDataConverter.SetInt16(data, offset, Ports[i].Usage); offset += 1;
            }

            return data;
        }
    }

    // --- Chiều C# (PLC) gửi yêu cầu Specific Validation lên CIM ---
    // Sử dụng chung cho:
    // - SpecificValidationRequest1 (Start: 2980, Length: 45)
    // - SpecificValidationRequest2 (Start: 29AD, Length: 45)
    // - SpecificValidationRequest3 (Start: 29DA, Length: 45)
    // - SpecificValidationRequest4 (Start: 2A07, Length: 45)
    // - SpecificValidationRequest5 (Start: 2A34, Length: 45)
    public class SpecificValidationRequestArea
    {
        public const int TOTAL_WORDS = 45;

        public string OptionCode { get; set; }   // Length 5 (ASCII) - Ví dụ: "CELL", "PRETRAY"
        public string CellID { get; set; }       // Length 20 (ASCII) - Unique ID của Item
        public string OptionInfo { get; set; }   // Length 20 (ASCII) - Dữ liệu mở rộng

        // Hàm đọc Data (Dùng khi C# cần parse lại gói tin nội bộ hoặc debug)
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            int offset = 0;

            OptionCode = PlcDataConverter.GetString(data, offset, 5); offset += 5;
            CellID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            OptionInfo = PlcDataConverter.GetString(data, offset, 20); offset += 20;
        }

        // Hàm đóng gói Data để C# ghi xuống PLC (Gửi lên CIM)
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetString(data, offset, 5, OptionCode); offset += 5;
            PlcDataConverter.SetString(data, offset, 20, CellID); offset += 20;
            PlcDataConverter.SetString(data, offset, 20, OptionInfo); offset += 20;

            return data;
        }
    }

    // --- Chiều CIM phản hồi dữ liệu Specific Validation xuống C# (PLC) ---
    // Sử dụng chung cho 5 luồng:
    // - SpecificValidationDataSend1 (Start: 11256, Length: 172, Bit: B10DE)
    // - SpecificValidationDataSend2 (Start: 11302, Length: 172, Bit: B10DF)
    // - SpecificValidationDataSend3 (Start: 113AE, Length: 172, Bit: B10E0)
    // - SpecificValidationDataSend4 (Start: 1145A, Length: 172, Bit: B10E1)
    // - SpecificValidationDataSend5 (Start: 11506, Length: 172, Bit: B10E2)
    public class SpecificValidationDataSendArea
    {
        public const int TOTAL_WORDS = 172;

        // --- Cấu trúc dữ liệu (Toàn bộ là ASCII) ---
        public string CarrierID { get; set; }     // Length 20 
        public string CellID { get; set; }        // Length 40 
        public string UniqueType { get; set; }    // Length 10 
        public string ProductID { get; set; }     // Length 20 
        public string StepID { get; set; }        // Length 20 
        public string ReplyStatus { get; set; }   // Length 2  (Trạng thái kết quả)
        public string ReplyText { get; set; }     // Length 60 (Nội dung chi tiết trả về)

        // --- Hàm đọc Data do CIM gửi xuống C# ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            int offset = 0;

            CarrierID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            CellID = PlcDataConverter.GetString(data, offset, 40); offset += 40;
            UniqueType = PlcDataConverter.GetString(data, offset, 10); offset += 10;
            ProductID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            StepID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            ReplyStatus = PlcDataConverter.GetString(data, offset, 2); offset += 2;
            ReplyText = PlcDataConverter.GetString(data, offset, 60); offset += 60;
        }

        // --- Hàm đóng gói Data (Dùng khi C# cần giả lập CIM hoặc debug nội bộ) ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetString(data, offset, 20, CarrierID); offset += 20;
            PlcDataConverter.SetString(data, offset, 40, CellID); offset += 40;
            PlcDataConverter.SetString(data, offset, 10, UniqueType); offset += 10;
            PlcDataConverter.SetString(data, offset, 20, ProductID); offset += 20;
            PlcDataConverter.SetString(data, offset, 20, StepID); offset += 20;
            PlcDataConverter.SetString(data, offset, 2, ReplyStatus); offset += 2;
            PlcDataConverter.SetString(data, offset, 60, ReplyText); offset += 60;

            return data;
        }
    }

    // --- Chiều C# (PLC) gửi thông tin Track In (Cell Start) lên CIM ---
    // Sử dụng chung cho các Event:
    // - CellStartPort1 (Start: 4A0, Length: 107)
    // - CellStartPort2 (Start: 510, Length: 107)
    // - CellStartPort3 (Start: 580, Length: 107)
    // - CellStartPort4 (Start: 5F0, Length: 107)
    public class CellStartPortArea
    {
        public const int TOTAL_WORDS = 107;

        // --- Nhóm thông tin cơ bản (ASCII) ---
        public string TrackInCellID { get; set; }           // Length 40 
        public string TrackInProductID { get; set; }        // Length 20 
        public string TrackInStepID { get; set; }           // Length 20 
        public string TrackInProcessJobID { get; set; }     // Length 20 

        // --- Nhóm số lượng (DEC - 2 Words = 32bit Int) ---
        public int TrackInPlanQuantity { get; set; }        // Length 2 
        public int TrackInProcessQuantity { get; set; }     // Length 2 

        // --- Nhóm trạng thái Reader (ASCII) ---
        // Reader ID: 공백(Trống)=Không dùng, 1=MCR #1, 2=MCR #2, 3=MCR #3, 9=EQ To EQ
        public string TrackInReaderID { get; set; }         // Length 1 

        // RRC: 0=OK, 1=Error, 2=Không dùng H/W, 3=Không dùng Cell Tracking
        public string TrackInRRC { get; set; }              // Length 1 
        public string TrackInReasonCode { get; set; }       // Length 1 

        // --- Hàm đọc Data (Dùng khi C# cần parse lại gói tin hoặc test nội bộ) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            int offset = 0;

            TrackInCellID = PlcDataConverter.GetString(data, offset, 40); offset += 40;
            TrackInProductID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            TrackInStepID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            TrackInProcessJobID = PlcDataConverter.GetString(data, offset, 20); offset += 20;

            // Đọc dữ liệu DEC 32-bit
            TrackInPlanQuantity = PlcDataConverter.GetInt32(data, offset); offset += 2;
            TrackInProcessQuantity = PlcDataConverter.GetInt32(data, offset); offset += 2;

            TrackInReaderID = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            TrackInRRC = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            TrackInReasonCode = PlcDataConverter.GetString(data, offset, 1); offset += 1;
        }

        // --- Hàm đóng gói Data để C# ghi xuống PLC (Gửi lên CIM) ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetString(data, offset, 40, TrackInCellID); offset += 40;
            PlcDataConverter.SetString(data, offset, 20, TrackInProductID); offset += 20;
            PlcDataConverter.SetString(data, offset, 20, TrackInStepID); offset += 20;
            PlcDataConverter.SetString(data, offset, 20, TrackInProcessJobID); offset += 20;

            // Ghi dữ liệu DEC 32-bit
            PlcDataConverter.SetInt32(data, offset, TrackInPlanQuantity); offset += 2;
            PlcDataConverter.SetInt32(data, offset, TrackInProcessQuantity); offset += 2;

            PlcDataConverter.SetString(data, offset, 1, TrackInReaderID); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, TrackInRRC); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, TrackInReasonCode); offset += 1;

            return data;
        }
    }

    // --- Chiều CIM gửi thông tin Cell Job Process xuống C# (PLC) ---
    // Sử dụng chung cho:
    // - CellJobProcess1 (Start: D339, Length: 112, Bit: B1049)
    // - CellJobProcess2 (Start: D3A9, Length: 112, Bit: B104A)
    // - CellJobProcess3 (Start: D419, Length: 112, Bit: B104B)
    // - CellJobProcess4 (Start: D489, Length: 112, Bit: B104C)
    public class CellJobProcessCimToPlcArea
    {
        public const int TOTAL_WORDS = 112;

        // --- Cấu trúc dữ liệu (Toàn bộ là ASCII) ---
        // RCMD: 21(Start), 22(Cancel), 23(Pause), 24(Resume)
        public string CellJobProcessRCMD { get; set; }       // Length 2
        public string CellJobProcessJobID { get; set; }      // Length 20
        public string CellJobProcessCellID { get; set; }     // Length 40
        public string CellJobProcessProductID { get; set; }  // Length 20
        public string CellJobProcessStepID { get; set; }     // Length 20
        public string CellJobProcessActionType { get; set; } // Length 10

        public string UserDefine_TrackOutDescription { get; set; }
        public string UserDefine_TrackOutJudge { get; set; }

        // --- Hàm đọc Data do CIM gửi xuống ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            int offset = 0;

            CellJobProcessRCMD = PlcDataConverter.GetString(data, offset, 2); offset += 2;
            CellJobProcessJobID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            CellJobProcessCellID = PlcDataConverter.GetString(data, offset, 40); offset += 40;
            CellJobProcessProductID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            CellJobProcessStepID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            CellJobProcessActionType = PlcDataConverter.GetString(data, offset, 10); offset += 10;
        }

        // --- Hàm đóng gói Data (Dùng khi C# cần giả lập CIM) ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetString(data, offset, 2, CellJobProcessRCMD); offset += 2;
            PlcDataConverter.SetString(data, offset, 20, CellJobProcessJobID); offset += 20;
            PlcDataConverter.SetString(data, offset, 40, CellJobProcessCellID); offset += 40;
            PlcDataConverter.SetString(data, offset, 20, CellJobProcessProductID); offset += 20;
            PlcDataConverter.SetString(data, offset, 20, CellJobProcessStepID); offset += 20;
            PlcDataConverter.SetString(data, offset, 10, CellJobProcessActionType); offset += 10;

            return data;
        }
    }

    // --- Chiều C# (PLC) gửi thông tin Track Out (Cell Process Complete) lên CIM ---
    // Sử dụng chung cho các Event:
    // - CellCompPort1 (Start: 760, Length: 192)
    // - CellCompPort2 (Start: 820, Length: 192)
    // - CellCompPort3 (Start: 8E0, Length: 192)
    // - CellCompPort4 (Start: 9A0, Length: 192)
    public class CellCompPortArea
    {
        public const int TOTAL_WORDS = 177;

        // --- Nhóm thông tin cơ bản (ASCII) ---
        public string TrackOutCellID { get; set; }           // Length 40
        public string TrackOutProductID { get; set; }        // Length 20
        public string TrackOutStepID { get; set; }           // Length 20
        public string TrackOutProcessJobID { get; set; }     // Length 20

        // --- Nhóm số lượng (DEC - 2 Words = 32bit Int) ---
        public int TrackOutPlanQuantity { get; set; }        // Length 2
        public int TrackOutProcessQuantity { get; set; }     // Length 2

        // --- Nhóm trạng thái Reader & Điều khiển (ASCII) ---
        public string TrackOutReaderID { get; set; }         // Length 1 (MCR#1: 1, MCR#2: 2, v.v...)
        public string TrackOutRRC { get; set; }              // Length 1 (0: OK, 1: Error, 2: Reader 미사용)

        // --- Nhóm Operator (ASCII) ---
        public string TrackOutOperatorID_1 { get; set; }     // Length 10
        public string TrackOutOperatorID_2 { get; set; }     // Length 10
        public string TrackOutOperatorID_3 { get; set; }     // Length 10

        // --- Nhóm kết quả đánh giá (ASCII) ---
        public string TrackOutJudge { get; set; }            // Length 1 (G: Good, N: No Good, R: Retest, v.v...)

        // Reserve Length 10 ở giữa (W 7E9 ~ 7F2) - Không tạo Property, chỉ cộng dồn Offset

        public string TrackOutReasonCode { get; set; }       // Length 10
        public string TrackOutDescription { get; set; }      // Length 20

        // Reserve Length 15 ở cuối (W 811 ~ 81F) - Chỉ cộng dồn Offset để khớp 192 Words

        // --- Hàm đọc Data ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            int offset = 0;

            TrackOutCellID = PlcDataConverter.GetString(data, offset, 40); offset += 40;
            TrackOutProductID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            TrackOutStepID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            TrackOutProcessJobID = PlcDataConverter.GetString(data, offset, 20); offset += 20;

            TrackOutPlanQuantity = PlcDataConverter.GetInt32(data, offset); offset += 2;
            TrackOutProcessQuantity = PlcDataConverter.GetInt32(data, offset); offset += 2;

            TrackOutReaderID = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            TrackOutRRC = PlcDataConverter.GetString(data, offset, 1); offset += 1;

            TrackOutOperatorID_1 = PlcDataConverter.GetString(data, offset, 10); offset += 10;
            TrackOutOperatorID_2 = PlcDataConverter.GetString(data, offset, 10); offset += 10;
            TrackOutOperatorID_3 = PlcDataConverter.GetString(data, offset, 10); offset += 10;

            TrackOutJudge = PlcDataConverter.GetString(data, offset, 1); offset += 1;

            offset += 10; // Bỏ qua Reserve (10 Words)

            TrackOutReasonCode = PlcDataConverter.GetString(data, offset, 10); offset += 10;
            TrackOutDescription = PlcDataConverter.GetString(data, offset, 20); offset += 20;
        }

        // --- Hàm đóng gói Data ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetString(data, offset, 40, TrackOutCellID); offset += 40;
            PlcDataConverter.SetString(data, offset, 20, TrackOutProductID); offset += 20;
            PlcDataConverter.SetString(data, offset, 20, TrackOutStepID); offset += 20;
            PlcDataConverter.SetString(data, offset, 20, TrackOutProcessJobID); offset += 20;

            PlcDataConverter.SetInt32(data, offset, TrackOutPlanQuantity); offset += 2;
            PlcDataConverter.SetInt32(data, offset, TrackOutProcessQuantity); offset += 2;

            PlcDataConverter.SetString(data, offset, 1, TrackOutReaderID); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, TrackOutRRC); offset += 1;

            PlcDataConverter.SetString(data, offset, 10, TrackOutOperatorID_1); offset += 10;
            PlcDataConverter.SetString(data, offset, 10, TrackOutOperatorID_2); offset += 10;
            PlcDataConverter.SetString(data, offset, 10, TrackOutOperatorID_3); offset += 10;

            PlcDataConverter.SetString(data, offset, 1, TrackOutJudge); offset += 1;

            offset += 10; // Bỏ qua khoảng Reserve

            PlcDataConverter.SetString(data, offset, 10, TrackOutReasonCode); offset += 10;
            PlcDataConverter.SetString(data, offset, 20, TrackOutDescription); offset += 20;

            return data;
        }
    }

    // --- Chiều C# (PLC) gửi thông tin TPM Loss lên CIM ---
    // Sự kiện: TPMLoss (BIT: B000E)
    // Dải địa chỉ: 120F ~ 1224 (Chiều dài: 22 Words)
    public class TPMLossArea
    {
        public const int TOTAL_WORDS = 22;

        // --- Cấu trúc dữ liệu ---
        public int TPMLossCode { get; set; }           // Length 2 (DEC - 32bit Int)
        public string TPMLossDescp { get; set; }       // Length 20 (ASCII)

        // --- Hàm đọc Data (Dùng khi C# cần parse lại gói tin hoặc test nội bộ) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            int offset = 0;

            // Đọc mã lỗi (DEC)
            TPMLossCode = PlcDataConverter.GetInt32(data, offset); offset += 2;

            // Đọc chuỗi mô tả (ASCII)
            TPMLossDescp = PlcDataConverter.GetString(data, offset, 20); offset += 20;
        }

        // --- Hàm đóng gói Data để C# ghi xuống PLC (Gửi lên CIM) ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            // Ghi mã lỗi (DEC)
            PlcDataConverter.SetInt32(data, offset, TPMLossCode); offset += 2;

            // Ghi chuỗi mô tả (ASCII)
            PlcDataConverter.SetString(data, offset, 20, TPMLossDescp); offset += 20;

            return data;
        }
    }

    // --- Vùng nhớ EQPStatus (Trạng thái thiết bị) ---
    // Start: W 0000, Length: 57 Words
    // Không có BIT đi kèm (Đọc/Ghi trực tiếp hoặc Polling)
    public class EQPStatusArea
    {
        public const int TOTAL_WORDS = 57;

        // --- Thông tin nhận dạng ---
        public string EQPID { get; set; }           // Length 20 (ASCII)
        public string EQPPPID { get; set; }         // Length 20 (ASCII) - PPID đang chạy hiện tại

        // --- Nhóm trạng thái điều khiển (ASCII: 1 Word) ---
        public string EQPControl { get; set; }      // 0: Off-Line, 1: Online Remote, 2: OnlineLocal

        // Reserve W 29 ~ 2B (Length 3)

        // --- Nhóm trạng thái vận hành (ASCII: 1 Word) ---
        public string EQPAvailability { get; set; } // 1: Down (Cần báo Alarm ID), 2: UP
        public string EQPInterlock { get; set; }    // 1: Interlock On, 2: Interlock Off
        public string EQPMove { get; set; }         // 1: Pause (Tạm dừng), 2: Running (Đang chạy)
        public string EQPRun { get; set; }          // 1: IDLE (Không có Cell), 2: RUN (Có Cell)
        public string EQPFront { get; set; }        // 1: Down (Lỗi dòng trên), 2: Up (Dòng trên bình thường)
        public string EQPRear { get; set; }         // 1: Down (Lỗi dòng dưới), 2: Up (Dòng dưới bình thường)
        public string EQPState_PP { get; set; }     // 1: Phát triển/Sample Lot, 2: Normal Process Lot

        // Reserve W 33 ~ 34 (Length 2)

        // --- Nhóm trạng thái MCR / Đầu đọc (ASCII: 1 Word) ---
        public string EQPMCR1 { get; set; }         // 0: MCR OFF, 1: MCR ON
        public string EQPMCR2 { get; set; }         // 0: MCR OFF, 1: MCR ON

        // Reserve W 37 ~ 38 (Length 2)

        // --- Hàm đọc Data (Từ mảng raw PLC) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            int offset = 0;

            EQPID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            EQPPPID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            EQPControl = PlcDataConverter.GetString(data, offset, 1); offset += 1;

            offset += 3; // Bỏ qua Reserve (W29 - W2B)

            EQPAvailability = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            EQPInterlock = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            EQPMove = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            EQPRun = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            EQPFront = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            EQPRear = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            EQPState_PP = PlcDataConverter.GetString(data, offset, 1); offset += 1;

            offset += 2; // Bỏ qua Reserve (W33 - W34)

            EQPMCR1 = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            EQPMCR2 = PlcDataConverter.GetString(data, offset, 1); offset += 1;

            offset += 2; // Bỏ qua Reserve cuối (W37 - W38)
        }

        // --- Hàm đóng gói Data để C# ghi xuống PLC ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetString(data, offset, 20, EQPID); offset += 20;
            PlcDataConverter.SetString(data, offset, 20, EQPPPID); offset += 20;
            PlcDataConverter.SetString(data, offset, 1, EQPControl); offset += 1;

            offset += 3; // Nhảy qua Reserve

            PlcDataConverter.SetString(data, offset, 1, EQPAvailability); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, EQPInterlock); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, EQPMove); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, EQPRun); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, EQPFront); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, EQPRear); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, EQPState_PP); offset += 1;

            offset += 2; // Nhảy qua Reserve

            PlcDataConverter.SetString(data, offset, 1, EQPMCR1); offset += 1;
            PlcDataConverter.SetString(data, offset, 1, EQPMCR2); offset += 1;

            // Đoạn Reserve cuối (offset += 2) mảng C# sẽ mặc định là 0/NULL
            return data;
        }
    }

    // --- Vùng nhớ riêng cho EQPPPID (PPID đang chạy hiện tại của thiết bị) ---
    // Start: W 14, Length: 20 Words
    // Không có BIT đi kèm (Thường được ghi sau khi thực hiện xong PPID Change)
    public class EQPPPIDArea
    {
        public const int TOTAL_WORDS = 20;

        // --- Cấu trúc dữ liệu ---
        public string EQPPPID { get; set; }     // Length 20 (ASCII)

        // --- Hàm đọc Data (Từ mảng raw PLC) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            int offset = 0;

            // Đọc chuỗi PPID (ASCII)
            EQPPPID = PlcDataConverter.GetString(data, offset, 20);
        }

        // --- Hàm đóng gói Data để C# ghi xuống PLC ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            // Ghi chuỗi PPID (ASCII)
            PlcDataConverter.SetString(data, offset, 20, EQPPPID);

            return data;
        }
    }

    // --- Vùng nhớ Parameter WORD (RMSPARAMETER) ---
    // Tối ưu với mảng int[] (Mỗi phần tử = 1 Parameter = 32-bit/2 Words)
    public class ParameterWordArea
    {
        public const int TOTAL_WORDS = 4000;

        // Kích thước mảng Parameter: (4000 - 22 Header) / 2 Words = 1989 phần tử int
        public const int PARAM_MAX_INT_COUNT = 1989;

        // --- Các trường bắt buộc (Header) ---
        // 1: Tạo, 2: Xóa, 3: Sửa / 0: OK, 4: NG
        public int PPIDMode { get; set; }           // Length 2 (DEC 32-bit) 
        public string PPIDName { get; set; }            // Length 20 (ASCII) 

        // --- Mảng lưu trữ toàn bộ dữ liệu Parameter (Đã chuyển sang int[]) ---
        public int[] Parameters { get; set; }

        public ParameterWordArea()
        {
            // Khởi tạo mảng thông số với tối đa 1989 phần tử
            Parameters = new int[PARAM_MAX_INT_COUNT];
        }

        // --- Hàm đọc Data (Từ mảng raw PLC) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu {TOTAL_WORDS} Words.");

            int offset = 0;

            // Đọc Header
            PPIDMode = PlcDataConverter.GetInt32(data, offset); offset += 2;
            PPIDName = PlcDataConverter.GetString(data, offset, 20); offset += 20;

            // Vòng lặp chuyển đổi tuần tự 2 words (short) thành 1 int
            for (int i = 0; i < PARAM_MAX_INT_COUNT; i++)
            {
                Parameters[i] = PlcDataConverter.GetInt32(data, offset);
                offset += 2; // Bước nhảy offset PLC vẫn là 2 words
            }
        }

        // --- Hàm đóng gói Data để C# ghi xuống PLC ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            // Ghi Header
            PlcDataConverter.SetInt32(data, offset, PPIDMode); offset += 2;
            PlcDataConverter.SetString(data, offset, 20, PPIDName); offset += 20;

            // Đóng gói mảng Parameters int[] xuống dạng short[]
            if (Parameters != null)
            {
                int copyCount = Math.Min(Parameters.Length, PARAM_MAX_INT_COUNT);
                for (int i = 0; i < copyCount; i++)
                {
                    PlcDataConverter.SetInt32(data, offset, Parameters[i]);
                    offset += 2; // Tịnh tiến 2 words trên mảng đích
                }
            }

            return data;
        }
    }

    // --- Vùng nhớ dành riêng cho PPID_MODE và PPID (Header của RMSPARAMETER) ---
    // PLC -> CIM: Start: 9224, Length: 22 Words
    // CIM -> PLC: Start: DF80, Length: 22 Words
    public class PPIDModeAndPPIDArea
    {
        public const int TOTAL_WORDS = 22;

        // --- Cấu trúc dữ liệu ---
        // PPID_MODE: 1 (Tạo/Create), 2 (Xóa/Delete), 3 (Sửa/Modify) | 0 (OK), 4 (NG)
        public int PPIDMode { get; set; }           // Length 2 (DEC 32-bit) 
        public string PPIDName { get; set; }            // Length 20 (ASCII) 

        // --- Hàm đọc Data (Từ mảng raw PLC) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            int offset = 0;

            PPIDMode = PlcDataConverter.GetInt32(data, offset); offset += 2;
            PPIDName = PlcDataConverter.GetString(data, offset, 20); offset += 20;
        }

        // --- Hàm đóng gói Data để C# ghi xuống PLC ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetInt32(data, offset, PPIDMode); offset += 2;
            PlcDataConverter.SetString(data, offset, 20, PPIDName); offset += 20;

            return data;
        }
    }

    // --- Vùng nhớ dành riêng cho Danh sách Parameter (Phần thân của RMSPARAMETER) ---
    // Bỏ qua 22 Words Header (PPID_MODE và PPID)
    // PLC -> CIM: Start: 923A, Length: 3978 Words
    // CIM -> PLC: Start: DF96, Length: 3978 Words
    public class RecipeParametersOnlyArea
    {
        // Tổng chiều dài 4000 - 22 (Header) = 3978 Words
        public const int TOTAL_WORDS = 3978;

        // --- Mảng lưu trữ toàn bộ dữ liệu Parameter ---
        public short[] Parameters { get; set; }

        public RecipeParametersOnlyArea()
        {
            // Khởi tạo mảng thông số với kích thước chuẩn 3978 Words
            Parameters = new short[TOTAL_WORDS];
        }

        // --- Hàm đọc Data (Từ mảng raw PLC) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

            // Copy toàn bộ dữ liệu vào mảng Parameters
            Array.Copy(data, 0, Parameters, 0, TOTAL_WORDS);
        }

        // --- Hàm đóng gói Data để C# ghi xuống PLC ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];

            if (Parameters != null)
            {
                int copyLength = Math.Min(Parameters.Length, TOTAL_WORDS);
                Array.Copy(Parameters, 0, data, 0, copyLength);
            }

            return data;
        }
    }


    // --- Lệnh CIM gửi Formatted Process Program xuống C# (PLC) ---
    // Sự kiện: FormattedProcessProgramSend (Bit: B1035)
    // Dải địa chỉ: W DF7D ~ W EF22 (Tổng chiều dài: 4006 Words)
    public class FormattedProcessProgramSendArea
    {
        public const int TOTAL_WORDS = 4006;
        public const int PARAM_LIST_LENGTH = 4000;

        // --- Header ---
        public string PPIDType { get; set; }           // Length 1 (ASCII) - W DF7D
        public string CCode { get; set; }              // Length 2 (ASCII) - W DF7E ~ DF7F

        // --- Data Body ---
        // Mảng chứa toàn bộ dữ liệu Parameter (Dài 4000 Words) - W DF80 ~ EF1F
        public short[] RmsParameterList { get; set; }

        // --- Trailer ---
        public string RecipeNumber { get; set; }       // Length 3 (ASCII) - W EF20 ~ EF22 (RECIPE ID 번호)

        public FormattedProcessProgramSendArea()
        {
            // Khởi tạo mảng short[] với kích thước chuẩn 4000 Words
            RmsParameterList = new short[PARAM_LIST_LENGTH];
        }

        // --- Hàm đọc Data (Từ mảng raw 4006 Words do PLC nhận về) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu {TOTAL_WORDS} Words.");

            int offset = 0;

            PPIDType = PlcDataConverter.GetString(data, offset, 1); offset += 1;
            CCode = PlcDataConverter.GetString(data, offset, 2); offset += 2;

            // Copy nhanh 4000 Words vào mảng RmsParameterList
            Array.Copy(data, offset, RmsParameterList, 0, PARAM_LIST_LENGTH);
            offset += PARAM_LIST_LENGTH;

            RecipeNumber = PlcDataConverter.GetString(data, offset, 3); offset += 3;
        }

        // --- Hàm đóng gói Data (Dùng khi C# cần test giả lập CIM) ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetString(data, offset, 1, PPIDType); offset += 1;
            PlcDataConverter.SetString(data, offset, 2, CCode); offset += 2;

            // Đổ mảng RmsParameterList vào mảng gửi đi
            if (RmsParameterList != null)
            {
                int copyLength = Math.Min(RmsParameterList.Length, PARAM_LIST_LENGTH);
                Array.Copy(RmsParameterList, 0, data, offset, copyLength);
            }
            offset += PARAM_LIST_LENGTH;

            PlcDataConverter.SetString(data, offset, 3, RecipeNumber); offset += 3;

            return data;
        }
    }

    // --- Lệnh CIM yêu cầu thông số Recipe (S7F25 Process Program Request) ---
    // Sự kiện: FormattedProcessProgramRequest (CIM Request BIT: B1036, PLC Reply BIT: B0036)
    // Dải địa chỉ nhận (CIM -> PLC): Request PPID WORD (Ví dụ: W 4217)
    // Chiều dài: 23 Words
    public class FormattedProcessProgramRequestArea
    {
        public const int TOTAL_WORDS = 23;

        // --- Cấu trúc dữ liệu 23 Words ---
        public string ReqPPID { get; set; }        // Length 20 Words (ASCII) - Tên Recipe
        public string ReqPPIDType { get; set; }    // Length 2 Words (ASCII) - Loại PPID
        public short ReqPPIDIndex { get; set; }    // Length 1 Word (DEC 16-bit) - Index

        // --- Hàm đọc Data từ mảng raw 23 Words do PLC nhận về ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu đúng {TOTAL_WORDS} Words.");

            int offset = 0;

            ReqPPID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
            ReqPPIDType = PlcDataConverter.GetString(data, offset, 2); offset += 2;
            ReqPPIDIndex = PlcDataConverter.GetInt16(data, offset);
        }

        // --- Hàm đóng gói Data (Dùng khi C# cần test giả lập CIM) ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            PlcDataConverter.SetString(data, offset, 20, ReqPPID); offset += 20;
            PlcDataConverter.SetString(data, offset, 2, ReqPPIDType); offset += 2;
            PlcDataConverter.SetInt16(data, offset, ReqPPIDIndex);

            return data;
        }
    }

    // --- Vùng nhớ PPID List ---
    // PLC -> CIM: Start: 8A54, Length: 2000 Words
    // Chức năng: Chứa tối đa 100 PPID (mỗi PPID 20 Words)
    public class PPIDListArea
    {
        public const int TOTAL_WORDS = 2000;
        public const int MAX_PPID_COUNT = 100;
        public const int PPID_LENGTH = 20; // 20 Words = 40 ký tự ASCII

        // --- Mảng chứa danh sách 100 PPID ---
        public string[] PPIDs { get; set; }

        public PPIDListArea()
        {
            // Khởi tạo mảng với 100 phần tử
            PPIDs = new string[MAX_PPID_COUNT];
        }

        // --- Hàm đọc Data từ mảng raw PLC (Dùng khi debug/test) ---
        public void FromCIMData(short[] data)
        {
            if (data.Length < TOTAL_WORDS)
                throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu đúng {TOTAL_WORDS} Words.");

            int offset = 0;

            for (int i = 0; i < MAX_PPID_COUNT; i++)
            {
                // Đọc từng khối 20 words
                PPIDs[i] = PlcDataConverter.GetString(data, offset, PPID_LENGTH);
                offset += PPID_LENGTH;
            }
        }

        // --- Hàm đóng gói Data để C# ghi xuống PLC ---
        public short[] ToCIMData()
        {
            short[] data = new short[TOTAL_WORDS];
            int offset = 0;

            for (int i = 0; i < MAX_PPID_COUNT; i++)
            {
                // Tránh lỗi Null Reference nếu C# chưa gán đủ 100 PPID
                string ppid = PPIDs[i] ?? string.Empty;

                // Ghi 20 words cho từng phần tử
                PlcDataConverter.SetString(data, offset, PPID_LENGTH, ppid);
                offset += PPID_LENGTH;
            }

            return data;
        }

        // --- Sự kiện Parameter Change 2 (S7F217 PPID Body Change with Recipe Number) ---
        // Event Bit: B0016 (CIM Reply: B1016)
        // Start Address: W 23D9, Length: 3 Words
        /// <summary>
        /// PPID NUMBER AREA / RECIPE NUMBER AREA
        /// </summary>
        public class ParameterChange2Area
        {
            public const int TOTAL_WORDS = 3;

            // --- Cấu trúc dữ liệu ---
            // ParameterChange2RecipeNumber (Length 3 Words = 6 ký tự ASCII)
            public string RecipeNumber { get; set; }

            // --- Hàm đọc Data từ mảng raw do PLC nhận về ---
            public void FromCIMData(short[] data)
            {
                if (data.Length < TOTAL_WORDS)
                    throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

                // Đọc 3 words ở vị trí offset 0
                RecipeNumber = PlcDataConverter.GetString(data, 0, TOTAL_WORDS);
            }

            // --- Hàm đóng gói Data để C# ghi xuống PLC ---
            public short[] ToCIMData()
            {
                short[] data = new short[TOTAL_WORDS];

                // Ghi chuỗi độ dài 3 words (6 byte/ký tự ASCII)
                PlcDataConverter.SetString(data, 0, TOTAL_WORDS, RecipeNumber);

                return data;
            }
        }

        // --- Sự kiện Equip Function Change (S6F11 / CEID 111) ---
        // Event Bit: B00E3 (CIM Reply: B10E3)
        // Start Address: W 5952, Length: 77 Words
        public class EquipFunctionChangeEventArea
        {
            public const int TOTAL_WORDS = 77;
            public const int EFST_COUNT = 15;
            public const int EFST_LENGTH = 5;

            // --- Cấu trúc dữ liệu ---
            // BYWHO: Chủ thể thay đổi (HOST, EQP, OPER,... ETC) - Length 2 Words (ASCII)
            public string ByWho { get; set; }

            // Mảng chứa 15 trạng thái chức năng (EFST1 ~ EFST15) - Mỗi phần tử Length 5 Words (ASCII)
            // Các chức năng VD: CELL TRACKING, MATERIAL TRACKING, SORT MODE, v.v...
            public string[] EFST { get; set; }

            public EquipFunctionChangeEventArea()
            {
                EFST = new string[EFST_COUNT];
            }

            // --- Hàm đọc Data từ mảng raw PLC ---
            public void FromCIMData(short[] data)
            {
                if (data.Length < TOTAL_WORDS)
                    throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

                int offset = 0;

                ByWho = PlcDataConverter.GetString(data, offset, 2); offset += 2;

                for (int i = 0; i < EFST_COUNT; i++)
                {
                    EFST[i] = PlcDataConverter.GetString(data, offset, EFST_LENGTH);
                    offset += EFST_LENGTH;
                }
            }

            // --- Hàm đóng gói Data để C# ghi xuống PLC ---
            public short[] ToCIMData()
            {
                short[] data = new short[TOTAL_WORDS];
                int offset = 0;

                PlcDataConverter.SetString(data, offset, 2, ByWho); offset += 2;

                for (int i = 0; i < EFST_COUNT; i++)
                {
                    // Tránh lỗi Null Reference nếu chưa gán đủ 15 phần tử
                    string efstValue = EFST[i] ?? string.Empty;
                    PlcDataConverter.SetString(data, offset, EFST_LENGTH, efstValue);
                    offset += EFST_LENGTH;
                }

                return data;
            }
        }

        // --- Nhận lệnh Equip Function Change Command (S2F41 RCMD 10) ---
        // Event Bit: B10E4
        // Start Address: W D211, Length: 66 Words
        public class EquipFunctionChangeCommandReceiveArea
        {
            public const int TOTAL_WORDS = 66;

            // --- Cấu trúc dữ liệu ---
            public string EquipCmdEFID { get; set; }     // Length 1 Word (ASCII) - ID chức năng
            public string EquipCmdEFST { get; set; }     // Length 5 Words (ASCII) - Trạng thái chức năng thiết lập
            public string EquipCmdMessage { get; set; }  // Length 60 Words (ASCII) - Lời nhắn đi kèm

            // --- Hàm đọc Data từ mảng 66 Words do PLC nhận về ---
            public void FromCIMData(short[] data)
            {
                if (data.Length < TOTAL_WORDS)
                    throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

                int offset = 0;

                EquipCmdEFID = PlcDataConverter.GetString(data, offset, 1); offset += 1;
                EquipCmdEFST = PlcDataConverter.GetString(data, offset, 5); offset += 5;
                EquipCmdMessage = PlcDataConverter.GetString(data, offset, 60);
            }
        }

        // --- Vùng nhớ Parameter WORD (CurrentProcessControlData APC) ---
        // Tối ưu với mảng int[] (Mỗi phần tử = 1 Parameter = 32-bit/2 Words)
        public class APCWordArea
        {
            public const int TOTAL_WORDS = 112;

            // Kích thước mảng Parameter: (4000 - 22 Header) / 2 Words = 1989 phần tử int
            public const int APC_MAX_INT_COUNT = 56;

            // --- Mảng lưu trữ toàn bộ dữ liệu Parameter (Đã chuyển sang int[]) ---
            public int[] APCs { get; set; }

            public APCWordArea()
            {
                // Khởi tạo mảng thông số với tối đa 1989 phần tử
                APCs = new int[APC_MAX_INT_COUNT];
            }

            // --- Hàm đọc Data (Từ mảng raw PLC) ---
            public void FromCIMData(short[] data)
            {
                if (data.Length < TOTAL_WORDS)
                    throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu {TOTAL_WORDS} Words.");

                int offset = 0;

                // Vòng lặp chuyển đổi tuần tự 2 words (short) thành 1 int
                for (int i = 0; i < APC_MAX_INT_COUNT; i++)
                {
                    APCs[i] = PlcDataConverter.GetInt32(data, offset);
                    offset += 2; // Bước nhảy offset PLC vẫn là 2 words
                }
            }

            // --- Hàm đóng gói Data để C# ghi xuống PLC ---
            public short[] ToCIMData()
            {
                short[] data = new short[TOTAL_WORDS];
                int offset = 0;

                // Đóng gói mảng Parameters int[] xuống dạng short[]
                if (APCs != null)
                {
                    int copyCount = Math.Min(APCs.Length, APC_MAX_INT_COUNT);
                    for (int i = 0; i < copyCount; i++)
                    {
                        PlcDataConverter.SetInt32(data, offset, APCs[i]);
                        offset += 2; // Tịnh tiến 2 words trên mảng đích
                    }
                }

                return data;
            }
        }

        // --- Sự kiện Current Process Control Data Request (S16F101/S16F102) ---
        // Tên Map: CurrentProcessControlDataReq
        // CIM Request Bit: B1095 | PLC Reply Bit: B0095
        // Dải địa chỉ phản hồi (PLC -> CIM): W 1910, Length: 64 Words
        public class CurrentProcessControlDataReqArea
        {
            public const int TOTAL_WORDS = 64;

            // --- Cấu trúc dữ liệu ---
            public string TMACK { get; set; }     // Length 2 Words (ASCII) - Total Message Acknowledge (W 1910)
            public string CellID { get; set; }    // Length 40 Words (ASCII) - Cell Unique ID (W 1912)
            public string SeqNo { get; set; }     // Length 2 Words (ASCII) - Công đoạn (Thường là "0") (W 193A)
            public string ModuleID { get; set; }  // Length 20 Words (ASCII) - Module ID (W 193C)

            public CurrentProcessControlDataReqArea()
            {
                // Khởi tạo các giá trị mặc định theo kịch bản (APC List WORD)
                TMACK = "";
                CellID = "NULL";
                SeqNo = "0";
                ModuleID = "";
            }

            // --- Hàm đọc Data từ mảng raw PLC (Dùng khi Debug) ---
            public void FromCIMData(short[] data)
            {
                if (data.Length < TOTAL_WORDS)
                    throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

                int offset = 0;
                TMACK = PlcDataConverter.GetString(data, offset, 2); offset += 2;
                CellID = PlcDataConverter.GetString(data, offset, 40); offset += 40;
                SeqNo = PlcDataConverter.GetString(data, offset, 2); offset += 2;
                ModuleID = PlcDataConverter.GetString(data, offset, 20);
            }

            // --- Hàm đóng gói Data để C# ghi xuống PLC phản hồi cho CIM ---
            public short[] ToCIMData()
            {
                short[] data = new short[TOTAL_WORDS];
                int offset = 0;

                PlcDataConverter.SetString(data, offset, 2, TMACK); offset += 2;
                PlcDataConverter.SetString(data, offset, 40, CellID); offset += 40;
                PlcDataConverter.SetString(data, offset, 2, SeqNo); offset += 2;
                PlcDataConverter.SetString(data, offset, 20, ModuleID);

                return data;
            }
        }

        // --- Vùng nhớ Process Control Information (CIM -> PLC) ---
        // Chức năng: Chứa thông tin Metadata của Kênh (CH1 ~ CH4) nhận thông số APC
        // Start Address: Tùy Index kênh (VD CH1: W DE5A, CH2: W DEA0...), Length: 70 Words
        public class ProcessControlInformReceiveArea
        {
            public const int TOTAL_WORDS = 70;

            // --- Cấu trúc dữ liệu 70 Words ---
            public string Mode { get; set; }        // Length 2 Words (ASCII) - 1: Creation, 2: Deletion
            public string CellID { get; set; }      // Length 40 Words (ASCII) 
            public string SeqNo { get; set; }       // Length 2 Words (ASCII) - Mặc định 0
            public string ModuleID { get; set; }    // Length 5 Words (ASCII) - CH01/CH02/CH03/CH04
            public string PPID { get; set; }        // Length 20 Words (ASCII) - Tên Recipe
            public string PPIDType { get; set; }    // Length 1 Word (ASCII) - 1: EQP PPID, 2: HOST PPID

            public void FromCIMData(short[] data)
            {
                if (data.Length < TOTAL_WORDS)
                    throw new ArgumentException($"Độ dài mảng không hợp lệ. Yêu cầu {TOTAL_WORDS} Words.");

                int offset = 0;
                Mode = PlcDataConverter.GetString(data, offset, 2); offset += 2;
                CellID = PlcDataConverter.GetString(data, offset, 40); offset += 40;
                SeqNo = PlcDataConverter.GetString(data, offset, 2); offset += 2;
                ModuleID = PlcDataConverter.GetString(data, offset, 5); offset += 5;
                PPID = PlcDataConverter.GetString(data, offset, 20); offset += 20;
                PPIDType = PlcDataConverter.GetString(data, offset, 1);
            }
        }

        // --- Phản hồi Process Control Information Send (PLC -> CIM) ---
        // S16F104: Trả lời kết quả cho Host
        // Chiều dài: 2 Words / Kênh
        public class ProcessControlInformReplyArea
        {
            public const int TOTAL_WORDS = 2;

            // --- Cấu trúc dữ liệu ---
            // ProcessCtrInformSendAck (Mã NAK/ACK code) - Length 2 Words (ASCII)
            // VD: "0" là OK, các mã khác là lỗi theo tài liệu
            public string AckCode { get; set; }

            // --- Hàm đóng gói Data để C# ghi xuống PLC ---
            public short[] ToCIMData()
            {
                short[] data = new short[TOTAL_WORDS];
                PlcDataConverter.SetString(data, 0, 2, AckCode);
                return data;
            }
        }

        // --- Sự kiện Process Control Data Create/Delete Report (S16F105) ---
        // Hướng: PLC -> CIM
        // Chiều dài: 51 Words / Kênh
        public class ProcessControlDataCreateDeleteArea
        {
            public const int TOTAL_WORDS = 51;

            // --- Cấu trúc dữ liệu 51 Words ---
            public string Mode { get; set; }        // Length 2 Words (ASCII) - 1: Creation, 2: Deletion
            public string ByWho { get; set; }       // Length 2 Words (ASCII) - HOST, EQP, hoặc OPER
            public string CellID { get; set; }      // Length 40 Words (ASCII)
            public string SeqNo { get; set; }       // Length 2 Words (ASCII) - Mặc định "0"
            public string ModuleID { get; set; }    // Length 5 Words (ASCII) - VD: CH01, CH02...

            // --- Hàm đọc Data từ PLC (Dùng khi debug) ---
            public void FromCIMData(short[] data)
            {
                if (data.Length < TOTAL_WORDS)
                    throw new ArgumentException($"Yêu cầu mảng dữ liệu dài {TOTAL_WORDS} Words.");

                int offset = 0;
                Mode = PlcDataConverter.GetString(data, offset, 2); offset += 2;
                ByWho = PlcDataConverter.GetString(data, offset, 2); offset += 2;
                CellID = PlcDataConverter.GetString(data, offset, 40); offset += 40;
                SeqNo = PlcDataConverter.GetString(data, offset, 2); offset += 2;
                ModuleID = PlcDataConverter.GetString(data, offset, 5);
            }

            // --- Hàm đóng gói Data để C# ghi xuống PLC báo cáo ---
            public short[] ToCIMData()
            {
                short[] data = new short[TOTAL_WORDS];
                int offset = 0;

                PlcDataConverter.SetString(data, offset, 2, Mode); offset += 2;
                PlcDataConverter.SetString(data, offset, 2, ByWho); offset += 2;
                PlcDataConverter.SetString(data, offset, 40, CellID); offset += 40;
                PlcDataConverter.SetString(data, offset, 2, SeqNo); offset += 2;
                PlcDataConverter.SetString(data, offset, 5, ModuleID);

                return data;
            }
        }

        // --- Sự kiện Operator Log Information (S6F11 / CEID 607) ---
        // Event Bit: B0040 (CIM Reply: B1040)
        // Start Address: W 1226, Length: 55 Words
        public class OperatorLogInformationArea
        {
            public const int TOTAL_WORDS = 55;

            // --- Cấu trúc dữ liệu 55 Words ---

            // OperatorInfoOption (Length 5 Words - ASCII)
            // Giá trị: "LOGIN" (khi đăng nhập) hoặc "LOGOUT" (khi đăng xuất)
            public string Option { get; set; }

            // OperatorInfoComment (Length 20 Words - ASCII)
            public string Comment { get; set; }

            // OperatorInfoOPID (Length 10 Words - ASCII) - ID của người vận hành
            public string OPID { get; set; }

            // OperatorInfoOPPW (Length 20 Words - ASCII) - Mật khẩu của người vận hành
            public string OPPW { get; set; }

            // --- Hàm đọc Data từ PLC (Dùng khi test/debug) ---
            public void FromCIMData(short[] data)
            {
                if (data.Length < TOTAL_WORDS)
                    throw new ArgumentException($"Yêu cầu mảng dữ liệu dài {TOTAL_WORDS} Words.");

                int offset = 0;
                Option = PlcDataConverter.GetString(data, offset, 5); offset += 5;
                Comment = PlcDataConverter.GetString(data, offset, 20); offset += 20;
                OPID = PlcDataConverter.GetString(data, offset, 10); offset += 10;
                OPPW = PlcDataConverter.GetString(data, offset, 20);
            }

            // --- Hàm đóng gói Data để C# ghi xuống PLC báo cáo lên CIM ---
            public short[] ToCIMData()
            {
                short[] data = new short[TOTAL_WORDS];
                int offset = 0;

                PlcDataConverter.SetString(data, offset, 5, Option); offset += 5;
                PlcDataConverter.SetString(data, offset, 20, Comment); offset += 20;
                PlcDataConverter.SetString(data, offset, 10, OPID); offset += 10;
                PlcDataConverter.SetString(data, offset, 20, OPPW);
                
                return data;
            }
        }

        // --- Lệnh Equip Approve Process (S2F43 RCMD 31, 32) ---
        // Tên trên Map: EquipApproveProecss
        // CIM Request Bit: B104D | PLC Reply Bit: B004D
        // Start Address (CIM -> PLC): W D7D3, Length: 94 Words
        public class EquipApproveProcessReceiveArea
        {
            public const int TOTAL_WORDS = 94;

            // --- Cấu trúc dữ liệu 94 Words ---

            // RCMD (Length 2 Words - ASCII) 
            // "31" : Equipment Approve Process Permit
            // "32" : Equipment Approve Process
            public string RCMD { get; set; }

            public string ApproveCode { get; set; }     // Length 10 Words (ASCII)
            public string ApproveInfo { get; set; }     // Length 10 Words (ASCII)
            public string ApproveID { get; set; }       // Length 10 Words (ASCII)
            public string ApproveByWho { get; set; }    // Length 2 Words (ASCII)
            public string ApproveText { get; set; }     // Length 60 Words (ASCII)

            // --- Hàm đọc Data từ mảng raw 94 Words do PLC nhận về ---
            public void FromCIMData(short[] data)
            {
                if (data.Length < TOTAL_WORDS)
                    throw new ArgumentException($"Độ dài mảng dữ liệu không hợp lệ. Yêu cầu ít nhất {TOTAL_WORDS} Words.");

                int offset = 0;
                RCMD = PlcDataConverter.GetString(data, offset, 2); offset += 2;
                ApproveCode = PlcDataConverter.GetString(data, offset, 10); offset += 10;
                ApproveInfo = PlcDataConverter.GetString(data, offset, 10); offset += 10;
                ApproveID = PlcDataConverter.GetString(data, offset, 10); offset += 10;
                ApproveByWho = PlcDataConverter.GetString(data, offset, 2); offset += 2;
                ApproveText = PlcDataConverter.GetString(data, offset, 60);
            }
        }
    }
}
