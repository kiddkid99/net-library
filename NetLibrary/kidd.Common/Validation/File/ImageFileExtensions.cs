using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kidd.Common.Validation.File
{
    /// <summary>
    /// 圖片類型副檔名驗證
    /// </summary>
    public class ImageFileExtensions : IFileExtensions
    {

        public List<string> ValidExtensions
        {
            get
            {
                var result = new List<string>();
                result.Add(".jpg");
                result.Add(".jpeg");
                result.Add(".gif");
                result.Add(".png");
                result.Add(".bmp");
                return result;

            }
        }
    }
}
