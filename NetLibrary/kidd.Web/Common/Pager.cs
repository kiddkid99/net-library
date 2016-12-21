using kidd.Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace kidd.Web.Common
{
    public class Pager
    {
        private HttpRequestBase request;
        private string pageNoField;
        private string pageSizeField;


        private int pageNo;
        /// <summary>
        /// 取得目前頁數
        /// </summary>
        public int PageNo
        {
            get
            {
                int pageNo = ConvertUtility.ConvertType<Int32>(request.QueryString[this.pageNoField], true);
                this.pageNo = pageNo > 0 ? pageNo : 1;
                return this.pageNo;
            }
        }

        private int pageSize;
        /// <summary>
        /// 取得目前每頁資料數
        /// </summary>
        public int PageSize
        {
            get
            {
                int pageSize = ConvertUtility.ConvertType<Int32>(request.QueryString[this.pageSizeField], true);
                this.pageSize = this.pageSizeArray.Contains(pageSize) ? pageSize : this.pageSizeArray[0];
                return this.pageSize;
            }
        }

        private int[] pageSizeArray = { 10, 20, 50 };
        /// <summary>
        /// 取得每頁資料數陣列
        /// </summary>
        public int[] PageSizeArray
        {
            get { return this.pageSizeArray; }
        }



        private string navigateUrl;
        public String NavigateUrl
        {
            get { return this.navigateUrl; }
        }

        private string currentNavigateUrl;
        public string CurrentNavigateUrl
        {
            get { return this.currentNavigateUrl; }
        }

        private int totalPage;
        /// <summary>
        /// 取得所有頁數
        /// </summary>
        public int TotalPage
        {
            get { return this.totalPage; }
        }

        private int totalRecord;
        /// <summary>
        /// 取得所有資料數
        /// </summary>
        public int TotalRecord
        {
            get { return this.totalRecord; }
        }

        public Pager(HttpRequestBase request, string navigateUrl)
            : this(request, navigateUrl, "page", "pageSize")
        {
        }

        public Pager(HttpRequestBase request, string navigateUrl, string pageNoField, string pageSizeField)
        {
            this.request = request;
            this.navigateUrl = navigateUrl;
            this.pageNoField = pageNoField;
            this.pageSizeField = pageSizeField;
        }


        public void SetPageSizeArray(int[] array)
        {
            this.pageSizeArray = array;
        }

        public void SetPagerWithHttpGet(int totalPage, int totalRecord, string[] exclude_querystring)
        {
            this.navigateUrl = Path.GetFileName(navigateUrl);

            this.totalPage = totalPage;
            this.totalRecord = totalRecord;


            List<string> query_list = new List<string>();
            foreach (String key in request.QueryString.AllKeys)
            {
                //排除 page, pageSize,  指定 exclude_querystring 的參數。
                if (key != this.pageNoField && key != this.pageSizeField)
                {
                    if (exclude_querystring == null)
                    {
                        query_list.Add(String.Format("{0}={1}", key, HttpUtility.UrlEncode(request.QueryString[key])));
                    }
                    else if (exclude_querystring != null && !exclude_querystring.Contains(key))
                    {
                        query_list.Add(String.Format("{0}={1}", key, HttpUtility.UrlEncode(request.QueryString[key])));
                    }
                }
            }

            if (query_list.Count > 0)
            {
                this.navigateUrl += "?" + String.Join("&", query_list.ToArray()) + "&";
            }
            else
            {
                this.navigateUrl += "?";
            }


            this.currentNavigateUrl = String.Format("{0}{1}={2}&{3}={4}", this.navigateUrl,
                this.pageNoField, this.pageNo,
                this.pageSizeField, this.pageSize);
        }



    }
}
