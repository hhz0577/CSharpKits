using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.util
{
    using System.IO;

    public class FileUtil
    {
        public static void createPath(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
        public static void createFile(string fullfilename)
        {
            
            FileStream f = null;
            try
            {
                if (!File.Exists(fullfilename))
                {
                    f = File.Create(fullfilename);
                    f.Flush();
                }
            }
            finally
            {
                if(f != null)
                    f.Close();
            }
        }
        public static byte[] readFileToByte(string filename)
        {
            FileStream f = null;
            try
            {
                f = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read);
                byte[] b = new byte[f.Length];
                f.Read(b, 0, b.Length);
                return b;
            }
            finally
            {
                f.Close();
            }
        }
        public static string readFileToBase64(string filename)
        {
            FileStream f = null;
            try
            {
                f = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read);
                byte[] b = new byte[f.Length];
                f.Read(b, 0, b.Length);
                return Convert.ToBase64String(b);
            }
            finally
            {
                f.Close();
            }
        }
        #region 移动
        /// <summary>
        /// 根据文件前缀和后缀移动文件
        /// </summary>
        /// <param name="frompath">文件所在目录</param>
        /// <param name="topath">目标目录</param>
        /// <param name="prefix">文件前缀</param>
        /// <param name="postfix">文件后缀</param>
        /// <param name="number">移动数量</param>
        /// <returns>移动文件列表</returns>
        public static List<string> moveFile(string frompath, string topath, string prefix, string postfix, int number)
        {
            int filenumber = 0;
            if (number <= 0) number = int.MaxValue;
            string[] filelists = Directory.GetFiles(frompath);
            List<string> rtn = new List<string>();
            foreach (string filename in filelists)
            {
                FileInfo finfo = new FileInfo(filename);
                if (isProcessFile(finfo, prefix, postfix))
                {
                    try
                    {
                        if (File.Exists(topath + "\\" + finfo.Name)) File.Delete(topath + "\\" + finfo.Name);
                        File.Move(frompath + "\\" + finfo.Name, topath + "\\" + finfo.Name);
                        rtn.Add(finfo.Name);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                    filenumber++;
                    if (filenumber >= number) break;
                }
            }
            return rtn;
        }
        #endregion

        #region 复制
        /// <summary>
        /// 根据文件前缀和后缀复制文件
        /// </summary>
        /// <param name="frompath">文件所在目录</param>
        /// <param name="topath">目标目录</param>
        /// <param name="prefix">文件前缀</param>
        /// <param name="postfix">文件后缀</param>
        /// <param name="number">复制数量</param>
        /// <returns>复制文件列表</returns>
        public static List<string> copyFile(string frompath, string topath, string prefix, string postfix, int number)
        {
            int filenumber = 0;
            if (number <= 0) number = int.MaxValue;
            string[] filelists = Directory.GetFiles(frompath);
            List<string> rtn = new List<string>();
            foreach (string filename in filelists)
            {
                FileInfo finfo = new FileInfo(filename);
                if (isProcessFile(finfo, prefix, postfix))
                {
                    try
                    {
                        if (File.Exists(topath + "\\" + finfo.Name)) File.Delete(topath + "\\" + finfo.Name);
                        File.Copy(frompath + "\\" + finfo.Name, topath + "\\" + finfo.Name);
                        rtn.Add(finfo.Name);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                    filenumber++;
                    if (filenumber >= number) break;
                }
            }
            return rtn;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 根据文件前缀和后缀删除文件
        /// </summary>
        /// <param name="path">文件所在目录</param>
        /// <param name="prefix">文件前缀</param>
        /// <param name="postfix">文件后缀</param>
        /// <param name="number">删除数量</param>
        /// <returns>删除文件列表</returns>
        public static List<string> deleteFile(string path, string prefix, string postfix, int number)
        {
            int filenumber = 0;
            if (number <= 0) number = int.MaxValue;
            string[] filelists = Directory.GetFiles(path);
            List<string> rtn = new List<string>();
            foreach (string filename in filelists)
            {
                FileInfo finfo = new FileInfo(filename);
                if (isProcessFile(finfo, prefix, postfix))
                {
                    try
                    {
                        if (File.Exists(path + "\\" + finfo.Name)) File.Delete(path + "\\" + finfo.Name);
                        rtn.Add(finfo.Name);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                    filenumber++;
                    if (filenumber >= number) break;
                }
            }
            return rtn;
        }
        #endregion

        #region 判断文件是否是配置中要求处理的文件
        private static bool isProcessFile(string filename, string prefix, string postfix)
        {
            bool rtn = false;
            if (string.IsNullOrEmpty(prefix))
            {
                if (filename.EndsWith(postfix))
                {
                    rtn = true;
                }
            }
            else
            {
                string[] ss = prefix.Split(',');
                foreach (string s in ss)
                {
                    if (filename.StartsWith(s))
                    {
                        if (filename.EndsWith(postfix))
                        {
                            rtn = true;
                            break;
                        }
                    }
                }
            }
            return rtn;
        }
        private static bool isProcessFile(FileInfo finfo, string prefix, string postfix)
        {
            bool rtn = false;
            if (finfo.Length <= 0) return false;
            if (string.IsNullOrEmpty(prefix))
            {
                if (finfo.Extension.ToLower().Equals(postfix))
                {
                    rtn = true;
                }
            }
            else
            {
                string[] ss = prefix.Split(',');
                foreach (string s in ss)
                {
                    if (finfo.Name.StartsWith(s))
                    {
                        if (finfo.Extension.ToLower().Equals(postfix))
                        {
                            rtn = true;
                            break;
                        }
                    }
                }
            }
            return rtn;
        }
        #endregion
    }
}
