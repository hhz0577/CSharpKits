using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace HhzOrm
{
    /// <summary>
    /// 数据访问接口
    /// </summary>
    public interface IDAOClass
    {
        string execDml(string id, Object obj);
        string execDml(string id, string sql);
        string execTranctionDml(List<string> ids, List<Object> objs);
        string execPrepare(string id, Object obj);
        string execPrepare(List<string> ids, List<Object> objs);
        List<T> getData<T>(string id, Object obj) where T : class;
        IDataReader getDataRead(string sql, Object objs);
        DataTable getDataTable(string id, Object objs);
        DataSet getDataSet(string id, Object objs);
        DataTable getDataTable(string id, string sql);
        DataSet getDataSet(string id, string sql);
        string getSql();
        string getDbType();
        string getDbType(string id);
        string getOperationType(string id);
        void disposeConn(string id);
        void disposeAllConn();
        void BackupFileData(string id, string destFilename);
    }
}
