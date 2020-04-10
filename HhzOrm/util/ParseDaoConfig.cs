using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;

namespace HhzOrm.util
{
    public class ParseDaoConfig
    {
        private static ParseDaoConfig newinstance;
        private XmlDocument oXmlDoc;
        private static string BASEDIC = AppDomain.CurrentDomain.BaseDirectory;
        private string dicConfig = BASEDIC + @"\DAO\Config\";
        private ParseDaoConfig()
        {
            oXmlDoc = new XmlDocument();
            oXmlDoc.Load(BASEDIC + @"\DAO\Config\daoConfig.xml");
        }
        private Hashtable hash = new Hashtable();
        public Hashtable getHash()
        {
            return hash;
        }
        public void setHash(Hashtable v)
        {
            hash = v;
        }
        public static ParseDaoConfig createInstance()
        {
            if (newinstance == null)
            {
                newinstance = new ParseDaoConfig();
                newinstance.setHash(newinstance.getDaoConfig());
            }
            return newinstance;
        }
        /// <summary>
        /// 将配置信息加载到HASH表中,Hash.key = 配置中的ID属性  Hash.Value = DaoStruct结构
        /// </summary>
        /// <returns></returns>
        private Hashtable getDaoConfig()
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Clear();
            XmlDocument oChildDoc = new XmlDocument();
            XmlNode oXmlNodes;
            XmlNode oXmlNode;
            XmlNode sqlNodes;
            XmlNode sqlNode;
            XmlNode dbNodes;
            XmlNode dbNode;

            string sconstr = "";
            string sdbtype = "";
            string sid = "";
            string sqlid = "";
            string spacehold = "";
            
            try
            {
                oXmlNodes = oXmlDoc.DocumentElement.SelectSingleNode("sqlmapconfig");
                dbNodes = oXmlDoc.DocumentElement.SelectSingleNode("dao");
                for (int i = 0; i < oXmlNodes.ChildNodes.Count; i++)
                {
                    oXmlNode = oXmlNodes.ChildNodes[i];
                    sid = oXmlNode.Attributes.GetNamedItem("dbtype").InnerXml;
                    oChildDoc.Load(dicConfig + oXmlNode.Attributes.GetNamedItem("resource").InnerXml);
                    for (int k = 0; k < dbNodes.ChildNodes.Count; k++)
                    {
                        dbNode = dbNodes.ChildNodes[k];
                        if (sid == dbNode.Attributes.GetNamedItem("id").InnerXml)
                        {
                            sdbtype = dbNode.Attributes.GetNamedItem("type").InnerXml;
                            spacehold = dbNode.Attributes.GetNamedItem("spacehold").InnerXml;
                            if (spacehold.ToLower() == "true")
                            {
                                string tempstr = "";
                                sconstr = dbNode.SelectSingleNode("property").InnerXml;
                                int b = sconstr.IndexOf('{') + 1;
                                int e = sconstr.IndexOf('}');
                                tempstr = sconstr.Substring(b, e - b);
                                sconstr = sconstr.Replace("{" + tempstr + "}", BASEDIC + tempstr);
                            }
                            else
                            {
                                sconstr = dbNode.SelectSingleNode("property").InnerXml;
                            }
                            break;
                        }
                    }
                    sqlNodes = oChildDoc.SelectSingleNode("sqlmap");

                    for (int j = 0; j < sqlNodes.ChildNodes.Count; j++)
                    {
                        sqlNode = sqlNodes.ChildNodes[j];
                        DaoStruct daostruct = new DaoStruct();
                        daostruct.ConStr = sconstr;
                        daostruct.DbType = sdbtype.ToLower();
                        daostruct.OperationType = sqlNode.Attributes.GetNamedItem("type").InnerXml.ToLower();
                        daostruct.ClassName = sqlNode.Attributes.GetNamedItem("class").InnerXml;
                        if (sqlNode.Attributes.GetNamedItem("resultclass") != null)
                            daostruct.ResultClassName = sqlNode.Attributes.GetNamedItem("resultclass").InnerXml;
                        else
                            daostruct.ResultClassName = "";
                        if (sqlNode.Attributes.GetNamedItem("desc") != null)
                            daostruct.Desc = sqlNode.Attributes.GetNamedItem("desc").InnerXml;
                        else
                            daostruct.Desc = "";
                        if (sqlNode.Attributes.GetNamedItem("islog") != null)
                        {
                            string logstr = sqlNode.Attributes.GetNamedItem("islog").InnerXml.ToLower();
                            if (logstr == "true")
                                daostruct.IsLog = true;
                            else
                                daostruct.IsLog = false;
                        }
                        else
                            daostruct.IsLog = false;
                        daostruct.SqlStr = sqlNode.InnerXml;
                        sqlid = sqlNode.Attributes.GetNamedItem("id").InnerXml;
                        hashtable.Add(sqlid, daostruct);
                    }
                }
            }
            catch (XmlException e)
            {
                throw new Exception("Class [ParseDaoConfig'] 解析数据访问对象配置出错" + e.Message);
            }
            return hashtable;
        }
        /// <summary>
        /// 在Hash表中查找ID编号的配置 并将结果放在DaoStruct结构中
        /// </summary>
        /// <param name="sid"></param>
        /// <returns></returns>
        public DaoStruct ObtainConfig(string sid)
        {
            Hashtable hash = new Hashtable();
            hash = newinstance.getHash();
            if (hash == null)
            {
                throw new Exception("Class [ParseDaoConfig'] 解析数据访问对象配置没有数据");
            }
            DaoStruct daostruct = new DaoStruct();
            foreach (DictionaryEntry de in hash)
            {
                if (de.Key.ToString() == sid)
                {
                    daostruct = (DaoStruct)de.Value;
                    break;
                }
            }
            return daostruct;
        }

        /// <summary>
        /// 替换SQL语句中的WHERE条件
        /// </summary>
        /// <param name="sqlStr">SQL语句</param>
        /// <returns>最终的SQL语句</returns>
        public string setWhere(string sqlStr)
        {
            string whereStr = "";
            if (sqlStr.Length <= 0) return "";
            string sql = "<sql>" + sqlStr + "</sql>";
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(sql);
                XmlNode xmlChild;
                XmlNode xmlWheres;
                XmlNode xmlWhere;
                string lg = "";
                string field = "";
                string swhere = "";

                xmlChild = xmlDoc.SelectSingleNode("sql");
                for (int j = 0; j < xmlChild.ChildNodes.Count; j++)
                {
                    xmlWheres = xmlChild.ChildNodes[j];
                    whereStr = "";
                    if (xmlWheres.ChildNodes.Count > 0)
                    {
                        for (int i = 0; i < xmlWheres.ChildNodes.Count; i++)
                        {
                            xmlWhere = xmlWheres.ChildNodes[i];
                            string where = xmlWhere.LocalName;
                            switch (where)
                            {
                                case "isNotNull":
                                    lg = xmlWhere.Attributes.GetNamedItem("prepend").InnerXml.Trim();
                                    field = xmlWhere.Attributes.GetNamedItem("property").InnerXml.Trim();
                                    if (xmlWhere.InnerXml.Trim().Contains("null")) swhere = "";
                                    else swhere = lg + " " + xmlWhere.InnerXml.Trim();
                                    if (swhere == "")
                                        whereStr += " ";
                                    else
                                        whereStr += swhere + " ";
                                    break;
                                case "isNull":
                                    lg = xmlWhere.Attributes.GetNamedItem("prepend").InnerXml.Trim();
                                    field = xmlWhere.Attributes.GetNamedItem("property").InnerXml.Trim();
                                    if (xmlWhere.InnerXml.Trim().Contains("null")) swhere = lg + " " + xmlWhere.InnerXml.Trim();
                                    else swhere = "";
                                    if (swhere == "")
                                        whereStr += " ";
                                    else
                                        whereStr += swhere + " ";
                                    break;
                                default:
                                    throw new XmlException("在查询条件中，不支持[" + where + "]XML标记");

                            }
                        }
                        xmlWheres.InnerXml = whereStr.Trim();
                    }
                }

                whereStr = xmlChild.InnerXml.Trim();
                whereStr = whereStr.Replace("<where>", "");
                whereStr = whereStr.Replace("</where>", "");
                whereStr = whereStr.Replace("\r\n", "");
            }
            catch (XmlException ex)
            {
                throw new Exception("Class [ParseDaoConfig'] 解析数据访问对象配置出错" + ex.Message,ex);
            }
            return whereStr;
        }
    }

}
