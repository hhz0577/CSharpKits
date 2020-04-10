using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.util
{
    using System.Data;
    using System.Xml;

    public class DsUtil
    {
        #region XML 格式
        /// <summary>
        /// 将Net的数据集转化为XML格式
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="rootstr">根节点名</param>
        /// <returns>XML格式</returns>
        public static string toXml(DataSet ds, string rootstr)
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
                    if (col.DataType.Name.ToLower() == "byte[]")
                    {
                        byte[] b = new byte[0];
                        b = (Byte[])row[col.ColumnName];
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
        #endregion

        #region JSON 格式
        /// <summary>
        /// 将Net的数据集转化为JSON格式
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="TableName">表名</param>
        /// <returns>DELPHI的ClientDataSet格式的XML字符串</returns>
        public static string toJson(DataSet ds, string headstr)
        {
            string rowstr = "";
            string colstr = "";
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (rowstr.Length > 0) rowstr += ",";
                colstr = "";
                foreach (DataColumn col in ds.Tables[0].Columns)
                {
                    if (DBNull.Value == row[col.ColumnName]) continue;
                    if (colstr.Length > 0) colstr += ",";
                    if (col.DataType.Name.ToLower() == "datetime")
                    {
                        colstr += "\"" + col.ColumnName.ToLower() + "\":\"" + Convert.ToDateTime(row[col.ColumnName]).ToString("yyyy-MM-dd HH:mm:ss") + "\"";
                    }
                    else if (col.DataType.Name.ToLower() == "byte[]")
                    {
                        byte[] b = new byte[0];
                        b = (Byte[])row[col.ColumnName];
                        colstr += "\"" + col.ColumnName.ToLower() + "\":\"" + Convert.ToBase64String(b) + "\"";
                    }
                    else
                        colstr += "\"" + col.ColumnName.ToLower() + "\":\"" + row[col.ColumnName].ToString() + "\"";

                }
                rowstr += "{" + colstr + "}";
            }
            if (headstr == string.Empty)
            {
                return "{\"success\":true,\"recordcount\":#recordcount#,\"message\":\"成功\",\"datas\":[" + rowstr + "]}";
            }
            else
            {
                return "{\"success\":true,\"recordcount\":#recordcount#,\"message\":\"成功\",\"" + headstr + "\":[" + rowstr + "]}";
            }
        }
        #endregion

        /// <summary>
        /// 根据XML字符创建数据集
        /// </summary>
        /// <param name="xmlstr">XML字符</param>
        /// <returns></returns>
        public static DataSet getDataSetByXml(string xmlstr)
        {
            XmlDocument oXmlDoc = new XmlDocument();
            oXmlDoc.LoadXml(xmlstr);
            XmlNodeReader xnread = new XmlNodeReader(oXmlDoc);
            DataSet ds = new DataSet();
            ds.ReadXml(xnread);
            xnread.Close();
            return ds;
        }
        /// <summary>
        /// 加载字典文件到数据视图(DataView)
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="filename">字典文件名</param>
        /// <returns></returns>
        public static DataView getViewByFile(string path,string filename)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(path + "\\" + filename);
            DataViewManager dvm = new DataViewManager(ds);
            return dvm.CreateDataView(ds.Tables["row"]);
        }
        public static DataView getViewByFile(string filename)
        {
            return getViewByFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Dic\", filename);
        }
        /// <summary>
        /// 加载字典文件到Tables
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="filename">字典文件名</param>
        /// <returns></returns>
        public static DataTable getTableByFile(string path, string filename)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(path + "\\" + filename);
            return ds.Tables[0];
        }
        public static DataTable getTableByFile(string filename)
        {
            return getTableByFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Dic\", filename);
        }
        /// <summary>
        /// 根据字典的代码获得名称
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="filename">字典文件名</param>
        /// <param name="dm">代码</param>
        /// <returns></returns>
        public static string getDicMcByDm(string path, string filename, string dm)
        {
            string rtn = "";
            DataTable td = getTableByFile(path, filename);
            foreach (DataRow row in td.Rows)
            {
                if (row["DM"].ToString() == dm)
                    return row["MC"].ToString();
            }
            return rtn;
        }
    }
}
