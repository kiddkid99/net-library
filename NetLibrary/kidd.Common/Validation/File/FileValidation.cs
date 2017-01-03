using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace kidd.Common.Validation.File
{
    /// <summary>
    /// 檔案驗證類別
    /// </summary>
    public class FileValidation
    {
        public FileValidation()
        {
        }

        /// <summary>
        /// 副檔名是否符合指定驗證介面的副檔名集合
        /// </summary>
        /// <param name="fileName">檔案名稱（含副檔名）</param>
        /// <param name="extension">副檔名驗證介面</param>
        /// <returns></returns>
        public bool IsExtension(string fileName, IFileExtensions extension)
        {
            var validateExtensions = extension.ValidExtensions;
            string fileExtension = Path.GetExtension(fileName);
            return validateExtensions.Contains(fileExtension);
        }


        /// <summary>
        /// 檔案大小是否超過指定大小
        /// </summary>
        /// <param name="fileSize">檔案大小</param>
        /// <param name="maxSize">指定最大值</param>
        /// <returns></returns>
        public bool ExceedSize(int fileSize, int maxSize)
        {
            return fileSize > maxSize;
        }



    }
}
