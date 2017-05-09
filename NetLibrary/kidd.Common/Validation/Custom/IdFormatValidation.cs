using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kidd.Common.Validation.Custom
{
    /// <summary>
    /// 台灣身分證字號驗證
    /// </summary>
    public class IdFormatValidation : ICustomValidation
    {
        public bool Validate(string value)
        {
            bool result = false;
            if (value.Length == 10)
            {
                value = value.ToUpper();
                if (value[0] >= 0x41 && value[0] <= 0x5A)
                {
                    int[] Location_No = new int[] { 10, 11, 12, 13, 14, 15, 16, 17, 34, 18, 19, 20, 21, 22, 35, 23, 24, 25, 26, 27, 28, 29, 32, 30, 31, 33 };
                    int[] Temp = new int[11];
                    Temp[1] = Location_No[(value[0]) - 65] % 10;
                    int Sum = Temp[0] = Location_No[(value[0]) - 65] / 10;
                    for (int i = 1; i <= 9; i++)
                    {
                        Temp[i + 1] = value[i] - 48;
                        Sum += Temp[i] * (10 - i);
                    }
                    if (((Sum % 10) + Temp[10]) % 10 == 0)
                    {
                        result = true;
                    }
                }
            }

            return result;

        }
    }
}
