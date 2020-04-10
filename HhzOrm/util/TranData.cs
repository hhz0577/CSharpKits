using System;
using System.Collections.Generic;
using System.Text;

namespace HhzOrm.util
{
    using System.IO;
    using System.Data;
    using HhzUtil.util;

    public class TranData
    {
        private string datapath = "";
        private string zippath = "";
        private string tranSql = "";
        private DaoUtil dao;
        private StringBuilder logstr = new StringBuilder();
        private FtpUtil ftputil = null;
        public string getLogStr()
        {
            return logstr.ToString();
        }

        public TranData(string datapath, string zippath, string tranSql, string hostname, string uid, string pwd, string ftpdir)
        {
            this.datapath = datapath;
            FileUtil.createPath(datapath);
            this.zippath = zippath;
            FileUtil.createPath(zippath);
            this.tranSql = tranSql;
            dao = new DaoUtil();
            ftputil = new FtpUtil(hostname, uid, pwd, ftpdir);
        }
        private bool getData()
        {
            ConditionUtil c = new ConditionUtil();
            DataSet ds = new DataSet();

            string[] sqls = tranSql.Split(';');
            if (sqls.Length <= 0) return true;
            foreach (string sql in sqls)
            {
                ds.Clear();
                string[] ss = sql.Split(',');
                try
                {
                    ds = dao.getDataSet(ss[1], c);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ss[1].Contains("tx"))
                        {
                            //处理Sqlite图像数据
                            if (dao.getDbType() == "sqllite")
                            {
                                logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 获得上传数据" + ss[0] + "，数据总数为：" + ds.Tables[0].Rows.Count.ToString());
                                wirteXmlToFile(toXmlForSqlite(ds, "datas"), ss[0] + ".xml");
                            }
                            else
                            {
                                logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 获得上传数据" + ss[0] + "，数据总数为：" + ds.Tables[0].Rows.Count.ToString());
                                wirteXmlToFile(DsUtil.toXml(ds, "datas"), ss[0] + ".xml");
                            }
                        }
                        else
                        {
                            logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 获得上传数据" + ss[0] + "，数据总数为：" + ds.Tables[0].Rows.Count.ToString());
                            wirteXmlToFile(DsUtil.toXml(ds, "datas"), ss[0] + ".xml");
                        }
                    }
                    updateOneData(ss[2], ss[0]);
                }
                catch (Exception e)
                {
                    logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 获得上传数据" + ss[0] + "失败，失败原因：" + e.Message + e.StackTrace);
                    return false;
                }
            }
            return true;
        }
        private void updateOneData(string sql, string tablename)
        {
            ConditionUtil c = new ConditionUtil();
            c.F_transmit_date = DateTime.Now;
            try
            {
                dao.execDml(sql.Trim(), c);
                logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 更新数据" + tablename + "标志成功");
            }
            catch (Exception ex)
            {
                logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 更新数据" + tablename + "标志失败，失败原因：" + ex.Message);
            }
        }
        private string toXmlForSqlite(DataSet ds, string rootstr)
        {
            string rowstr = "";
            string colname = "";
            if (rootstr == string.Empty) rootstr = "rows";
            if (ds.Tables[0].Rows.Count <= 0) return "";
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                rowstr += "<row>";
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    if (DBNull.Value == row[col.ColumnName]) continue;
                    colname = col.ColumnName.ToLower();
                    if (row[col.ColumnName].ToString().Contains("##HHZ_"))
                    {
                        string[] s = row[col.ColumnName].ToString().Split(';');
                        byte[] b = getTx(row[s[0].Replace("##HHZ_", "")].ToString(), s[1].ToLower());
                        rowstr += "<" + colname + ">" + Convert.ToBase64String(b) + "</" + colname + ">";
                    }
                    else if (col.DataType.Name.ToLower() == "datetime")
                    {
                        rowstr += "<" + colname + ">" + ((DateTime)row[col.ColumnName]).ToString("yyyyMMdd HH:mm:ss") + "</" + colname + ">";
                    }
                    else
                    {
                        rowstr += "<" + colname + ">" + row[col.ColumnName].ToString() + "</" + colname + ">";
                    }
                }
                rowstr += "</row>";
            }
            return "<?xml version=\"1.0\" encoding=\"utf-8\" ?><" + rootstr + ">" + rowstr + "</" + rootstr + ">";
        }
        private byte[] getTx(string f_id, string sqlid)
        {
            byte[] buf = null;
            ConditionUtil c = new ConditionUtil();
            c.Sitem1 = f_id;
            DataSet ds = dao.getDataSet(sqlid, c);
            if (ds.Tables[0].Rows.Count > 0)
            {
                IDataReader dr = dao.getDataRead(sqlid, c);
                if (dr != null)
                {
                    dr.Read();
                    int ndatalen = (int)dr.GetBytes(0, 0, null, 0, 0);
                    buf = new byte[ndatalen];
                    dr.GetBytes(0, 0, buf, 0, buf.Length);
                }
            }
            return buf;
        }
        private bool zipFile(string zipfileprefix)
        {
            if (Directory.Exists(datapath))
            {

                string[] filelists = Directory.GetFiles(datapath);
                if (filelists.Length <= 0) return true;
                ZipUtil zipobj = new ZipUtil();
                if (zipobj.ZipFile(zipfileprefix, zippath, datapath))
                    logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 压缩文件成功。");
                else
                    logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 压缩文件失败。");
            }
            FileUtil.deleteFile(datapath, "", ".xml", 0);
            return true;
        }
        public bool run(int filenum, string zipfileprefix)
        {
            bool issucess = false;
            int uppackfilenum = 0;
            int maxnum = 0;
            if (filenum <= 0) maxnum = 100;
            logstr.Clear();
            logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 上传任务开始");
            try
            {
                //判断是否要上传新数据
                if (Directory.Exists(zippath))
                {
                    string[] filelists = Directory.GetFiles(zippath);
                    uppackfilenum = filelists.Length;
                }
                if (uppackfilenum >= maxnum)
                {
                    logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 未上传文件超过最大的设置数，上传任务结束。");
                    return true;
                }
                getData();
                zipFile(zipfileprefix);
                List<string> uplist = ftputil.uploadFile(zippath);
                if (!string.IsNullOrEmpty(ftputil.Err))
                {
                    logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] " + ftputil.Err);
                }
                else
                {
                    foreach (string upfilename in uplist)
                    {
                        logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 上传文件成功，" + upfilename);
                    }
                    FileUtil.deleteFile(zippath, "", ".zip", 0);
                }
                logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 上传任务结束");
            }
            catch (Exception e)
            {
                issucess = false;
                logstr.Append("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... 上传任务失败，原因：" + e.Message);
            }
            return issucess;
        }

        private void wirteXmlToFile(string str, string filename)
        {
            FileStream fs = new FileStream(datapath + "\\" + filename, FileMode.Create, FileAccess.Write);
            StreamWriter _streamWriter = new StreamWriter(fs);
            _streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            _streamWriter.WriteLine(str);
            _streamWriter.Flush();
            _streamWriter.Close();
            fs.Close();
            fs.Dispose();
            fs = null;
        }
    }
}
