using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace kidd.Common.Utility
{
    public static class ConvertUtility
    {
        /// <summary>
        /// 將字串轉換成指定泛型的資料型態，若轉換失敗會產生例外錯誤
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T ConvertType<T>(string input)
        {
            return ConvertType<T>(input, false);
        }

        /// <summary>
        /// 將字串轉換成指定泛型的資料型態
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="input">輸入資料</param>
        /// <param name="useDefault">轉換失敗是否使用資料型態預設值</param>
        /// <returns></returns>
        public static T ConvertType<T>(string input, bool useDefault)
        {
            try
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    return (T)converter.ConvertFromString(input);
                }

                return default(T);
            }
            catch (NotSupportedException)
            {
                return default(T);
            }
            catch (Exception ex)
            {
                if (useDefault)
                {
                    return default(T);
                }
                else
                {
                    throw ex;
                }
            }

        }

        /// <summary>
        /// 判斷字串是否可轉型成指定泛型資料型態
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsType<T>(string input)
        {
            try
            {
                var type = ConvertType<T>(input);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
