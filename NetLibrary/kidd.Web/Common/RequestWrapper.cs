using kidd.Common.Utility;
using kidd.Common.Validation;
using kidd.Common.Validation.File;
using kidd.Common.Validation.Format;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace kidd.Web.Common
{
    public class RequestWrapper
    {
        private HttpRequestBase request;

        private List<String> errorMessageList = new List<string>();

        /// <summary>
        /// 取得及設定錯誤訊息集合
        /// </summary>
        public List<String> ErrorMessageList
        {
            get { return this.errorMessageList; }
            set { this.errorMessageList = value; }
        }

        public RequestWrapper(HttpRequest request)
        {
            HttpRequestWrapper wrapper = new HttpRequestWrapper(request);
            this.request = wrapper;
        }

        public RequestWrapper(HttpRequestBase request)
        {
            this.request = request;
        }

        public T GetByForm<T>(string name)
        {
            return GetByForm<T>(name, false);
        }

        public T GetByForm<T>(string name, bool useDefault)
        {
            return ConvertUtility.ConvertType<T>(this.request.Form[name], useDefault);
        }

        public T GetByQueryString<T>(string name)
        {
            return GetByQueryString<T>(name, false);
        }

        public T GetByQueryString<T>(string name, bool useDefault)
        {
            return ConvertUtility.ConvertType<T>(this.request.QueryString[name], useDefault);
        }

        /// <summary>
        /// 驗證必填欄位，使用預設錯誤訊息格式
        /// </summary>
        /// <param name="value">資料</param>
        /// <param name="field">顯示欄位</param>
        public void RequiredValidate(string value, string field)
        {
            RequiredValidate(value, field, "請輸入{0}");
        }

        /// <summary>
        /// 驗證必填欄位
        /// </summary>
        /// <param name="value">資料</param>
        /// <param name="field">顯示欄位</param>
        /// <param name="messageFormat">錯誤訊息字串格式, {0} 為顯示欄位</param>
        public void RequiredValidate(string value, string field, string messageFormat)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                this.errorMessageList.Add(String.Format(messageFormat, field));
            }
        }


        /// <summary>
        /// 驗證欄位資料型態，使用預設錯誤訊息格式
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="ignoreEmpty">是否忽略驗證空白資料</param>
        /// <param name="field">顯示欄位</param>
        public void DataTypeValidate<T>(string value, bool ignoreEmpty, string field)
        {
            DataTypeValidate<T>(value, ignoreEmpty, field, "{0}格式不正確");
        }

        /// <summary>
        /// 驗證欄位資料型態
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="ignoreEmpty">是否忽略驗證空白資料</param>
        /// <param name="field">顯示欄位</param>
        /// <param name="messageFormat">錯誤訊息字串格式, {0} 為顯示欄位</param>
        public void DataTypeValidate<T>(string value, bool ignoreEmpty, string field, string messageFormat)
        {
            if (!ignoreEmpty || !String.IsNullOrWhiteSpace(value))
            {
                if (ConvertUtility.IsType<T>(value) == false)
                {
                    this.errorMessageList.Add(String.Format(messageFormat, field));
                }
            }
        }

        /// <summary>
        /// 依照資料型態，驗證資料值等於比較值，使用預設錯誤訊息
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="single">比較值</param>
        /// <param name="ignoreEmpty">是否忽略空白資料</param>
        /// <param name="field">顯示欄位</param>
        public void EqualValidate<T>(string value, T single, bool ignoreEmpty, string field) where T : IComparable
        {
            RangeValidate(value, single, single, ignoreEmpty, field, "{0}的值必須等於{1}");
        }

        /// <summary>
        /// 依照資料型態，驗證資料值等於比較值
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="single">比較值</param>
        /// <param name="ignoreEmpty">是否忽略空白資料</param>
        /// <param name="field">顯示欄位</param>
        /// <param name="messageFormat">錯誤訊息格式, , {0} 為顯示欄位, {1} 為比較值</param>
        public void EqualValidate<T>(string value, T single, bool ignoreEmpty, string field, string messageFormat) where T : IComparable
        {
            RangeValidate(value, single, single, ignoreEmpty, field, messageFormat);
        }

        /// <summary>
        /// 依照資料型態，驗證資料值大於比較值，使用預設錯誤訊息
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="single">比較值</param>
        /// <param name="ignoreEmpty">是否忽略空白資料</param>
        /// <param name="field">顯示欄位</param>
        public void GreaterValidate<T>(string value, T single, bool ignoreEmpty, string field) where T : IComparable
        {
            GreaterValidate<T>(value, single, ignoreEmpty, field, "{0}的值必須大於{1}");
        }

        /// <summary>
        /// 依照資料型態，驗證資料值大於比較值
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="single">比較值</param>
        /// <param name="ignoreEmpty">是否忽略空白資料</param>
        /// <param name="field">顯示欄位</param>
        /// <param name="messageFormat">錯誤訊息格式, , {0} 為顯示欄位, {1} 為比較值</param>
        public void GreaterValidate<T>(string value, T single, bool ignoreEmpty, string field, string messageFormat) where T : IComparable
        {
            if (!ignoreEmpty || !String.IsNullOrWhiteSpace(value))
            {
                T convertValue = ConvertUtility.ConvertType<T>(value, true);
                RangeValidation<T> range = new RangeValidation<T>(single);
                if (range.IsGreater(convertValue) == false)
                {
                    this.errorMessageList.Add(String.Format(messageFormat, field, single));

                }
            }
        }

        /// <summary>
        /// 依照資料型態，驗證資料值小於比較值，使用預設錯誤訊息
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="single">比較值</param>
        /// <param name="ignoreEmpty">是否忽略空白資料</param>
        /// <param name="field">顯示欄位</param>
        public void LessValidate<T>(string value, T single, bool ignoreEmpty, string field) where T : IComparable
        {
            LessValidate<T>(value, single, ignoreEmpty, field, "{0}的值必須小於{1}");
        }


        /// <summary>
        /// 依照資料型態，驗證資料值小於比較值
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="single">比較值</param>
        /// <param name="ignoreEmpty">是否忽略空白資料</param>
        /// <param name="field">顯示欄位</param>
        /// <param name="messageFormat">錯誤訊息格式, , {0} 為顯示欄位, {1} 為比較值</param>
        public void LessValidate<T>(string value, T single, bool ignoreEmpty, string field, string messageFormat) where T : IComparable
        {
            if (!ignoreEmpty || !String.IsNullOrWhiteSpace(value))
            {
                T convertValue = ConvertUtility.ConvertType<T>(value, true);
                RangeValidation<T> range = new RangeValidation<T>(single);
                if (range.IsLess(convertValue) == false)
                {
                    this.errorMessageList.Add(String.Format(messageFormat, field, single));

                }
            }
        }

        /// <summary>
        /// 依照資料型態，驗證資料值範圍，使用預設錯誤訊息
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="ignoreEmpty">是否忽略空白資料</param>
        /// <param name="field">顯示欄位</param>
        public void RangeValidate<T>(string value, T min, T max, bool ignoreEmpty, string field) where T : IComparable
        {
            RangeValidate<T>(value, min, max, ignoreEmpty, field, "{0}的值必須介於{1}~{2}");
        }


        /// <summary>
        /// 依照資料型態，驗證資料值範圍
        /// </summary>
        /// <typeparam name="T">資料型態</typeparam>
        /// <param name="value">資料</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <param name="ignoreEmpty">是否忽略空白資料</param>
        /// <param name="field">顯示欄位</param>
        /// <param name="messageFormat">錯誤訊息字串格式, {0} 為顯示欄位, {1} 為最小值, {2} 為最大值</param>
        public void RangeValidate<T>(string value, T min, T max, bool ignoreEmpty, string field, string messageFormat) where T : IComparable
        {
            if (!ignoreEmpty || !String.IsNullOrWhiteSpace(value))
            {
                T convertValue = ConvertUtility.ConvertType<T>(value, true);
                RangeValidation<T> range = new RangeValidation<T>(min, max);
                if (range.IsRange(convertValue) == false)
                {
                    if (min.CompareTo(max) == 0)
                    {
                        this.errorMessageList.Add(String.Format(messageFormat, field, min));
                    }
                    else
                    {
                        this.errorMessageList.Add(String.Format(messageFormat, field, min, max));
                    }

                }
            }
        }

        /// <summary>
        /// 驗證資料格式，使用預設錯誤訊息
        /// </summary>
        /// <param name="format">驗證格式介面</param>
        /// <param name="value">資料</param>
        /// <param name="ignoreEmpty">是否忽略驗證空白資料</param>
        /// <param name="field">顯示欄位</param>
        public void FormatValidate(IFormatValidation format, string value, bool ignoreEmpty, string field)
        {
            FormatValidate(format, value, ignoreEmpty, field, "{0}格式不正確");
        }


        /// <summary>
        /// 驗證資料格式
        /// </summary>
        /// <param name="format">驗證格式介面</param>
        /// <param name="value">資料</param>
        /// <param name="ignoreEmpty">是否忽略驗證空白資料</param>
        /// <param name="field">顯示欄位</param>
        /// <param name="messageFormat">錯誤訊息字串格式, {0} 為顯示欄位</param>
        public void FormatValidate(IFormatValidation format, string value, bool ignoreEmpty, string field, string messageFormat)
        {
            if (!ignoreEmpty || !String.IsNullOrWhiteSpace(value))
            {
                if (!format.Validate(value))
                {
                    this.errorMessageList.Add(String.Format(messageFormat, field));
                }
            }
        }


        public void FileExtensionValidate(IFileExtensions extensions, string value, bool ignoreEmpty, string field)
        {
            FileExtensionValidate(extensions, value, ignoreEmpty, field, "{0}不支援 {1} 檔案格式");
        }

        public void FileExtensionValidate(IFileExtensions extensions, string value, bool ignoreEmpty, string field, string messageFormat)
        {
            FileValidation validate = new FileValidation();
            if (!ignoreEmpty || !String.IsNullOrWhiteSpace(value))
            {
                if (!validate.IsExtension(value, extensions))
                {
                    string extension = Path.GetExtension(value).ToLower();
                    this.errorMessageList.Add(String.Format(messageFormat, field, extension));
                }
            }
        }

        public void FileSizeValidate(int fileSize, int maxSize, string field)
        {
            FileSizeValidate(fileSize, maxSize, field, "{0}超過檔案限制 {1}");
        }

        public void FileSizeValidate(int fileSize, int maxSize, string field, string messageFormat)
        {
            FileValidation validate = new FileValidation();
            if (validate.ExceedSize(fileSize, maxSize))
            {
                this.errorMessageList.Add(String.Format(messageFormat, field, FileUtility.GetFileSizeFormat(maxSize)));
            }    
        }



        /// <summary>
        /// 取得應用程式的絕對路徑
        /// </summary>
        /// <returns></returns>
        public String GetAbsoluteApplication()
        {
            return String.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, VirtualPathUtility.ToAbsolute("~"));
        }

        /// <summary>
        /// 取得 Request 的呼叫端 IP 資訊
        /// </summary>
        /// <returns></returns>
        public String GetClientIP()
        {
            //程式碼調整，直接判斷 HTTP_X_FORWARDED_FOR 是否有值
            if (request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null)
            {
                return request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            else
            {
                //只回傳第一個IP位置
                return request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().Split(',')[0].Trim();
            }
        }

        /// <summary>
        /// 取得錯誤訊息字串
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetErrorMessage(ErrorOuputType type)
        {
            //定義換行字元
            string saperator = "";

            switch (type)
            {
                case ErrorOuputType.Javascript:
                    saperator = "\\n";
                    break;
                case ErrorOuputType.Html:
                    saperator = "<br/>";
                    break;
                case ErrorOuputType.Text:
                    saperator = "\n";
                    break;
            }

            string result = String.Join(saperator, this.errorMessageList.ToArray());
            return result;
        }

        /// <summary>
        /// 是否有錯誤訊息
        /// </summary>
        /// <returns></returns>
        public bool HasError()
        {
            return this.errorMessageList.Count > 0;
        }
    }
}
