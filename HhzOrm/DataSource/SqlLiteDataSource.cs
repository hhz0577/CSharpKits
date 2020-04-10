using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace HhzOrm.DataSource
{
    using System.IO;
    using util;

    class SqlLiteDataSource : IDataSource
    {
        private SQLiteConnection sqlConnection;
        private SQLiteCommand sqlCommand;
        private SQLiteDataAdapter sqlDataAdapter;
        private SQLiteTransaction sqlTran;
        private DataSet sqlDataSet;
        private DaoStruct daoStruct;
        public SqlLiteDataSource(DaoStruct daoStruct)
        {
            this.daoStruct = daoStruct;
            DBConnectionPool.initConnection(sqlConnection, daoStruct);
            sqlConnection = (SQLiteConnection)DBConnectionPool.GetConnection(daoStruct);
            sqlCommand = new SQLiteCommand();
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
                throw new Exception("Class [SqlLiteDataSource] " + ex.Message,ex);
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
            sqlDataAdapter = new SQLiteDataAdapter();
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
                throw new Exception("Class [SqlLiteDataSource] " + ex.Message,ex);
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
                throw new Exception("Class [SqlLiteDataSource] " + ex.Message,ex);
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
                throw new Exception("Class [SqlLiteDataSource] " + ex.Message, ex);
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
                                //sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, DbType.Boolean));
                                if (dto.value != null) sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, (bool)dto.value));
                                else sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, null));
                                break;
                            case "string":
                                if (dto.value != null) sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, (string)dto.value));
                                else sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, null));
                                break;
                            case "int32":
                                if (dto.value != null) sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, (int)dto.value));
                                else sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, null));
                                break;
                            case "int64":
                                if (dto.value != null) sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, (int)dto.value));
                                else sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, null));
                                break;
                            case "single":
                                if (dto.value != null) sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, (float)dto.value));
                                else sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, null));
                                break;
                            case "double":
                                if (dto.value != null) sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, (double)dto.value));
                                else sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, null));
                                break;
                            case "decimal":
                                if (dto.value != null) sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, (decimal)dto.value));
                                else sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, null));
                                break;
                            case "datetime":
                                if (dto.value != null) sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, (DateTime)dto.value));
                                else sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, null));
                                break;
                            case "byte[]":
                                if (dto.value != null) sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, (byte[])dto.value));
                                else sqlCommand.Parameters.Add(new SQLiteParameter(dto.name, null));
                                break;
                        }
                    }
                    sql = sql.Replace("#" + dto.name + "#", ":" + dto.name);
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
                throw new Exception("Class [SqlLiteDataSource] " + ex.Message, ex);
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
                throw new Exception("Class [SqlLiteDataSource] " + ex.Message, ex);
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
            if (sqlTran != null)
            {
                sqlTran.Dispose();
                sqlTran = null;
            }
            //SQLiteConnection.ClearPool(sqlConnection);
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
