using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace kidd.Common.Security
{
    public static class StringFilter
    {
        /// <summary>
        /// 移除相關HTML tag 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveHtml(string source)
        {
            Regex regex = new Regex("<[^>]*>");
            return regex.Replace(source, "");
        }

        /// <summary>
        /// Remove Sqlij
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveSqlStatement(string source)
        {
            Regex regex = new Regex("(select|drop|;|--|insert|delete|xp_|exec|'|declare|exe|set|cast|varchar|create|table| and)?", RegexOptions.IgnoreCase);
            return regex.Replace(source, "");
        }
    }
}
