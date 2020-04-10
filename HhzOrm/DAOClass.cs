using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Data;

namespace HhzOrm
{
    using util;
    using dblogin;
    using DataSource;

    //using ModuleLibrary.Util;
    /// <summary>
    /// 数据访问实现类
    /// </summary>
    public class DAOClass : IDAOClass
    {
        private DaoStruct daoStruct;
        private ParseDaoConfig parseDao;
        private string resultSql = "";

        public DAOClass()
        {
            parseDao = ParseDaoConfig.createInstance();
            daoStruct = new DaoStruct();
        }
        public string getDbType()
        {
            return daoStruct.DbType;
        }
        public string getDbType(string id)
        {
            return parseDao.ObtainConfig(id).DbType;
        }
        public string getOperationType(string id)
        {
            return parseDao.ObtainConfig(id).OperationType;
        }
        private List<DTOClass> setDto(Object obj)
        {
            if (obj.GetType().FullName != daoStruct.ClassName) throw new Exception("类和配置的类名不一致");
            List<DTOClass> dtolist = new List<DTOClass>();
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (PropertyInfo pinfo in props)
            {
                DTOClass dto = new DTOClass();
                dto.name = pinfo.Name.ToLower();
                dto.fieldtype = pinfo.PropertyType.Name.ToLower();
                dto.value = pinfo.GetValue(obj, new object[] { });
                if (dto.value != null)
                {
                    switch (dto.fieldtype)
                    {
                        case "datetime":
                            long sdate = ((DateTime)dto.value).ToBinary();
                            if (sdate == 0) { dto.value = null; }
                            else dto.value = (DateTime)dto.value;
                            break;
                        case "boolean":
                            dto.value = (bool)dto.value;
                            break;
                        case "int32":
                            dto.value = (int)dto.value;
                            break;
                        case "int64":
                            dto.value = (long)dto.value;
                            break;
                        case "single":
                            dto.value = (float)dto.value;
                            break;
                        case "double":
                            dto.value = (double)dto.value;
                            break;
                        case "decimal":
                            dto.value = (decimal)dto.value;
                            break;
                        case "byte[]":
                            dto.value = (byte[])dto.value;
                            break;
                        default:
                            dto.value = (string)dto.value;
                            break;
                    }
                }
                dtolist.Add(dto);
            }
            return dtolist;
        }
        private string setSql(Object obj,string sql)
        {
            List<DTOClass> dtolist = setDto(obj);
            foreach (DTOClass dto in dtolist)
            {
                if (dto.value != null)
                {
                    if (dto.fieldtype == "string")
                    {
                        if (((string)dto.value).ToString() == "") sql = sql.Replace("#" + dto.name + "#", "null");
                        else sql = sql.Replace("#" + dto.name + "#", "'" + ((string)dto.value).ToString() + "'");
                    }
                    else if (dto.fieldtype == "datetime")
                    {
                        switch (daoStruct.DbType)
                        {
                            case "oracle":
                                sql = sql.Replace("#" + dto.name + "#", "TO_DATE('" + ((DateTime)dto.value).ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-MM-dd hh24:mi:ss')");
                                break;
                            case "sql":
                            case "mysql":
                            case "sqllite":
                            case "accsee":
                                sql = sql.Replace("#" + dto.name + "#", "'" + ((DateTime)dto.value).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                break;
                        }
                    }
                    else if (dto.fieldtype == "byte[]")
                        throw new Exception("此操作方式不支持byte[]数据类型，请使用Prepare");
                    else if (dto.fieldtype == "boolean")
                    {
                        if ((bool)dto.value) sql = sql.Replace("#" + dto.name + "#", "'1'");
                        else sql = sql.Replace("#" + dto.name + "#", "'0'");
                    }
                    else
                        sql = sql.Replace("#" + dto.name + "#", dto.value.ToString());
                    sql = sql.Replace("$" + dto.name + "$", dto.value.ToString());
                }
                else
                {
                    sql = sql.Replace("#" + dto.name + "#", "null");
                    sql = sql.Replace("$" + dto.name + "$", "null");
                }
            }
            sql = parseDao.setWhere(sql);
            sql = sql.Replace("&lt;", "<");
            sql = sql.Replace("&gt;", ">");
            return sql;
        }
       
        /// <summary>
        /// 单DML语句操作
        /// </summary>
        /// <param name="id">SQL语句ID编号</param>
        /// <param name="obj">输入输出对象</param>
        /// <returns></returns>
        public string execDml(string id, Object obj)
        {
            resultSql = "";
            if (id == "") throw new Exception("参数不全");
            if (obj == null) throw new Exception("参数不全");
            daoStruct = parseDao.ObtainConfig(id);
            if (daoStruct.DbType.Length <= 0 || daoStruct.ConStr.Length <= 0 || daoStruct.SqlStr.Length <= 0) throw new Exception("配置参数不全");
            string sql = "";
            sql = setSql(obj, daoStruct.SqlStr);
            resultSql = sql;
            //记录SQL
            if (daoStruct.IsLog)
                WriteLogin.writeDBLog(daoStruct.Desc, resultSql);
            if (sql.Length <= 0) throw new Exception("构造SQL语句出错");
            IDataSource dataSource = null;
            switch (daoStruct.DbType)
            {
                case "oracle":
                    dataSource = new OracleDataSource(daoStruct);
                    break;
                case "mysql":
                    dataSource = new MySqlDataSource(daoStruct);
                    break;
                case "sql":
                    dataSource = new SqlDataSource(daoStruct);
                    break;
                case "access":
                    dataSource = new AccessDataSource(daoStruct);
                    break;
                case "sqllite":
                    dataSource = new SqlLiteDataSource(daoStruct);
                    break;
            }
            if(dataSource != null) dataSource.SingleExecute(sql);
            
            return "";
        }
        /// <summary>
        /// 单DML语句操作
        /// </summary>
        /// <param name="id">SQL语句ID编号</param>
        /// <param name="obj">输入输出对象</param>
        /// <returns></returns>
        public string execDml(string id, string sqlstr)
        {
            resultSql = "";
            if (id == "") throw new Exception("参数不全");
            daoStruct = parseDao.ObtainConfig(id);
            if (daoStruct.DbType.Length <= 0 || daoStruct.ConStr.Length <= 0 || daoStruct.SqlStr.Length <= 0) throw new Exception("配置参数不全");
            string sql = sqlstr;
            resultSql = sql;
            //记录SQL
            if (daoStruct.IsLog)
                WriteLogin.writeDBLog(daoStruct.Desc, resultSql);
            if (sql.Length <= 0) throw new Exception("构造SQL语句出错");
            IDataSource dataSource = null;
            switch (daoStruct.DbType)
            {
                case "oracle":
                    dataSource = new OracleDataSource(daoStruct);
                    break;
                case "mysql":
                    dataSource = new MySqlDataSource(daoStruct);
                    break;
                case "sql":
                    dataSource = new SqlDataSource(daoStruct);
                    break;
                case "access":
                    dataSource = new AccessDataSource(daoStruct);
                    break;
                case "sqllite":
                    dataSource = new SqlLiteDataSource(daoStruct);
                    break;
            }
            if (dataSource != null) dataSource.SingleExecute(sql);
            
            return "";
        }
        /// <summary>
        /// 多DML语句操作
        /// </summary>
        /// <param name="ids">SQL语句ID编号列表</param>
        /// <param name="objs">输入输出对象列表</param>
        /// <returns></returns>
        public string execTranctionDml(List<string> ids, List<Object> objs)
        {
            resultSql = "";
            List<string> idlist = new List<string>();
            idlist = ids;
            List<Object> objectlist = new List<Object>();
            objectlist = objs;
            if (idlist.Count <= 0) throw new Exception("缺少参数");
            if (idlist.Count != objectlist.Count) throw new Exception("参数数量不一致");
            List<string> sqls = new List<string>();
            for (int i = 0; i < idlist.Count; i++)
            {
                daoStruct = parseDao.ObtainConfig(idlist[i]);
                if (daoStruct.DbType.Length <= 0 || daoStruct.ConStr.Length <= 0 || daoStruct.SqlStr.Length <= 0) throw new Exception("配置参数不全");
                string sql = "";
                sql = setSql(objectlist[i], daoStruct.SqlStr);
                if (sql.Length <= 0) throw new Exception("构造SQL语句出错");
                sqls.Add(sql);
                if (resultSql != "") resultSql += "\r\n";
                resultSql += sql;
            }
            if (daoStruct.IsLog)
                WriteLogin.writeDBLog(daoStruct.Desc, resultSql);
            IDataSource dataSource = null;
            switch (daoStruct.DbType)
            {
                case "oracle":
                    dataSource = new OracleDataSource(daoStruct);
                    break;
                case "mysql":
                    dataSource = new MySqlDataSource(daoStruct);
                    break;
                case "sql":
                    dataSource = new SqlDataSource(daoStruct);
                    break;
                case "access":
                    dataSource = new AccessDataSource(daoStruct);
                    break;
                case "sqllite":
                    dataSource = new SqlLiteDataSource(daoStruct);
                    break;
            }
            dataSource.TranctionExcute(sqls);
            //记录SQL
            
            return "";
        }
        /// <summary>
        /// Prepare DML
        /// </summary>
        /// <param name="ids">SQL语句ID编号</param>
        /// <param name="objs">输入对象</param>
        /// <returns></returns>
        public string execPrepare(string id, Object obj)
        {
            try
            {
                resultSql = "";
                if (id == null) throw new Exception("参数不全");
                if (obj == null) throw new Exception("参数不全");
                daoStruct = parseDao.ObtainConfig(id);
                if (daoStruct.DbType.Length <= 0 || daoStruct.ConStr.Length <= 0 || daoStruct.SqlStr.Length <= 0) throw new Exception("配置参数不全");
                string sql = daoStruct.SqlStr;
                resultSql = sql;
                if (daoStruct.IsLog)
                    WriteLogin.writeDBLog(daoStruct.Desc, sql);
                List<DTOClass> dtolist = setDto(obj);
                if (sql.Length <= 0) throw new Exception("构造SQL语句出错");
                IDataSource dataSource = null;
                switch (daoStruct.DbType)
                {
                    case "oracle":
                        dataSource = new OracleDataSource(daoStruct);
                        break;
                    case "mysql":
                        dataSource = new MySqlDataSource(daoStruct);
                        break;
                    case "sql":
                        dataSource = new SqlDataSource(daoStruct);
                        break;
                    case "access":
                        dataSource = new AccessDataSource(daoStruct);
                        break;
                    case "sqllite":
                        dataSource = new SqlLiteDataSource(daoStruct);
                        break;
                }
                dataSource.PrepareExcute(sql, dtolist);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message,ex);
            }
            return "";
        }
        /// <summary>
        /// Prepare DML
        /// </summary>
        /// <param name="ids">SQL语句ID编号</param>
        /// <param name="objs">输入对象</param>
        /// <returns></returns>
        public string execPrepare(List<string> ids, List<Object> objs)
        {
            try
            {
                resultSql = "";
                List<string> idlist = new List<string>();
                idlist = ids;
                List<Object> objectlist = new List<Object>();
                objectlist = objs;
                if (idlist.Count <= 0) throw new Exception("缺少参数");
                if (idlist.Count != objectlist.Count) throw new Exception("参数数量不一致");
                List<string> sqls = new List<string>();
                List<List<DTOClass>> dtolists = new List<List<DTOClass>>();
                for (int i = 0; i < idlist.Count; i++)
                {
                    daoStruct = parseDao.ObtainConfig(idlist[i]);
                    if (daoStruct.DbType.Length <= 0 || daoStruct.ConStr.Length <= 0 || daoStruct.SqlStr.Length <= 0) throw new Exception("配置参数不全");
                    string sql = daoStruct.SqlStr;
                    List<DTOClass> dtolist = setDto(objectlist[i]);
                    if (sql.Length <= 0) throw new Exception("构造SQL语句出错");
                    sqls.Add(sql);
                    dtolists.Add(dtolist);
                    if (resultSql != "") resultSql += "\r\n";
                    resultSql += sql;
                }
                if (daoStruct.IsLog)
                    WriteLogin.writeDBLog(daoStruct.Desc, resultSql);
                IDataSource dataSource = null;
                switch (daoStruct.DbType)
                {
                    case "oracle":
                        dataSource = new OracleDataSource(daoStruct);
                        break;
                    case "mysql":
                        dataSource = new MySqlDataSource(daoStruct);
                        break;
                    case "sql":
                        dataSource = new SqlDataSource(daoStruct);
                        break;
                    case "access":
                        dataSource = new AccessDataSource(daoStruct);
                        break;
                    case "sqllite":
                        dataSource = new SqlLiteDataSource(daoStruct);
                        break;
                }
                dataSource.PrepareExcute(sqls, dtolists);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return "";
        }
        

        public string getSql()
        {
            return resultSql;
        }
        /// <summary>
        /// 根据DataRow为对象赋值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private object setObjectValue(Object obj, DataRow row)
        {
            object fieldvalue = null;
            string fieldtype = "";
            PropertyInfo[] props = obj.GetType().GetProperties();
            foreach (DataColumn col in row.Table.Columns)
            {
                fieldvalue = row[col.ColumnName];
                if (fieldvalue != DBNull.Value)
                {
                    foreach (PropertyInfo pinfo in props)
                    {
                        if (pinfo.Name.ToLower() == col.ColumnName.ToLower())
                        {
                            fieldtype = pinfo.PropertyType.Name.ToLower();
                            switch (fieldtype)
                            {
                                case "datetime":
                                    DateTime datevalue = DateTime.Parse(fieldvalue.ToString());
                                    pinfo.SetValue(obj, datevalue, new object[] { });
                                    break;
                                case "boolean":
                                    bool boolvalue = bool.Parse(fieldvalue.ToString());
                                    pinfo.SetValue(obj, boolvalue, new object[] { });
                                    break;
                                case "int32":
                                    int ivalue = int.Parse(fieldvalue.ToString());
                                    pinfo.SetValue(obj, ivalue, new object[] { });
                                    break;
                                case "int64":
                                    long lvalue = long.Parse(fieldvalue.ToString());
                                    pinfo.SetValue(obj, lvalue, new object[] { });
                                    break;
                                case "single":
                                    float fvalue = float.Parse(fieldvalue.ToString());
                                    pinfo.SetValue(obj, fvalue, new object[] { });
                                    break;
                                case "double":
                                    double doublevalue = double.Parse(fieldvalue.ToString());
                                    pinfo.SetValue(obj, doublevalue, new object[] { });
                                    break;
                                case "decimal":
                                    decimal decimalvalue = decimal.Parse(fieldvalue.ToString());
                                    pinfo.SetValue(obj, decimalvalue, new object[] { });
                                    break;
                                case "guid":
                                    Guid guidvalue = (Guid)fieldvalue;
                                    pinfo.SetValue(obj, guidvalue, new object[] { });
                                    break;
                                case "byte[]":
                                    byte[] b = new byte[0];
                                    b = (Byte[])fieldvalue;
                                    pinfo.SetValue(obj, b, new object[] { });
                                    break;
                                default:
                                    string stringvalue = fieldvalue.ToString();
                                    pinfo.SetValue(obj, stringvalue, new object[] { });
                                    break;
                            }
                        }
                    }
                }
            }
            return obj;
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="id">SQL语句ID编号</param>
        /// <param name="obj">输入对象</param>
        /// <returns>输出对象列表</returns>
        public List<T> getData<T>(string id, Object obj) where T : class
        {
            DataSet rtnDataSet = getDataSet(id, obj);
            List<T> objlist = new List<T>();
            if (rtnDataSet == null) return null;
            Assembly assembly = Assembly.Load(daoStruct.ResultClassName.Substring(0, daoStruct.ResultClassName.LastIndexOf(".")));
            foreach (DataRow row in rtnDataSet.Tables[0].Rows)
            {
                T objdataset = (T)setObjectValue(assembly.CreateInstance(daoStruct.ResultClassName),row);// assembly.CreateInstance(daoStruct.ResultClassName);
                objlist.Add(objdataset);
            }
            rtnDataSet.Dispose();
            rtnDataSet = null;
            return objlist;
        }
        /// <summary>
        /// 查询数据根据SQL语句
        /// </summary>
        /// <param name="id">SQL编号</param>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据结果</returns>
        public DataSet getDataSet(string id, string sql)
        {
            resultSql = sql;
            DataSet rtnDataSet = new DataSet();
            IDataSource dataSource = null;
            daoStruct = parseDao.ObtainConfig(id);
            switch (daoStruct.DbType)
            {
                case "oracle":
                    dataSource = new OracleDataSource(daoStruct);
                    break;
                case "mysql":
                    dataSource = new MySqlDataSource(daoStruct);
                    break;
                case "sql":
                    dataSource = new SqlDataSource(daoStruct);
                    break;
                case "access":
                    dataSource = new AccessDataSource(daoStruct);
                    break;
                case "sqllite":
                    dataSource = new SqlLiteDataSource(daoStruct);
                    break;
            }
            if (dataSource != null) return dataSource.SelectExecute(sql);
            else return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public DataSet getDataSet(string id, Object objs)
        {
            resultSql = "";
            daoStruct = parseDao.ObtainConfig(id);
            if (daoStruct.DbType.Length <= 0 || daoStruct.ConStr.Length <= 0 || daoStruct.SqlStr.Length <= 0) throw new Exception("配置参数不全");
            string sql = setSql(objs, daoStruct.SqlStr);
            resultSql = sql;
            if (sql.Length <= 0) throw new Exception("构造SQL语句出错");
            IDataSource dataSource = null;
            switch (daoStruct.DbType)
            {
                case "oracle":
                    dataSource = new OracleDataSource(daoStruct);
                    break;
                case "mysql":
                    dataSource = new MySqlDataSource(daoStruct);
                    break;
                case "sql":
                    dataSource = new SqlDataSource(daoStruct);
                    break;
                case "access":
                    dataSource = new AccessDataSource(daoStruct);
                    break;
                case "sqllite":
                    dataSource = new SqlLiteDataSource(daoStruct);
                    break;
            }
            if (dataSource != null) return dataSource.SelectExecute(sql);
            else return null;
        }
        /// <summary>
        /// 查询数据根据SQL语句
        /// </summary>
        /// <param name="id">SQL编号</param>
        /// <param name="sql">SQL语句</param>
        /// <returns>数据结果</returns>
        public DataTable getDataTable(string id, string sql)
        {
            return getDataSet(id, sql).Tables[0];
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public DataTable getDataTable(string id, Object objs)
        {
            return getDataSet(id, objs).Tables[0];
        }
        /// <summary>
        /// 获得数据，只有数据列少时用他
        /// </summary>
        /// <param name="id"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public IDataReader getDataRead(string id, Object objs)
        {
            resultSql = "";
            daoStruct = parseDao.ObtainConfig(id);
            if (daoStruct.DbType.Length <= 0 || daoStruct.ConStr.Length <= 0 || daoStruct.SqlStr.Length <= 0) throw new Exception("配置参数不全");
            string sql = setSql(objs, daoStruct.SqlStr);
            resultSql = sql;
            if (sql.Length <= 0) throw new Exception("构造SQL语句出错");
            IDataSource dataSource = null;
            List<Object> objlist = new List<Object>();
            switch (daoStruct.DbType)
            {
                case "oracle":
                    dataSource = new OracleDataSource(daoStruct);
                    break;
                case "mysql":
                    dataSource = new MySqlDataSource(daoStruct);
                    break;
                case "sql":
                    dataSource = new SqlDataSource(daoStruct);
                    break;
                case "access":
                    dataSource = new AccessDataSource(daoStruct);
                    break;
                case "sqllite":
                    dataSource = new SqlLiteDataSource(daoStruct);
                    break;
            }
            if (dataSource != null) return dataSource.GetDataRead(sql);
            else return null;
        }
        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="path"></param>
        /// <param name="database"></param>
        public void BackupData(string id, string path, string database)
        {
            daoStruct = parseDao.ObtainConfig(id);
            string backupsql = "";
            IDataSource dataSource = null;
            switch (daoStruct.DbType)
            {
                case "oracle":
                    dataSource = new OracleDataSource(daoStruct);
                    break;
                case "sql":
                    backupsql = "BACKUP DATABASE [" + database + "] TO DISK =N'" + path + "' WITH NOFORMAT, INIT, NAME=N'" + database + "-完整 数据库 备份', SKIP, NOREWIND, NOUNLOAD,  STATS = 10";
                    dataSource = new SqlDataSource(daoStruct);
                    break;
                case "mysql":
                    dataSource = new MySqlDataSource(daoStruct);
                    break;
            }
            dataSource.Backup(backupsql);
        }
        public void BackupFileData(string id, string destFilename)
        {
            daoStruct = parseDao.ObtainConfig(id);
            IDataSource dataSource = null;
            switch (daoStruct.DbType)
            {
                case "access":
                    dataSource = new AccessDataSource(daoStruct);
                    break;
                case "sqllite":
                    dataSource = new SqlLiteDataSource(daoStruct);
                    break;
            }
            string[] srcfiles = daoStruct.ConStr.Split(';');
            string srcfile = srcfiles[0].Replace("Data Source=","");
            dataSource.Backup(srcfile, destFilename);
        }
        public void disposeConn(string id)
        {
            try
            {
                daoStruct = parseDao.ObtainConfig(id);
                IDataSource dataSource;
                switch (daoStruct.DbType)
                {
                    case "oracle":
                        dataSource = new OracleDataSource(daoStruct);
                        dataSource.DisposeConn();
                        break;
                    case "mysql":
                        dataSource = new MySqlDataSource(daoStruct);
                        dataSource.DisposeConn();
                        break;
                    case "sql":
                        dataSource = new SqlDataSource(daoStruct);
                        dataSource.DisposeConn();
                        break;
                    case "access":
                        dataSource = new AccessDataSource(daoStruct);
                        dataSource.DisposeConn();
                        break;
                    case "sqllite":
                        dataSource = new SqlLiteDataSource(daoStruct);
                        dataSource.DisposeConn();
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void disposeAllConn()
        {
            try
            {
                DBConnectionPool.disposeAllConn();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
