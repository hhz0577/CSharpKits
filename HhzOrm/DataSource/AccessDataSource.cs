using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace HhzOrm.DataSource
{
    using System.IO;
    using util;

    class AccessDataSource : IDataSource
    {
        private OleDbConnection sqlConnection;
        private OleDbCommand sqlCommand;
        private OleDbDataAdapter sqlDataAdapter;
        private DataSet sqlDataSet;
        private DaoStruct daoStruct;
        public AccessDataSource(DaoStruct daoStruct)
        {
            this.daoStruct = daoStruct;
            DBConnectionPool.initConnection(sqlConnection, daoStruct);
            sqlConnection = (OleDbConnection)DBConnectionPool.GetConnection(daoStruct);
            sqlCommand = new OleDbCommand();
            sqlCommand.Connection = sqlConnection;
        }

        public IDataReader GetDataRead(string sql)
        {
            if (sql.Length <= 0) return null;
            sqlCommand.CommandText = sql;
            try
            {
                IDataReader dataread = sqlCommand.ExecuteReader();
                return dataread;
            }
            catch (Exception ex)
            {
                throw new Exception("Class [AccessDataSource] " + ex.Message, ex);
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
            sqlDataAdapter = new OleDbDataAdapter();
            sqlDataAdapter.SelectCommand = sqlCommand;
            sqlDataSet = new DataSet();
            sqlCommand.CommandText = sql;
            sqlDataSet.Clear();
            try
            {
                sqlDataAdapter.Fill(sqlDataSet);
                return sqlDataSet;
            }
            catch (Exception ex)
            {
                throw new Exception("Class [AccessDataSource] " + ex.Message,ex);
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
            sqlCommand.CommandText = sql;
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Class [AccessDataSource] " + ex.Message,ex);
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
            try
            {
                foreach (string sql in sqls)
                {
                    sqlCommand.CommandText = sql;
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Class [AccessDataSource] " + ex.Message,ex);
            }
            finally
            {
                ConnClose();
            }
        }
        /// <summary>
        /// PrepareSQL语句DML操作
        /// </summary>
        /// <param name="sqls">SQL语句列表</param>
        public void PrepareExcute(string sql, List<DTOClass> dtolist)
        {
            if (sql.Length <= 0) return;
            try
            {
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
                                    sqlCommand.Parameters.Add(new OleDbParameter(dto.name, SqlDbType.Bit));
                                    if (dto.value != null) sqlCommand.Parameters[dto.name].Value = (bool)dto.value;
                                    else sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    break;
                                case "string":
                                    sqlCommand.Parameters.Add(new OleDbParameter(dto.name, SqlDbType.NVarChar));
                                    if (dto.value != null) sqlCommand.Parameters[dto.name].Value = (string)dto.value;
                                    else sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    break;
                                case "int32":
                                    sqlCommand.Parameters.Add(new OleDbParameter(dto.name, SqlDbType.Int));
                                    if (dto.value != null) sqlCommand.Parameters[dto.name].Value = (int)dto.value;
                                    else sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    break;
                                case "int64":
                                    sqlCommand.Parameters.Add(new OleDbParameter(dto.name, SqlDbType.BigInt));
                                    if (dto.value != null) sqlCommand.Parameters[dto.name].Value = (Int64)dto.value;
                                    else sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    break;
                                case "single":
                                    sqlCommand.Parameters.Add(new OleDbParameter(dto.name, SqlDbType.Real));
                                    if (dto.value != null) sqlCommand.Parameters[dto.name].Value = (float)dto.value;
                                    else sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    break;
                                case "double":
                                    sqlCommand.Parameters.Add(new OleDbParameter(dto.name, SqlDbType.Float));
                                    if (dto.value != null) sqlCommand.Parameters[dto.name].Value = (decimal)dto.value;
                                    else sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    break;
                                case "decimal":
                                    sqlCommand.Parameters.Add(new OleDbParameter(dto.name, SqlDbType.Decimal));
                                    if (dto.value != null) sqlCommand.Parameters[dto.name].Value = (decimal)dto.value;
                                    else sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    break;
                                case "datetime":
                                    sqlCommand.Parameters.Add(new OleDbParameter(dto.name, SqlDbType.DateTime));
                                    if (dto.value != null) sqlCommand.Parameters[dto.name].Value = (DateTime)dto.value;
                                    else sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    break;
                                case "byte[]":
                                    sqlCommand.Parameters.Add(new OleDbParameter(dto.name, SqlDbType.Image));
                                    if (dto.value != null)
                                    {
                                        byte[] b = (byte[])dto.value;
                                        if (b.Length > 10)
                                            sqlCommand.Parameters[dto.name].Value = (byte[])dto.value;
                                        else
                                            sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    }
                                    else sqlCommand.Parameters[dto.name].Value = DBNull.Value;
                                    break;
                            }
                        }
                        sql = sql.Replace("#" + dto.name + "#", ":" + dto.name);
                    }
                }
                sqlCommand.CommandText = sql;
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Class [AccessDataSource] " + ex.Message,ex);
            }
            finally
            {
                ConnClose();
            }
        }
        public void PrepareExcute(List<string> sqls, List<List<DTOClass>> dtolists)
        {
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
            if (sqlCommand != null)
            {
                sqlCommand.Dispose();
                sqlCommand = null;
            }
            if (sqlDataAdapter != null)
            {
                sqlDataAdapter.Dispose();
                sqlDataAdapter = null;
            }
        }
        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="sql"></param>
        public void Backup(string sql)
        {
            
        }
        public void Backup(string sourceFilename, string destFilename)
        {
            ConnClose();
            if (sourceFilename == string.Empty) return;
            if (destFilename == string.Empty) return;
            if (!File.Exists(sourceFilename)) return;
            if (File.Exists(destFilename)) File.Delete(destFilename);
            File.Copy(sourceFilename, destFilename);
        }
    }
}
