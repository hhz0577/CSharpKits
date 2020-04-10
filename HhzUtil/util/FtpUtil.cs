using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.util
{
    using System.Net;
    using System.IO;

    public class FtpUtil
    {
        private string hostname;
        private string uid;
        private string password;
        private string ftpdir;
        private string err = "";

        public FtpUtil(string hostname, string uid, string pwd, string ftpdir)
        {
            this.hostname = hostname;
            this.uid = uid;
            password = pwd;
            this.ftpdir = ftpdir;
        }
        public string Err
        {
            get { return err; }
        }
        private FtpWebRequest initFtp(string filename)
        {
            FtpWebRequest reqFTP = null;

            if (ftpdir == string.Empty)
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(hostname + "//" + filename);
            else
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(hostname + "//" + ftpdir + "//" + filename);
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(uid, password);
            reqFTP.UseBinary = true;
            reqFTP.Proxy = null;
            return reqFTP;
        }
        public bool checkConn()
        {
            string filename = "checkconn.txt";
            try
            {
                FtpWebRequest reqFTP = initFtp(filename);//FTP上必须有一个文件叫checkconn.txt
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long fileSize = response.ContentLength;

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (Exception ex)
            {
                err = "... [FTP]连接失败，原因：" + ex.Message + ex.StackTrace;
                return false;
            }
        }
        public List<string> uploadFile(string path)
        {
            //得到文件列表
            List<string> rtnlist = new List<string>();
            if (!Directory.Exists(path)) throw new Exception("目录不存在");
            string[] filelists = Directory.GetFiles(path);
            if (filelists.Length <= 0) return rtnlist;
            Uri uri = new Uri(hostname);
            FtpWebRequest reqFTP = null;
            FtpWebResponse uploadResponse = null;
            FileStream fs = null;
            Stream strm = null;
            foreach (string filename in filelists)
            {
                FileInfo finfo = new FileInfo(filename);
                try
                {
                    reqFTP = initFtp(finfo.Name);
                    reqFTP.KeepAlive = false;
                    // 指定执行什么命令 
                    reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
                    reqFTP.ContentLength = finfo.Length;
                    // 缓冲大小设置为2kb 
                    int buffLength = 2048;
                    byte[] buff = new byte[buffLength];
                    int contentLen;
                    // 打开一个文件流 (System.IO.FileStream) 去读上传的文件 
                    fs = finfo.OpenRead();
                    // 把上传的文件写入流 
                    strm = reqFTP.GetRequestStream();
                    // 每次读文件流的2kb 
                    contentLen = fs.Read(buff, 0, buffLength);
                    // 流内容没有结束 
                    while (contentLen != 0)
                    {
                        // 把内容从file stream 写入 upload stream 
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }
                    strm.Close();
                    fs.Close();
                    uploadResponse = (FtpWebResponse)reqFTP.GetResponse();
                    if (uploadResponse.StatusDescription.Contains("226"))
                    {
                        if (File.Exists(path + "\\" + finfo.Name)) File.Delete(path + "\\" + finfo.Name);
                        rtnlist.Add(finfo.Name);
                    }
                }
                catch (Exception ex)
                {
                    err = "... [FTP]上传失败，原因：" + ex.Message + ex.StackTrace;
                    return rtnlist;
                }
                finally
                {
                    // 关闭流 
                    if (uploadResponse != null) uploadResponse.Close();
                    if (fs != null) fs.Close();
                    if (strm != null) strm.Close();
                }

            }
            return rtnlist;
        }

        public bool downFile(string path, string filename)
        {
            if (!Directory.Exists(path)) return true;
            FtpWebResponse response = null;
            Stream ftpstream = null;
            FileStream outputStream = null;
            try
            {
                FtpWebRequest reqFTP = initFtp(filename);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                response = (FtpWebResponse)reqFTP.GetResponse();
                ftpstream = response.GetResponseStream();
                long cl = response.ContentLength;
                outputStream = new FileStream(path + "\\" + filename, FileMode.Create);
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpstream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpstream.Read(buffer, 0, bufferSize);
                }
                return true;
            }
            catch (Exception ex)
            {
                err = "... [FTP]下载失败，原因：" + ex.Message + ex.StackTrace;
                return false;
            }
            finally
            {
                if (ftpstream != null) ftpstream.Close();
                if (outputStream != null) outputStream.Close();
                if (response != null) response.Close();
            }
        }
        public string list()
        {
            StreamReader reader = null;
            try
            {
                FtpWebRequest listRequest = initFtp("");
                listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse();
                reader = new StreamReader(listResponse.GetResponseStream());
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                err = "... [FTP]显示文件列表失败，原因：" + ex.Message + ex.StackTrace;
                return "";
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}
