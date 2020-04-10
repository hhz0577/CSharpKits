using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzOrm.DataSource
{
    class SqlUtil
    {
        public static List<string> getFieldOrder(string sql)
        {
            List<string> fieldorders = new List<string>();
            int index = 0;
            string[] ss = sql.Split('#');
            foreach (string s in ss)
            {

                if (index % 2 > 0) fieldorders.Add(s);
                index++;
            }
            return fieldorders;
        }
    }
}
