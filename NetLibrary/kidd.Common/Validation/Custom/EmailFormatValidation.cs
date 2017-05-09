using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace kidd.Common.Validation.Custom
{
    /// <summary>
    /// 電子郵件驗證
    /// </summary>
    public class EmailFormatValidation : ICustomValidation
    {
        public bool Validate(string value)
        {
            return Regex.IsMatch(value, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }
    }
}
