using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace TOPENG_Device
{
    public class CPLC_Mitsubishi : CPLC_Base
    {
        public CPLC_Mitsubishi()
        {

        }

        #region User Defined Private Functions
        #endregion

        #region User Defined Public Functions
        public override bool Connect()
        {
            return base.Connect();
        }
        public override bool Disconnect()
        {
            return base.Disconnect();
        }
        #endregion
    }

    public class MEXCEPTION
    {
        //
        //public static string mFolderLog = @"d:\Log";
        //
        public static object[] mArrayOld = null;  // 18.10.10 YHLEE
                                                    //
        public static void FileSave(string pFileDivision, params object[] pArray)
        {
            //
            try
            {
                //
                if (mArrayOld != null)  // 18.10.10 YHLEE
                {
                    //
                    if (pArray.Length == mArrayOld.Length)
                    {
                        //
                        if (pArray[pArray.Length - 1].ToString() == mArrayOld[mArrayOld.Length - 1].ToString())
                        {
                            return;
                        }
                    }
                }
                //
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(string.Format("[{0}]", System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()))));
                pFileDivision = regex.Replace(pFileDivision, "");
                //
                if ((pFileDivision == "") || (pFileDivision == null))
                {
                    pFileDivision = "Exception";
                }
                //
                DateTime datetime = DateTime.Now;  // 현재 시간
                                                    //
                string sFolder = MFOLDER.FolderLog + @"\" + datetime.ToString("yyMMdd");
                if (System.IO.Directory.Exists(sFolder) == false)
                {
                    System.IO.Directory.CreateDirectory(sFolder);
                }

                //
                string sFileName = sFolder + @"\" + datetime.ToString("yyMMdd_HHmmss_fff") + "_" + pFileDivision + ".TXT";
                if (File.Exists(sFileName) == true)  // 파일이 있으면 다른 이름으로 저장하기  // 19.11.26 YHLEE
                {
                    while (true)
                    {
                        datetime = DateTime.Now;  // 현재 시간
                        sFileName = sFolder + @"\" + datetime.ToString("yyMMdd_HHmmss_fff") + "_" + pFileDivision + ".TXT";
                        if (File.Exists(sFileName) == false)
                        {
                            break;
                        }
                    }
                }
                System.IO.StreamWriter streamwriter = new System.IO.StreamWriter(sFileName, false, Encoding.UTF8);
                streamwriter.WriteLine(datetime.ToString("yy-MM-dd"));
                streamwriter.WriteLine(datetime.ToString("HH:mm:ss"));
                streamwriter.WriteLine(datetime.Millisecond.ToString("000"));
                streamwriter.WriteLine(pFileDivision);
                for (int i = 0; i < pArray.Length; i++)
                {
                    try
                    {
                        streamwriter.WriteLine(pArray[i]);
                    }
                    catch
                    {
                    }
                }
                streamwriter.Close();
                //
                mArrayOld = pArray;  // 18.10.10 YHLEE
            }
            catch (Exception exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }
        }

        //
        public static int FileCheck()
        {
            //
            int returnvalue = 0;
            //
            try
            {
                //
                DateTime datetime = DateTime.Now;
                //string sDateTime = datetime.ToString("yyMMdd_HHmmss_fff");
                //
                string sFolder = MFOLDER.FolderLog + @"\" + datetime.ToString("yyMMdd");
                //
                if (Directory.Exists(sFolder) == false) { return 0; }
                //
                string[] sFileList = Directory.GetFiles(sFolder, "*_Exception.TXT");
                //
                returnvalue = sFileList.Length;
            }
            catch (Exception exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }
            //
            return returnvalue;
        }

    }  // class end

    public class MFOLDER
    {
        //==========
        public static string FolderRoot = @"D:\TOP\LENS_VN_HKMC_DB_AUTO_1CG2MD_DB_MES_BY_PC";//@"d:\Vision";  // 루뜨 폴더명 변경 금지
        public static string FolderSystem = FolderRoot + @"\System";
        public static string FolderSystemLog = FolderRoot + @"\SystemLog";
        public static string FolderRecipe = FolderRoot + @"\Recipe";
        public static string FolderErrorLog = FolderRoot + @"\ErrorLog";
        public static string FolderWorkLog = @"d:\WorkLog";
        public static string FolderSampleImage = FolderRoot + @"\SampleImage";
        public static string FolderLog = @"d:\Log";
        //public static string[] FolderUnitName;  // 170508 by RSJ
        public static string[] FolderStage; // 170508 by RSJ
        public static string[] FolderIndexNo;   // 170519 by RSJ
        public static string[] FolderIndexNoStage;  // 170519 by RSJ
                                                    //==========
        public static string FileSystem = FolderSystem + @"\System.CSV";

        public static string mFolderCameraFiles = FolderRoot + @"\CameraFiles";

        //==========
        public static bool mNoteBookMode = false;
        public static bool mEHDDUse = false;

        //==========
        public static void FolderCheck()
        {
            FolderCheck(mNoteBookMode, mEHDDUse);
        }
        public static void FolderCheck(bool pNoteBookMode, bool pEHDDUse)
        {
            try
            {
                //----------
                mNoteBookMode = pNoteBookMode;
                mEHDDUse = pEHDDUse;

                //----------
                if (pEHDDUse == true)
                {
                    if (pNoteBookMode == true)
                    {
                        MFOLDER.FolderWorkLog = @"d:\WorkLog";
                        MFOLDER.FolderLog = @"d:\Log";
                    }
                    else
                    {
                        //                     if (UTILITY.HDDNameFind("E:") == true)
                        //                     {
                        //                         MFOLDER.FolderWorkLog = @"e:\WorkLog";
                        //                         MFOLDER.FolderLog = @"e:\Log";
                        //                     }
                        //                     else
                        //                     {
                        //                         MFOLDER.FolderWorkLog = @"d:\WorkLog";
                        //                         MFOLDER.FolderLog = @"d:\Log";
                        //                     }
                        MFOLDER.FolderWorkLog = @"d:\WorkLog";
                        MFOLDER.FolderLog = @"d:\Log";
                    }
                }

                //----------
                Directory.CreateDirectory(FolderRoot);
                Directory.CreateDirectory(FolderSystem);
                Directory.CreateDirectory(FolderSystemLog);
                Directory.CreateDirectory(FolderRecipe);
                Directory.CreateDirectory(FolderErrorLog);
                Directory.CreateDirectory(FolderWorkLog);
                if (MNOTEBOOKMODE.NoteBookMode == true)
                {
                    Directory.CreateDirectory(FolderSampleImage);
                }
                Directory.CreateDirectory(FolderLog);
            }
            catch
            {
            }
        }

        //==========
        public static void RecipeCopy(string pOldRecipeName, string pNewRecipeName)
        {
            try
            {
            }
            catch
            {
            }
        }

        //==========
        public static void RecipeDelete(string pRecipeName)
        {
            try
            {
            }
            catch
            {
            }
        }

        //==========
        public static void RecipeRename(string pOldRecipeName, string pNewRecipeName)
        {
            try
            {
            }
            catch
            {
            }
        }
    }

    public class MNOTEBOOKMODE
    {
        //==========
        public static bool NoteBookMode = false;

        //==========
        public static bool Check()
        {
            if (File.Exists(MFOLDER.FolderRoot + @"\NoteBookMode.txt") || File.Exists(@"d:\NoteBookMode.txt"))
            {
                NoteBookMode = true;
            }
            else
            {
                NoteBookMode = false;
            }
            return NoteBookMode;
        }
    }

    //미쓰비시 PLC용 Socket Class 추가
    public class classSOCKET
    {
        public string mExceptionMessage = "";

        public Socket mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public string mSocket_DefineIPAddress = "100.100.100.21";
        public int mSocket_DefinePortNo = 8001;
        public IPAddress mSocket_IPAddress = null;
        public IPEndPoint mSocket_IPEndPoint = null;
        public int mSocket_Timeout = 500;

        public string mSendString = "";

        public string mReceiveString = "";
        public bool mReceiveStringOK = false;
        public int mReceiveStringTime = 0;
        public System.Diagnostics.Stopwatch mReceiveStringTimer = new System.Diagnostics.Stopwatch();

        //public EndPoint endpointLocal;
        public EndPoint mEndPointRemote;

        public classSOCKET()
        {
        }

        public bool Open(string pIPAddress, int pPortNo)
        {
            mExceptionMessage = "";

            bool returnvalue = true;

            try
            {
                mSocket_DefineIPAddress = pIPAddress;
                mSocket_DefinePortNo = pPortNo;

                mSocket_IPAddress = IPAddress.Parse(mSocket_DefineIPAddress);
                mSocket_IPEndPoint = new IPEndPoint(mSocket_IPAddress, mSocket_DefinePortNo);

                mEndPointRemote = new IPEndPoint(mSocket_IPAddress, mSocket_DefinePortNo);

                if (mSocket != null)
                {
                    if (mSocket.Connected == true)
                    {
                        mSocket.Disconnect(true);
                    }
                    mSocket.Close();
                    mSocket = null;
                }

                mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                mSocket_IPAddress = IPAddress.Parse(mSocket_DefineIPAddress);
                mSocket_IPEndPoint = new IPEndPoint(mSocket_IPAddress, mSocket_DefinePortNo);
                mSocket.SendTimeout = mSocket_Timeout;
                mSocket.ReceiveTimeout = mSocket_Timeout;
                mSocket.Connect(mSocket_IPEndPoint);

                byte[] byteBuffer = new byte[1024];
                mSocket.BeginReceiveFrom(byteBuffer, 0, byteBuffer.Length, SocketFlags.None, ref mEndPointRemote, new AsyncCallback(MessageCallBack), byteBuffer);

                returnvalue = mSocket.Connected;
            }
            catch (System.Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
                returnvalue = false;
            }

            return returnvalue;
        }  // public end

        public bool Connected()
        {
            //mExceptionMessage = "";

            bool returnvalue = true;

            try
            {
                if (mSocket == null)
                {
                    returnvalue = false;
                }
                else
                {
                    returnvalue = mSocket.Connected;
                }
            }
            catch (System.Exception exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
                //mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
                returnvalue = false;
            }

            return returnvalue;
        }  // public end

        public bool Close()
        {
            mExceptionMessage = "";

            bool returnvalue = true;

            try
            {
                if (mSocket == null)
                {
                    returnvalue = true;
                }
                else
                {
                    mSocket.Disconnect(true);
                    mSocket.Close();

                    mSocket = null;

                    returnvalue = !mSocket.Connected;
                }
            }
            catch (System.Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
                returnvalue = false;
            }

            return returnvalue;
        }  // public end

        //
        public static string mSocketPLC_Receive = "";
        public byte[] Receive()
        {
            byte[] returnvalue = new byte[1];

            try
            {
                //if (Connected == true)
                //{
                byte[] byteBuffer = new byte[1024];
                int iBufferSize = 0;

                try
                {
                    //if (mSocketPLC.Connected == true)
                    //{
                    try
                    {
                        iBufferSize = mSocket.Receive(byteBuffer);
                    }
                    catch
                    {
                        iBufferSize = 0;
                    }
                    //}
                    //else
                    //{
                    //    iBufferSize = 0;
                    //}
                }
                catch (SocketException exception)
                {
                    if (mSocket.Connected == false)
                    {
                        Connected();
                    }

                    MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
                }
                catch (Exception exception)
                {
                    MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
                }

                //
                if (iBufferSize > 0)
                {
                    returnvalue = new byte[iBufferSize];

                    //
                    mSocketPLC_Receive = "";
                    for (int i = 0; i < iBufferSize; i++)
                    {
                        returnvalue[i] = byteBuffer[i];
                        mSocketPLC_Receive += Convert.ToString(byteBuffer[i], 16).PadLeft(2, '0');
                    }
                }
                else
                {
                    returnvalue = new byte[1];
                }
                //}
            }
            catch (SocketException exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }
            catch (Exception exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }

            return returnvalue;
        }

        public void MessageCallBack(IAsyncResult pIAsyncResult)
        {
            mExceptionMessage = "";

            try
            {
                int iSize = mSocket.EndReceiveFrom(pIAsyncResult, ref mEndPointRemote);

                if (iSize > 0)
                {
                    byte[] byteReceiveData = new byte[1024];

                    byteReceiveData = (byte[])pIAsyncResult.AsyncState;

                    mReceiveString = Encoding.Default.GetString(byteReceiveData);

                    mReceiveStringOK = true;
                    mReceiveStringTime = (int)mReceiveStringTimer.ElapsedMilliseconds;
                }

                byte[] byteBuffer = new byte[1024];

                mSocket.BeginReceiveFrom(byteBuffer, 0, byteBuffer.Length, SocketFlags.None, ref mEndPointRemote, new AsyncCallback(MessageCallBack), byteBuffer);
            }
            catch (System.Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }
        }  // public end

        public bool Send(byte[] pSendString)
        {
            mExceptionMessage = "";

            bool returnvalue = true;

            try
            {
                mReceiveStringOK = false;
                mReceiveString = "";

                mReceiveStringTimer.Reset();
                mReceiveStringTimer.Start();

                if (Connected() == true)
                {
                    //byte[] byteSendString = Encoding.Default.GetBytes(pSendString);

                    int iBufferSize = mSocket.Send(pSendString);

                    if (pSendString.Length != iBufferSize)
                    {
                        returnvalue = false;
                    }
                }
                else
                {
                    returnvalue = false;
                }
            }
            catch (SocketException exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
                returnvalue = false;
            }
            catch (Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
                returnvalue = false;
            }

            return returnvalue;
        }  // public end

    }

    //MCCLinkIE Class 추가
    public class MCCLinkIE
    {
        //Private Declare Function mdopen Lib "MDFUNC32.DLL" (ByVal Chan As Short, ByVal Mode As Short, ByRef Path As Integer) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdopen", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern short mdopen(short Chan, short Mode, ref int Path);

        //Private Declare Function mdclose Lib "MDFUNC32.DLL" (ByVal Path As Integer) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdclose")]
        public static extern short mdclose(int Path);

        //Private Declare Function mdsend Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal Stno As Short, ByVal Devtyp As Short, ByVal Devno As Short, ByRef Size_Renamed As Short, ByRef Buf As Short) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdsend")]
        public static extern short mdsend(out int Path, int netno, out short Stno, out short Devtyp, out short Devno, ref short Size_Renamed, ref short Buf);

        //Private Declare Function mdreceive Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal Stno As Short, ByVal Devtyp As Short, ByVal Devno As Short, ByRef Size_Renamed As Short, ByRef Buf As Short) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdreceive", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern short mdreceive(int Path, int Stno, int Devtyp, int Devno, ref int Size_Renamed, ref int Buf);

        //Private Declare Function mddevset Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal Stno As Short, ByVal Devtyp As Short, ByVal Devno As Short) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mddevset", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        //public static extern short mddevset(out int  Path, out short Stno, out short Devtyp, out short Devno);
        public static extern short mddevset(int Path, int Stno, int Devtyp, int Devno);

        //Private Declare Function mddevrst Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal Stno As Short, ByVal Devtyp As Short, ByVal Devno As Short) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mddevrst")]
        public static extern short mddevrst(int Path, int Stno, int Devtyp, int Devno);
        //public static extern short mddevrst(out int Path, out short Stno, out short Devtyp, out short Devno);

        //Private Declare Function mdrandw Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal Stno As Short, ByRef dev As Short, ByRef Buf As Short, ByVal bufsiz As Short) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdrandw")]
        public static extern short mdrandw(out int Path, out short ByVal, ref short dev, ref short Buf, out short bufsiz);

        //Private Declare Function mdrandr Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal Stno As Short, ByRef dev As Short, ByRef Buf As Short, ByVal bufsiz As Short) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdrandr")]
        public static extern short mdrandr(out int Path, out short ByVal, ref short dev, ref short ByRef, out short bufsiz);

        //Private Declare Function mdcontrol Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal Stno As Short, ByVal Buf As Short) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdcontrol")]
        public static extern short mdcontrol(out int Path, out short Stno, out short Buf);

        //Private Declare Function mdtyperead Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal Stno As Short, ByRef Buf As Short) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdtyperead")]
        public static extern short mdtyperead(out int Path, out short Stno, ref short Buf);

        //
        //
        //

        //Private Declare Function mdsendex Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal netno As Integer, ByVal Stno As Integer, ByVal Devtyp As Integer, ByVal Devno As Integer, ByRef Size_Renamed As Integer, ByRef Buf As Integer) As Integer
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdsendex")]
        public static extern int mdsendex(int Path, int netno, int Stno, int Devtyp, int Devno, ref int Size_Renamed, ref int Buf);

        //Private Declare Function mdreceiveex Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal netno As Integer, ByVal Stno As Integer, ByVal Devtyp As Integer, ByVal Devno As Integer, ByRef Size_Renamed As Integer, ByRef Buf As Integer) As Integer
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdreceiveex")]
        public static extern int mdreceiveex(int Path, int netno, int Stno, int Devtyp, int Devno, ref int Size_Renamed, ref int Buf);

        //Private Declare Function mddevsetex Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal netno As Integer, ByVal Stno As Integer, ByVal Devtyp As Integer, ByVal Devno As Integer) As Integer
        [DllImport("MDFUNC32.DLL", EntryPoint = "mddevsetex", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        public static extern int mddevsetex(out int Path, out int netno, out int Stno, out int Devtyp, out int Devno);

        //Private Declare Function mddevrstex Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal netno As Integer, ByVal Stno As Integer, ByVal Devtyp As Integer, ByVal Devno As Integer) As Integer
        [DllImport("MDFUNC32.DLL", EntryPoint = "mddevrstex")]
        public static extern int mddevrstex(out int Path, out int netno, out int Stno, out int Devtyp, out int Devno);

        //Private Declare Function mdrandwex Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal netno As Integer, ByVal Stno As Integer, ByRef dev As Integer, ByRef Buf As Integer, ByVal bufsiz As Integer) As Integer
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdrandwex")]
        public static extern int mdrandwex(out int Path, out int netno, out int Stno, ref int dev, ref int Buf, out int bufsiz);

        //Private Declare Function mdrandrex Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal netno As Integer, ByVal Stno As Integer, ByRef dev As Integer, ByRef Buf As Integer, ByVal bufsiz As Integer) As Integer
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdrandrex")]
        public static extern int mdrandrex(out int Path, out int netno, out int Stno, ref int dev, ref int Buf, out int bufsiz);

        //Private Declare Function mdwaitbdevent Lib "MDFUNC32.DLL" (ByVal Path As Integer, ByVal eventno As Short, ByRef timeout As Integer, ByRef signaledno As Short, ByVal details As Short) As Short
        [DllImport("MDFUNC32.DLL", EntryPoint = "mdwaitbdevent")]
        public static extern int mdwaitbdevent(out int Path, out short eventno, ref int timeout, ref short signaledno, out short details);

        //
        //
        //
        public enum enumChannelType
        {
            MELSECNET_H_1 = 51,
            MELSECNET_H_2 = 52,
            MELSECNET_H_3 = 53,
            MELSECNET_H_4 = 54,
            //
            CC_LINK_1 = 81,
            CC_LINK_2 = 82,
            CC_LINK_3 = 83,
            CC_LINK_4 = 84,
            //
            CC_Link_IE_Controller_1 = 151,
            CC_Link_IE_Controller_2 = 152,
            CC_Link_IE_Controller_3 = 153,
            CC_Link_IE_Controller_4 = 154,
            //
            CC_Link_IE_Field_1 = 181,
            CC_Link_IE_Field_2 = 182,
            CC_Link_IE_Field_3 = 183,
            CC_Link_IE_Field_4 = 184,
        };

        //
        //
        //


        //
        //
        //
        public struct structDeviceType
        {
            public string DeviceTypeString;
            public int DeviceType;
        }
        //
        public static structDeviceType[] mDeviceTypeList = new structDeviceType[50];

        //
        //
        //
        public static bool mCCLinkIEUse = false;

        public static short mChannelType = (short)enumChannelType.CC_Link_IE_Controller_1;
        public static short mChannelMode = -1;
        public static int mChannelPath = 0;

        public static int mNetworkNo = 0;       //Remark. Setting
        public static int mStationNo = 255;     //Remark. Setting

        public static int mRDeviceType = (int)MPLC.enumDeviceType.B;
        public static int mRDeviceNo = 0;
        public static int mRDeviceNoLength = 4096;

        public static int mSDeviceType = (int)MPLC.enumDeviceType.B;
        public static int mSDeviceNo = 0;
        public static int mSDeviceNoLength = 0;
        //
        //
        //
        public static void Initial()
        {
            try
            {
                //
                //
                //
                mDeviceTypeList[0].DeviceTypeString = "X                 "; mDeviceTypeList[0].DeviceType = 1;
                mDeviceTypeList[1].DeviceTypeString = "Y                 "; mDeviceTypeList[1].DeviceType = 2;
                mDeviceTypeList[2].DeviceTypeString = "L                 "; mDeviceTypeList[2].DeviceType = 3;
                mDeviceTypeList[3].DeviceTypeString = "M                 "; mDeviceTypeList[3].DeviceType = 4;
                mDeviceTypeList[4].DeviceTypeString = "SB(Special M)     "; mDeviceTypeList[4].DeviceType = 5;
                mDeviceTypeList[5].DeviceTypeString = "F                 "; mDeviceTypeList[5].DeviceType = 6;
                mDeviceTypeList[6].DeviceTypeString = "TT                "; mDeviceTypeList[6].DeviceType = 7;
                mDeviceTypeList[7].DeviceTypeString = "TC                "; mDeviceTypeList[7].DeviceType = 8;
                mDeviceTypeList[8].DeviceTypeString = "TN                "; mDeviceTypeList[8].DeviceType = 11;
                mDeviceTypeList[9].DeviceTypeString = "CT                "; mDeviceTypeList[9].DeviceType = 9;
                mDeviceTypeList[10].DeviceTypeString = "CC               "; mDeviceTypeList[10].DeviceType = 10;
                mDeviceTypeList[11].DeviceTypeString = "CN               "; mDeviceTypeList[11].DeviceType = 12;
                mDeviceTypeList[12].DeviceTypeString = "STT              "; mDeviceTypeList[12].DeviceType = 26;
                mDeviceTypeList[13].DeviceTypeString = "STC              "; mDeviceTypeList[13].DeviceType = 27;
                mDeviceTypeList[14].DeviceTypeString = "STN              "; mDeviceTypeList[14].DeviceType = 35;
                mDeviceTypeList[15].DeviceTypeString = "D                "; mDeviceTypeList[15].DeviceType = 13;
                mDeviceTypeList[16].DeviceTypeString = "SW(Special D)    "; mDeviceTypeList[16].DeviceType = 14;
                mDeviceTypeList[17].DeviceTypeString = "A                "; mDeviceTypeList[17].DeviceType = 19;
                mDeviceTypeList[18].DeviceTypeString = "Z                "; mDeviceTypeList[18].DeviceType = 20;
                mDeviceTypeList[19].DeviceTypeString = "V                "; mDeviceTypeList[19].DeviceType = 21;
                mDeviceTypeList[20].DeviceTypeString = "R                "; mDeviceTypeList[20].DeviceType = 22;
                mDeviceTypeList[21].DeviceTypeString = "B                "; mDeviceTypeList[21].DeviceType = 23;
                mDeviceTypeList[22].DeviceTypeString = "W                "; mDeviceTypeList[22].DeviceType = 24;
                mDeviceTypeList[23].DeviceTypeString = "SB               "; mDeviceTypeList[23].DeviceType = 25;
                mDeviceTypeList[24].DeviceTypeString = "SW               "; mDeviceTypeList[24].DeviceType = 28;
                mDeviceTypeList[25].DeviceTypeString = "QV               "; mDeviceTypeList[25].DeviceType = 30;
                mDeviceTypeList[26].DeviceTypeString = "RWw              "; mDeviceTypeList[26].DeviceType = 36;
                mDeviceTypeList[27].DeviceTypeString = "RWr              "; mDeviceTypeList[27].DeviceType = 37;
                mDeviceTypeList[28].DeviceTypeString = "LZ               "; mDeviceTypeList[28].DeviceType = 38;
                mDeviceTypeList[29].DeviceTypeString = "RD               "; mDeviceTypeList[29].DeviceType = 39;
                mDeviceTypeList[30].DeviceTypeString = "LTT              "; mDeviceTypeList[30].DeviceType = 41;
                mDeviceTypeList[31].DeviceTypeString = "LTC              "; mDeviceTypeList[31].DeviceType = 42;
                mDeviceTypeList[32].DeviceTypeString = "LTN              "; mDeviceTypeList[32].DeviceType = 43;
                mDeviceTypeList[33].DeviceTypeString = "LCT              "; mDeviceTypeList[33].DeviceType = 44;
                mDeviceTypeList[34].DeviceTypeString = "LCC              "; mDeviceTypeList[34].DeviceType = 45;
                mDeviceTypeList[35].DeviceTypeString = "LCN              "; mDeviceTypeList[35].DeviceType = 46;
                mDeviceTypeList[36].DeviceTypeString = "LSTT             "; mDeviceTypeList[36].DeviceType = 47;
                mDeviceTypeList[37].DeviceTypeString = "LSTC             "; mDeviceTypeList[37].DeviceType = 48;
                mDeviceTypeList[38].DeviceTypeString = "LSTN             "; mDeviceTypeList[38].DeviceType = 49;
                mDeviceTypeList[39].DeviceTypeString = "Extended R       "; mDeviceTypeList[39].DeviceType = 22000;
                mDeviceTypeList[40].DeviceTypeString = "TM               "; mDeviceTypeList[40].DeviceType = 15;
                mDeviceTypeList[41].DeviceTypeString = "TS               "; mDeviceTypeList[41].DeviceType = 16;
                mDeviceTypeList[42].DeviceTypeString = "TS2              "; mDeviceTypeList[42].DeviceType = 16002;
                mDeviceTypeList[43].DeviceTypeString = "TS3              "; mDeviceTypeList[43].DeviceType = 16003;
                mDeviceTypeList[44].DeviceTypeString = "CM               "; mDeviceTypeList[44].DeviceType = 17;
                mDeviceTypeList[45].DeviceTypeString = "CS               "; mDeviceTypeList[45].DeviceType = 18;
                mDeviceTypeList[46].DeviceTypeString = "CS2              "; mDeviceTypeList[46].DeviceType = 18002;
                mDeviceTypeList[47].DeviceTypeString = "CS3              "; mDeviceTypeList[47].DeviceType = 18003;
                mDeviceTypeList[48].DeviceTypeString = "Own buffer memory"; mDeviceTypeList[48].DeviceType = 50;
                mDeviceTypeList[49].DeviceTypeString = "SEND/RECV        "; mDeviceTypeList[49].DeviceType = 101;
            }
            catch
            {
            }
        }

        //
        //
        //
        public static short Open()
        {
            short returnvalue = -1;

            try
            {
                returnvalue = mdopen(mChannelType, mChannelMode, ref mChannelPath);
            }
            catch
            {
            }

            return returnvalue;
        }

        //
        //
        //
        public static short Close()
        {
            short returnvalue = 0;

            try
            {
                returnvalue = mdclose(mChannelPath);
            }
            catch
            {
            }

            return returnvalue;
        }

        public static int Receive_RDevice(int nDeviceType, bool bIsWriteAddres)
        {
            int returnvalue = 0;
            Stopwatch Time1 = new Stopwatch();
            Time1.Start();
            switch (nDeviceType)
            {
                case (int)MPLC.enumDeviceType.B://Bit
                    mRDeviceType = (int)MPLC.enumDeviceType.B;
                    if (bIsWriteAddres)
                    {
                        mRDeviceNo = MPLC.PLCWriteBaseAddress_B;
                        mRDeviceNoLength = MPLC.PLCWriteBaseAddressLength_B / 16;
                    }
                    else
                    {
                        mRDeviceNo = MPLC.PLCReadBaseAddress_B;
                        mRDeviceNoLength = MPLC.PLCReadBaseAddressLength_B / 16;
                    }
                    break;
                case (int)MPLC.enumDeviceType.W://Word
                    mRDeviceType = (int)MPLC.enumDeviceType.W;
                    if (bIsWriteAddres)
                    {
                        mRDeviceNo = MPLC.PLCWriteBaseAddress_W;
                        mRDeviceNoLength = MPLC.PLCWriteBaseAddressLength_W;
                    }
                    else
                    {
                        mRDeviceNo = MPLC.PLCReadBaseAddress_W;
                        mRDeviceNoLength = MPLC.PLCReadBaseAddressLength_W;
                    }
                    break;
            }

            try
            {
                if ((mRDeviceNoLength % 2) > 0)
                {
                    mRDeviceNoLength++;
                }

                //mRDeviceData = new short[mRDeviceNoLength];

                int[] iReciveData = new int[mRDeviceNoLength / 2];
                int iiReciveDataLength = mRDeviceNoLength * 2;

                returnvalue = mdreceiveex(mChannelPath, mNetworkNo, mStationNo, mRDeviceType, mRDeviceNo, ref iiReciveDataLength, ref iReciveData[0]);
                System.Threading.Thread.Sleep(1);  // 20.01.16 YHLEE
                                                    //Debug.WriteLine("Receive_RDevice\t" + nDeviceType + "\t" + iReciveData[0]);
                switch (nDeviceType)
                {
                    case (int)MPLC.enumDeviceType.B://Bit
                        if (bIsWriteAddres)
                        {
                            for (int i = 0; i < iReciveData.Length; i++)
                            {
                                string sHex = Convert.ToString(iReciveData[i], 16).ToUpper().PadLeft(8, '0');
                                MPLC.PLCWriteValue_B_AS_1WORD[0 + (2 * i)] = (short)Convert.ToInt32(sHex.Substring(4, 4), 16);
                                MPLC.PLCWriteValue_B_AS_1WORD[1 + (2 * i)] = (short)Convert.ToInt32(sHex.Substring(0, 4), 16);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < iReciveData.Length; i++)
                            {
                                string sHex = Convert.ToString(iReciveData[i], 16).ToUpper().PadLeft(8, '0');
                                MPLC.PLCReadValue_B_AS_1WORD[0 + (2 * i)] = (short)Convert.ToInt32(sHex.Substring(4, 4), 16);
                                MPLC.PLCReadValue_B_AS_1WORD[1 + (2 * i)] = (short)Convert.ToInt32(sHex.Substring(0, 4), 16);
                            }
                        }
                        break;
                    case (int)MPLC.enumDeviceType.W://Word
                        if (bIsWriteAddres)
                        {
                            for (int i = 0; i < iReciveData.Length; i++)
                            {
                                string sHex = Convert.ToString(iReciveData[i], 16).ToUpper().PadLeft(8, '0');
                                MPLC.PLCWriteValue_W_AS_1WORD[0 + (2 * i)] = (short)Convert.ToInt32(sHex.Substring(4, 4), 16);
                                MPLC.PLCWriteValue_W_AS_1WORD[1 + (2 * i)] = (short)Convert.ToInt32(sHex.Substring(0, 4), 16);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < iReciveData.Length; i++)
                            {
                                string sHex = Convert.ToString(iReciveData[i], 16).ToUpper().PadLeft(8, '0');
                                MPLC.PLCReadValue_W_AS_1WORD[0 + (2 * i)] = (short)Convert.ToInt32(sHex.Substring(4, 4), 16);
                                MPLC.PLCReadValue_W_AS_1WORD[1 + (2 * i)] = (short)Convert.ToInt32(sHex.Substring(0, 4), 16);
                            }
                        }
                        break;
                }
                //Convert
                switch (nDeviceType)
                {
                    case (int)MPLC.enumDeviceType.B://Bit
                        if (bIsWriteAddres)
                        {
                            //for (int i = 0; i < iReciveData.Length/*MPLC.PLCWriteBaseAddressLength_B*/; i++)
                            for (int i = 0; i < mRDeviceNoLength; i++)
                            {
                                for (int j = 0; j < 16; j++)
                                {
                                    //MPLC.PLCWriteValue_B[(j + (32 * i))] = (short)((MPLC.PLCWriteValue_B_AS_1WORD[0 + (2 * i)] >> j) & 0x00000001);
                                    //MPLC.PLCWriteValue_B[16 + (j + (32 * i))] = (short)((MPLC.PLCWriteValue_B_AS_1WORD[1 + (2 * i)] >> j) & 0x00000001);
                                    MPLC.PLCWriteValue_B[(j + (16 * i))] = (short)((MPLC.PLCWriteValue_B_AS_1WORD[i] >> j) & 0x00000001);
                                }
                            }
                        }
                        else
                        {
                            //for (int i = 0; i < iReciveData.Length/*MPLC.PLCReadBaseAddressLength_B*/; i++)
                            for (int i = 0; i < mRDeviceNoLength; i++)
                            {
                                for (int j = 0; j < 16; j++)
                                {
                                    //MPLC.PLCReadValue_B[(j + (32 * i))] = (short)((MPLC.PLCReadValue_B_AS_1WORD[0 + (2 * i)] >> j) & 0x00000001);
                                    //MPLC.PLCReadValue_B[16 + (j + (32 * i))] = (short)((MPLC.PLCReadValue_B_AS_1WORD[1 + (2 * i)] >> j) & 0x00000001);
                                    MPLC.PLCReadValue_B[(j + (16 * i))] = (short)((MPLC.PLCReadValue_B_AS_1WORD[i] >> j) & 0x00000001);
                                }
                            }
                        }
                        break;
                    case (int)MPLC.enumDeviceType.W://Word
                        if (bIsWriteAddres)
                        {
                            for (int i = 0; i < mRDeviceNoLength; i++)
                                MPLC.PLCWriteValue_W[i] = MPLC.PLCWriteValue_W_AS_1WORD[i];
                        }
                        else
                        {
                            for (int i = 0; i < mRDeviceNoLength; i++)
                                MPLC.PLCReadValue_W[i] = MPLC.PLCReadValue_W_AS_1WORD[i];
                        }
                        break;
                }
            }
            catch
            {
            }
            Time1.Stop();
            //Debug.WriteLine("Data Read Time\t" + Time1.ElapsedMilliseconds);
            return returnvalue;
        }

        public static int Receive_RDevice_Bit(int nDeviceType, int pBaseStartNo)
        {
            int returnvalue = 0;
            int nResult = -1;
            Stopwatch Time1 = new Stopwatch();
            Time1.Start();

            try
            {
                int[] iReciveData = new int[2];
                int iiReciveDataLength = 2;

                int nBitCount = pBaseStartNo % 16;
                int nStartAddress = pBaseStartNo - nBitCount;

                returnvalue = mdreceive(mChannelPath, mStationNo, nDeviceType, nStartAddress, ref iiReciveDataLength, ref iReciveData[0]);
                System.Threading.Thread.Sleep(1);  // 20.01.16 YHLEE

                nResult = (iReciveData[0] >> nBitCount) & 0x0001;

                return nResult;
            }
            catch
            {
            }
            Time1.Stop();
            //Debug.WriteLine("Data Read Time\t" + Time1.ElapsedMilliseconds);
            return returnvalue;
        }

        public static int Receive_RDevice_Word(int nDeviceType, int pBaseStartNo, int nLength)
        {
            int returnvalue = 0;
            int nResult = -1;
            Stopwatch Time1 = new Stopwatch();
            Time1.Start();

            try
            {
                if ((mRDeviceNoLength % 2) > 0)
                {
                    mRDeviceNoLength++;
                }

                int[] iReciveData = new int[2];
                int iiReciveDataLength = nLength * 2;

                returnvalue = mdreceive(mChannelPath, mStationNo, nDeviceType, pBaseStartNo, ref iiReciveDataLength, ref iReciveData[0]);
                System.Threading.Thread.Sleep(1);  // 20.01.16 YHLEE

                nResult = iReciveData[0];

                return nResult;
            }
            catch
            {
            }
            Time1.Stop();
            //Debug.WriteLine("Data Read Time\t" + Time1.ElapsedMilliseconds);
            return nResult;
        }

        public static short[] Receive_RDevice3(int pBaseStartNo, int pLength)
        {
            if ((pLength % 2) > 0)
            {
                pLength++;
            }

            short[] shortData = new short[pLength];

            return shortData;
        }

        public static int Receive_SDevice()
        {
            int returnvalue = 0;

            return returnvalue;
        }
        public static int nLength;
        public static string strData;
        public static string strTemp;
        public static int Send_SDevice(int nDeviceType, bool bIsReadAddress)
        {
            int returnvalue = 0;
            Stopwatch Time1 = new Stopwatch();
            Time1.Start();
            switch (nDeviceType)
            {
                case (int)MPLC.enumDeviceType.B://Bit
                    mSDeviceType = nDeviceType;
                    if (bIsReadAddress)
                    {
                        mSDeviceNo = MPLC.PLCReadBaseAddress_B;
                        mSDeviceNoLength = MPLC.PLCReadBaseAddressLength_B / 16;
                    }
                    else
                    {
                        mSDeviceNo = MPLC.PLCWriteBaseAddress_B;
                        mSDeviceNoLength = MPLC.PLCWriteBaseAddressLength_B / 16;
                    }
                    break;
                case (int)MPLC.enumDeviceType.W://Word
                    mSDeviceType = nDeviceType;
                    if (bIsReadAddress)
                    {
                        mSDeviceNo = MPLC.PLCReadBaseAddress_W;
                        mSDeviceNoLength = MPLC.PLCReadBaseAddressLength_W;
                    }
                    else
                    {
                        mSDeviceNo = MPLC.PLCWriteBaseAddress_W;
                        mSDeviceNoLength = MPLC.PLCWriteBaseAddressLength_W;
                    }
                    break;
            }

            try
            {
                int iSendLength_B;
                int[] iSendData_B;
                int[] iSendData_W;
                int iSendLength_W;

                short[] tempPLCWriteValue_B = new short[MPLC.PLCWriteBaseAddressLength_B / 16];

                switch (nDeviceType)
                {
                    case (int)MPLC.enumDeviceType.B://Bit
                        iSendData_B = new int[mSDeviceNoLength / 2]; // short -> int
                        iSendLength_B = mSDeviceNoLength * 2;

                        // 231108 srkim - Convert
                        for (int i = 0; i < mSDeviceNoLength; i++)    //2Word씩 변환?
                        {
                            for (int j = 0; j < 16; j++)
                            {
                                tempPLCWriteValue_B[i] += (short)(MPLC.PLCWriteValue_B[(j + (16 * i))] << j);
                                //tempPLCWriteValue_B[2 * i +1] += (short)(MPLC.PLCWriteValue_B[(16 + j + (32 * i))] << j);
                                //MPLC.PLCWriteValue_B[(j + (32 * i))] = (short)((MPLC.PLCWriteValue_B_AS_1WORD[0 + (2 * i)] >> j) & 0x00000001);
                                //MPLC.PLCWriteValue_B[16 + (j + (32 * i))] = (short)((MPLC.PLCWriteValue_B_AS_1WORD[1 + (2 * i)] >> j) & 0x00000001);
                            }
                            MPLC.PLCWriteValue_B_AS_1WORD_FromPCRead[i] = tempPLCWriteValue_B[i];
                        }

                        for (int i = 0; i < iSendData_B.Length; i++)
                        {
                            string sV = Convert.ToString(MPLC.PLCWriteValue_B_AS_1WORD_FromPCRead[1 + (2 * i)], 16).PadLeft(4, '0') + Convert.ToString(MPLC.PLCWriteValue_B_AS_1WORD_FromPCRead[0 + (2 * i)], 16).PadLeft(4, '0');
                            int iV = Convert.ToInt32(sV, 16);

                            iSendData_B[i] = iV;
                        }

                        returnvalue = mdsendex(mChannelPath, mNetworkNo, mStationNo, mSDeviceType, mSDeviceNo, ref iSendLength_B, ref iSendData_B[0]);
                        System.Threading.Thread.Sleep(1);  // 20.01.16 YHLEE
                                                            //Debug.WriteLine("Send_SDevice\t" + nDeviceType + "\tBIT\t" + iSendData_B[0]);
                        break;

                    case (int)MPLC.enumDeviceType.W://Word
                        iSendData_W = new int[mSDeviceNoLength / 2]; // short -> int
                        iSendLength_W = mSDeviceNoLength * 2;

                        // 231108 srkim - Convert
                        for (int i = 0; i < mSDeviceNoLength; i++)
                        {
                            MPLC.PLCWriteValue_W_AS_1WORD_FromPCRead[i] = MPLC.PLCWriteValue_W[i];
                        }
                        for (int i = 0; i < mSDeviceNoLength/2; i++)
                        {
                            string sV = Convert.ToString(MPLC.PLCWriteValue_W_AS_1WORD_FromPCRead[1 + (2 * i)], 16).PadLeft(4, '0') + Convert.ToString(MPLC.PLCWriteValue_W_AS_1WORD_FromPCRead[0 + (2 * i)], 16).PadLeft(4, '0');
                            int iV = Convert.ToInt32(sV, 16);

                            iSendData_W[i] = iV;
                        }

                        returnvalue = mdsendex(mChannelPath, mNetworkNo, mStationNo, mSDeviceType, mSDeviceNo, ref iSendLength_W, ref iSendData_W[0]);
                        System.Threading.Thread.Sleep(1);  // 20.01.16 YHLEE
                                                            //Debug.WriteLine("Send_SDevice\t" + nDeviceType + "WORD\t" + Convert.ToByte(iSendData_W[0]));
                        break;
                }
            }
            catch
            {
            }
            Time1.Stop();
            //Debug.WriteLine("Send_SDevice\t" + nDeviceType + "\tCount\t" + strData);
            return returnvalue;
        }


        public static int Send_SDevice_Bit(int nDeviceType, int pBaseStartNo, bool BitFlag)
        {
            int returnvalue = 0;
            Stopwatch Time1 = new Stopwatch();
            Time1.Start();
            try
            {
                if (BitFlag == true)
                    returnvalue = mddevset(mChannelPath, mStationNo, nDeviceType, pBaseStartNo);
                else
                    returnvalue = mddevrst(mChannelPath, mStationNo, nDeviceType, pBaseStartNo);
            }
            catch
            {
            }
            Time1.Stop();
            //Debug.WriteLine("Data Read Time\t" + Time1.ElapsedMilliseconds);
            return returnvalue;
        }
        public static int Send_SDevice_Word(int nDeviceType, int pBaseStartNo, int nData, int nLength)
        {
            int returnvalue = 0;
            Stopwatch Time1 = new Stopwatch();
            Time1.Start();
            try
            {
                int[] iSendData_W = new int[nLength];
                for (int i = 0; i < iSendData_W.Length; i++)
                {
                    iSendData_W[i] = nData;
                }

                int iSendLength_W = nLength * 2;

                returnvalue = mdsendex(mChannelPath, mNetworkNo, mStationNo, nDeviceType, pBaseStartNo, ref iSendLength_W, ref iSendData_W[0]);
                System.Threading.Thread.Sleep(1);  // 20.01.16 YHLEE
            }
            catch
            {
            }
            Time1.Stop();
            //Debug.WriteLine("Data Read Time\t" + Time1.ElapsedMilliseconds);
            return returnvalue;
        }
    }  // class end

    //MPLC Class 추가
    public class MPLC
    {
        public enum enumPC2PLC_B
        {
            WB_EQ_ALIVE = 0x0800,
            WB_EQ_ONEPANEL_SET = 0x0801,
            WB_EQ_WORK_CHECK_REPORT_ACK = 0x0810,
            WB_EQ_LOADING_REPORT_ACK,
            WB_EQ_MATERIAL_CHECK_CG_REPORT_ACK,
            WB_EQ_START_REPORT_ACK,
            WB_EQ_END_REPORT_ACK,
            WB_EQ_MACHINE_STATUS_REPORT_ACK,
            WB_EQ_AUTO_CHANGE_REPORT_ACK,
            WB_EQ_MANUAL_CHANGE_REPORT_ACK,
            WB_EQ_MACHINE_STATUS_MES_REPORT_ACK,
            WB_EQ_MATERIAL_CHECK_BA1_REPORT_ACK = 0x0822,
            WB_EQ_BA1_START_REPORT_ACK,
            WB_EQ_BA1_END_REPORT_ACK,
            WB_EQ_MATERIAL_CHECK_BA2_REPORT_ACK = 0x0832,
            WB_EQ_BA2_START_REPORT_ACK,
            WB_EQ_BA2_END_REPORT_ACK,
        };

        public enum enumPC2PLC_W
        {
            WW_EQ_TIME_YEAR = 0x1000,
            WW_EQ_TIME_MONTH,
            WW_EQ_TIME_DAY,
            WW_EQ_TIME_HOUR,
            WW_EQ_TIME_MINUTE,
            WW_EQ_TIME_SECOND,

            WW_EQ_WORK_CHECK_RESPONSE_CODE = 0x1020,
            WW_EQ_LOADING_RESPONSE_CODE,
            WW_EQ_MATERIAL_CHECK_RESPONSE_CODE,
            WW_EQ_START_RESPONSE_CODE,
            WW_EQ_END_RESPONSE_CODE,
            WW_EQ_MACHINE_STATUS_RESPONSE_CODE,
            WW_EQ_AUTO_CHANGE_RESPONSE_CODE,
            WW_EQ_MANUAL_CHANGE_RESPONSE_CODE,
            WW_EQ_CHECK_SITE_AND_RESOURCE_CODE,

            WW_CG_MO_01 = 0x1040,
            WW_CG_MO_02,
            WW_CG_MO_03,
            WW_CG_MO_04,
            WW_CG_MO_05,
            WW_CG_MO_06,
            WW_CG_MO_07,
            WW_CG_MO_08,
            WW_CG_MO_09,
            WW_CG_MO_10,

            WW_CG_BOM_COMPONENT_01 = 0x1050,
            WW_CG_BOM_COMPONENT_02,
            WW_CG_BOM_COMPONENT_03,
            WW_CG_BOM_COMPONENT_04,
            WW_CG_BOM_COMPONENT_05,
            WW_CG_BOM_COMPONENT_06,
            WW_CG_BOM_COMPONENT_07,
            WW_CG_BOM_COMPONENT_08,
            WW_CG_BOM_COMPONENT_09,
            WW_CG_BOM_COMPONENT_10,

            WW_CG_NAME_01 = 0x1060,
            WW_CG_NAME_02,
            WW_CG_NAME_03,
            WW_CG_NAME_04,
            WW_CG_NAME_05,
            WW_CG_NAME_06,
            WW_CG_NAME_07,
            WW_CG_NAME_08,
            WW_CG_NAME_09,
            WW_CG_NAME_10,

            WW_BA1_MO_01 = 0x1070,
            WW_BA1_MO_02,
            WW_BA1_MO_03,
            WW_BA1_MO_04,
            WW_BA1_MO_05,
            WW_BA1_MO_06,
            WW_BA1_MO_07,
            WW_BA1_MO_08,
            WW_BA1_MO_09,
            WW_BA1_MO_10,

            WW_BA1_BOM_COMPONENT_01 = 0x1080,
            WW_BA1_BOM_COMPONENT_02,
            WW_BA1_BOM_COMPONENT_03,
            WW_BA1_BOM_COMPONENT_04,
            WW_BA1_BOM_COMPONENT_05,
            WW_BA1_BOM_COMPONENT_06,
            WW_BA1_BOM_COMPONENT_07,
            WW_BA1_BOM_COMPONENT_08,
            WW_BA1_BOM_COMPONENT_09,
            WW_BA1_BOM_COMPONENT_10,

            WW_BA1_NAME_01 = 0x1090,
            WW_BA1_NAME_02,
            WW_BA1_NAME_03,
            WW_BA1_NAME_04,
            WW_BA1_NAME_05,
            WW_BA1_NAME_06,
            WW_BA1_NAME_07,
            WW_BA1_NAME_08,
            WW_BA1_NAME_09,
            WW_BA1_NAME_10,

            WW_BA2_MO_01 = 0x10A0,
            WW_BA2_MO_02,
            WW_BA2_MO_03,
            WW_BA2_MO_04,
            WW_BA2_MO_05,
            WW_BA2_MO_06,
            WW_BA2_MO_07,
            WW_BA2_MO_08,
            WW_BA2_MO_09,
            WW_BA2_MO_10,

            WW_BA2_BOM_COMPONENT_01 = 0x10B0,
            WW_BA2_BOM_COMPONENT_02,
            WW_BA2_BOM_COMPONENT_03,
            WW_BA2_BOM_COMPONENT_04,
            WW_BA2_BOM_COMPONENT_05,
            WW_BA2_BOM_COMPONENT_06,
            WW_BA2_BOM_COMPONENT_07,
            WW_BA2_BOM_COMPONENT_08,
            WW_BA2_BOM_COMPONENT_09,
            WW_BA2_BOM_COMPONENT_10,

            WW_BA2_NAME_01 = 0x10C0,
            WW_BA2_NAME_02,
            WW_BA2_NAME_03,
            WW_BA2_NAME_04,
            WW_BA2_NAME_05,
            WW_BA2_NAME_06,
            WW_BA2_NAME_07,
            WW_BA2_NAME_08,
            WW_BA2_NAME_09,
            WW_BA2_NAME_10,

            WW_RESOURCE_01 = 0x10D0,
            WW_RESOURCE_02,
            WW_RESOURCE_03,
            WW_RESOURCE_04,
            WW_RESOURCE_05,
            WW_RESOURCE_06,
            WW_RESOURCE_07,
            WW_RESOURCE_08,
            WW_RESOURCE_09,
            WW_RESOURCE_10,

            WW_OPERATION_01 = 0x10E0,
            WW_OPERATION_02,
            WW_OPERATION_03,
            WW_OPERATION_04,
            WW_OPERATION_05,
            WW_OPERATION_06,
            WW_OPERATION_07,
            WW_OPERATION_08,
            WW_OPERATION_09,
            WW_OPERATION_10,
        };

        public enum enumPLC2PC_B
        {
            RB_EQ_ALIVE = 0x0000,
            
            RB_EQ_WORK_CHECK_REPORT_SET = 0x0010,
            RB_EQ_LOADING_REPORT_SET,
            RB_EQ_MATERIAL_CHECK_CG_REPORT_SET,
            RB_EQ_START_REPORT_SET,
            RB_EQ_END_REPORT_SET,
            RB_EQ_MACHINE_STATUS_REPORT_SET,
            RB_EQ_AUTO_CHANGE_REPORT_SET,
            RB_EQ_MANUAL_CHANGE_REPORT_SET,
            RB_EQ_MACHINE_STATUS_MES_REPORT_SET,
            RB_EQ_MATERIAL_CHECK_BA1_REPORT_SET = 0x0022,
            RB_EQ_BA1_START_REPORT_SET,
            RB_EQ_BA1_END_REPORT_SET,
            RB_EQ_MATERIAL_CHECK_BA2_REPORT_SET = 0x0032,
            RB_EQ_BA2_START_REPORT_SET,
            RB_EQ_BA2_END_REPORT_SET
        };

        public enum enumPLC2PC_W
        {
            RW_EQ_STATUS = 0x0000,
            RW_ALARM_REPORT_CODE,
            RW_SEC_TIMER,
            RW_LOADING_LOT_01 = 0x0030,
            RW_LOADING_LOT_02,
            RW_LOADING_LOT_03,
            RW_LOADING_LOT_04,
            RW_LOADING_LOT_05,
            RW_LOADING_LOT_06,
            RW_LOADING_LOT_07,
            RW_LOADING_LOT_08,
            RW_LOADING_LOT_09,
            RW_LOADING_LOT_10,
            RW_LOADING_LOT_11,
            RW_LOADING_LOT_12,
            RW_LOADING_LOT_13,
            RW_LOADING_LOT_14,
            RW_LOADING_LOT_15,
            RW_LOADING_LOT_16,
            RW_MATERIAL_CHECK_NAME1_01 = 0x0050,
            RW_MATERIAL_CHECK_NAME1_02,
            RW_MATERIAL_CHECK_NAME1_03,
            RW_MATERIAL_CHECK_NAME1_04,
            RW_MATERIAL_CHECK_NAME1_05,
            RW_MATERIAL_CHECK_NAME1_06,
            RW_MATERIAL_CHECK_NAME1_07,
            RW_MATERIAL_CHECK_NAME1_08,
            RW_MATERIAL_CHECK_NAME1_09,
            RW_MATERIAL_CHECK_NAME1_10,
            RW_MATERIAL_CHECK_VALUE1_01 = 0x0060,
            RW_MATERIAL_CHECK_VALUE1_02,
            RW_MATERIAL_CHECK_VALUE1_03,
            RW_MATERIAL_CHECK_VALUE1_04,
            RW_MATERIAL_CHECK_VALUE1_05,
            RW_MATERIAL_CHECK_VALUE1_06,
            RW_MATERIAL_CHECK_VALUE1_07,
            RW_MATERIAL_CHECK_VALUE1_08,
            RW_MATERIAL_CHECK_VALUE1_09,
            RW_MATERIAL_CHECK_VALUE1_10,
            RW_MATERIAL_CHECK_VALUE1_12,
            RW_MATERIAL_CHECK_VALUE1_13,
            RW_MATERIAL_CHECK_VALUE1_14,
            RW_MATERIAL_CHECK_VALUE1_15,
            RW_MATERIAL_CHECK_VALUE1_16,
            /*
            RW_MATERIAL_CHECK_NAME2_01 = 0x0070,
            RW_MATERIAL_CHECK_NAME2_02,
            RW_MATERIAL_CHECK_NAME2_03,
            RW_MATERIAL_CHECK_NAME2_04,
            RW_MATERIAL_CHECK_NAME2_05,
            RW_MATERIAL_CHECK_NAME2_06,
            RW_MATERIAL_CHECK_NAME2_07,
            RW_MATERIAL_CHECK_NAME2_08,
            RW_MATERIAL_CHECK_NAME2_09,
            RW_MATERIAL_CHECK_NAME2_10,
            */
            RW_MATERIAL_CHECK_VALUE2_01 = 0x0090,
            RW_MATERIAL_CHECK_VALUE2_02,
            RW_MATERIAL_CHECK_VALUE2_03,
            RW_MATERIAL_CHECK_VALUE2_04,
            RW_MATERIAL_CHECK_VALUE2_05,
            RW_MATERIAL_CHECK_VALUE2_06,
            RW_MATERIAL_CHECK_VALUE2_07,
            RW_MATERIAL_CHECK_VALUE2_08,
            RW_MATERIAL_CHECK_VALUE2_09,
            RW_MATERIAL_CHECK_VALUE2_10,
            RW_MATERIAL_CHECK_VALUE2_12,
            RW_MATERIAL_CHECK_VALUE2_13,
            RW_MATERIAL_CHECK_VALUE2_14,
            RW_MATERIAL_CHECK_VALUE2_15,
            RW_MATERIAL_CHECK_VALUE2_16,
            RW_MATERIAL_CHECK_VALUE2_17,
            RW_MATERIAL_CHECK_VALUE2_18,
            RW_START_ID_01 = 0x00B0,
            RW_START_ID_02,
            RW_START_ID_03,
            RW_START_ID_04,
            RW_START_ID_05,
            RW_START_ID_06,
            RW_START_ID_07,
            RW_START_ID_08,
            RW_START_ID_09,
            RW_START_ID_10,
            RW_START_ID_11,
            RW_START_ID_12,
            RW_START_ID_13,
            RW_START_ID_14,
            RW_START_ID_15,
            RW_START_ID_16,
            RW_END_ID_01 = 0x00D0,
            RW_END_ID_02,
            RW_END_ID_03,
            RW_END_ID_04,
            RW_END_ID_05,
            RW_END_ID_06,
            RW_END_ID_07,
            RW_END_ID_08,
            RW_END_ID_09,
            RW_END_ID_10,
            RW_END_ID_11,
            RW_END_ID_12,
            RW_END_ID_13,
            RW_END_ID_14,
            RW_END_ID_15,
            RW_END_ID_16,

            RW_TEST_START_TIME_YEAR = 0x0200,
            RW_TEST_START_TIME_MONTH,
            RW_TEST_START_TIME_DAY,
            RW_TEST_START_TIME_HOUR,
            RW_TEST_START_TIME_MINUTE,
            RW_TEST_START_TIME_SECOND,
            RW_TEST_END_TIME_YEAR = 0x0208,
            RW_TEST_END_TIME_MONTH,
            RW_TEST_END_TIME_DAY,
            RW_TEST_END_TIME_HOUR,
            RW_TEST_END_TIME_MINUTE,
            RW_TEST_END_TIME_SECOND,
            RW_END_ID_CG_BOX_01 = 0x0210,
            RW_END_ID_CG_BOX_02,
            RW_END_ID_CG_BOX_03,
            RW_END_ID_CG_BOX_04,
            RW_END_ID_CG_BOX_05,
            RW_END_ID_CG_BOX_06,
            RW_END_ID_CG_BOX_07,
            RW_END_ID_CG_BOX_08,
            RW_END_ID_CG_BOX_09,
            RW_END_ID_CG_BOX_10,
            RW_END_ID_CG_BOX_11,
            RW_END_ID_CG_BOX_12,
            RW_END_ID_CG_BOX_13,
            RW_END_ID_CG_BOX_14,
            RW_END_ID_CG_BOX_15,
            RW_END_ID_CG_BOX_16,
            RW_END_ID_CG_BOX_17,
            RW_END_ID_CG_BOX_18,
            RW_END_ID_CG_BOX_19,
            RW_END_ID_CG_BOX_20,
            RW_END_ID_CG_01 = 0x0230,
            RW_END_ID_CG_02,
            RW_END_ID_CG_03,
            RW_END_ID_CG_04,
            RW_END_ID_CG_05,
            RW_END_ID_CG_06,
            RW_END_ID_CG_07,
            RW_END_ID_CG_08,
            RW_END_ID_CG_09,
            RW_END_ID_CG_10,
            RW_END_ID_CG_11,
            RW_END_ID_CG_12,
            RW_END_ID_CG_13,
            RW_END_ID_CG_14,
            RW_END_ID_CG_15,
            RW_END_ID_CG_16,
            RW_END_ID_CG_17,
            RW_END_ID_CG_18,
            RW_END_ID_CG_19,
            RW_END_ID_CG_20,
            RW_END_ID_BA1_01 = 0x0250,
            RW_END_ID_BA1_02,
            RW_END_ID_BA1_03,
            RW_END_ID_BA1_04,
            RW_END_ID_BA1_05,
            RW_END_ID_BA1_06,
            RW_END_ID_BA1_07,
            RW_END_ID_BA1_08,
            RW_END_ID_BA1_09,
            RW_END_ID_BA1_10,
            RW_END_ID_BA1_11,
            RW_END_ID_BA1_12,
            RW_END_ID_BA1_13,
            RW_END_ID_BA1_14,
            RW_END_ID_BA1_15,
            RW_END_ID_BA1_16,
            RW_END_ID_BA1_17,
            RW_END_ID_BA1_18,
            RW_END_ID_BA1_19,
            RW_END_ID_BA1_20,
            RW_END_ID_BA2_01 = 0x0270,
            RW_END_ID_BA2_02,
            RW_END_ID_BA2_03,
            RW_END_ID_BA2_04,
            RW_END_ID_BA2_05,
            RW_END_ID_BA2_06,
            RW_END_ID_BA2_07,
            RW_END_ID_BA2_08,
            RW_END_ID_BA2_09,
            RW_END_ID_BA2_10,
            RW_END_ID_BA2_11,
            RW_END_ID_BA2_12,
            RW_END_ID_BA2_13,
            RW_END_ID_BA2_14,
            RW_END_ID_BA2_15,
            RW_END_ID_BA2_16,
            RW_END_ID_BA2_17,
            RW_END_ID_BA2_18,
            RW_END_ID_BA2_19,
            RW_END_ID_BA2_20,
            RW_TEST_ACCURACY_STAGE1_X1 = 0X0290,
            RW_TEST_ACCURACY_STAGE1_Y1,
            RW_TEST_ACCURACY_STAGE1_X2,
            RW_TEST_ACCURACY_STAGE1_Y2,
            RW_TEST_ACCURACY_STAGE1_X3,
            RW_TEST_ACCURACY_STAGE1_Y3,
            RW_TEST_ACCURACY_STAGE1_X4,
            RW_TEST_ACCURACY_STAGE1_Y4,
            RW_TEST_ACCURACY_STAGE2_X1,
            RW_TEST_ACCURACY_STAGE2_Y1,
            RW_TEST_ACCURACY_STAGE2_X2,
            RW_TEST_ACCURACY_STAGE2_Y2,
            RW_TEST_ACCURACY_STAGE2_X3,
            RW_TEST_ACCURACY_STAGE2_Y3,
            RW_TEST_ACCURACY_STAGE2_X4,
            RW_TEST_ACCURACY_STAGE2_Y4,
            RW_TEST_TOTAL_RESULT,
            RW_TEST_NG_CODE_01,
            RW_TEST_NG_CODE_02,
            RW_TEST_NG_CODE_03,
            RW_TEST_NG_CODE_04,
            RW_TEST_NG_CODE_05,
            RW_PARAMETER_REPORT_ID_01 = 0X02B0,
            RW_PARAMETER_REPORT_ID_02,
            RW_PARAMETER_REPORT_ID_03,
            RW_PARAMETER_REPORT_ID_04,
            RW_PARAMETER_REPORT_ID_05,
            RW_PARAMETER_REPORT_ID_06,
            RW_PARAMETER_REPORT_ID_07,
            RW_PARAMETER_REPORT_ID_08,
            RW_PARAMETER_REPORT_ID_09,
            RW_PARAMETER_REPORT_ID_10,
            RW_KEY_PARAMETER_01_01 = 0X0400,
            RW_KEY_PARAMETER_01_02,
            RW_KEY_PARAMETER_02_01,
            RW_KEY_PARAMETER_02_02,
            RW_KEY_PARAMETER_03_01,
            RW_KEY_PARAMETER_03_02,
            RW_KEY_PARAMETER_04_01,
            RW_KEY_PARAMETER_04_02,
            RW_KEY_PARAMETER_05_01,
            RW_KEY_PARAMETER_05_02,
            RW_KEY_PARAMETER_06_01,
            RW_KEY_PARAMETER_06_02,
            RW_KEY_PARAMETER_07_01,
            RW_KEY_PARAMETER_07_02,
            RW_KEY_PARAMETER_08_01,
            RW_KEY_PARAMETER_08_02,
            RW_KEY_PARAMETER_09_01,
            RW_KEY_PARAMETER_09_02,
            RW_KEY_PARAMETER_10_01,
            RW_KEY_PARAMETER_10_02,
            RW_KEY_PARAMETER_11_01,
            RW_KEY_PARAMETER_11_02,
            RW_KEY_PARAMETER_12_01,
            RW_KEY_PARAMETER_12_02,
            RW_KEY_PARAMETER_13_01,
            RW_KEY_PARAMETER_13_02,
            RW_KEY_PARAMETER_14_01,
            RW_KEY_PARAMETER_14_02,
            RW_KEY_PARAMETER_15_01,
            RW_KEY_PARAMETER_15_02,
            RW_KEY_PARAMETER_16_01 = 0x0430,
            RW_KEY_PARAMETER_16_02,
            RW_KEY_PARAMETER_17_01,
            RW_KEY_PARAMETER_17_02,
            RW_KEY_PARAMETER_18_01,
            RW_KEY_PARAMETER_18_02,
            RW_KEY_PARAMETER_19_01,
            RW_KEY_PARAMETER_19_02,
            RW_KEY_PARAMETER_20_01,
            RW_KEY_PARAMETER_20_02,
            RW_KEY_PARAMETER_21_01,
            RW_KEY_PARAMETER_21_02,
            RW_KEY_PARAMETER_22_01,
            RW_KEY_PARAMETER_22_02,
            RW_KEY_PARAMETER_23_01,
            RW_KEY_PARAMETER_23_02,
            RW_KEY_PARAMETER_24_01,
            RW_KEY_PARAMETER_24_02,
            RW_KEY_PARAMETER_25_01,
            RW_KEY_PARAMETER_25_02,
            RW_KEY_PARAMETER_26_01,
            RW_KEY_PARAMETER_26_02,
            RW_KEY_PARAMETER_27_01,
            RW_KEY_PARAMETER_27_02,
            RW_KEY_PARAMETER_28_01,
            RW_KEY_PARAMETER_28_02,
            RW_KEY_PARAMETER_29_01,
            RW_KEY_PARAMETER_29_02,
            RW_KEY_PARAMETER_30_01,
            RW_KEY_PARAMETER_30_02,
            RW_KEY_PARAMETER_31_01,
            RW_KEY_PARAMETER_31_02,
            RW_KEY_PARAMETER_32_01,
            RW_KEY_PARAMETER_32_02
        };

        public static string str_IN_BA_ID1;
        public static string str_IN_BA_ID2;
        public static string str_IN_CG_ID;
        public static string str_OUT_BA_ID1;
        public static string str_OUT_BA_ID2;
        public static string str_OUT_CG_ID;

        //==========
        public static string mExceptionMessage = "";

        //====================================================================================================
        public static int mRecipeNo = 0;
        public static string mRecipeName = "";

        public static bool PLCError = false;  // 17.03.22 YHLEE

        //====================================================================================================
        public static System.DateTime timerPLCRead = System.DateTime.Now;
        public static System.DateTime timerPLCWrite = System.DateTime.Now;
        public static System.DateTime timerPLCRead2 = System.DateTime.Now;
        public static System.DateTime timerPLCWrite2 = System.DateTime.Now;

        public static double mTimePLCRead = 0;
        public static double mTimePLCRead2 = 0;
        public static double mTimePLCWrite = 0;
        public static double mTimePLCWrite2 = 0;

        public static object mLock = new object();

        //====================================================================================================
        public static int PLCReadTime = 0;
        public static int PLC2ReadTime = 0;
        public static string PLCReadBaseArea = "D";
        public static int PLCReadBaseAddress = 20000;
        public static int PLCReadBaseAddressLength = 2048;


        public static string ReadBaseArea1;
        public static string PLCReadBaseArea_B = "B";
        public static string PLCReadBaseArea_W = "W";
        public static int PLCReadBaseAddress_B = 0x1000;    //Local에서 읽어야할 주소
        public static int PLCReadBaseAddress_W = 0xD000;
        public static int PLCReadBaseAddressLength_B = 0x1FFF+1;//4096;
        public static int PLCReadBaseAddressLength_W = 0x12FFF+1;//24576;//4096;
        public static short[] PLCReadValue_B_AS_1WORD = new short[PLCReadBaseAddressLength_B / 16];//1WORD단위로 Read
        public static short[] PLCReadValue_W_AS_1WORD = new short[PLCReadBaseAddressLength_W];//1WORD단위로 Read
        public static short[] PLCReadValue_B = new short[PLCReadBaseAddressLength_B];
        public static short[] PLCReadValue_W = new short[PLCReadBaseAddressLength_W];

        public static string mSocketPLC_CommandReadString = "";
        public static string mSocketPLC_CommandWriteString = "";

        public enum MATERIAL_ID_TYPE
        {
            BA1 = 0,
            BA2,
            CG,
        }

        public enum enumDeviceType
        {
            X = 1,
            Y = 2,
            L = 3,
            M = 4,
            SpecialM = 5,
            F = 6,
            TT = 7,
            TC = 8,
            TN = 11,
            CT = 9,
            CC = 10,
            CN = 12,
            STT = 26,
            STC = 27,
            STN = 35,
            D = 13,
            SpecialD = 14,
            A = 19,
            Z = 20,
            V = 21,
            R = 22,
            B = 23,
            W = 24,
            SB = 25,
            SW = 28,
            QV = 30,
            RWw = 36,
            RWr = 37,
            LZ = 38,
            RD = 39,
            LTT = 41,
            LTC = 42,
            LTN = 43,
            LCT = 44,
            LCC = 45,
            LCN = 46,
            LSTT = 47,
            LSTC = 48,
            LSTN = 49,
            ExtendedR = 22000,
            TM = 15,
            TS = 16,
            TS2 = 16002,
            TS3 = 16003,
            CM = 17,
            CS = 18,
            CS2 = 18002,
            CS3 = 18003,
            OwnBufferMemory = 50,
            SendReceive = 101,
        }

        public static short[] GetPLCReadValue(int plcIndex, int nDeviceType, bool bIsWriteAddres)
        {
            switch (nDeviceType)
            {
                case (int)enumDeviceType.B:
                    if (bIsWriteAddres)
                        return PLCWriteValue_B;
                    else
                        return PLCReadValue_B;
                case (int)enumDeviceType.W:
                    if (bIsWriteAddres)
                        return PLCWriteValue_W;
                    else
                        return PLCReadValue_W;
                default:
                    return null;
            }
        }
        public static short[] GetPLCWriteValue(int plcIndex, int nDeviceType)
        {
            switch (nDeviceType)
            {
                case (int)enumDeviceType.B:
                    return PLCWriteValue_B;
                case (int)enumDeviceType.W:
                    return PLCWriteValue_W;
                default:
                    return null;
            }
        }

        public static int PLCWriteTime = 0;

        public static string PLCWriteBaseArea_B = "B";
        public static string PLCWriteBaseArea_W = "W";
        public static int PLCWriteBaseAddress_B = 0x0000;   //Local에서 Write하는 주소
        public static int PLCWriteBaseAddress_W = 0x0000;

        public static int PLCWriteBaseAddressLength_B = 0x0FFF+1;//4096;//128;
        public static int PLCWriteBaseAddressLength_W = 0xCFFF+1;//53248;//1024;
        //PLC Write 영역 쓸 값
        public static short[] PLCWriteValue_B_AS_1WORD_FromPCRead = new short[PLCWriteBaseAddressLength_B / 16];//1WORD단위로 Write.
        public static short[] PLCWriteValue_W_AS_1WORD_FromPCRead = new short[PLCWriteBaseAddressLength_W];//1WORD단위로 Write
        //PLC Write 영역 읽어온 값
        public static short[] PLCWriteValue_B_AS_1WORD = new short[PLCWriteBaseAddressLength_B / 16];//1WORD단위로 Write
        public static short[] PLCWriteValue_W_AS_1WORD = new short[PLCWriteBaseAddressLength_W];//1WORD단위로 Write
        //PLC Write 영역 Update할 값
        public static short[] PLCWriteValue_B = new short[PLCWriteBaseAddressLength_B];
        public static short[] PLCWriteValue_W = new short[PLCWriteBaseAddressLength_W];

        public static classSOCKET mSOCKET = new classSOCKET();
        public static string mSocket_DefineIPAddress;
        public static int mSocket_DefinePortNo = 8001;

        public static string mSocket_DefineIPAddress2;
        public static int mSocket_DefinePortNo2 = 8002;

        public static string Command1EFrameRead(byte pSubHeader, byte pPLCNo, ushort pACPUMonitoringTimer, char pHeadDevice1, uint pHeadDevice2, byte pNumberOfDevicePoints, byte pBlank)
        {
            mExceptionMessage = "";

            //
            string returnvalue = "";

            //
            if (pHeadDevice1.ToString() == "")
            {
                pHeadDevice1 = 'D';
            }

            //
            try
            {
                //
                string[] mSocketPLC_CommandRead = new string[7];
                mSocketPLC_CommandRead[0] = Convert.ToString(pSubHeader, 16).PadLeft(2, '0').ToUpper();
                mSocketPLC_CommandRead[1] = Convert.ToString(pPLCNo, 16).PadLeft(2, '0').ToUpper();
                mSocketPLC_CommandRead[2] = Convert.ToString(pACPUMonitoringTimer, 16).PadLeft(4, '0').ToUpper();
                mSocketPLC_CommandRead[3] = Convert.ToString(Convert.ToByte(pHeadDevice1), 16).PadLeft(2, '0').ToUpper();
                mSocketPLC_CommandRead[4] = Convert.ToString(pHeadDevice2, 16).PadLeft(8, '0').ToUpper();
                mSocketPLC_CommandRead[5] = Convert.ToString(pNumberOfDevicePoints, 16).PadLeft(2, '0').ToUpper();
                mSocketPLC_CommandRead[6] = Convert.ToString(pBlank, 16).PadLeft(2, '0').ToUpper();

                //
                returnvalue += mSocketPLC_CommandRead[0];
                returnvalue += mSocketPLC_CommandRead[1];
                returnvalue += mSocketPLC_CommandRead[2].Substring(2, 2) + mSocketPLC_CommandRead[2].Substring(0, 2);
                returnvalue += mSocketPLC_CommandRead[4].Substring(6, 2) + mSocketPLC_CommandRead[4].Substring(4, 2) + mSocketPLC_CommandRead[4].Substring(2, 2) + mSocketPLC_CommandRead[4].Substring(0, 2);
                returnvalue += "20" + mSocketPLC_CommandRead[3];
                returnvalue += mSocketPLC_CommandRead[5];
                returnvalue += mSocketPLC_CommandRead[6];

            }
            catch (SocketException exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }
            catch (Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }

            //
            return returnvalue;
        }  // public end

        public static string Command1EFrameWrite(byte pSubHeader, byte pPLCNo, ushort pACPUMonitoringTimer, char pHeadDevice1, uint pHeadDevice2, byte pNumberOfDevicePoints, byte pBlank, short[] pValues)
        {
            mExceptionMessage = "";

            //
            string returnvalue = "";

            //
            if (pHeadDevice1.ToString() == "")
            {
                pHeadDevice1 = 'D';
            }

            //
            if ((pValues == null) || (pValues.Length == 0))
            {
                pNumberOfDevicePoints = 1;
                pValues = new short[1];
                pValues[0] = 0;
            }

            //
            if (pNumberOfDevicePoints != pValues.Length)
            {
                pNumberOfDevicePoints = (byte)pValues.Length;
            }

            //
            try
            {
                //
                string[] mSocketPLC_CommandWrite = new string[7];
                mSocketPLC_CommandWrite[0] = Convert.ToString(pSubHeader, 16).PadLeft(2, '0').ToUpper();
                mSocketPLC_CommandWrite[1] = Convert.ToString(pPLCNo, 16).PadLeft(2, '0').ToUpper();
                mSocketPLC_CommandWrite[2] = Convert.ToString(pACPUMonitoringTimer, 16).PadLeft(4, '0').ToUpper();
                mSocketPLC_CommandWrite[3] = Convert.ToString(Convert.ToByte(pHeadDevice1), 16).PadLeft(2, '0').ToUpper();
                mSocketPLC_CommandWrite[4] = Convert.ToString(pHeadDevice2, 16).PadLeft(8, '0').ToUpper();
                mSocketPLC_CommandWrite[5] = Convert.ToString(pNumberOfDevicePoints, 16).PadLeft(2, '0').ToUpper();
                mSocketPLC_CommandWrite[6] = Convert.ToString(pBlank, 16).PadLeft(2, '0').ToUpper();

                returnvalue += mSocketPLC_CommandWrite[0];
                returnvalue += mSocketPLC_CommandWrite[1];
                returnvalue += mSocketPLC_CommandWrite[2].Substring(2, 2) + mSocketPLC_CommandWrite[2].Substring(0, 2);
                returnvalue += mSocketPLC_CommandWrite[4].Substring(6, 2) + mSocketPLC_CommandWrite[4].Substring(4, 2) + mSocketPLC_CommandWrite[4].Substring(2, 2) + mSocketPLC_CommandWrite[4].Substring(0, 2);
                returnvalue += "20" + mSocketPLC_CommandWrite[3];
                returnvalue += mSocketPLC_CommandWrite[5];
                returnvalue += mSocketPLC_CommandWrite[6];

                //
                for (int i = 0; i < pValues.Length; i++)
                {
                    string sHex = Convert.ToString(pValues[i], 16).PadLeft(4, '0');
                    returnvalue += sHex.Substring(2, 2) + sHex.Substring(0, 2);
                }
            }
            catch (SocketException exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }
            catch (Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }

            //
            return returnvalue;
        }

        /// <summary>
        /// SLMP Write Protocol
        /// </summary>
        /// <param name="pSubHead">5000</param>
        /// <param name="pNetworkNo">00</param>
        /// <param name="pStationNo">FF</param>
        /// <param name="pModuleIONo">03E0</param>
        /// <param name="pMultiDropNo">00</param>
        /// <param name="pTimer">000A</param>
        /// <param name="pCommandMain">1401</param>
        /// <param name="pCommandSub">0000</param>
        /// <param name="pDeviceCode">D*</param>
        /// <param name="pDeviceFirstNo">004100</param>
        /// <returns></returns>
        public static string CommandSLMPWrite(
            string pSubHead, string pNetworkNo, string pStationNo, string pModuleIONo, string pMultiDropNo, string pTimer,
            string pCommandMain, string pCommandSub, string pDeviceCode, string pDeviceFirstNo,
            short[] pValues)
        {
            //
            string returnvalue = "";

            //
            try
            {
                // 요구 데이터
                string sData = "";
                sData += pTimer;          //"000A";    // 감시 타이머
                sData += pCommandMain;    //"1401";    // 커맨드
                sData += pCommandSub;     //"0000";    // 서브 커맨드  // 0001=비트단위, 0000=워드단위
                sData += pDeviceCode;     //"D*";      // 디바이스 코드
                sData += pDeviceFirstNo;  //"004100";  // 선두 디바이스 번호
                sData += Convert.ToString(pValues.Length, 16).ToUpper().PadLeft(4, '0');   //"0064";    // 읽을 개수
                for (int i = 0; i < pValues.Length; i++)
                {
                    sData += Convert.ToString(pValues[i], 16).PadLeft(4, '0');
                }

                //    = 헤드(자동) | 서브 헤더 | 요구 상대 네트워크 번호 | 요구 상태 국번호 | 요구 상대 모듈 I/O 번호 | 요구 상대 멀티 드롭 국번호 | 요구 데이터 길이 | 감시 타이머 | 요구 데이터 | 푸터(자동)
                string sSend = "";
                sSend += pSubHead;      //"5000";  // 서브 헤더
                sSend += pNetworkNo;    //"00";    // 요구 상대 네트워크 번호
                sSend += pStationNo;    //"FF";    // 요구 상대 국번호
                sSend += pModuleIONo;   //"03E0";  //"03FF";  // 요구 상대 모듈 I/O 번호
                sSend += pMultiDropNo;  //"00";    // 요구 상대 멀티 드롭 국번호
                sSend += Convert.ToString(sData.Length, 16).PadLeft(4, '0').ToUpper();  // 요구 데이터 길이
                sSend += sData;  // 요구 데이터

                returnvalue = sSend;
            }
            catch (Exception exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }

            //
            mSocketPLC_CommandWriteString = returnvalue;

            //
            return returnvalue;
        }

        public static byte[] Hexdecimal2Byte(string pHexdecimal)
        {
            mExceptionMessage = "";

            byte[] returnvalue = null;

            try
            {
                returnvalue = new byte[pHexdecimal.Length / 2];

                int iStringCount = 0;

                for (int i = 0; i < pHexdecimal.Length; i = i + 2)
                {
                    string s = pHexdecimal.Substring((iStringCount * 2), 2);

                    returnvalue[iStringCount] = Convert.ToByte(s, 16);

                    iStringCount++;
                }
            }
            catch (Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }

            return returnvalue;
        }

        public static void ValueHighLow(long pValue, ref short pHigh, ref short pLow)
        {
            mExceptionMessage = "";

            try
            {
                pLow = (short)(pValue & 0xFFFF);
                pHigh = (short)((pValue >> 16) % 0xFFFF);
            }
            catch (Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }
        }

        public static short[] Receive_1EFrameConvert(byte[] pBuffer)
        {
            mExceptionMessage = "";

            short[] returnvalue = new short[1];

            if ((pBuffer == null) || (pBuffer.Length == 0))
            {
                returnvalue = new short[1];
                returnvalue[0] = 0;
                return returnvalue;
            }

            if (pBuffer.Length <= 2)
            {
                returnvalue = new short[1];
                returnvalue[0] = (short)pBuffer[0];
                return returnvalue;
            }

            try
            {
                int iBufferSize = pBuffer.Length;
                int iResults = (iBufferSize - 2) / 2;

                returnvalue = new short[iResults];

                int iCount = 0;

                for (int i = 2; i < iBufferSize; i += 2)
                {
                    returnvalue[iCount] = (short)((pBuffer[i + 1] << 8) + pBuffer[i + 0]);
                    iCount++;
                }
            }
            catch (Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }

            return returnvalue;
        }
        public static short[] Receive_1EFrameConvertSiemens(byte[] pBuffer)
        {
            short[] returnvalue = new short[1];

            if ((pBuffer == null) || (pBuffer.Length == 0))
            {
                returnvalue = new short[1];
                returnvalue[0] = 0;
                return returnvalue;
            }

            if (pBuffer.Length <= 2)
            {
                returnvalue = new short[1];
                returnvalue[0] = (short)pBuffer[0];
                return returnvalue;
            }

            try
            {
                int iBufferSize = pBuffer.Length;
                int iResults = (iBufferSize) / 2;

                returnvalue = new short[iResults];

                int iCount = 0;

                for (int i = 0; i < iBufferSize; i += 2)
                {
                    returnvalue[iCount] = (short)((pBuffer[i + 0] << 8) + pBuffer[i + 1]);
                    iCount++;
                }
            }
            catch (Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }

            return returnvalue;
        }


        //====================================================================================================
        public static void PLCRead(bool pNoteBookMode, int plcIndex, int nDeviceType, bool bIsWriteAddres)
        {
            mExceptionMessage = "";

            if ((pNoteBookMode == true) || (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false))
            {
                return;
            }

            if (MCCLinkIE.mCCLinkIEUse == true)  // 20.01.09 YHLEE
            {
                lock (mLock)
                {
                    if (MCCLinkIE.Receive_RDevice(nDeviceType, bIsWriteAddres) == -6672)
                    {
                        MCCLinkIE.Open();
                    }
                }
                return;
            }
            else
            {
                if (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false)
                {
                    return;
                }

            }
        }

        public static void PLCWrite(bool pNoteBookMode, int plcIndex, int nDeviceType, bool bIsReadAddress)
        {
            mExceptionMessage = "";

            if ((pNoteBookMode == true) || (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false))
            {
                return;
            }

            if (MCCLinkIE.mCCLinkIEUse == true)
            {
                lock (mLock)
                {
                    MCCLinkIE.Send_SDevice(nDeviceType, bIsReadAddress);
                };
                return;
            }
            else
            {
                if (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false)
                {
                    return;
                }
            }
        }

        public static int Read_Bit(bool pNoteBookMode, int nDeviceType, int nAddress)
        {
            mExceptionMessage = "";
            int nReturnValue = -1;
            if ((pNoteBookMode == true) || (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false))
            {
                return nReturnValue;
            }

            if (MCCLinkIE.mCCLinkIEUse == true)  // 20.01.09 YHLEE
            {
                lock (mLock)
                {
                    if (MCCLinkIE.Receive_RDevice_Bit(nDeviceType, nAddress) == -6672)
                    {
                        MCCLinkIE.Open();
                    }
                    nReturnValue = MCCLinkIE.Receive_RDevice_Bit(nDeviceType, nAddress);
                }
                return nReturnValue;
            }
            else
            {
                if (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false)
                {
                    return nReturnValue;
                }

            }
            return nReturnValue;
        }

        public static int Read_Word(bool pNoteBookMode, int nDeviceType, int nAddress, int nLength)
        {
            mExceptionMessage = "";
            int nReturnValue = -1;
            if ((pNoteBookMode == true) || (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false))
            {
                return nReturnValue;
            }

            if (MCCLinkIE.mCCLinkIEUse == true)  // 20.01.09 YHLEE
            {
                lock (mLock)
                {
                    if (MCCLinkIE.Receive_RDevice_Word(nDeviceType, nAddress, nLength) == -6672)
                    {
                        MCCLinkIE.Open();
                    }
                    nReturnValue = MCCLinkIE.Receive_RDevice_Word(nDeviceType, nAddress, nLength);
                }
                return nReturnValue;
            }
            else
            {
                if (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false)
                {
                    return nReturnValue;
                }

            }
            return nReturnValue;
        }

        public static int Write_Bit(bool pNoteBookMode, int nDeviceType, int nAddress, bool BitFlag)
        {
            mExceptionMessage = "";
            int nResult = 0;
            if ((pNoteBookMode == true) || (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false))
            {
                return nResult;
            }

            if (MCCLinkIE.mCCLinkIEUse == true)
            {
                lock (mLock)
                {
                    MCCLinkIE.Send_SDevice_Bit(nDeviceType, nAddress, BitFlag);
                };
                return nResult;
            }
            else
            {
                if (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false)
                {
                    return nResult;
                }
            }
            return nResult;
        }

        public static int Write_Word(bool pNoteBookMode, int nDeviceType, int nAddress, int nData, int nLength)
        {
            mExceptionMessage = "";
            int nResult = 0;
            if ((pNoteBookMode == true) || (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false))
            {
                return nResult;
            }

            if (MCCLinkIE.mCCLinkIEUse == true)
            {
                lock (mLock)
                {
                    MCCLinkIE.Send_SDevice_Word(nDeviceType, nAddress, nData, nLength);
                };
                return nResult;
            }
            else
            {
                if (MPLC.mSOCKET.Connected() == false && MCCLinkIE.mCCLinkIEUse == false)
                {
                    return nResult;
                }
            }
            return nResult;
        }

        public static short[] PLCReadDataConvert()
        {
            mExceptionMessage = "";

            short[] returnvalue = new short[MPLC.PLCReadBaseAddressLength];

            try
            {
                short[] shortBuffer = Receive_1EFrameConvert(Encoding.Default.GetBytes(mSOCKET.mReceiveString));

                for (int i = 0; i < shortBuffer.Length; i++)
                {
                    if (returnvalue.Length > i) returnvalue[i] = shortBuffer[i];
                }
            }
            catch (Exception exception)
            {
                mExceptionMessage = exception.TargetSite.ReflectedType.FullName + Environment.NewLine + exception.TargetSite.Name + Environment.NewLine + exception.StackTrace.ToString().Trim() + Environment.NewLine + exception.Message.Trim();
            }

            return returnvalue;
        }  // public end

        public static short[] Receive_SLMPConvert(byte[] pBuffer)
        {
            short[] returnvalue = new short[1];

            return returnvalue;
        }

        //====================================================================================================

        public static bool PLCWrite2(int pStartAddress, int pWriteLength)
        {
            return true;
        }

        /// <summary>
        /// SLMP Read Protocol
        /// </summary>
        /// <param name="pSubHead">5000</param>
        /// <param name="pNetworkNo">00</param>
        /// <param name="pStationNo">FF</param>
        /// <param name="pModuleIONo">03E0</param>
        /// <param name="pMultiDropNo">00</param>
        /// <param name="pTimer">000A</param>
        /// <param name="pCommandMain">0401</param>
        /// <param name="pCommandSub">0000</param>
        /// <param name="pDeviceCode">D*</param>
        /// <param name="pDeviceFirstNo">004000</param>
        /// <param name="pDeviceLength">0100</param>
        /// <returns></returns>
        public static string CommandSLMPRead(
            string pSubHead, string pNetworkNo, string pStationNo, string pModuleIONo, string pMultiDropNo, string pTimer,
            string pCommandMain, string pCommandSub, string pDeviceCode, string pDeviceFirstNo, string pDeviceLength)
        {
            //
            string returnvalue = "";

            //
            try
            {
                // 요구 데이터
                string sData = "";
                sData += pTimer;          //"000A";    // 감시 타이머
                sData += pCommandMain;    //"0401";    // 커맨드
                sData += pCommandSub;     //"0000";    // 서브 커맨드  // 0001=비트단위, 0000=워드단위
                sData += pDeviceCode;     //"D*";      // 디바이스 코드
                sData += pDeviceFirstNo;  //"004000";  // 선두 디바이스 번호
                sData += pDeviceLength;   //"0001";    // 읽을 개수

                //    = 헤드(자동) | 서브 헤더 | 요구 상대 네트워크 번호 | 요구 상태 국번호 | 요구 상대 모듈 I/O 번호 | 요구 상대 멀티 드롭 국번호 | 요구 데이터 길이 | 감시 타이머 | 요구 데이터 | 푸터(자동)
                string sSend = "";
                sSend += pSubHead;      //"5000";  // 서브 헤더
                sSend += pNetworkNo;    //"00";    // 요구 상대 네트워크 번호
                sSend += pStationNo;    //"FF";    // 요구 상대 국번호
                sSend += pModuleIONo;   //"03E0";  //"03FF";  // 요구 상대 모듈 I/O 번호
                sSend += pMultiDropNo;  //"00";    // 요구 상대 멀티 드롭 국번호
                sSend += Convert.ToString(sData.Length, 16).PadLeft(4, '0').ToUpper();  // 요구 데이터 길이
                sSend += sData;  // 요구 데이터

                //
                returnvalue = sSend;
            }
            catch (SocketException exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }
            catch (Exception exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }

            //
            mSocketPLC_CommandReadString = returnvalue;

            //
            return returnvalue;
        }
        public static string CommandSLMPRead2(
            string pSubHead, string pNetworkNo, string pStationNo, string pModuleIONo, string pMultiDropNo, string pTimer,
            string pCommandMain, string pCommandSub, string pDeviceCode, int pDeviceFirstNo, int pDeviceLength)
        {
            //
            string returnvalue = "";

            //
            try
            {
                // 요구 데이터
                string sData = "";
                sData += pTimer;          //"000A";    // 감시 타이머
                sData += pCommandMain;    //"0401";    // 커맨드
                sData += pCommandSub;     //"0000";    // 서브 커맨드  // 0001=비트단위, 0000=워드단위
                sData += pDeviceCode;     //"D*";      // 디바이스 코드
                sData += pDeviceFirstNo.ToString().PadLeft(6, '0');  //"004000";  // 선두 디바이스 번호
                sData += Convert.ToString((pDeviceLength > 255 ? 255 : pDeviceLength), 16).ToUpper().PadLeft(4, '0');   //"0001";    // 읽을 개수

                //    = 헤드(자동) | 서브 헤더 | 요구 상대 네트워크 번호 | 요구 상태 국번호 | 요구 상대 모듈 I/O 번호 | 요구 상대 멀티 드롭 국번호 | 요구 데이터 길이 | 감시 타이머 | 요구 데이터 | 푸터(자동)
                string sSend = "";
                sSend += pSubHead;      //"5000";  // 서브 헤더
                sSend += pNetworkNo;    //"00";    // 요구 상대 네트워크 번호
                sSend += pStationNo;    //"FF";    // 요구 상대 국번호
                sSend += pModuleIONo;   //"03E0";  //"03FF";  // 요구 상대 모듈 I/O 번호
                sSend += pMultiDropNo;  //"00";    // 요구 상대 멀티 드롭 국번호
                sSend += Convert.ToString(sData.Length, 16).PadLeft(4, '0').ToUpper();  // 요구 데이터 길이
                sSend += sData;  // 요구 데이터

                //
                returnvalue = sSend;
            }
            catch (SocketException exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }
            catch (Exception exception)
            {
                MEXCEPTION.FileSave("Exception", exception.TargetSite.ReflectedType.FullName, exception.TargetSite.Name, exception.StackTrace.ToString().Trim(), exception.Message.Trim());
            }

            //
            mSocketPLC_CommandReadString = returnvalue;

            //
            return returnvalue;
        }
        public static int Convert_HEXToDecimal_1Character(string strData)
        {
            int nRtn = 0;

            if ((string.Compare(strData, "0", false) != 0)
                && (string.Compare(strData, "A", false) != 0)
                && (string.Compare(strData, "B", false) != 0)
                && (string.Compare(strData, "C", false) != 0)
                && (string.Compare(strData, "D", false) != 0)
                && (string.Compare(strData, "E", false) != 0)
                && (string.Compare(strData, "F", false) != 0))
            {
                bool bOK = int.TryParse(strData, out nRtn);// atoi(strData);
            }
            else
            {
                if (string.Compare(strData, "A", false) == 0) nRtn = 10;
                if (string.Compare(strData, "B", false) == 0) nRtn = 11;
                if (string.Compare(strData, "C", false) == 0) nRtn = 12;
                if (string.Compare(strData, "D", false) == 0) nRtn = 13;
                if (string.Compare(strData, "E", false) == 0) nRtn = 14;
                if (string.Compare(strData, "F", false) == 0) nRtn = 15;
            }
            return nRtn;
        }

        //	문자 -> HEX Data
        public static string MakeHexDatatoPLC(string strData)
        {
            string resultHex = string.Empty;
            byte[] arr_byteStr = Encoding.Default.GetBytes(strData);

            foreach (byte byteStr in arr_byteStr)
                resultHex += string.Format("{0:X2}", byteStr);

            return resultHex;
        }
        //srkim(231115) - hex string 두개를 받아 doubleword형으로 반환;
        public static float MakeDobleWordDatafromPLC(string strData)
        {
            float resultDoubleWord = 0;

            resultDoubleWord = Convert.ToInt64(strData, 16);

            return resultDoubleWord;
        }

        //	HEX Data -> 문자
        //	"00"은 Space로 처리한다.
        public static string MakeStringDatafromPLC(string lText)
        {
            int dCount;

            string strReturn = string.Empty;
            string strTemp;

            //strReturn.Empty();

            for (dCount = 0; dCount < lText.Length; dCount += 4)
            {
                strTemp = lText.Substring(dCount + 2, 2);
                if (strTemp.Equals("00"))
                {
                    strReturn += " ";
                }
                else
                {
                    // Hex 값을 10진수로 변환후 문자로 변경한다.
                    int nTemp = HexToByte(strTemp);
                    char str = Convert.ToChar(nTemp);
                    strReturn += str;
                }

                strTemp = lText.Substring(dCount, 2);
                if (strTemp.Equals("00"))
                {
                    strReturn += " ";
                }
                else
                {
                    // Hex 값을 10진수로 변환후 문자로 변경한다.
                    int nTemp = HexToByte(strTemp);
                    char str = Convert.ToChar(nTemp);
                    strReturn += str;
                }
            }
            return strReturn;
        }

        public static int HexToByte(string lData)
        {
            int dFirst, dSecond;

            dFirst = Convert_HEXToDecimal_1Character(lData.Substring(0, 1));
            dSecond = Convert_HEXToDecimal_1Character(lData.Substring(1, 1));

            return (dFirst * 16 + dSecond);
        }
        public static short HexString2PLC(string strData)    //srkim(231112) - 형변환 ("1A2B" -> "2B1A" -> 10진수로 바꾸고 int형으로 리턴)
        {
            int nTemp = 0;
            short nValue = 0;

            nTemp = Convert_HEXToDecimal_1Character(strData.Substring(0, 1));
            nValue = (short)(nTemp * 16);
            nTemp = Convert_HEXToDecimal_1Character(strData.Substring(1, 1));
            nValue += (short)nTemp;
            nTemp = Convert_HEXToDecimal_1Character(strData.Substring(2, 1));
            nValue += (short)(nTemp * 16 * 16 * 16);
            nTemp = Convert_HEXToDecimal_1Character(strData.Substring(3, 1));
            nValue += (short)(nTemp * 16 * 16);

            return nValue;
        }
        public static string str2hex(string strData)
        {
            string resultHex = string.Empty;
            byte[] arr_byteStr = Encoding.Default.GetBytes(strData);

            foreach (byte byteStr in arr_byteStr)
                resultHex += string.Format("{0:X2}", byteStr);

            return resultHex;
        }
    }    
}
