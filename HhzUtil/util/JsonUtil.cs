using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzUtil.util
{
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Xml;
    using System.IO;

    public class JsonUtil
    {
        public static object jsonToObject(String jsonstr,object obj)
        {
            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            ////NULL的处理
            jSetting.NullValueHandling = NullValueHandling.Ignore;//忽略为NULL的值
            ////默认值的处理
            ////jSetting.DefaultValueHandling = DefaultValueHandling.Include;//序列化和反序列化时,包含默认值
            return JsonConvert.DeserializeObject(jsonstr, obj.GetType(), jSetting);
        }
        public static T jsonToObject<T>(String jsonstr) where T : class
        {
            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            ////NULL的处理
            jSetting.NullValueHandling = NullValueHandling.Ignore;//忽略为NULL的值
            ////默认值的处理
            ////jSetting.DefaultValueHandling = DefaultValueHandling.Include;//序列化和反序列化时,包含默认值
            return JsonConvert.DeserializeObject<T>(jsonstr);
        }
        public static object jsonToObjectForName(String jsonstr, string objname)
        {
            ClassUtil cutil = new ClassUtil();
            object obj = cutil.createInstanceByName(objname);
            return jsonToObject(jsonstr, obj);
        }
        public static List<object> jsonToObject(String jsonstr, List<object> objs)
        {
            List<object> rtnobj = new List<object>();
            object o = null;
            foreach(object obj in objs)
            {
                o = jsonToObject(jsonstr, obj);
                if(o != null)
                    rtnobj.Add(o);
            }
            return rtnobj;
        }
        public static Hashtable jsonToHash(string json)
        {
            Hashtable hash = (Hashtable)JsonConvert.DeserializeObject(json, typeof(Hashtable));
            return hash;
        }
        public static List<T> jsonToObject<T>(String jsonstr, string rootname) where T : class
        {
            string jsonaray = jsonstr;
            if (!string.IsNullOrEmpty(rootname))
            {
                Hashtable hash = jsonToHash(jsonstr);
                foreach (DictionaryEntry de in hash)
                {
                    if(de.Key.ToString() == rootname)
                        jsonaray = de.Value.ToString();
                }
                //Hashtable hashtable 
            }
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(jsonaray);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }
        public static string objectToJson(object obj)
        {
            JsonSerializerSettings jSetting = new JsonSerializerSettings();
            ////NULL的处理
            jSetting.NullValueHandling = NullValueHandling.Ignore;//忽略为NULL的值
            ////默认值的处理
            ////jSetting.DefaultValueHandling = DefaultValueHandling.Include;//序列化和反序列化时,包含默认值
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented, jSetting);
        }
        public static XmlNodeReader jsonToXmlRead(string jsonstr, string rootNode)
        {
            XmlDocument oXmlDoc = JsonConvert.DeserializeXmlNode(jsonstr, rootNode);
            XmlNodeReader xmlread = new XmlNodeReader(oXmlDoc);
            return xmlread;
        }
    }
}
