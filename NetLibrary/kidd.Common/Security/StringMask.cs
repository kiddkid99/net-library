using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kidd.Common.Security
{
    /// <summary>
    /// 字串遮罩
    /// </summary>
    public static class StringMask
    {
        /// <summary>
        /// 將傳入的字串以自訂字元遮罩
        /// </summary>
        /// <param name="source">x來源字串</param>
        /// <param name="maskRule">遮罩規則，使用1 或 0 表示，0 表示遮罩該索引位置字元</param>
        /// <param name="maskChar">自訂遮罩字元</param>
        /// <param name="ignore">忽略特定字元</param>
        /// <returns></returns>
        public static string Mask(string source, string maskRule, char maskChar, char[] ignore)
        {
            String result = "";

            for (int i = 0; i < source.Length; i++)
            {
                int maskIndex = i % maskRule.Length;

                //目前遮罩字元
                char current_pattern = maskRule[maskIndex];

                //目前字元
                char current_char = source[i];

                //判斷是否遮罩
                if (current_pattern == '1' || (ignore != null && ignore.Contains(current_char)))
                {
                    result += current_char;
                }
                else
                {
                    result += maskChar;
                }

            }

            return result;
        }

        /// <summary>
        /// 將傳入的字串以星號遮罩
        /// </summary>
        /// <param name="source">來源字串</param>
        /// <param name="maskRule">遮罩規則，使用1 或 0 表示，0 表示遮罩該索引位置字元</param>
        /// <param name="ignore">忽略特定字元</param>
        /// <returns></returns>
        public static string Mask(string source, string maskRule, char[] ignore)
        {
            return Mask(source, maskRule, '*', ignore);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source">來源字串</param>
        /// <param name="maskRule">遮罩規則，使用1 或 0 表示，0 表示遮罩該索引位置字元</param>
        /// <returns></returns>
        public static string Mask(string source, string maskRule)
        {
            return Mask(source, maskRule, null);
        }
    }
}
