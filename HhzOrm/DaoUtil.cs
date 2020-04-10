using System;
using System.Collections.Generic;
using System.Text;

namespace HhzOrm
{
    using System.Data;
    using HhzUtil.util;

    public class DaoUtil
    {
        private IDAOClass dao;
        public DaoUtil()
        {
            dao = new DAOClass();
        }
        public string exec(string id, Object obj)
        {
            if (dao.getOperationType(id).ToLower() == "prepare")
                return execPrepare(id, obj);
            else
                return dao.execDml(id, obj);
        }
        public string execDml(string id, Object obj)
        {
            return dao.execDml(id, obj);
        }
        public string execDml(string id, string sqlstr)
        {
            return dao.execDml(id, sqlstr);
        }
        public string execTranctionDml(List<string> ids, List<Object> objs)
        {
            return dao.execTranctionDml(ids, objs);
        }
        public string execPrepare(string id, Object obj)
        {
            return dao.execPrepare(id, obj);
        }
        public string execPrepare(List<string> ids, List<Object> objs)
        {
            return dao.execPrepare(ids, objs);
        }
        public string getSql()
        {
            return dao.getSql();
        }
        public int getDataCount(string id, Object obj)
        {
            int count = 0;
            DataSet ds = dao.getDataSet(id, obj);
            if (ds.Tables[0].Rows.Count > 0)
                count = int.Parse(ds.Tables[0].Rows[0][0].ToString());
            return count;
        }
        public string getJson(string id, Object obj, string headstr)
        {
            DataSet ds = dao.getDataSet(id, obj);
            return DsUtil.toJson(ds, headstr);
        }
        private string getJson(string id, Object obj, string headstr, int count)
        {
            DataSet ds = dao.getDataSet(id, obj);
            string rtn = DsUtil.toJson(ds, headstr);
            if (count <= 0)
                rtn = rtn.Replace("#recordcount#", ds.Tables[0].Rows.Count.ToString());
            else
                rtn = rtn.Replace("#recordcount#", count.ToString());
            return rtn;
        }
        public string getJson(string countid, string id, Object obj, string headstr)
        {
            int count = 0;
            if (string.IsNullOrEmpty(countid)) count = 0;
            else count = getDataCount(countid, obj);
            return getJson(id, obj, headstr, count);
        }
        public int getDataCount(string id, string sql)
        {
            int count = 0;
            DataSet ds = dao.getDataSet(id, sql);
            if (ds.Tables[0].Rows.Count > 0)
                count = int.Parse(ds.Tables[0].Rows[0][0].ToString());
            return count;
        }
        public string getJson(string id, string sql, string headstr)
        {
            DataSet ds = dao.getDataSet(id, sql);
            return DsUtil.toJson(ds, headstr);
        }
        private string getJson(string id, string sql, string headstr, int count)
        {
            DataSet ds = dao.getDataSet(id, sql);
            string rtn = DsUtil.toJson(ds, headstr);
            if (count <= 0)
                rtn = rtn.Replace("#recordcount#", ds.Tables[0].Rows.Count.ToString());
            else
                rtn = rtn.Replace("#recordcount#", count.ToString());
            return rtn;
        }
        public string getJson(string id, string countsqlid, string sql, string headstr)
        {
            int count = 0;
            if (string.IsNullOrEmpty(countsqlid)) count = 0;
            else count = getDataCount(id, countsqlid);
            return getJson(id, sql, headstr, count);
        }
        public List<T> getData<T>(string id, Object obj)  where T : class
        {
            return dao.getData<T>(id, obj);
        }
        public DataSet getDataSet(string id, string sql)
        {
            return dao.getDataSet(id, sql);
        }
        public DataSet getDataSet(string id, Object objs)
        {
            return dao.getDataSet(id, objs);
        }
        public DataTable getDataTable(string id, string sql)
        {
            return dao.getDataTable(id, sql);
        }
        public DataTable getDataTable(string id, Object objs)
        {
            return dao.getDataTable(id, objs);
        }
        public IDataReader getDataRead(string id, Object objs)
        {
            return dao.getDataRead(id, objs);
        }
        public void disposeConn(string id)
        {
            dao.disposeConn(id);
        }
        public void disposeAllConn()
        {
            dao.disposeAllConn();
        }
        public string getDbType()
        {
            return dao.getDbType().ToLower();
        }
        public string getDbType(string id)
        {
            return dao.getDbType(id).ToLower();
        }
        public void BackupFileData(string id,string destFilename)
        {
            dao.BackupFileData(id,destFilename);
        }
        //string connStr = "Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=\"Excel 8.0;HDR=Yes;\";data source=" + xlsPath;
        //string sql = "SELECT 员工编号,姓名,大病套班数,病假套班数,事假套班数,探亲假班数,旷工套班数 FROM [Sheet1$A2:G1000]";
        ////OleDbConnection excelConn = new OleDbConnection(connStr);
        ////excelConn.Open();
        ////OleDbCommand excelCommand = new OleDbCommand(sql, excelConn);
        //OleDbDataAdapter da = new OleDbDataAdapter(sql, connStr);
        ////OleDbDataReader excelReader = excelCommand.ExecuteReader(CommandBehavior.CloseConnection);
        //da.Fill(dataSet1);    // 填充DataSet
        //dataSet1 = ConvertToDataSet(excelReader);
        //protected DataSet ConvertToDataSet(OleDbDataReader dataReader)
        //{
                
        //    DataSet dataSet = new DataSet();
        //    do
        //    {
        //        // Create new data table

        //        DataTable schemaTable = dataReader.GetSchemaTable();
        //        DataTable dataTable = new DataTable();

        //        if (schemaTable != null)
        //        {
        //            // A query returning records was executed

        //            for (int i = 0; i < schemaTable.Rows.Count; i++)
        //            {
        //                DataRow dataRow = schemaTable.Rows[i];
        //                // Create a column name that is unique in the data table
        //                string columnName = (string)dataRow["ColumnName"]; //+ "<C" + i + "/>";
        //                // Add the column definition to the data table
        //                DataColumn column = new DataColumn(columnName, (Type)dataRow["DataType"]);
        //                dataTable.Columns.Add(column);
        //            }

        //            //Add the data table to the dataset.
        //            dataSet.Tables.Add(dataTable);

        //            // Fill the data table.
        //            while (dataReader.Read())
        //            {
        //                DataRow dataRow = dataTable.NewRow();

        //                for (int i = 0; i < dataReader.FieldCount; i++)
        //                    dataRow[i] = dataReader.GetValue(i);

        //                dataTable.Rows.Add(dataRow);
        //            }
        //        }
        //        else
        //        {
        //            // No records were returned.

        //            DataColumn column = new DataColumn("RowsAffected");
        //            dataTable.Columns.Add(column);
        //            dataSet.Tables.Add(dataTable);
        //            DataRow dataRow = dataTable.NewRow();
        //            dataRow[0] = dataReader.RecordsAffected;
        //            dataTable.Rows.Add(dataRow);
        //        }
        //    }
        //    while (dataReader.NextResult());
        //    return dataSet;
        //}
    }
}
