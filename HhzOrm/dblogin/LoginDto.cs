using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzOrm.dblogin
{
    class LoginDto
    {
        public string _id { get; set; }
        public string _userid { get; set; }
        public string _funcname { get; set; }
        public DateTime _writetime { get; set; }
        public string _sql { get; set; }
    }
}
