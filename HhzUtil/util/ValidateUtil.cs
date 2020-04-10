using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HhzUtil.util
{
    using System.Text.RegularExpressions;

    public class ValidateUtil
    {
        /// <summary>
        /// 计算身份证验证码
        /// </summary>
        /// <param name="sfz">15位或18位身份证号</param>
        /// <returns>验证码</returns>
        public static string calculateSfzWm(string sfz)
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
        /// <summary>
        /// 检查身份证的有效性
        /// 规则：
        ///      身份证号  1、和性别一致；  2、和出生日期一致；  3、长度是15或18位；  4、如果长度是18位，就要验证校验码是否正确
        /// </summary>
        /// <param name="sexstr">性别信息</param>
        /// <param name="birthstr">出生信息</param>
        /// <param name="sfzhstr">身份证信息</param>
        /// <returns>true or false</returns>
        public static bool validateSfz(string sexstr, string birthstr, string sfzhstr)
        {
            if (string.IsNullOrEmpty(sfzhstr)) throw new Exception("身份证号为空。");
            if (!string.IsNullOrEmpty(birthstr)) throw new Exception("出生为空。");
            if (!string.IsNullOrEmpty(sexstr)) throw new Exception("性别为空。");
            if (!validateSfz(sfzhstr)) return false;
            string sfzBirth = "";
            string sfzSex = "";
            //if (!((sfzhstr.Length != 15) || (sfzhstr.Length != 18))) return false;
            if (sfzhstr.Length == 15)
            {
                sfzBirth = "19" + sfzhstr.Substring(6, 6);
                sfzSex = sfzhstr.Substring(12);
            }
            if (sfzhstr.Length == 18)
            {
                sfzBirth = sfzhstr.Substring(6, 8);
                sfzSex = sfzhstr.Substring(14, 3);
                //if (sfzhstr.Substring(17) != calculateSfzWm(sfzhstr)) return false;
            }

            if (sfzBirth != birthstr) throw new Exception("身份证号中的出生信息和出生日期不符。");
            int sexint = int.Parse(sfzSex) % 2;
            if ((sexstr == "1") && (sexint == 0)) return false;
            if ((sexstr == "2") && (sexint != 0)) return false;
            return true;
        }
        
        public static bool validateSfz(string sfzhstr)
        {
            bool rtn = validateSfzReg(sfzhstr);
            if (sfzhstr.Length == 18)
            {
                if (sfzhstr.Substring(17) != calculateSfzWm(sfzhstr))
                    return false;
                else
                    return true;
            }
            return rtn;
        }
        private static bool validateSfzReg(string sfzhstr)
        {
            if (string.IsNullOrEmpty(sfzhstr)) return true;
            Regex r = null;
            if (sfzhstr.Length == 15)
                r = new Regex(@"^([1-9]{1})([0-9]{14})");
            else if (sfzhstr.Length == 18)
                r = new Regex(@"^([1-9]{1})([0-9]{16})([0-9]|X{1})");
            else
                return false;
            return r.Match(sfzhstr).Success;
        }
        public static bool validateMobie(string mobie)
        {
            if (string.IsNullOrEmpty(mobie)) return true;
            if (mobie.Length != 11)
                return false;
            else
            {
                Regex r = new Regex(@"^(1)(\d{10}$)");
                return r.Match(mobie).Success;
            }
        }
        public static bool validateDouble(string ddata)
        {
            if (string.IsNullOrEmpty(ddata)) return true;
            if (validateInteger(ddata)) return true;
            Regex r = new Regex(@"^([0-9]*\.[0-9]*)$");
            return r.Match(ddata).Success;
        }
        public static bool validateInteger(string idata)
        {
            if (string.IsNullOrEmpty(idata)) return true;
            Regex r = new Regex(@"^\d*$");
            return r.Match(idata).Success;
        }
        public static bool validateInteger(string idata, int maxlength)
        {
            if (string.IsNullOrEmpty(idata)) return true;
            if (idata.Length > maxlength) return false;
            return validateInteger(idata);
        }
        public static bool validateEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return true;
            Regex r = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return r.Match(email).Success;
        }
        public static bool validateDateRange(DateTime fromdate, DateTime todate)
        {
            if (DateUtil.isNull(fromdate)) return true;
            if (DateUtil.isNull(todate)) return true;
            if (todate < fromdate) return false;
            else return true;
        }
    }
}
