using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.edz
{
    /// <summary>
    /// 读二代证接口
    /// </summary>
    public interface IReadCard
    {
        bool Open(int iport);//连接设备
        bool GetSAM();//读取安全模块编号
        bool ReadData();//读取数据
        bool ReadICData();//读取身份证
        bool ReadGOTData();//读取港澳台居民居住证
        bool HasCard();//验证卡是否放在设备上
        bool Close();//关闭设备
        string getError();
        void setIstatus(int istatus);
        void setIport(int iport);
        string getXm();
        string getSex();
        string getMz();
        string getBirth();
        string getAddress();
        string getSfz();
        string getQfjg();//签发机关
        string getYxqq();//有效期始
        string getYxqz();//有效期至
        string getSamid();//安全模块编号
        //string photourl { get; set; }
        string getPhotoUrl();
        void setPhotoUrl(string photourl);
        string getTxzh();//通行证号
        string getQfcs();//签发次数
        string getCartType();//卡类型
    }
}
