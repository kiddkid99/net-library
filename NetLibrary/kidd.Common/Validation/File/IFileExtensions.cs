using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace kidd.Common.Validation.File
{
    /// <summary>
    /// 檔案副檔名驗證介面
    /// </summary>
    public interface IFileExtensions
    {
        /// <summary>
        /// 實作合法的副檔名字串集合
        /// </summary>
        List<string> ValidExtensions { get; }
    }
}
