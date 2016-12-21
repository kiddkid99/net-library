using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace kidd.Common.Validation.File
{
    public class FileValidation
    {
        public FileValidation()
        {
        }


        public bool IsExtension(string fileName, IFileExtensions extension)
        {
            var validateExtensions = extension.ValidExtensions;
            string fileExtension = Path.GetExtension(fileName);
            return validateExtensions.Contains(fileExtension);
        }


        public bool ExceedSize(int fileSize, int maxSize)
        {
            return fileSize > maxSize;
        }



    }
}
