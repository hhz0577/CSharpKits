using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzUtil.edz
{
    using System.Runtime.InteropServices;
    using System.IO;
    //神思串口100二代证读卡类
    public class SS100 : IReadCard
    {
        [DllImport(@"ser\newdll.dll", EntryPoint = "OpenComm", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int OpenComm(int Comm, int nBaud);
        [DllImport(@"ser\newdll.dll", EntryPoint = "CloseComm", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseComm();

        [DllImport(@"ser\newdll.dll", EntryPoint = "CardhaveRd", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CardhaveRd();//监测到有身份证卡并已读到正确数据

        [DllImport(@"ser\newdll.dll", EntryPoint = "ClearData", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ClearData();
        [DllImport(@"ser\newdll.dll", EntryPoint = "RdTxt", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int RdTxt(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "ReadName", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadName(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "ReadSex", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadSex(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "Readminzu", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Readminzu(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "ReadBirth", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadBirth(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "ReadAddr", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadAddr(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "ReadNumber", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadNumber(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "Readqianfa", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Readqianfa(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "ReadBegin", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadBegin(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "ReadEnd", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadEnd(char[] data);
        [DllImport(@"ser\newdll.dll", EntryPoint = "ReadBmpAll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadBmpAll(byte[] data);

        [DllImport(@"ser\WltRS.dll", EntryPoint = "GetBmp", CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetBmp(char[] Wlt_File, int intf);//Wlt_File 为从机具中读出的数据存储的文件名(pic.wlt),intf = 1


        private int nBaud = 115200;//波特率,19200 或者115200，根据机具情况确定
        public void setIstatus(int istatus)//设置波特率
        {
            this.nBaud = istatus;
        }
        private int iport = 1;

        public void setIport(int iport)
        {
            this.iport = iport;
        }

        private string xm = "";

        public string getXm()
        {
            return xm;
        }
        private string sex = "";

        public string getSex()
        {
            return sex;
        }
        private string mz = "";

        public string getMz()
        {
            return mz;
        }
        private string birth = "";

        public string getBirth()
        {
            return birth;
        }
        private string address = "";

        public string getAddress()
        {
            return address;
        }
        private string sfz = "";

        public string getSfz()
        {
            return sfz;
        }
        private string qfjg = "";

        public string getQfjg()
        {
            return qfjg;
        }
        private string yxqq = "";

        public string getYxqq()
        {
            return yxqq;
        }
        private string yxqz = "";

        public string getYxqz()
        {
            return yxqz;
        }
        private string samid = "";
        public string getSamid()
        {
            return samid;
        }

        private string txzh = "";
        public string getTxzh()
        {
            return txzh;
        }

        private string qfcs = "";
        public string getQfcs()
        {
            return qfcs;
        }

        private string carttype = "公民身份证";
        public string getCartType()
        {
            return carttype;
        }
        private string error = "";

        public string getError()
        {
            return error;
        }
        private string photourl;
        public void setPhotoUrl(string photourl)
        {
        }
        public string getPhotoUrl()
        {
            return photourl;
        }
        public bool Open(int iport)
        {
            error = "";
            this.iport = iport;
            int ir = OpenComm(iport,nBaud);
            if (ir != 1)
            {
                error = "阅读机具未连接";
                return false;
            }
            else
                return true;
        }
        public bool GetSAM()//脚手架
        {
            return true;
        }
        public bool HasCard()
        {
            int irtn = CardhaveRd();
            if (irtn == 1) return true;
            else return false;
        }
        public bool ReadData()
        {
            
            //Open(1);
            bool f = HasCard();
            if (!f)
            {
                error = "请将身份证放好，再试试";
                return false;
            }
            f = _readDate();
            if (!f)
            {
                ClearData();
                return false;
            }
            //f= readName();
            //if (!f)
            //{
            //    ClearData();
            //    return false;
            //}
            //f = readSex();
            //if (!f)
            //{
            //    ClearData();
            //    return false;
            //}
            //f = readMz();
            //if (!f)
            //{
            //    ClearData();
            //    return false;
            //}
            //f = readBirth();
            //if (!f)
            //{
            //    ClearData();
            //    return false;
            //}
            //f = readAddr();
            //if (!f)
            //{
            //    ClearData();
            //    return false;
            //}
            //f = readNumber();
            //if (!f)
            //{
            //    ClearData();
            //    return false;
            //}
            //f = readQfjg();
            //if (!f)
            //{
            //    ClearData();
            //    return false;
            //}
            //f = readYxqq();
            //if (!f)
            //{
            //    ClearData();
            //    return false;
            //}
            //f = readYxqz();
            //if (!f)
            //{
            //    ClearData();
            //    return false;
            //}
            f = readBmp();
            if (!f)
            {
                ClearData();
                return false;
            }
            ClearData();
            return true;
        }
        public bool ReadGOTData()//脚手架
        {
            return true;
        }
        public bool ReadICData()
        {
            return ReadData();
        }
        public bool Close()
        {
            error = "";
            int ir = CloseComm();
            if (ir != 1)
            {
                error = "关闭端口失败";
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool _readDate()
        {
            char[] data = new char[256];
            int iread = RdTxt(data);
            return processAllData(iread,data);
        }
        private bool readName()
        {
            char[] data = new char[30];
            int iread = ReadName(data);
            return processData(iread, data,ref xm);
        }
        private bool readSex()
        {
            char[] data = new char[1];
            int iread = ReadSex(data);
            return processData(iread, data,ref sex);
        }
        private bool readMz()
        {
            char[] data = new char[2];
            int iread = Readminzu(data);
            return processData(iread, data, ref mz);
        }
        private bool readBirth()
        {
            char[] data = new char[8];
            int iread = ReadBirth(data);
            return processDateData(iread, data, ref birth);
        }
        private bool readAddr()
        {
            char[] data = new char[70];
            int iread = ReadAddr(data);
            return processData(iread, data, ref address);
        }
        private bool readNumber()
        {
            char[] data = new char[18];
            int iread = ReadNumber(data);
            return processData(iread, data, ref sfz);
        }
        private bool readQfjg()
        {
            char[] data = new char[30];
            int iread = Readqianfa(data);
            return processData(iread, data, ref qfjg);
        }
        private bool readYxqq()
        {
            char[] data = new char[16];
            int iread = ReadBegin(data);
            return processDateData(iread, data, ref yxqq);
        }
        private bool readYxqz()
        {
            char[] data = new char[16];
            int iread = ReadEnd(data);
            return processDateData(iread, data, ref yxqz);
        }
        private bool readBmp()
        {
            error = "";
            byte[] data = new byte[1024];
            int ireadimage = ReadBmpAll(data);
            if (ireadimage == 1)
            {
                using (FileStream fsWrite = new FileStream(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "pic.wlt", FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fsWrite.Write(data, 0, data.Length);
                }
                string imagefilename = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "pic.wlt";
                char[] cc = imagefilename.ToCharArray();
                int iimage = GetBmp(cc, 2);
                if (iimage == 1)
                {
                    photourl = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "pic.bmp";
                    return true;
                }
                else
                {
                    error = "读卡失败";
                    photourl = "";
                    return false;
                }
            }
            else
            {
                error = "读卡失败";
                return false;
            }
        }
        private bool processAllData(int iread, char[] data)
        {
            error = "";
            StringBuilder sb = new StringBuilder();
            if (iread == 1)
            {

                foreach (char c in data)
                {
                    sb.Append(c);
                }
                xm = sb.ToString().Substring(0, 15).Trim();
                sex = sb.ToString().Substring(15, 1).Trim();
                mz = sb.ToString().Substring(16, 2).Trim();
                birth = sb.ToString().Substring(18, 8).Trim();
                birth = birth.Substring(0, 4) + "-" + birth.Substring(4, 2) + "-" + birth.Substring(6);
                address = sb.ToString().Substring(26, 35).Trim();
                sfz = sb.ToString().Substring(61, 18).Trim();
                qfjg = sb.ToString().Substring(79, 15).Trim();
                yxqq = sb.ToString().Substring(94, 8).Trim();
                yxqq = yxqq.Substring(0, 4) + "-" + yxqq.Substring(4, 2) + "-" + yxqq.Substring(6);
                yxqz = sb.ToString().Substring(102, 8).Trim();
                if(yxqz.Length == 8) yxqz = yxqz.Substring(0, 4) + "-" + yxqz.Substring(4, 2) + "-" + yxqz.Substring(6);
                return true;
            }
            else
            {
                error = "读卡失败";
                return false;
            }
        }
        private bool processDateData(int iread, char[] data, ref string value)
        {
            error = "";
            StringBuilder sb = new StringBuilder();
            if (iread == 1)
            {
                foreach (char c in data)
                {
                    if (c != '\0') sb.Append(c);
                }
                value = sb.ToString().Trim();
                if (value.Length == 8)
                {
                    value = value.Substring(0, 4) + "-" + value.Substring(4, 2) + "-" + value.Substring(6);
                }
                return true;
            }
            else
            {
                error = "读卡失败";
                return false;
            }
        }
        private bool processData(int iread, char[] data,ref string value)
        {
            error = "";
            StringBuilder sb = new StringBuilder();
            if (iread == 1)
            {
                foreach (char c in data)
                {
                    if (c != '\0') sb.Append(c);
                }
                value = sb.ToString().Trim();
                return true;
            }
            else
            {
                error = "读卡失败";
                return false;
            }
        }
        
    }
}
