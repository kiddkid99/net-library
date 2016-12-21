using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kidd.Common.Utility
{
    public static class RandomUtility
    {
        /// <summary>
        /// 產生一個 0 ~ number 的隨機數字 
        /// </summary>
        /// <param name="number">最大數字</param>
        /// <returns></returns>
        public static int GetNumber(int maxNumber)
        {
            Random rnd = new Random(Guid.NewGuid().GetHashCode());
            return rnd.Next(0, maxNumber + 1);
        }

        /// <summary>
        /// 產生一個包含數字及字元，長度為 0 ~ number 的隨機字串
        /// </summary>
        /// <param name="maxNumber"></param>
        /// <returns></returns>
        public static string GetString(int maxNumber)
        {
            char[] charSet =   
              {   
                '0','1','2','3','4','5','6','7','8','9',   
                'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'   
              };

            return GetString(maxNumber, charSet);
        }


        /// <summary>
        /// 產生一個指定字元集合，長度為 0 ~ number 的隨機字串
        /// </summary>
        /// <param name="maxNumber"></param>
        /// <param name="charSet"></param>
        /// <returns></returns>
        public static string GetString(int maxNumber, char[] charSet)
        {
            StringBuilder newRandom = new StringBuilder();
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < maxNumber; i++)
            {
                newRandom.Append(charSet[rd.Next(charSet.Length)]);
            }
            return newRandom.ToString();
        }
    }
}
