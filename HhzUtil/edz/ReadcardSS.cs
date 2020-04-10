using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.edz
{
    using System.Runtime.InteropServices;
    /// <summary>
    /// 读二代证 FOR 神思
    /// </summary>
    public class ReadcardSS : IReadCard
    {
        /************************端口类API *************************/
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "InitComm", CallingConvention = CallingConvention.StdCall)]
        public static extern int InitComm(int port);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "CloseComm", CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseComm();
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetSAMIDToStr", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetSAMIDToStr(StringBuilder Samid);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "Authenticate", CallingConvention = CallingConvention.StdCall)]
        public static extern int Authenticate();
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "UCommand1", CallingConvention = CallingConvention.StdCall)]
        private static extern int UCommand1(ref byte pCmd, ref int parg0, ref int parg1, ref int parg2);
        /*************************身份证卡类函数 ***************************/
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetName", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetName(StringBuilder name);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetSex", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetSex(StringBuilder sex);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetSexGB", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetSexGB(StringBuilder sex);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetFolk", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFolk(StringBuilder mz);//读取民族
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetFolkGB", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetFolkGB(StringBuilder mz);//读取民族
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetBirth", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetBirth(StringBuilder birth);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetAddr", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAddr(StringBuilder address);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetIDNum", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetIDNum(StringBuilder idnum);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetDep", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetDep(StringBuilder qfjg);//读取签发机关
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetBegin", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetBegin(StringBuilder yxqs);//读取有效期起
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetEnd", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetEnd(StringBuilder yxjz);//读取有效期止
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GetBmpPath", CallingConvention = CallingConvention.StdCall)]
        public static extern int GetBmpPath(StringBuilder photourl);//读取头像图片路径
        /*************************港澳台居民居住证类函数 ***************************/
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetName", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetName(StringBuilder name);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetSex", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetSex(StringBuilder sex);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetSexGB", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetSexGB(StringBuilder sex);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetBirth", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetBirth(StringBuilder birth);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetAddr", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetAddr(StringBuilder address);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetIDNum", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetIDNum(StringBuilder idnum);
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetDep", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetDep(StringBuilder qfjg);//读取签发机关
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetBegin", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetBegin(StringBuilder yxqs);//读取有效期起
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetEnd", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetEnd(StringBuilder yxjz);//读取有效期止
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetPassPortId", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetPassPortId(StringBuilder passportid);//读取通行证号
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_SignCount", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_SignCount(StringBuilder signCount);//读取签发次数
        [DllImport(@"Edz\ss\RdCard.dll", EntryPoint = "GID_GetCardType", CallingConvention = CallingConvention.StdCall)]
        public static extern int GID_GetCardType(StringBuilder cardtype);//读取卡类型

        private string error;

        public string getError()
        {
            return error;
        }
        private int istatus = 1;

        public void setIstatus(int istatus)
        {
            this.istatus = istatus;
        }

        private int iport = 1001;

        public void setIport(int iport)
        {
            this.iport = iport;
        }

        public string getXm()
        {
            StringBuilder Name = new StringBuilder(31);
            if (cardtype == "港澳台居民居住证")
            {
                GID_GetName(Name);
            }
            else
            {
                GetName(Name);
            }
            return Name.ToString().Trim();
        }

        public string getSex()
        {
            StringBuilder sex = new StringBuilder(31);
            if (cardtype == "港澳台居民居住证")
            {
                GID_GetSexGB(sex);
            }
            else
            {
                GetSexGB(sex);
            }
            return sex.ToString().Trim();
        }

        public string getMz()
        {
            StringBuilder mz = new StringBuilder(31);
            GetFolkGB(mz);
            return mz.ToString().Trim();
        }

        public string getBirth()
        {
            StringBuilder birth = new StringBuilder(31);
            if (cardtype == "港澳台居民居住证")
            {
                GID_GetBirth(birth);
            }
            else
            {
                GetBirth(birth);
            }
            string birthstr = birth.ToString().Trim();
            if (birthstr.Length == 8)
            {
                birthstr = birthstr.Substring(0, 4) + "-" + birthstr.Substring(4, 2) + "-" + birthstr.Substring(6);
            }
            return birthstr;
        }

        public string getAddress()
        {
            StringBuilder address = new StringBuilder(31);
            if (cardtype == "港澳台居民居住证")
            {
                GID_GetAddr(address);
            }
            else
            {
                GetAddr(address);
            }
            return address.ToString().Trim();
        }

        public string getSfz()
        {
            StringBuilder sfz = new StringBuilder(31);
            if (cardtype == "港澳台居民居住证")
            {
                GID_GetIDNum(sfz);
            }
            else
            {
                GetIDNum(sfz);
            }
            return sfz.ToString().Trim();
        }

        public string getQfjg()
        {
            StringBuilder qfjg = new StringBuilder(31);
            if (cardtype == "港澳台居民居住证")
            {
                GID_GetDep(qfjg);
            }
            else
            {
                GetDep(qfjg);
            }
            return qfjg.ToString().Trim();
        }

        public string getYxqq()
        {
            StringBuilder yxqq = new StringBuilder(31);
            if (cardtype == "港澳台居民居住证")
            {
                GID_GetBegin(yxqq);
            }
            else
            {
                GetBegin(yxqq);
            }
            string yxqqstr = yxqq.ToString().Trim();
            if (yxqqstr.Length == 8)
            {
                yxqqstr = yxqqstr.Substring(0, 4) + "-" + yxqqstr.Substring(4, 2) + "-" + yxqqstr.Substring(6);
            }
            return yxqqstr;
        }

        public string getYxqz()
        {
            StringBuilder yxqz = new StringBuilder(31);
            if (cardtype == "港澳台居民居住证")
            {
                GID_GetEnd(yxqz);
            }
            else
            {
                GetEnd(yxqz);
            }
            string yxqzstr = yxqz.ToString().Trim();
            if (yxqzstr.Length == 8)
            {
                yxqzstr = yxqzstr.Substring(0, 4) + "-" + yxqzstr.Substring(4, 2) + "-" + yxqzstr.Substring(6);
            }
            return yxqzstr;
        }
        private string samid = "";
        public string getSamid()
        {
            return samid;
        }
        public void setPhotoUrl(string photourl)
        {
        }
        public string getPhotoUrl()
        {
            StringBuilder photourl = new StringBuilder(31);
            GetBmpPath(photourl);
            return photourl.ToString().Trim();
        }

        public string getTxzh()
        {
            StringBuilder txzh = new StringBuilder(31);
            GID_GetPassPortId(txzh);
            return txzh.ToString().Trim();
        }

        public string getQfcs()
        {
            StringBuilder qfcs = new StringBuilder(31);
            GID_SignCount(qfcs);
            return qfcs.ToString().Trim();
        }
        private string cardtype = "";
        public string getCartType()
        {
            return cardtype;
        }

        private bool callCommand(string cmd, int parg0 = 1001)
        {
            error = "";
            int ir = 0;
            int parg1 = 8811;
            int parg2 = 9986;
            byte pcmd;
            bool rtn = false;
            switch (cmd)
            {
                case "open":
                    pcmd = 65;
                    ir = UCommand1(ref pcmd, ref parg0, ref parg1, ref parg2);
                    if (ir == -6)
                    {
                        error = "阅读机具未连接";
                        rtn = false;
                    }
                    else
                    {
                        rtn = true;
                    }
                    break;
                case "close":
                    pcmd = 66;
                    parg0 = 0;
                    ir = UCommand1(ref pcmd, ref parg0, ref parg1, ref parg2);
                    if (ir == -6)
                    {
                        error = "关闭端口失败";
                        rtn = false;
                    }
                    else
                    {
                        rtn = true;
                    }
                    break;
                case "authenticate":
                    pcmd = 67;
                    parg0 = 0;
                    ir = UCommand1(ref pcmd, ref parg0, ref parg1, ref parg2);
                    if (ir == -5)
                    {
                        error = "卡认证失败";
                        rtn = false;
                    }
                    else
                    {
                        rtn = true;
                    }
                    break;
                case "readdata":
                    pcmd = 73;
                    parg0 = 0;
                    ir = UCommand1(ref pcmd, ref parg0, ref parg1, ref parg2);
                    if (ir == 62171 || ir == 62172)
                    {
                        cardtype = "公民身份证";
                        rtn = true;
                    }
                    else if (ir == 62173)
                    {
                        cardtype = "外国居留证";
                        rtn = true;
                    }
                    else if (ir == 62174)
                    {
                        cardtype = "港澳台居民居住证";
                        rtn = true;
                    }
                    else
                    {
                        error = "读卡失败";
                        rtn = false;
                    }
                    break;
                default:
                    rtn = false;
                    error = "不支持";
                    break;

            }
            return rtn;
        }
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="iport"></param>
        /// <returns></returns>
        public bool Open(int iport)
        {
            return callCommand("open", iport);
        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            return callCommand("close");
        }
        /// <summary>
        /// 读取安全模块编号
        /// </summary>
        /// <returns></returns>
        public bool GetSAM()
        {
            error = "";
            StringBuilder samstr = new StringBuilder(64);
            int rtn = GetSAMIDToStr(samstr);
            if (rtn == 1)
            {
                samid = samstr.ToString();
                return true;
            }
            else
            {
                error = "读模块序列号出错";
                return false;
            }
        }
        /// <summary>
        /// 验证卡是否放在设备上
        /// </summary>
        /// <returns></returns>
        public bool HasCard()
        {
            return false;
        }
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <returns></returns>
        public bool ReadData()
        {
            return _readData();
        }
        /// <summary>
        /// 读取港澳台居民居住证
        /// </summary>
        /// <returns></returns>
        public bool ReadGOTData()
        {
            return _readData();
        }
        /// <summary>
        /// 读取身份证
        /// </summary>
        /// <returns></returns>
        public bool ReadICData()
        {
            return _readData();
        }
        private bool _readData()
        {
            bool authenticate = callCommand("authenticate");
            if (authenticate)
            {
                return callCommand("readdata");
            }
            else
            {
                return false;
            }
        }
    }
}
