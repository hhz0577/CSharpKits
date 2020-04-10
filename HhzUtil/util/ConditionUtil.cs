using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzUtil.util
{
    using Newtonsoft.Json;
    using converter;
    public class ConditionUtil
    {
        public string Sitem1 { get; set; }
        public string Sitem2 { get; set; }
        public string Sitem3 { get; set; }
        public string Sitem4 { get; set; }
        public string Sitem5 { get; set; }
        public string Sitem6 { get; set; }
        public int Iitem1{ get; set; }
        public int Iitem2{ get; set; }
        [JsonConverter(typeof(HhzDateTimeConverter))]
        public DateTime Ditem1 { get; set; }
        [JsonConverter(typeof(HhzDateTimeConverter))]
        public DateTime Ditem2 { get; set; }
        [JsonConverter(typeof(HhzDateTimeConverter))]
        public DateTime Ditem3 { get; set; }
        public string Nowstr { get; set; }
        [JsonConverter(typeof(HhzDateTimeConverter))]
        public DateTime F_transmit_date { get; set; }
        public string Where { get; set; }
    }
}
