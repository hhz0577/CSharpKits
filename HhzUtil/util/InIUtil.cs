using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzUtil.util
{
    using System.Runtime.InteropServices;
    using System.IO;
    public class IniUtil
    {
        private string fileName = "";
        public IniUtil(string filename)
        {
            this.fileName = filename;
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public T ReadConfig<T>(string section, string key)
        {
            if (File.Exists(fileName))
            {
                IniFile f = new IniFile(fileName);
                string value = f.ReadContentValue(section, key);
                if (String.IsNullOrWhiteSpace(value)) return default(T);
                if (typeof(T).IsEnum) return (T)Enum.Parse(typeof(T), value, true);
                return (T)Convert.ChangeType(value, typeof(T));
            }
            else
            {
                return default(T);
            }
        }
        /// <summary>
        /// 写配置文件,文件不存在，则创建
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void WriteConfig(string section, string key, string value)
        {
            if (!File.Exists(fileName))
            {
                FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter _streamWriter = new StreamWriter(fs, Encoding.Unicode);
                _streamWriter.Flush();
                _streamWriter.Close();
                fs.Close();
            }
            IniFile f = new IniFile(fileName);
            f.WriteContentValue(section, key, value);
        }
    }
    class IniFile
    {
        public string Path;

        public IniFile(string path)
        {
            this.Path = path;
        }

        /// <summary>
        /// 写入INI文件
        /// </summary>
        /// <param name="section">节点名称[如[TypeName]]</param>
        /// <param name="key">键</param>
        /// <param name="val">值</param>
        /// <param name="filepath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);

        /// <summary>
        /// 读取INI文件
        /// </summary>
        /// <param name="section">节点名称</param>
        /// <param name="key">键</param>
        /// <param name="def">值</param>
        /// <param name="retval">stringbulider对象</param>
        /// <param name="size">字节大小</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retval, int size, string filePath);

        /// <summary>
        /// 写入
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="iValue"></param>
        public void WriteContentValue(string section, string key, string iValue)
        {
            WritePrivateProfileString(section, key, iValue, this.Path);
        }

        /// <summary>
        /// 读取INI文件中的内容方法
        /// </summary>
        /// <param name="Section">键</param>
        /// <param name="key">值</param>
        /// <returns></returns>
        public string ReadContentValue(string Section, string key)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(Section, key, "", temp, 1024, this.Path);
            return temp.ToString();
        }
    }
}
