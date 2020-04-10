using System;
using System.Collections.Generic;
using System.Text;

namespace HhzUtil.util
{
    using System.Text.RegularExpressions;
    public class DateUtil
    {
        /// <summary>
        /// 判断是否是闰年
        /// </summary>
        /// <param name="year"></param>
        /// <returns>true or false</returns>
        public static bool isLeap(int year)
        {
            return DateTime.IsLeapYear(year);
        }
        /// <summary>
        /// 判断是否是闰年
        /// </summary>
        /// <returns>true or false</returns>
        public static bool isLeap()
        {
            int year = DateTime.Now.Year;
            return isLeap(year);
        }
        /// <summary>
        /// 返回当前的周期数
        /// </summary>
        /// <returns></returns>
        public static long getNowTick()
        {
            return DateTime.Now.Ticks;
        }
        /// <summary>
        /// 返回所给日期的周期数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long getTick(DateTime d)
        {
            return d.Ticks;
        }
        /// <summary>
        /// 根据字符串获得日期对象
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static DateTime getDate(String str)
        {
            return DateTime.Parse(str);
        }
        /// <summary>
        /// 根据字符串获得日期对象
        /// </summary>
        /// <param name="ticks">周期数</param>
        /// <returns></returns>
        public static DateTime getDate(long ticks)
        {
            DateTime baseDate = DateTime.MinValue;
            return baseDate.AddTicks(ticks);
        }
        public static bool validateString(string currDate)
        {
            if (string.IsNullOrEmpty(currDate)) throw new Exception("日期字符串为空, 错误!");
            try
            {
                DateTime.Parse(currDate);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
           
        }
        /// <summary>
        /// 验证日期字符串格式
        /// </summary>
        /// <param name="currDate">日期字符串</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static bool validateDateFormat(string currDate, string format)
	    {
            if (string.IsNullOrEmpty(currDate)) throw new Exception("日期字符串为空, 错误!");
            if (string.IsNullOrEmpty(format)) throw new Exception("格式为空, 错误!");
            Regex reg = null;
		    if(format == "yyyyMMdd")
            {
                reg =  new Regex("^(1|2{1})([0-9]{3})([0-1]{1})([0-9]{1})([0-3]{1})([0-9]{1})");
                return reg.Match(currDate).Success;
            }
            else if (format == "yyyy-MM-dd")
            {
                reg = new Regex("^(1|2{1})([0-9]{3})-([0-1]{1})([0-9]{1})-([0-3]{1})([0-9]{1})");
                return reg.Match(currDate).Success;
            }
            else if (format == "yyyy年MM月dd日")
            {
                reg = new Regex("^(1|2{1})([0-9]{3})年([0-1]{1})([0-9]{1})月([0-3]{1})([0-9]{1})日");
                return reg.Match(currDate).Success;
            }
            else
                return false;
	    }
        /// <summary>
        /// 根据格式获得日期
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string convertDateToString(string format)
        {
            DateTime baseDate = DateTime.Now;
            return convertDateToString(baseDate, format);
        }
        /// <summary>
        /// 根据格式获得转换所给日期
        /// </summary>
        /// <param name="baseDate"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string convertDateToString(DateTime baseDate, string format)
        {
            if (string.IsNullOrEmpty(format)) return "";
            if (baseDate == null) return "";
            return baseDate.ToString(format);
        }
        /// <summary>
        /// 根据格式获得转换所给周期数
        /// </summary>
        /// <param name="ticks">周期数</param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string convertDateToString(long ticks, string format)
        {
            if (string.IsNullOrEmpty(format)) return "";
            DateTime baseDate = DateTime.MinValue;
            baseDate.AddTicks(ticks);
            return convertDateToString(baseDate, format);
        }
        /// <summary>
        /// 判定日期是否为NULL
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool isNull(DateTime p)
        {
            if (DateTime.Compare(p, DateTime.MinValue) <= 0) return true;
            else return true;
        }
        /// <summary>
        /// 获得年龄
        /// </summary>
        /// <param name="birth"></param>
        /// <returns></returns>
        public static int getAge(DateTime birth) 
        {
            if(DateTime.Compare(birth,DateTime.MinValue)<=0) return 0;
		    int age = 0;
            DateTime now = DateTime.Now;
            age = between(now, birth, "year");
            int monthage = between(now, birth, "month");
            if (monthage < 0)
			    age -= 1;
            int dayage = between(now, birth, "day");
            if (dayage < 0)
			    age -= 1;
		    if (age < 0)
			    age = 0;
		    return age;
	    }
        /// <summary>
        /// 返回两个日期相差天数
        /// </summary>
        /// <param name="end"></param>
        /// <param name="begin"></param>
        /// <returns></returns>
        public static int betweenDays(DateTime end, DateTime begin)
        {
            return between(end, begin, "day");
        }
        /// <summary>
        /// 返回两个日期相差小时
        /// </summary>
        /// <param name="end"></param>
        /// <param name="begin"></param>
        /// <returns></returns>
        public static int betweenHours(DateTime end, DateTime begin)
        {
            return between(end, begin, "hour");
        }
        /// <summary>
        /// 返回两个日期相差分钟
        /// </summary>
        /// <param name="end"></param>
        /// <param name="begin"></param>
        /// <returns></returns>
        public static int betweenMinutes(DateTime end, DateTime begin)
        {
            return between(end, begin, "minute");
        }
        /// <summary>
        /// 根据标志返回两个日期相差值
        /// </summary>
        /// <param name="end"></param>
        /// <param name="begin"></param>
        /// <param name="flag">标志</param>
        /// <returns></returns>
        public static int between(DateTime end, DateTime begin,string flag)
        {
            int rtnnum = 0;
            switch(flag.ToLower())
            {
                case "day":
                    rtnnum = (end - begin).Days;
                    break;
                case "hour":
                    rtnnum = (end - begin).Hours;
                    break;
                case "minute":
                    rtnnum = (end - begin).Minutes;
                    break;
                case "year":
                    rtnnum = end.Year - begin.Year;
                    break;
                case "month":
                    rtnnum = (end.Year - begin.Year) * 12 + (end.Month - begin.Month);
                    break;
            }
            return rtnnum;
        }
    }
}
