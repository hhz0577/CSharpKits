using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.edz
{
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    /// <summary>
    /// 读二代证 FOR 精伦
    /// </summary>
    public class ReadCardJL : IReadCard
    {
        [DllImport(@"Edz\jl\sdtapi.dll")]
        public static extern int InitComm(int port);
        [DllImport(@"Edz\jl\sdtapi.dll")]
        public static extern int CloseComm();
        [DllImport(@"Edz\jl\sdtapi.dll")]
        public static extern int Authenticate();
        [DllImport(@"Edz\jl\sdtapi.dll")]
        private static extern int ReadBaseInfos(StringBuilder Name, StringBuilder Gender, StringBuilder Folk,
                                                    StringBuilder BirthDay, StringBuilder Code, StringBuilder Address,
                                                    StringBuilder Agency, StringBuilder ExpireStart, StringBuilder ExpireEnd);
        [DllImport(@"Edz\jl\sdtapi.dll")]
        private static extern int ReadBaseInfosPhoto(StringBuilder Name, StringBuilder Gender, StringBuilder Folk,
                                                    StringBuilder BirthDay, StringBuilder Code, StringBuilder Address,
                                                    StringBuilder Agency, StringBuilder ExpireStart, StringBuilder ExpireEnd, string directory);
        [DllImport(@"Edz\jl\sdtapi.dll")]
        private static extern int Routon_ReadAllGATBaseInfos(StringBuilder Name, StringBuilder Gender, StringBuilder FutureItem1,
                                                    StringBuilder BirthDay, StringBuilder Address, StringBuilder Code,
                                                    StringBuilder Agency, StringBuilder ExpireStart, StringBuilder ExpireEnd,
                                                    StringBuilder PassID, StringBuilder SignCnt, StringBuilder FutureItem2,
                                                    StringBuilder CardType, StringBuilder FutureItem3);
        [DllImport(@"Edz\jl\sdtapi.dll")]
        private static extern int GetSAMIDToStr(StringBuilder Samid);
        [DllImport(@"Edz\jl\sdtapi.dll")]
        private static extern int Routon_DecideIDCardType();

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

        private string carttype = "";
        public string getCartType()
        {
            return carttype;
        }

        public bool Open(int iport)
        {
            error = "";
            this.iport = iport;
            int ir = InitComm(iport);
            if (ir != 1)
            {
                error = "阅读机具未连接";
                return false;
            }
            else
                return true;
        }

        public bool GetSAM()
        {
            error = "";
            StringBuilder samstr = new StringBuilder(37);
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
        public bool HasCard()
        {
            int irtn = Authenticate();
            if (irtn == 1) return true;
            else return false;
        }

        public bool ReadData()
        {
            error = "";
            int rtn = 0;
            int irtn = Authenticate();
            if (irtn != 1) return false;
            else
            {
                rtn = Routon_DecideIDCardType();
                if (rtn == 100)
                {
                    carttype = "公民身份证";
                    return _readICData();
                }
                else if (rtn == 101)
                {
                    carttype = "外国居留证";
                    return true;
                }
                else if (rtn == 102)
                {
                    carttype = "港澳台居民居住证";
                    return _readGOTData();
                }
                else
                {
                    carttype = "未知";
                    return true;
                }
            }
        }

        public bool ReadGOTData()
        {
            error = "";
            int irtn = Authenticate();
            if ((irtn == 0) || (irtn == 1))
            {
                carttype = "港澳台居民居住证";
                return _readGOTData();
            }
            else
            {
                error = "卡认证失败";
                return false;
            }
        }

        public bool ReadICData()
        {
            error = "";
            int irtn = Authenticate();
            if ((irtn == 0) || (irtn == 1))
            {
                carttype = "公民身份证";
                return _readICData();
            }
            else
            {
                error = "卡认证失败";
                return false;
            }
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
                error = "";
                return true;
            }
        }

        private bool _readGOTData()
        {
            StringBuilder Name = new StringBuilder(31);
            StringBuilder Gender = new StringBuilder(3);
            StringBuilder FutureItem1 = new StringBuilder(100);
            StringBuilder BirthDay = new StringBuilder(9);
            StringBuilder Code = new StringBuilder(19);
            StringBuilder Address = new StringBuilder(71);
            StringBuilder Agency = new StringBuilder(31);
            StringBuilder ExpireStart = new StringBuilder(9);
            StringBuilder ExpireEnd = new StringBuilder(9);
            StringBuilder PassID = new StringBuilder(100);//通行证号
            StringBuilder SignCnt = new StringBuilder(100);//签发次数
            StringBuilder FutureItem2 = new StringBuilder(100);
            StringBuilder CardType = new StringBuilder(100);
            StringBuilder FutureItem3 = new StringBuilder(100);
            if ((istatus < 1) || (istatus >= 6) || (istatus == 4))
            {
                error = "超出读取信息类型";
                return false;
            }
            int rea = Routon_ReadAllGATBaseInfos(Name, Gender, FutureItem1, BirthDay, Address, Code, Agency, ExpireStart, ExpireEnd, PassID, SignCnt, FutureItem2, CardType, FutureItem3);
            if (rea != 1)
            {
                error = "读卡失败";
                return false;
            }
            else
            {
                sfz = Code.ToString().Trim();
                xm = Name.ToString().Trim();
                sex = Gender.ToString().Trim();
                birth = BirthDay.ToString().Trim();
                birth = birth.Substring(0, 4) + "-" + birth.Substring(4, 2) + "-" + birth.Substring(6);
                address = Address.ToString().Trim();
                qfjg = Agency.ToString().Trim();
                yxqq = ExpireStart.ToString().Trim();
                yxqq = yxqq.Substring(0, 4) + "-" + yxqq.Substring(4, 2) + "-" + yxqq.Substring(6);
                yxqz = ExpireEnd.ToString().Trim();
                if (yxqz.Length == 8)
                {
                    yxqz = yxqz.Substring(0, 4) + "-" + yxqz.Substring(4, 2) + "-" + yxqz.Substring(6);
                }
                txzh = PassID.ToString().Trim();
                qfcs = SignCnt.ToString().Trim();
                return true;
            }
        }
        private bool _readICData()
        {
            
            StringBuilder Name = new StringBuilder(31);
            StringBuilder Gender = new StringBuilder(3);
            StringBuilder Folk = new StringBuilder(10);
            StringBuilder BirthDay = new StringBuilder(9);
            StringBuilder Code = new StringBuilder(19);
            StringBuilder Address = new StringBuilder(71);
            StringBuilder Agency = new StringBuilder(31);
            StringBuilder ExpireStart = new StringBuilder(9);
            StringBuilder ExpireEnd = new StringBuilder(9);
            //StringBuilder directory = new StringBuilder(255);
            //directory = directory.Append(photourl);
            if ((istatus < 1) || (istatus >= 6) || (istatus == 4))
            {
                error = "超出读取信息类型";
                return false;
            }
            int rea = ReadBaseInfosPhoto(Name, Gender, Folk, BirthDay, Code, Address, Agency, ExpireStart, ExpireEnd, photourl);
            //int rea = ReadBaseInfos(Name, Gender, Folk, BirthDay, Code, Address, Agency, ExpireStart, ExpireEnd);
            if (rea != 1)
            {
                error = "读卡失败";
                return false;
            }
            else
            {
                sfz = Code.ToString().Trim();
                xm = Name.ToString().Trim();
                sex = Gender.ToString().Trim();
                mz = Folk.ToString().Trim();
                birth = BirthDay.ToString().Trim();
                birth = birth.Substring(0, 4) + "-" + birth.Substring(4, 2) + "-" + birth.Substring(6);
                address = Address.ToString().Trim();
                qfjg = Agency.ToString().Trim();
                yxqq = ExpireStart.ToString().Trim();
                yxqq = yxqq.Substring(0, 4) + "-" + yxqq.Substring(4, 2) + "-" + yxqq.Substring(6);
                yxqz = ExpireEnd.ToString().Trim();
                if (yxqz.Length == 8)
                {
                    yxqz = yxqz.Substring(0, 4) + "-" + yxqz.Substring(4, 2) + "-" + yxqz.Substring(6);
                }
                return true;
            }
        }

        //private void readData()
        //{
        //    string fileName = "";
        //    string currpath = AppDomain.CurrentDomain.BaseDirectory;
        //    if ((istatus == 1) || (istatus == 2)) fileName = currpath + @"\WZ.TXT";
        //    if (istatus == 3) fileName = currpath + @"\NEWADD.TXT";
        //    if (istatus == 5) fileName = currpath + @"\IINSNDN.bin";

        //    FileStream f = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        //    StreamReader sr = new StreamReader(f, Encoding.Unicode);
        //    char[] buff = new char[15];
        //    //姓名
        //    sr.Read(buff, 0, buff.Length);
        //    xm = new string(buff);
        //    xm = xm.Trim();
        //    //性别
        //    buff = new char[1];
        //    sr.Read(buff, 0, buff.Length);
        //    sex = new string(buff);
        //    //民族
        //    buff = new char[2];
        //    sr.Read(buff, 0, buff.Length);
        //    mz = new string(buff);
        //    //出生
        //    buff = new char[8];
        //    sr.Read(buff, 0, buff.Length);
        //    birth = new string(buff);
        //    birth = birth.Substring(0, 4) + "-" + birth.Substring(4, 2) + "-" + birth.Substring(6);
        //    //地址
        //    buff = new char[35];
        //    sr.Read(buff, 0, buff.Length);
        //    address = new string(buff);
        //    address = address.Trim();
        //    //身份证
        //    buff = new char[18];
        //    sr.Read(buff, 0, buff.Length);
        //    sfz = new string(buff);
        //    //签发机关
        //    buff = new char[15];
        //    sr.Read(buff, 0, buff.Length);
        //    qfjg = new string(buff);
        //    qfjg = qfjg.Trim();
        //    //有效期起始
        //    buff = new char[8];
        //    sr.Read(buff, 0, buff.Length);
        //    yxqq = new string(buff);
        //    yxqq = yxqq.Substring(0, 4) + "-" + yxqq.Substring(4, 2) + "-" + yxqq.Substring(6);
        //    //签发机关
        //    buff = new char[8];
        //    sr.Read(buff, 0, buff.Length);
        //    yxqz = new string(buff);
        //    yxqz = yxqz.Substring(0, 4) + "-" + yxqz.Substring(4, 2) + "-" + yxqz.Substring(6);
        //    sr.Close();
        //    f.Close();
        //}
    }
}
