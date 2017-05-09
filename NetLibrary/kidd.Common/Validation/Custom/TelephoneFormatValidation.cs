using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace kidd.Common.Validation.Custom
{
    /// <summary>
    /// 電話格式驗證
    /// 可接受的格式範例 "02-2550-9196", "(02)25509196", "0225509196", "02-25509196", "(02)2550-9196"
    /// </summary>
    public class TelephoneFormatValidation : ICustomValidation
    {
        public bool Validate(string value)
        {
            return Regex.IsMatch(value, @"^\(?0\d{1,3}\)?[\s\-]?\d{3,4}\-?\d{4}$");
        }
    }
}
