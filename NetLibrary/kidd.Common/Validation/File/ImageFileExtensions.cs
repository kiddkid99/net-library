using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kidd.Common.Validation.File
{
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
