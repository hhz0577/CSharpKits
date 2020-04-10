using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.util.IC
{
    /// <summary>
    /// 场所从业人员IC卡管理
    /// 从业人员卡
    ///     0扇区：  0块（物理块号0） 卡系列号（不用）
    ///              1块（物理块号1） 卡编号（前9位）
    ///              2块（物理块号2） 真实姓名
    ///              3块（物理块号3） 控制密码块（不用）
    ///     1扇区：  0块（物理块号4） 证件号码（前6位）
    ///              1块（物理块号5） 证件号码（后12位）
    ///              2块（物理块号6） 性别
    ///              3块（物理块号7） 控制密码块（不用）
    ///     2扇区：  0块（物理块号8） 户籍代码
    ///              1块（物理块号9） 出生
    ///              2块（物理块号10） 民族
    ///              3块（物理块号11） 控制密码块（不用）
    ///     3扇区：  0块（物理块号12） 现住详址（前16位）
    ///              1块（物理块号13） 现住详址（中16位）
    ///              2块（物理块号14） 现住详址（后16位）
    ///              3块（物理块号15） 控制密码块（不用）
    ///     4扇区：  0块（物理块号16） 管理机构名称
    ///              1块（物理块号17） 最后刷卡日期
    ///              2块（物理块号18） 最后激活(年检)日期 (要加上一年)
    ///              3块（物理块号19） 控制密码块（不用）
    ///     5扇区：  0块（物理块号20） 卡类别 从业人员01；公安管理卡GA；维护卡WH
    ///              1块（物理块号21） 发卡日期
    ///              2块（物理块号22） 卡编号（后12位）
    ///              3块（物理块号23） 控制密码块（不用）
    /// </summary>
    public class YlcsICCard
    {
        private DCRF32 dcr = null;
        public YlcsICCard(int port, int band)
        {
            dcr = new DCRF32();
            if (port > 0) dcr.Port = port;
            if (band > 0) dcr.Band = band;
        }
        private string icCode = "";//卡编号

        public string IcCode
        {
            get { return icCode; }
            set { icCode = value; }
        }

        private string name = "";//姓名

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string sex = "";//性别

        public string Sex
        {
            get { return sex; }
            set { sex = value; }
        }
        private string cardValue = "";//证件号码

        public string CardValue
        {
            get { return cardValue; }
            set { cardValue = value; }
        }
        private string hj = "";//户籍代码

        public string Hj
        {
            get { return hj; }
            set { hj = value; }
        }
        private string mz = "";//民族

        public string Mz
        {
            get { return mz; }
            set { mz = value; }
        }
        private string birth = "";//出生日期

        public string Birth
        {
            get { return birth; }
            set { birth = value; }
        }
        private string address = "";//住址

        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        private string manageUtilName = "昆明市公安局";//管理机构名称

        public string ManageUtilName
        {
            get { return manageUtilName; }
            set { manageUtilName = value; }
        }
        private string endSkDate = "";//最后刷卡日期

        public string EndSkDate
        {
            get { return endSkDate; }
            set { endSkDate = value; }
        }
        private string njDate = "";//最后激活(年检)日期

        public string NjDate
        {
            get { return njDate; }
            set { njDate = value; }
        }
        private string rylb = "01";//人员类别

        public string Rylb
        {
            get { return rylb; }
            set { rylb = value; }
        }
        private string fkDate = "";//发卡日期

        public string FkDate
        {
            get { return fkDate; }
            set { fkDate = value; }
        }
        private string gajgdm = "";//公安卡的机构代码

        public string Gajgdm
        {
            get { return gajgdm; }
            set { gajgdm = value; }
        }

        //获得错误信息
        public string GetErrorDev()
        {
            return dcr.Error;
        }
        //打开设备
        public bool OpenDev()
        {
            return dcr.OpenDev();
        }
        //关闭设备
        public bool CloseDev()
        {
            return dcr.CloseDev();
        }
        //响铃
        public void BeepDev()
        {
            dcr.BeenDev();
        }
        private string icCodetmp = "";
        //清除数据
        private void ClearData()
        {
            icCodetmp = "";
            icCode = "";
            name = "";
            sex = "";
            cardValue = "";
            mz = "";
            birth = "";
            address = "";
            hj = "";
            endSkDate = "";
            njDate = "";
            fkDate = "";
            gajgdm = "";
        }
        
        //获得从业人员IC卡信息
        public bool GetICData()
        {
            try
            {
                ClearData();
                bool rtn = true;
                string data = "";
                List<int> bufs = new List<int>();
                string[] ss;
                int secnum = 0; //扇区数
                int beginBlockNum = 0;
                //读5扇区信息
                secnum = 5;
                beginBlockNum = secnum * 4;
                bufs.Clear();
                bufs.Add(beginBlockNum);
                bufs.Add(beginBlockNum + 1);
                bufs.Add(beginBlockNum + 2);
                data = dcr.ReadOneSectorData(secnum, bufs);
                if (dcr.Error == "")
                {
                    ss = data.Split(';');
                    rylb = ss[0].Trim();
                    fkDate = ss[1].Trim();
                    if (ss[2].Trim() == string.Empty) return false;
                    if (rylb.Contains("GA"))
                        gajgdm = ss[2].Trim().Substring(0, 6);
                    else
                        icCodetmp = ss[2].Trim().Substring(0, 12);
                }
                else if (dcr.Error == "寻卡失败")
                    return true;
                else return false;
                //读0扇区信息
                secnum = 0;
                beginBlockNum = secnum * 4;
                bufs.Clear();
                //bufs.Add(beginBlockNum);
                bufs.Add(beginBlockNum + 1);
                bufs.Add(beginBlockNum + 2);
                data = dcr.ReadOneSectorData(secnum, bufs);
                if (dcr.Error == "")
                {
                    ss = data.Split(';');
                    if (rylb.Contains("GA"))
                        icCode = ss[0].Trim().Substring(0, 11);
                    else
                        icCode = ss[0].Trim().Substring(0, 9) + icCodetmp;
                    name = ss[1].Trim();
                }
                else
                    return false;
                if (rylb.Contains("01"))
                {
                    //读1扇区信息
                    secnum = 1;
                    beginBlockNum = secnum * 4;
                    bufs.Clear();
                    bufs.Add(beginBlockNum);
                    bufs.Add(beginBlockNum + 1);
                    bufs.Add(beginBlockNum + 2);
                    data = dcr.ReadOneSectorData(secnum, bufs);
                    if (dcr.Error == "")
                    {
                        ss = data.Split(';');
                        cardValue = ss[0].Trim().Substring(0, 6) + ss[1].Trim().Substring(0, 12);
                        sex = ss[2].Trim();
                    }
                    else return false;
                    //读2扇区信息
                    secnum = 2;
                    beginBlockNum = secnum * 4;
                    bufs.Clear();
                    bufs.Add(beginBlockNum);
                    bufs.Add(beginBlockNum + 1);
                    bufs.Add(beginBlockNum + 2);
                    data = dcr.ReadOneSectorData(secnum, bufs);
                    if (dcr.Error == "")
                    {
                        ss = data.Split(';');
                        hj = ss[0].Trim();
                        birth = ss[1].Trim();
                        mz = ss[2].Trim();
                    }
                    else return false;
                    //读3扇区信息
                    secnum = 3;
                    beginBlockNum = secnum * 4;
                    bufs.Clear();
                    bufs.Add(beginBlockNum);
                    bufs.Add(beginBlockNum + 1);
                    bufs.Add(beginBlockNum + 2);
                    data = dcr.ReadOneSectorData(secnum, bufs);
                    if (dcr.Error == "")
                    {
                        ss = data.Split(';');
                        address = ss[0].Trim() + ss[1].Trim() + ss[2].Trim();
                    }
                    else return false;
                    //读4扇区信息
                    secnum = 4;
                    beginBlockNum = secnum * 4;
                    bufs.Clear();
                    bufs.Add(beginBlockNum);
                    bufs.Add(beginBlockNum + 1);
                    bufs.Add(beginBlockNum + 2);
                    data = dcr.ReadOneSectorData(secnum, bufs);
                    if (dcr.Error == "")
                    {
                        ss = data.Split(';');
                        manageUtilName = ss[0].Trim();
                        endSkDate = ss[1].Trim();
                        njDate = ss[2].Trim().Substring(0, 10);
                    }
                    else return false;
                }
                return rtn;
            }
            catch
            {
                return false;
            }
        }

        //写从业人员IC卡信息
        public bool WritePersonData()
        {
            bool rtn = true;
            List<string> bufs = new List<string>();
            int secnum = 0; //扇区数
            //写0扇区信息
            bufs.Add(icCode.Substring(0, 9));
            bufs.Add(name);
            rtn = dcr.WriteOneSectorData(secnum, bufs);
            if (!rtn) return false;
            //写1扇区信息
            secnum = 1;
            bufs.Clear();
            bufs.Add(cardValue.Substring(0, 6));
            bufs.Add(cardValue.Substring(6, 12));
            bufs.Add(sex);
            rtn = dcr.WriteOneSectorData(secnum, bufs);
            if (!rtn) return false;
            //写2扇区信息
            secnum = 2;
            bufs.Clear();
            bufs.Add(hj);
            bufs.Add(birth);
            bufs.Add(mz);
            rtn = dcr.WriteOneSectorData(secnum, bufs);
            if (!rtn) return false;
            //写3扇区信息
            secnum = 3;
            bufs.Clear();
            byte[] bb = Encoding.Default.GetBytes(address);
            if (bb.Length > 16)
            {
                bufs.Add(Encoding.Default.GetString(bb, 0, 16));
                if (bb.Length > 32)
                {
                    bufs.Add(Encoding.Default.GetString(bb, 16, 16));
                    bufs.Add(Encoding.Default.GetString(bb, 32, 16));
                }
                else
                    bufs.Add(Encoding.Default.GetString(bb, 16, 16));
            }
            else
                bufs.Add(address);
            rtn = dcr.WriteOneSectorData(secnum, bufs);
            if (!rtn) return false;
            //写4扇区信息
            secnum = 4;
            bufs.Clear();
            bufs.Add(manageUtilName);
            bufs.Add(endSkDate);
            bufs.Add(njDate);
            rtn = dcr.WriteOneSectorData(secnum, bufs);
            if (!rtn) return false;
            //写5扇区信息
            secnum = 5;
            bufs.Clear();
            bufs.Add(rylb);
            bufs.Add(fkDate);
            bufs.Add(icCode.Substring(9, 12));
            rtn = dcr.WriteOneSectorData(secnum, bufs);
            if (!rtn) return false;
            return rtn;
        }
        //写公安IC卡信息
        public bool WriteGaData()
        {
            bool rtn = true;
            List<string> bufs = new List<string>();
            int secnum = 0; //扇区数
            //写0扇区信息
            bufs.Add(icCode.Substring(0, 11));
            bufs.Add(name);
            rtn = dcr.WriteOneSectorData(secnum, bufs);
            if (!rtn) return false;
            //写5扇区信息
            secnum = 5;
            bufs.Clear();
            bufs.Add(rylb);
            bufs.Add(fkDate);
            bufs.Add(gajgdm);
            rtn = dcr.WriteOneSectorData(secnum, bufs);
            if (!rtn) return false;
            return rtn;
        }
    }
}
