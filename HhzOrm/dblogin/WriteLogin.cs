using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzOrm.dblogin
{
    using System.IO;
    class WriteLogin
    {
        private static string TEMPLATE = "功能名称：{#_funcname}，记录时间：{#_writetime}，SQL：{#_sql}";
        public static void writeDBLog(string funcname, string sql)
        {
            LoginDto dto = new LoginDto();
            dto._funcname = funcname;
            dto._sql = sql;
            dto._writetime = DateTime.Now;
            string logstr = TEMPLATE;
            if (logstr.Contains("{#_funcname}"))
                logstr = logstr.Replace("{#_funcname}", dto._funcname);
            if (logstr.Contains("{#_writetime}"))
                logstr = logstr.Replace("{#_writetime}", dto._writetime.ToString("yyyy-MM-dd HH:mm:ss"));
            if (logstr.Contains("{#_sql}"))
                logstr = logstr.Replace("{#_sql}", dto._sql);
            recordMsg(logstr);
        }
        /// <summary>
        /// 写日志函数
        /// </summary>
        /// <param name="str">要写入的日志内容字符串</param>
        private static void recordMsg(string str)
        {
            if (!Directory.Exists(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "dblog"))
                Directory.CreateDirectory(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "dblog");
            FileStream fs = new FileStream(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "dblog" + "/" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter _streamWriter = new StreamWriter(fs, Encoding.Default);
            _streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            _streamWriter.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... " + str + "\r\n");
            _streamWriter.Flush();
            _streamWriter.Close();
            fs.Close();
        }
    }
}
