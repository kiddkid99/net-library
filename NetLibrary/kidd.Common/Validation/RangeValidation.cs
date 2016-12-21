using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kidd.Common.Validation
{
    public class RangeValidation<T> where T : IComparable
    {
        public T Minimum { get; set; }
        public T Maximum { get; set; }


        public RangeValidation(T single)
        {
            Minimum = single;
            Maximum = single;
        }

        public RangeValidation(T min, T max)
        {
            Minimum = min;
            Maximum = max;
        }

        public bool IsRange(T value)
        {
            bool result = Minimum.CompareTo(value) <= 0 && Maximum.CompareTo(value) >= 0;
            return result;
        }

        public bool IsGreater(T value)
        {
            bool result = Minimum.CompareTo(value) <= 0;
            return result;
        }

        public bool IsLess(T value)
        {
            bool result = Maximum.CompareTo(value) >= 0;
            return result;
        }

    }
}
