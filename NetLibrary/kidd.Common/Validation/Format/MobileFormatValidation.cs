using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace kidd.Common.Validation.Format
{
    /// <summary>
    /// 行動電話驗證
    /// 可接受的範例：0912345678
    /// </summary>
    public class MobileFormatValidation : IFormatValidation
    {
        bool IFormatValidation.Validate(string value)
        {
            return Regex.IsMatch(value, "^09[0-9]{8}$");
        }
    }
}
