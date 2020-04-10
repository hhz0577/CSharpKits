using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace HhzOrm.DataSource
{
    using util;
    /// <summary>
    /// 数据操作接口
    /// </summary>
    public interface IDataSource
    {
        DataSet SelectExecute(string sql);
        void SingleExecute(string sql);
        IDataReader GetDataRead(string sql);
        void TranctionExcute(List<string> sqls);
        void PrepareExcute(string sql, List<DTOClass> dtolist);
        void PrepareExcute(List<string> sqls, List<List<DTOClass>> dtolists);
        void Backup(string sql);
        void Backup(string sourceFilename, string aimFilename);
        void DisposeConn();
    }

}
