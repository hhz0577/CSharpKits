using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.util.IC
{
    using System.Runtime.InteropServices;

    class DCRF32
    {
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_init", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_init(int prot, int baud);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_init", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_init(int prot, int baud);
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_beep", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_beep(int icdev, int stime);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_beep", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_beep(int icdev, int stime);
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_load_key_hex", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_load_key_hex(int icdev, int mode, int secor, char[] skey);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_load_key_hex", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_load_key_hex(int icdev, int mode, int secor, char[] skey);
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_card", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_card(int icdev, int mode, ref int snr);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_card", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_card(int icdev, int mode, ref int snr);
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_authentication", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_authentication(int icdev, int mode, int secor);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_authentication", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_authentication(int icdev, int mode, int secor);
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_read", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_read(int icdev, int adr, byte[] sdata);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_read", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_read(int icdev, int adr, byte[] sdata);
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_write", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_write(int icdev, int adr, string sdata);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_write", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_write(int icdev, int adr, string sdata);
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_write_hex", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_write_hex(int icdev, int adr, char[] sdata);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_write_hex", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_write_hex(int icdev, int adr, char[] sdata);
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_halt", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_halt(int icdev);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_halt", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_halt(int icdev);
        [DllImport(@"DLL/DCRF32.dll", SetLastError = true, EntryPoint = "dc_exit", CallingConvention = CallingConvention.StdCall)]
        public static extern int dc_exit(int icdev);
        [DllImport(@"DLL/mwrf32.dll", SetLastError = true, EntryPoint = "rf_exit", CallingConvention = CallingConvention.StdCall)]
        public static extern int rf_exit(int icdev);

        private int port = 100;//端口号

        public int Port
        {
            set { port = value; }
        }
        private int band = 115200;//波特率

        public int Band
        {
            set { band = value; }
        }
        private string error = "";

        public string Error
        {
            get { return error; }
        }
        private int cardmode = 0;

        private int icdev = 0;

        public bool OpenDev()
        {
            try
            {
                int idev;
                error = "";
                idev = dc_init(port, band);
                if (idev <= 0)
                    idev = rf_init(port, band); //串口1 波特率115200

                if (idev <= 0)
                {
                    error = "打开设备出错";
                    return false;
                }
                else
                {
                    icdev = idev;
                    
                }
            }
            catch 
            {
                //return false;
            }
            return true;
        }
        public bool CloseDev()
        {
            try
            {
                error = "";
                int irtn = dc_exit(icdev);
                if (irtn <= 0)
                    irtn = rf_exit(icdev);
                if (irtn <= 0)
                {
                    error = "关闭设备出错";
                    return false;
                }
            }
            catch { }
            return true;
        }
        public void BeenDev()
        {
            error = "";
            try
            {
                int irtn = dc_beep(icdev, 10);
                if (irtn <= 0)
                    irtn = rf_beep(icdev, 10);
            }
            catch { }
        }

        public bool WriteOneSectorData(int sector, List<string> lists)
        {
            error = "";
            if (lists.Count <= 0) return false;
            if (lists.Count > 4)
            {
                error = "一个扇区只能写四条数据，你给的参数大于四条了";
                return false;
            }
            bool rtn = true;
            int tempint = 0;
            try
            {
                //处理新卡
                char[] hexkey = new char[12] { 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F' };
                int st;
                st = dc_load_key_hex(icdev, 0, sector, hexkey);
                if (st != 0)
                    st = rf_load_key_hex(icdev, 0, sector, hexkey);
                if (st != 0)
                {
                    error = "加载密码失败";
                    rtn = false;
                }
                //寻卡
                st = dc_card(icdev, 1, ref tempint);
                if (st != 0)
                    st = rf_card(icdev, 1, ref tempint);
                st = dc_authentication(icdev, 0, sector);
                if (st != 0)
                    st = rf_authentication(icdev, 0, sector);
                if (st != 0)
                {
                    //处理旧卡
                    hexkey = new char[12] { '1', '1', '2', '2', '3', '3', '4', '4', '5', '5', '6', '6' };
                    st = dc_load_key_hex(icdev, 0, sector, hexkey);
                    if (st != 0)
                        st = rf_load_key_hex(icdev, 0, sector, hexkey);
                    if (st != 0)
                    {
                        error = "加载密码失败";
                        rtn = false;
                    }
                    st = dc_card(icdev, 1, ref tempint);
                    if (st != 0)
                        st = rf_card(icdev, 1, ref tempint);
                    st = dc_authentication(icdev, 0, sector);
                    if (st != 0)
                        st = rf_authentication(icdev, 0, sector);
                    if (st != 0)
                    {
                        error = "验证密码失败";
                        rtn = false;
                    }
                }
                else
                {
                    //现在把A密码改为112233445566,B密码写为FFFFFFFFFFFF
                    char[] data32 = new char[32] { '1', '1', '2', '2', '3', '3', '4', '4', '5', '5', '6', '6', 'F', 'F', '0', '7', '8', '0', '6', '9', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F' };
                    st = dc_write_hex(icdev, sector * 4 + 3, data32);
                    if (st != 0)
                        st = rf_write_hex(icdev, sector * 4 + 3, data32);
                    if (st != 0)
                    {
                        error = "写卡密码失败!";
                        rtn = false;
                    }
                }
                //开始写内容
                int i = 0;
                foreach (string scontent in lists)
                {
                    if (scontent.Length > 16)
                    {
                        error = "超过规定的数据长度!";
                        return false;
                    }
                    if (sector == 0)
                    {
                        st = dc_write(icdev, sector * 4 + i + 1, scontent);
                        if (st != 0)
                            st = rf_write(icdev, sector * 4 + i + 1, scontent);
                    }
                    else
                    {
                        st = dc_write(icdev, sector * 4 + i, scontent);
                        if (st != 0)
                            st = rf_write(icdev, sector * 4 + i, scontent);
                    }
                    if (st != 0) break;
                    i++;
                }
                if (st != 0)
                {
                    error = "写第" + sector + "扇区失败";
                    rtn = false;
                }
                if (dc_halt(icdev) != 0)
                    rf_halt(icdev);

            }
            catch (Exception ex)
            {
                error = ex.Message;
                rtn = false;
            }
            return rtn;
        }
        
        public string ReadOneSectorData(int sector, List<int> bufs)
        {
            error = "";
            int st = 0;
            if (bufs.Count > 4)
            {
                error = "一个扇区只有四条数据，你给的参数大于四条了";
                return "";
            }
            string rtn = "";
            try
            {
                int tempint = 0;
                char[] hexkey = new char[12] { '1', '1', '2', '2', '3', '3', '4', '4', '5', '5', '6', '6' };
                st = dc_load_key_hex(icdev, cardmode, sector, hexkey);
                if (st != 0)
                    st = rf_load_key_hex(icdev, cardmode, sector, hexkey);
                if (st != 0)
                {
                    error = "加载密码失败";
                    return "";
                }
                st = dc_card(icdev, 1, ref tempint);
                if (st != 0)
                    st = rf_card(icdev, 1, ref tempint);
                /*if (st != 0)
                {
                    error = "寻卡失败。";
                    return "";
                }*/
                st = dc_authentication(icdev, cardmode, sector);
                if (st != 0)
                    st = rf_authentication(icdev, cardmode, sector);
                if (st != 0)
                {
                    if (sector == 5) error = "寻卡失败";
                    else error = "验证密码失败";
                    return "";
                }
                byte[] d = new byte[16];
                foreach (int i in bufs)
                {
                    st = dc_read(icdev, i, d);
                    if (st != 0)
                        st = rf_read(icdev, i, d);
                    if (st != 0)
                    {
                        error = "读第" + sector + "扇区，第" + i + "块错误";
                        rtn = "";
                        break;
                    }
                    if (rtn.Length > 0) rtn += ";";
                    rtn += Encoding.Default.GetString(d).Trim();
                }
                if (dc_halt(icdev) != 0)
                    rf_halt(icdev);
            }
            catch { }
            return rtn;
        }
    }
}
