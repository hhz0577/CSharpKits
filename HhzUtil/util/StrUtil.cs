using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.util
{
    using System.Security.Cryptography;
    using System.IO;
    using System.Data;
    using System.Text.RegularExpressions;

    public class StrUtil
    {
        #region 工具函数
        /// <summary>
        /// GUID
        /// </summary>
        /// <returns></returns>
        public static string getGuid()
        {
            return Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 获得当前工作目录
        /// </summary>
        /// <returns></returns>
        public static string getWorkPath()
        {
            return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }
        /// <summary>
        /// 将字符串转换为字节数组
        /// </summary>
        /// <param name="v"></param>
        /// <param name="charset">字符集名称</param>
        /// <returns></returns>
        public static byte[] toBytes(string v, string charset)
        {
            if(IsEmpty(v)) return null;
            if (IsEmpty(charset)) charset = "default";
            string flag = charset.ToLower();
            byte[] b;
            switch (flag)
            {
                case "ascii":
                    b = Encoding.ASCII.GetBytes(v);
                    break;
                case "utf-8":
                    b = Encoding.UTF8.GetBytes(v);
                    break;
                case "unicode":
                    b = Encoding.Unicode.GetBytes(v);
                    break;
                default:
                    b = Encoding.Default.GetBytes(v);
                    break;
            }
            return b;
        }
        /// <summary>
        /// 将字节数组转换为字符串
        /// </summary>
        /// <param name="b"></param>
        /// <param name="charset">字符集名称</param>
        /// <returns></returns>
        public static string bytesToString(byte[] b, string charset)
        {
            if (b == null) return "";
            if (IsEmpty(charset)) charset = "default";
            string flag = charset.ToLower();
            string result = "";
            switch (flag)
            {
                case "ascii":
                    result = Encoding.ASCII.GetString(b);
                    break;
                case "utf-8":
                    result = Encoding.UTF8.GetString(b);
                    break;
                case "unicode":
                    result = Encoding.Unicode.GetString(b);
                    break;
                default:
                    result = Encoding.Default.GetString(b);
                    break;
            }
            return result;
        }
        /// <summary>
        /// 将15位身份证号转换为18位
        /// </summary>
        /// <param name="sfzh">15位身份证号</param>
        /// <returns>18位身份证</returns>
        public static string Sfzh15To18(string sfzh)
        {
            if (sfzh.Length != 15) return sfzh;
            string s = sfzh;
            string wm = calculateSfzWm(s);
            s = s.Substring(0, 6) + "19" + s.Substring(6) + wm;
            return s;
        }
        /// <summary>
        ///判断对象是否为空
        /// </summary>
        /// <param name="param">要判断的对象</param>
        /// <returns>空（true） 反之（false）</returns>
        public static bool IsEmpty(string param)
        {
            bool f = true;
            if (param == null) return true;
            if (param.Length > 0) f = false;
            return f;
        }
        /// <summary>
        /// 大写首字母
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string upperFirst(string str)
        {
            if (IsEmpty(str)) return "";
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }
        /// <summary>
        /// 小写首字母
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string lowerFirst(string str)
        {
            if (IsEmpty(str)) return "";
            return str.Substring(0, 1).ToLower() + str.Substring(1);
        }
        /// <summary>
        /// 去掉指定前缀
        /// </summary>
        /// <param name="str"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string removePrefix(string str, string prefix)
        {
            if (str != null && str.StartsWith(prefix))
            {
                return str.Substring(prefix.Length);
            }
            return str;
        }
        /// <summary>
        /// 去掉指定后缀
        /// </summary>
        /// <param name="str"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string removeSuffix(string str, string suffix)
        {
            if (str != null && str.EndsWith(suffix))
            {
                return str.Substring(0, str.Length - suffix.Length);
            }
            return str;
        }
        /// <summary>
        /// 根据身份证号获得性别、出生和户籍代码
        /// </summary>
        /// <param name="sfzhstr"></param>
        /// <param name="sex">性别</param>
        /// <param name="birth">出生</param>
        /// <param name="hjcode">户籍代码</param>
        public static void processSfzInfo(string sfzhstr, ref int sex, ref string birth, ref string hjcode)
        {
            if (string.IsNullOrEmpty(sfzhstr)) throw new Exception("身份证号为空。");
            string sfzBirth = "";
            string sfzSex = "";

            if (sfzhstr.Length == 15)
            {
                sfzhstr = Sfzh15To18(sfzhstr);
            }
            if (sfzhstr.Length != 18) throw new Exception("身份证号长度不足。");
            if (sfzhstr.Length == 18)
            {
                sfzBirth = sfzhstr.Substring(6, 4) + "-" + sfzhstr.Substring(10, 2) + "-" + sfzhstr.Substring(12, 2);
                if (!DateUtil.validateString(sfzBirth))
                    throw new Exception("身份证号中出生日期不是一个有效的日期");
                sfzSex = sfzhstr.Substring(14, 3);
                if (sfzhstr.Substring(17) != ValidateUtil.calculateSfzWm(sfzhstr)) throw new Exception("身份证号的最后一位不对。");
            }
            int sexint = int.Parse(sfzSex) % 2;
            if (sexint == 0) sex = 2;
            else sex = 1;
            hjcode = sfzhstr.Substring(0, 6);
            birth = sfzBirth;
        }
        #endregion

        #region 私有函数
        /// <summary>
        /// 计算身份证验证码
        /// </summary>
        /// <param name="sfz">15位或18位身份证号</param>
        /// <returns>验证码</returns>
        private static string calculateSfzWm(string sfz)
        {
            bool legalitySfzLength = false;
            if (sfz.Length == 18)
                legalitySfzLength = true;
            if (sfz.Length == 15)
            {
                sfz = sfz.Substring(0, 6) + "19" + sfz.Substring(6);
                legalitySfzLength = true;
            }
            if (!legalitySfzLength) return "";
            int s = 0;
            int j = 0;
            int[] w = { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1 };
            string[] a = { "1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2" };
            for (int i = 0; i < 17; i++)
            {
                j = int.Parse(sfz.Substring(i, 1)) * w[i];
                s = s + j;
            }
            j = s % 11;
            return a[j];
        }
        #endregion

        #region MD5 加密
        public static string encryptForMD5(string s)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(s));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        #endregion
        #region 验证MD5 加密结果
        public static bool vertifyMd5Hash(string s, string md5hash)
        {
            string hashOfInput = encryptForMD5(s);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, md5hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
        #region RSA 加密/解密
        /// <summary>
        /// 使用 RSA 算法对数据进行加密 
        /// </summary>
        /// <param name="s">要加密的数据</param>
        /// <returns>加密后的数据（并用了BASE64编码）</returns>
        public static string encryptForRSA(string s)
        {
            string KeyContainerName = "hhz";
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = KeyContainerName;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cspParams);
            Encoding u8 = Encoding.UTF8;
            byte[] result;
            byte[] data = u8.GetBytes(s);
            result = RSA.Encrypt(data, false);
            return Convert.ToBase64String(result);
        }
        /// <summary>
        /// 使用 RSA 算法对数据进行解密
        /// </summary>
        /// <param name="s">加密后的数据（BASE64编码）</param>
        /// <returns>解密后的数据</returns>
        public static string decryptFormRSA(string s)
        {
            string KeyContainerName = "hhz";
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = KeyContainerName;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cspParams);
            Encoding u8 = Encoding.UTF8;
            byte[] result;
            byte[] data = Convert.FromBase64String(s);
            result = RSA.Decrypt(data, false);
            return u8.GetString(result);
        }
        #endregion

        #region BASE64 编码/解码 （FOR UFT-8）
        /// <summary>
        /// 对信息进行BASE64编码
        /// </summary>
        /// <param name="s">要编码的数据</param>
        /// <returns>编码后的数据</returns>
        public static string EncodeBase64(string s)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
        }
        /// <summary>
        /// 将BASE64编码的数据还原
        /// </summary>
        /// <param name="s">编码的数据</param>
        /// <returns>还原后的数据</returns>
        public static string DecodeBase64(string s)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }
        #endregion
        #region BASE64 编码/解码 （FOR ASCII）
        /// <summary>
        /// 对信息进行BASE64编码
        /// </summary>
        /// <param name="s">要编码的数据</param>
        /// <returns>编码后的数据</returns>
        public static string EncodeBase64Ascii(string s)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(s));
        }
        /// <summary>
        /// 将BASE64编码的数据还原
        /// </summary>
        /// <param name="s">编码的数据</param>
        /// <returns>还原后的数据</returns>
        public static string DecodeBase64Ascii(string s)
        {
            return Encoding.ASCII.GetString(Convert.FromBase64String(s));
        }
        #endregion
        #region BASE64 编码/解码 （FOR Default）DELPHI可通用
        /// <summary>
        /// 对信息进行BASE64编码
        /// </summary>
        /// <param name="s">要编码的数据</param>
        /// <returns>编码后的数据</returns>
        public static string EncodeBase64Default(string s)
        {
            return Convert.ToBase64String(Encoding.Default.GetBytes(s));
        }
        /// <summary>
        /// 将BASE64编码的数据还原
        /// </summary>
        /// <param name="s">编码的数据</param>
        /// <returns>还原后的数据</returns>
        public static string DecodeBase64Default(string s)
        {
            return Encoding.Default.GetString(Convert.FromBase64String(s));
        }
        #endregion

        #region SHA1 加密
        /// <summary>
        /// 使用SHA1加密字符串。
        /// </summary>
        /// <param name="inputString">输入字符串。</param>
        /// <returns>加密后的字符串。（40个字符）</returns>
        public static string StringToSHA1Hash(string inputString)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] encryptedBytes = sha1.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < encryptedBytes.Length; i++)
            {
                sb.AppendFormat("{0:x2}", encryptedBytes[i]);
            }
            return sb.ToString();
        }
        #endregion
        #region DES 加密/解密
        private static byte[] key = ASCIIEncoding.ASCII.GetBytes("caikelun");
        private static byte[] iv = ASCIIEncoding.ASCII.GetBytes("caikelun");
        /// <summary>
        /// DES加密。
        /// 输出BASE64编码后的字符串
        /// </summary>
        /// <param name="inputString">输入字符串。</param>
        /// <returns>加密后的字符串(BASE64编码)。</returns>
        public static string DESEncrypt(string inputString)
        {
            MemoryStream ms = null;
            CryptoStream cs = null;
            StreamWriter sw = null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateEncryptor(key, iv), CryptoStreamMode.Write);
                sw = new StreamWriter(cs);
                sw.Write(inputString);
                sw.Flush();
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }
        }
        public static string DESEncrypt(string inputString,string keystring)
        {
            byte[] keybyte = ASCIIEncoding.ASCII.GetBytes(keystring);
            byte[] ivbyte = ASCIIEncoding.ASCII.GetBytes(keystring);
            MemoryStream ms = null;
            CryptoStream cs = null;
            StreamWriter sw = null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateEncryptor(keybyte, ivbyte), CryptoStreamMode.Write);
                sw = new StreamWriter(cs);
                sw.Write(inputString);
                sw.Flush();
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
            finally
            {
                if (sw != null) sw.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }
        }

        /// <summary>
        /// 先BASE64解码
        /// DES解密。
        /// </summary>
        /// <param name="inputString">输入字符串。</param>
        /// <returns>解密后的字符串。</returns>
        public static string DESDecrypt(string inputString)
        {
            MemoryStream ms = null;
            CryptoStream cs = null;
            StreamReader sr = null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                ms = new MemoryStream(Convert.FromBase64String(inputString));
                cs = new CryptoStream(ms, des.CreateDecryptor(key, iv), CryptoStreamMode.Read);
                sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            finally
            {
                if (sr != null) sr.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }
        }
        public static string DESDecrypt(string inputString,string keystring)
        {
            byte[] keybyte = ASCIIEncoding.ASCII.GetBytes(keystring);
            byte[] ivbyte = ASCIIEncoding.ASCII.GetBytes(keystring);
            MemoryStream ms = null;
            CryptoStream cs = null;
            StreamReader sr = null;

            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                ms = new MemoryStream(Convert.FromBase64String(inputString));
                cs = new CryptoStream(ms, des.CreateDecryptor(keybyte, ivbyte), CryptoStreamMode.Read);
                sr = new StreamReader(cs);
                return sr.ReadToEnd();
            }
            finally
            {
                if (sr != null) sr.Close();
                if (cs != null) cs.Close();
                if (ms != null) ms.Close();
            }
        }
        #endregion

        #region DELPHI ClientDataSet Xml格式
        /// <summary>
        /// 将Net的数据集转化为DELPHI的ClientDataSet格式
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <param name="TableName">表名</param>
        /// <returns>DELPHI的ClientDataSet格式的XML字符串</returns>
        public static string ToClientDataSet(DataSet ds, string TableName)
        {
            int FieldCount = ds.Tables[TableName].Columns.Count;
            string result = "<?xml version=\"1.0\" standalone=\"yes\"?>";
            result += "<DATAPACKET Version=\"2.0\">";
            result += "<METADATA><FIELDS>";
            for (int i = 0; i < FieldCount; i++)
            {
                DataColumn dc = ds.Tables[TableName].Columns[i];
                string FieldType = dc.DataType.Name.ToLower();
                // integer
                if (FieldType.ToLower() == "decimal")
                    FieldType = "i4";
                if (FieldType.ToLower() == "int32")
                    FieldType = "i4";
                //double
                if (FieldType.ToLower() == "double")
                    FieldType = "r8";
                // int64
                if (FieldType.ToLower() == "int64")
                    FieldType = "i8";
                // float
                if (FieldType.ToLower() == "single")
                    FieldType = "r8";
                // memo
                if (FieldType.ToLower() == "long")
                    FieldType = "bin.hex";
                if (FieldType.ToLower() == "string" && dc.MaxLength > 255)
                    FieldType = "bin.hex";
                // bytes
                if (FieldType.ToLower() == "byte[]")
                    FieldType = "bin.hex";
                result += string.Format("<FIELD attrname=\"{0}\" fieldtype=\"{1}\" ", dc.ColumnName, FieldType);
                if (FieldType == "bin.hex" && dc.DataType.Name.ToLower() == "string" || dc.DataType.Name.ToLower() == "long")
                    result += "SUBTYPE=\"Text\" ";
                else if (FieldType == "string")
                    result += string.Format("WIDTH=\"{0}\" ", 4000);
                else if (FieldType == "sqldatetime")
                    result += "SUBTYPE=\"Formatted\"";
                else if (FieldType == "bin.hex" && dc.DataType.Name.ToLower() != "string" || dc.DataType.Name.ToLower() == "byte[]")
                    result += "SUBTYPE=\"Binary\"";
                result += "/>";
            }
            result += "</FIELDS><PARAMS/></METADATA>";
            result += "<ROWDATA>";
            int RowCount = ds.Tables[TableName].Rows.Count;
            for (int i = 0; i < RowCount; i++)
            {
                result += "<ROW ";
                for (int j = 0; j < FieldCount; j++)
                {
                    DataTable dt = ds.Tables[TableName];
                    string value = "";
                    if (ds.Tables[TableName].Columns[j].DataType.Name.ToLower() == "datetime")
                    {
                        if (dt.Rows[i][j] == DBNull.Value)
                            value = "";
                        else
                            value = ((DateTime)dt.Rows[i][j]).ToString("yyyyMMddTHH:mm:ss");
                    }
                    else if (ds.Tables[TableName].Columns[j].DataType.Name.ToLower() == "byte[]")
                    {
                        if (dt.Rows[i][j] == DBNull.Value)
                            value = "";
                        else
                        {
                            byte[] b = new byte[0];
                            b = (Byte[])dt.Rows[i][j];
                            value = Convert.ToBase64String(b);
                        }
                    }
                    else
                    {
                        value = dt.Rows[i].ItemArray[j].ToString();
                    }
                    value = value.Replace("\"", "&quot;");
                    result += string.Format("{0}=\"{1}\" ",
                        dt.Columns[j].ColumnName,
                        value);
                }
                result += "/>";
            }
            result += "</ROWDATA></DATAPACKET>";
            return result;
        }
        #endregion

        #region 将消息写入日志文件 WirteLog(string str)
        /// <summary>
        /// LanMsg写日志函数
        /// </summary>
        /// <param name="str">要写入的日志内容字符串</param>
        public static void WirteLog(string str,string subpath)
        {
            if(string.IsNullOrEmpty(subpath))
            {
                subpath = "log";
            }
            if (!Directory.Exists(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + subpath))
                Directory.CreateDirectory(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + subpath);
            FileStream fs = new FileStream(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + subpath + "/" + DateTime.Now.ToString("yyyyMMdd") + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter _streamWriter = new StreamWriter(fs, Encoding.UTF8);
            _streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            _streamWriter.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "] ... " + str + "\r\n");
            _streamWriter.Flush();
            _streamWriter.Close();
            fs.Close();
        }
        public static void WirteLog(string str, string subpath,Exception e)
        {
            if (string.IsNullOrEmpty(subpath))
            {
                subpath = "log";
            }
            string msg = str + "失败,原因：" + e.Message + "\r\n" + e.StackTrace;
            WirteLog(msg, subpath);
        }
        #endregion
    }
}
