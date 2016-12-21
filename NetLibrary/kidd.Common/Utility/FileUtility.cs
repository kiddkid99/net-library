using System;
using System.IO;

namespace kidd.Common.Utility
{
    /// <summary>
    /// 檔案處理工具類別，提供處理檔案的靜態方法
    /// </summary>
    public class FileUtility
    {
        /// <summary>
        /// 檔案名稱+自訂字尾
        /// 範例：
        ///     原始檔案路徑：file.txt
        ///     自訂字尾：_small
        ///     回傳結果：file_small.txt
        /// </summary>
        /// <param name="filePath">原始檔案路徑</param>
        /// <param name="suffix">自訂字尾</param>
        /// <returns></returns>
        public static string CreateSuffixFilePath(string filePath, string suffix)
        {
            if (!String.IsNullOrEmpty(filePath))
            {
                string name = Path.GetFileNameWithoutExtension(filePath);
                return filePath.Replace(name, name + suffix);
            }
            else
            {
                return "";
            }

        }

        /// <summary>
        /// 建立檔案資料夾路徑，回傳 folder/{hasDateFolder}/file_name(or rename) 的格式
        /// </summary>
        /// <param name="folder">原始資料夾</param>
        /// <param name="file_name">檔案名稱</param>
        /// <param name="rename">是否重新命名</param>
        /// <param name="hasDateFolder">是否加入時間資料夾</param>
        /// <returns>回傳結果字串</returns>
        public static string CreateFilePath(string folder, string file_name, bool rename, bool hasDateFolder)
        {
            string final_folder = "";
            string final_file_name = "";

            //是否重新命名
            if (rename)
            {
                final_file_name = Guid.NewGuid().ToString("N");
            }
            else
            {
                final_file_name = Path.GetFileNameWithoutExtension(file_name);
            }

            //是否加入時間資料夾
            if (hasDateFolder)
            {
                final_folder = String.Format("{0}/{1}", folder.TrimStart('/').TrimEnd('/'), DateTime.Now.ToString("yyyyMMdd"));
            }
            else
            {
                final_folder = String.Format("{0}", folder.TrimStart('/').TrimEnd('/'));
            }

            string result = String.Format("{0}/{1}{2}", final_folder, final_file_name, Path.GetExtension(file_name));

            return result;

        }

        /// <summary>
        /// 回傳檔案大小格式字串
        /// </summary>
        /// <param name="file_length_byte">檔案大小的位元組</param>
        /// <returns></returns>
        public static string GetFileSizeFormat(long file_length_byte)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (file_length_byte >= 1024 && ++order < sizes.Length)
            {
                file_length_byte = file_length_byte / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            string result = String.Format("{0:0.##} {1}", file_length_byte, sizes[order]);

            return result;
        }

    }
}
