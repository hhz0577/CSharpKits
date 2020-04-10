using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data;

namespace HhzOrm.DataSource
{
    using util;
    /// <summary>
    /// ORACLE库 数据操作实现类
    /// </summary>
    public class OracleDataSource : IDataSource
    {
        private OracleConnection oracleConnection;
        private OracleDataAdapter oracleDataAdapter;
        private OracleCommand oracleCommand;
        private OracleTransaction oracleTran;
        private DataSet oracleDataSet;
        private DaoStruct daoStruct;
        /// <summary>
        /// 数据控件初始化
        /// </summary>
        /// <param name="scon">数据连接字符</param>
        public OracleDataSource(DaoStruct daoStruct)
        {
            this.daoStruct = daoStruct;
            DBConnectionPool.initConnection(oracleConnection, daoStruct);
            oracleConnection = (OracleConnection)DBConnectionPool.GetConnection(daoStruct);
            oracleCommand = new OracleCommand();
            oracleCommand.Connection = oracleConnection;
        }
        public IDataReader GetDataRead(string sql)
        {
            if (sql.Length <= 0) return null;
            oracleCommand.CommandText = sql;
            IDataReader dataread = null;
            try
            {
                dataread = oracleCommand.ExecuteReader();
                return dataread;
            }
            catch (Exception ex)
            {
                throw new Exception("Class [OracleDataSource] " + ex.Message, ex);
            }
            finally
            {
                ConnClose();
            }
        }
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>结果记录集</returns>
        public DataSet SelectExecute(string sql)
        {
            if (sql.Length <= 0) return null;
            oracleDataAdapter = new OracleDataAdapter();
            oracleDataAdapter.SelectCommand = oracleCommand;
            oracleDataSet = new DataSet();
            oracleCommand.CommandText = sql;
            oracleDataSet.Clear();
            try
            {
                oracleDataAdapter.Fill(oracleDataSet);
                return oracleDataSet;
            }
            catch (Exception ex)
            {
                throw new Exception("Class [OracleDataSource] " + ex.Message, ex);
            }
            finally
            {
                ConnClose();
            }
        }
        /// <summary>
        /// 执行单语句的DML操作 含事务
        /// </summary>
        /// <param name="sql">SQL语句</param>
        public void SingleExecute(string sql)
        {
            if (sql.Length <= 0) return;
            oracleTran = oracleConnection.BeginTransaction();
            oracleCommand.Transaction = oracleTran;
            oracleCommand.CommandText = sql;
            try
            {
                oracleCommand.ExecuteNonQuery();
                oracleCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                oracleCommand.Transaction.Rollback();
                throw new Exception("Class [OracleDataSource] " + ex.Message,ex);
            }
            finally
            {
                ConnClose();
            }
        }

        /// <summary>
        /// 多SQL语句DML操作
        /// </summary>
        /// <param name="sqls">SQL语句列表</param>
        public void TranctionExcute(List<string> sqls)
        {
            if (sqls == null) return;
            oracleTran = oracleConnection.BeginTransaction();
            oracleCommand.Transaction = oracleTran;
            try
            {
                foreach (string sql in sqls)
                {
                    oracleCommand.CommandText = sql;
                    oracleCommand.ExecuteNonQuery();
                }
                oracleCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                oracleCommand.Transaction.Rollback();
                throw new Exception("Class [OracleDataSource] " + ex.Message,ex);
            }
            finally
            {
                ConnClose();
            }
        }
        /// <summary>
        /// PrepareSQL语句DML操作，只能执行一条
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="dtolist">类结构</param>
        /// 要捕获异常
        private void prepare(string sql, List<DTOClass> dtolist)
        {
            if (sql.Length <= 0) return;
            oracleCommand.Parameters.Clear();
            List<string> fieldorders = SqlUtil.getFieldOrder(sql);
            foreach (string fieldorder in fieldorders)
            {
                foreach (DTOClass dto in dtolist)
                {

                    if (fieldorder == dto.name)
                    {
                        switch (dto.fieldtype.ToLower())
                        {
                            case "boolean":
                                oracleCommand.Parameters.Add(new OracleParameter(dto.name, OracleDbType.Varchar2));
                                if (dto.value != null) oracleCommand.Parameters[dto.name].Value = ((bool)dto.value).ToString();
                                else oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                break;
                            case "string":
                                oracleCommand.Parameters.Add(new OracleParameter(dto.name, OracleDbType.Varchar2));
                                if (dto.value != null) oracleCommand.Parameters[dto.name].Value = (string)dto.value;
                                else oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                break;
                            case "int32":
                                oracleCommand.Parameters.Add(new OracleParameter(dto.name, OracleDbType.Int32));
                                if (dto.value != null) oracleCommand.Parameters[dto.name].Value = (int)dto.value;
                                else oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                break;
                            case "int64":
                                oracleCommand.Parameters.Add(new OracleParameter(dto.name, OracleDbType.Int64));
                                if (dto.value != null) oracleCommand.Parameters[dto.name].Value = (Int64)dto.value;
                                else oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                break;
                            case "single":
                                oracleCommand.Parameters.Add(new OracleParameter(dto.name, OracleDbType.Single));
                                if (dto.value != null) oracleCommand.Parameters[dto.name].Value = (float)dto.value;
                                else oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                break;
                            case "double":
                                oracleCommand.Parameters.Add(new OracleParameter(dto.name, OracleDbType.Double));
                                if (dto.value != null) oracleCommand.Parameters[dto.name].Value = (double)dto.value;
                                else oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                break;
                            case "decimal":
                                oracleCommand.Parameters.Add(new OracleParameter(dto.name, OracleDbType.Decimal));
                                if (dto.value != null) oracleCommand.Parameters[dto.name].Value = (decimal)dto.value;
                                else oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                break;
                            case "datetime":
                                oracleCommand.Parameters.Add(new OracleParameter(dto.name, OracleDbType.Date));
                                if (dto.value != null) oracleCommand.Parameters[dto.name].Value = (DateTime)dto.value;
                                else oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                break;
                            case "byte[]":
                                oracleCommand.Parameters.Add(new OracleParameter(dto.name, OracleDbType.Blob));
                                if (dto.value != null)
                                {
                                    byte[] b = (byte[])dto.value;
                                    if (b.Length > 10)
                                        oracleCommand.Parameters[dto.name].Value = (byte[])dto.value;
                                    else
                                        oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                }
                                else oracleCommand.Parameters[dto.name].Value = DBNull.Value;
                                break;
                        }
                    }
                    sql = sql.Replace("#" + dto.name + "#", ":" + dto.name);
                }
            }
            oracleCommand.CommandText = sql;
            oracleCommand.ExecuteNonQuery();
        }
        public void PrepareExcute(string sql, List<DTOClass> dtolist)
        {
            try
            {
                prepare(sql, dtolist);
            }
            catch (Exception ex)
            {
                throw new Exception("Class [OracleDataSource] " + ex.Message,ex);
            }
            finally
            {
                ConnClose();
            }
        }
        /// <summary>
        /// PrepareSQL语句DML操作，执行多条
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="dtolist">类结构列表</param>
        public void PrepareExcute(List<string> sqls, List<List<DTOClass>> dtolists)
        {
            if (sqls == null) return;
            List<DTOClass> dtolist = new List<DTOClass>();
            int index = 0;
            oracleTran = oracleConnection.BeginTransaction();
            oracleCommand.Transaction = oracleTran;
            try
            {
                foreach (string sql in sqls)
                {
                    dtolist = dtolists[index];
                    prepare(sql, dtolist);
                    index++;
                }
                oracleCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                oracleCommand.Transaction.Rollback();
                throw new Exception("Class [OracleDataSource] " + ex.Message, ex);
            }
            finally
            {
                ConnClose();
            }
        }
        /// <summary>
        /// 释放数据连接资源
        /// </summary>
        public void DisposeConn()
        {
            DBConnectionPool.disposeConnection(daoStruct);
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        private void ConnClose()
        {
            if (oracleCommand != null)
            {
                oracleCommand.Dispose();
                oracleCommand = null;
            }
            if (oracleDataAdapter != null)
            {
                oracleDataAdapter.Dispose();
                oracleDataAdapter = null;
            }
            //OracleConnection.ClearPool(oracleConnection);
            if (oracleTran != null)
            {
                oracleTran.Dispose();
                oracleTran = null;
            }
        }
        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="sql"></param>
        public void Backup(string sql)
        {
            if (sql.Length <= 0) return;
            oracleCommand.CommandText = sql;
            try
            {
                oracleCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Class [OracleDataSource] " + ex.Message, ex);
            }
            finally
            {
                ConnClose();
            }
        }
        public void Backup(string sourceFilename, string aimFilename)
        {

        }
    }
}
