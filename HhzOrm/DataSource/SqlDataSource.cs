using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace HhzOrm.DataSource
{
    using util;
    using dblogin;

    class SqlDataSource : IDataSource
    {
        private SqlConnection sqlConnection;
        private SqlCommand sqlCommand;
        private SqlDataAdapter sqlDataAdapter;
        private SqlTransaction sqlTran;
        private DataSet sqlDataSet;
        private DaoStruct daoStruct;
        public SqlDataSource(DaoStruct daoStruct)
        {
            this.daoStruct = daoStruct;
            DBConnectionPool.initConnection(sqlConnection, daoStruct);
            sqlConnection = (SqlConnection)DBConnectionPool.GetConnection(daoStruct);
            sqlCommand = new SqlCommand();
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
                throw new Exception("Class [SqlDataSource] " + ex.Message,ex);
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
            sqlDataAdapter = new SqlDataAdapter();
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
                throw new Exception("Class [SqlDataSource] " + ex.Message,ex);
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
            sqlTran = sqlConnection.BeginTransaction();
            sqlCommand.Transaction = sqlTran;
            sqlCommand.CommandText = sql;
            try
            {
                sqlCommand.ExecuteNonQuery();
                sqlCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                sqlCommand.Transaction.Rollback();
                throw new Exception("Class [SqlDataSource] " + ex.Message,ex);
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
            sqlTran = sqlConnection.BeginTransaction();
            sqlCommand.Transaction = sqlTran;
            try
            {
                foreach (string sql in sqls)
                {
                    sqlCommand.CommandText = sql;
                    sqlCommand.ExecuteNonQuery();
                }
                sqlCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                sqlCommand.Transaction.Rollback();
                throw new Exception("Class [SqlDataSource] " + ex.Message,ex);
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
        private void prepare(string sql, List<DTOClass> dtolist)
        {
            if (sql.Length <= 0) return;
            sqlCommand.Parameters.Clear();
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
                                sqlCommand.Parameters.Add(new SqlParameter("@" + dto.name, SqlDbType.Bit));
                                if (dto.value != null) sqlCommand.Parameters["@" + dto.name].Value = (bool)dto.value;
                                else sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                break;
                            case "string":
                                sqlCommand.Parameters.Add(new SqlParameter("@"+dto.name, SqlDbType.VarChar));
                                if (dto.value != null) sqlCommand.Parameters["@" + dto.name].Value = (string)dto.value;
                                else sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                break;
                            case "int32":
                                sqlCommand.Parameters.Add(new SqlParameter("@" + dto.name, SqlDbType.Int));
                                if (dto.value != null) sqlCommand.Parameters["@" + dto.name].Value = (int)dto.value;
                                else sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                break;
                            case "int64":
                                sqlCommand.Parameters.Add(new SqlParameter("@" + dto.name, SqlDbType.BigInt));
                                if (dto.value != null) sqlCommand.Parameters["@" + dto.name].Value = (Int64)dto.value;
                                else sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                break;
                            case "single":
                                sqlCommand.Parameters.Add(new SqlParameter("@" + dto.name, SqlDbType.Real));
                                if (dto.value != null) sqlCommand.Parameters["@" + dto.name].Value = (float)dto.value;
                                else sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                break;
                            case "double":
                                sqlCommand.Parameters.Add(new SqlParameter("@" + dto.name, SqlDbType.Float));
                                if (dto.value != null) sqlCommand.Parameters["@" + dto.name].Value = (double)dto.value;
                                else sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                break;
                            case "decimal":
                                sqlCommand.Parameters.Add(new SqlParameter("@" + dto.name, SqlDbType.Decimal));
                                if (dto.value != null) sqlCommand.Parameters["@" + dto.name].Value = (decimal)dto.value;
                                else sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                break;
                            case "datetime":
                                sqlCommand.Parameters.Add(new SqlParameter("@" + dto.name, SqlDbType.DateTime));
                                if (dto.value != null) sqlCommand.Parameters["@" + dto.name].Value = (DateTime)dto.value;
                                else sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                break;
                            case "byte[]":
                                sqlCommand.Parameters.Add(new SqlParameter("@" + dto.name, SqlDbType.Image));
                                if (dto.value != null)
                                {
                                    byte[] b = (byte[])dto.value;
                                    if (b.Length > 10)
                                        sqlCommand.Parameters["@" + dto.name].Value = (byte[])dto.value;
                                    else
                                        sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                }
                                else sqlCommand.Parameters["@" + dto.name].Value = DBNull.Value;
                                break;
                        }
                    }
                    sql = sql.Replace("#" + dto.name + "#", "@" + dto.name);
                }
            }
            
            sqlCommand.CommandText = sql;
            sqlCommand.ExecuteNonQuery();
        }
        public void PrepareExcute(string sql, List<DTOClass> dtolist)
        {
            try
            {
                prepare(sql, dtolist);
            }
            catch (Exception ex)
            {
                throw new Exception("Class [SqlDataSource] " + ex.Message,ex);
            }
            finally
            {
                ConnClose();
            }
        }
        public void PrepareExcute(List<string> sqls, List<List<DTOClass>> dtolists)
        {
            if (sqls == null) return;
            List<DTOClass> dtolist = new List<DTOClass>();
            int index = 0;
            sqlTran = sqlConnection.BeginTransaction();
            sqlCommand.Transaction = sqlTran;
            try
            {
                foreach (string sql in sqls)
                {
                    dtolist = dtolists[index];
                    prepare(sql, dtolist);
                    index++;
                }
                sqlCommand.Transaction.Commit();
            }
            catch (Exception ex)
            {
                sqlCommand.Transaction.Rollback();
                throw new Exception("Class [SqlDataSource] " + ex.Message, ex);
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
            //SqlConnection.ClearPool(sqlConnection);
            if(sqlTran != null)
            {
                sqlTran.Dispose();
                sqlTran = null;
            }
        }
        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="sql"></param>
        public void Backup(string sql)
        {
            if (sql.Length <= 0) return;
            sqlCommand.CommandText = sql;
            try
            {
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Class [SqlDataSource] " + ex.Message, ex);
            }
            finally
            {
                ConnClose();
            }
            return;
        }
        public void Backup(string sourceFilename, string aimFilename)
        { }

    }
}
