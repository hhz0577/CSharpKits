using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.util
{
    using System.IO;
    using ICSharpCode.SharpZipLib.Zip;

    public class ZipUtil
    {
        private string err = "";

        public string Err
        {
            get { return err; }
        }

        public bool UpZipFile(string fileName, string workpath)
        {
            string upfilename = workpath + "\\" + fileName;

            try
            {
                using (ZipInputStream s = new ZipInputStream(File.OpenRead(upfilename)))
                {
                    ZipEntry theEntry;
                    while ((theEntry = s.GetNextEntry()) != null)
                    {
                        string filenameex = theEntry.Name;
                        using (FileStream streamWriter = File.Create(workpath + "\\" + filenameex))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                err = "... [UPZIP]解压失败，原因：" + ex.Message + ex.StackTrace;
                return false;
            }
        }
        public bool ZipFile(string fileprefix, string zippath, string sourcepath)
        {
            string zipfilename = fileprefix + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
            try
            {
                string[] filelists = Directory.GetFiles(sourcepath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(zippath + "\\" + zipfilename)))
                {
                    s.SetLevel(9);
                    byte[] buffer = new byte[4096];
                    foreach (string filename in filelists)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(filename));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(filename))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                    return true;
                }
            }
            catch (Exception ex)
            {
                err = "... [ZIP]压缩失败，原因：" + ex.Message + ex.StackTrace;
                return false;
            }
        }
    }
}
