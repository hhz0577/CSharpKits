using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.edz
{
    using System.Runtime.InteropServices;
    using System.Threading;
    /// <summary>
    /// 读二代证 FOR 新中新
    /// </summary>
    public class ReadcardXZX : IReadCard
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct IDCardData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]   //ByValTStr           ByValArray     CertVol
            public string Name;      //姓名   
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string Sex;       //性别
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string Nation;    //民族
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public string Born;      //出生日期
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 72)]
            public string Address;   //住址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 38)]
            public string IDCardNo;  //身份证号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string GrantDept; //发证机关
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public string UserLifeBegin; // 有效开始日期
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public string UserLifeEnd;   // 有效截止日期
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string PassID;        //通行证号码
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string IssuesTimes;   //签发次数
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
            public string reserved;      // 保留
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)]
            public string PhotoFileName;// 照片路径   
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string CardType;// 证件类型     
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 122)]
            public string EngName;// 英文名      
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string CertVol;// 证件版本号   
        }
        /************************端口类API *************************/
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetMaxRFByte", CharSet = CharSet.Unicode,CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetMaxRFByte(int iPort, byte ucByte, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_GetCOMBaud", CharSet = CharSet.Unicode,CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_GetCOMBaud(int iPort, ref uint puiBaudRate);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetCOMBaud", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetCOMBaud(int iPort, uint uiCurrBaud, uint uiSetBaud);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_OpenPort", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_OpenPort(int iPort);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_ClosePort", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_ClosePort(int iPort);
        /**************************SAM类函数 **************************/
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_ResetSAM", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_ResetSAM(int iPort, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_GetSAMStatus", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_GetSAMStatus(int iPort, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_GetSAMID", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_GetSAMID(int iPort, ref byte pucSAMID, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_GetSAMIDToStr", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_GetSAMIDToStr(int iPort, ref byte pcSAMID, int iIfOpen);
        /*************************身份证卡类函数 ***************************/
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_StartFindIDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_StartFindIDCard(int iPort, ref byte[] pucIIN, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SelectIDCard", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SelectIDCard(int iPort, ref byte[] pucIIN, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_ReadBaseMsg", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_ReadBaseMsg(int iPort, ref byte pucCHMsg, ref uint puiCHMsgLen, ref byte pucPHMsg, ref uint puiPHMsgLen, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_ReadIINSNDN", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_ReadIINSNDN(int iPort, ref byte pucIINSNDN, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_ReadBaseMsgToFile", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_ReadBaseMsgToFile(int iPort, ref byte pcCHMsgFileName, ref uint puiCHMsgFileLen, ref byte pcPHMsgFileName, ref uint puiPHMsgFileLen, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_ReadIINSNDNToASCII", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_ReadIINSNDNToASCII(int iPort, ref byte pucIINSNDN, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_ReadNewAppMsg", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_ReadNewAppMsg(int iPort, ref byte pucAppMsg, ref uint puiAppMsgLen, int iIfOpen);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_GetBmp", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_GetBmp(int iPort, ref byte Wlt_File);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_ReadMsg", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_ReadMsg(int iPortID, int iIfOpen, ref IDCardData pIDCardData);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_FindReader", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_FindReader();
        /***********************设置附加功能函数 ************************/
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetPhotoPath", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetPhotoPath(int iOption, ref byte[] cPhotoPath);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetPhotoType", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetPhotoType(int iType);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetPhotoName", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetPhotoName(int iType);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetSexType", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetSexType(int iType);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetNationType", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetNationType(int iType);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetBornType", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetBornType(int iType);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetUserLifeBType", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetUserLifeBType(int iType);
        [DllImport(@"Edz\xzx\SynIDCardAPI.dll", EntryPoint = "Syn_SetUserLifeEType", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern int Syn_SetUserLifeEType(int iType, int iOption);

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
        private string xm;

        public string getXm()
        {
            return xm;
        }
        private string sex;

        public string getSex()
        {
            return sex;
        }
        private string mz;

        public string getMz()
        {
            return mz;
        }
        private string birth;

        public string getBirth()
        {
            return birth;
        }
        private string address;

        public string getAddress()
        {
            return address;
        }
        private string sfz;

        public string getSfz()
        {
            return sfz;
        }
        private string qfjg;

        public string getQfjg()
        {
            return qfjg;
        }
        private string yxqq;

        public string getYxqq()
        {
            return yxqq;
        }
        private string yxqz;

        public string getYxqz()
        {
            return yxqz;
        }
        private string samid = "";
        public string getSamid()
        {
            return samid;
        }
        private string photourl;
        public void setPhotoUrl(string photourl)
        {
            this.photourl = photourl;
        }
        public string getPhotoUrl()
        {
            return photourl;
            //return "EDZ";
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

        private string carttype = "";
        public string getCartType()
        {
            return carttype;
        }
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="iport"></param>
        /// <returns></returns>
        public bool Open(int iport)
        {
            error = "";
            this.iport = iport;
            int nRet;
            nRet = Syn_SetPhotoType(0);//照片存放格式为Bmp
            byte[] cPath = new byte[255];
            nRet = Syn_SetPhotoPath(1, ref cPath);//照片存放路径设置为当前路径
            nRet = Syn_SetPhotoName(0);//照片保存文件名格式设置为 tmp
            Syn_SetNationType(1);//
            Syn_SetSexType(1);

            Syn_SetBornType(0);//设置返回出生日期的格式 0=YYYYMMDD,1=YYYY年MM月DD日,2=YYYY.MM.DD,3=YYYY-MM-DD,4=YYYY/MM/DD
            Syn_SetUserLifeBType(0);
            Syn_SetUserLifeEType(0, 0);
            nRet = Syn_OpenPort(1001);
            if (nRet != 0)
            {
                error = "阅读机具未连接";
                return false;
            }
            else
                return true;
        }
        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            error = "";
            int ir = Syn_ClosePort(iport);
            if (ir != 0)
            {
                error = "关闭端口失败";
                return false;
            }
            else
            {
                error = "";
                return true;
            }
        }
        /// <summary>
        /// 读取安全模块编号
        /// </summary>
        /// <returns></returns>
        public bool GetSAM()
        {
            error = "";
            byte[] uSAMID = new byte[128];
            int nRet;
            nRet = Syn_GetSAMIDToStr(iport, ref uSAMID[0], 0);
            if (nRet == 0)
            {
                samid = System.Text.Encoding.Default.GetString(uSAMID);
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
            int nRet;
            byte[] pucIIN = new byte[4];
            nRet = Syn_StartFindIDCard(iport, ref pucIIN, 0);
            if (nRet == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
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
            IDCardData CardMsg = new IDCardData();
            error = "";
            int nRet;
            byte[] pucIIN = new byte[4];
            byte[] pucSN = new byte[8];
            //if (Syn_SetMaxRFByte(iport, 80, 0) == 0)
            //{
                nRet = Syn_StartFindIDCard(iport, ref pucIIN, 0);
                //Thread.Sleep(10);
                nRet = Syn_SelectIDCard(iport, ref pucSN, 0);
                //Thread.Sleep(10);
                nRet = Syn_ReadMsg(iport, 0, ref CardMsg);
                if (nRet == 0 || nRet == 1)
                {
                    if (CardMsg.CardType.StartsWith("I"))
                    {
                        carttype = "外国居留证";
                        return true;
                    }
                    else
                    {
                        xm = CardMsg.Name.Trim();
                        sfz = CardMsg.IDCardNo.Trim();
                        sex = CardMsg.Sex.Trim();//代码
                        mz = CardMsg.Nation.Trim();//代码
                        birth = CardMsg.Born.Trim();
                        birth = birth.Substring(0, 4) + "-" + birth.Substring(4, 2) + "-" + birth.Substring(6);
                        address = CardMsg.Address.Trim();
                        qfjg = CardMsg.GrantDept.Trim();
                        yxqq = CardMsg.UserLifeBegin.Trim();
                        yxqq = yxqq.Substring(0, 4) + "-" + yxqq.Substring(4, 2) + "-" + yxqq.Substring(6);
                        yxqz = CardMsg.UserLifeEnd.Trim();
                        if (yxqz.Length == 8)
                        {
                            yxqz = yxqz.Substring(0, 4) + "-" + yxqz.Substring(4, 2) + "-" + yxqz.Substring(6);
                        }
                        photourl = CardMsg.PhotoFileName;
                        if (CardMsg.CardType.StartsWith("J"))
                        {
                            carttype = "港澳台居民居住证";
                            txzh = CardMsg.PassID.Trim();
                            qfcs = CardMsg.IssuesTimes.Trim();
                        }
                        else
                        {
                            carttype = "公民身份证";
                        }
                        return true;
                    }
                }
                else
                {
                    error = "读卡失败";
                    return false;
                }
            //}
            //else
            //{
            //    error = "读卡失败";
            //    return false;
            //}
        }
    }
}
