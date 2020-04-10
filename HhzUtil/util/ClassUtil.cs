using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzUtil.util
{
    using System.Reflection;
    using System.Collections;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    public class ClassUtil
    {
        /// <summary>
        /// 根据类名称创建类实例
        /// </summary>
        /// <param name="classname">类名称</param>
        /// <returns></returns>
        public object createInstanceByName(string classname)
        {
            Assembly assembly = Assembly.Load(classname.Substring(0, classname.IndexOf(".")));
            return assembly.CreateInstance(classname);
        }
        /// <summary>
        /// 根据类名称创建类实例，并且根据JSON赋值
        /// </summary>
        /// <param name="jsondata"></param>
        /// <param name="classname">类名称</param>
        /// <returns></returns>
        public object createInstanceByName(string jsondata, string classname)
        {
            object obj = createInstanceByName(classname);
            return JsonUtil.jsonToObject(jsondata, obj);
        }
        /// <summary>
        /// 对象序列化成byte[]
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] ObjectToBytes(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        /// <summary>
        /// byte[]序列化成对象
        /// </summary>
        /// <param name="Bytes"></param>
        /// <returns></returns>
        public object BytesToObject(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                IFormatter formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }
    }
}
