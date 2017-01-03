using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kidd.Common.Validation
{
    /// <summary>
    /// 資料範圍驗證類別
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RangeValidation<T> where T : IComparable
    {
        public T Minimum { get; set; }
        public T Maximum { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="single">單一基準值</param>
        public RangeValidation(T single)
        {
            Minimum = single;
            Maximum = single;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        public RangeValidation(T min, T max)
        {
            Minimum = min;
            Maximum = max;
        }

        /// <summary>
        /// 資料是否在指定範圍
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsRange(T value)
        {
            bool result = Minimum.CompareTo(value) <= 0 && Maximum.CompareTo(value) >= 0;
            return result;
        }

        /// <summary>
        /// 資料是否大於基準值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsGreater(T value)
        {
            bool result = Minimum.CompareTo(value) <= 0;
            return result;
        }

        /// <summary>
        /// 資料是否小於基準值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsLess(T value)
        {
            bool result = Maximum.CompareTo(value) >= 0;
            return result;
        }

    }
}
