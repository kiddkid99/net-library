using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kidd.Common.Validation.Format
{
    public interface IFormatValidation
    {
        bool Validate(string value);
    }
}
