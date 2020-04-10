using System;
using System.Collections.Generic;
using System.Text;

namespace HhzOrm.util
{
    /// <summary>
    /// DAO资源存放结构
    /// </summary>
    public struct DaoStruct
    {
        public string ConStr;
        public string DbType;
        public string OperationType;
        public string ClassName;
        public string ResultClassName;
        public string SqlStr;
        public string Desc;
        public bool IsLog;
    }
}
