using kidd.Common.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace kidd.Web.Common
{
    public class PageSort
    {
        private Dictionary<string, string> dicSortExpressionMapp;

        private HttpRequestBase request;
        private string navigateUrl;
        private string sortExpressionField;
        private string sortDirectionField;

        public string CurrentSortExpression
        {
            get
            {
                string key = ConvertUtility.ConvertType<string>(this.request.QueryString[this.sortExpressionField]);
                string result = GetSortExpression(key);
                return result;
            }
        }

        public string CurrentSortDirection
        {
            get
            {
                string result = ConvertUtility.ConvertType<string>(this.request.QueryString[this.sortDirectionField]);
                return result.ToLower();
            }
        }

        public PageSort(HttpRequestBase request, string navigateUrl, Dictionary<string, string> dicSortExpressionMapp)
            : this(request, navigateUrl, "se", "sd", dicSortExpressionMapp)
        {

        }

        public PageSort(HttpRequestBase request, string navigateUrl, string sortExpressionField, string sortDirectionField, Dictionary<string, string> dicSortExpressionMapp)
        {
            this.request = request;
            this.navigateUrl = navigateUrl;
            this.sortExpressionField = sortExpressionField;
            this.sortDirectionField = sortDirectionField;
            this.dicSortExpressionMapp = dicSortExpressionMapp;
        }

        public string GetSqlOrderStatement()
        {
            string sqlColumn = CurrentSortExpression;
            string direction = CurrentSortDirection;
            string sqlSortDirection = "";

            switch (direction)
            {
                case "up":
                    sqlSortDirection = "DESC";
                    break;
                case "down":
                    sqlSortDirection = "ASC";
                    break;
                default:
                    sqlSortDirection = "ASC";
                    break;
            }

            string result = "";

            if (!String.IsNullOrEmpty(sqlColumn))
            {
                result = sqlColumn + " " + sqlSortDirection;
            }

            return result;
        }

        public bool? IsAsc(string key)
        {
            bool? result = null;
            string sortExpression = GetSortExpression(key);

            if (!String.IsNullOrEmpty(sortExpression) && sortExpression == CurrentSortExpression)
            {
                switch (CurrentSortDirection)
                {
                    case "down":
                        result = true;
                        break;
                    case "up":
                        result = false;
                        break;
                }
            }

            return result;
        }

        public string GetSortNavigateUrl(string sortKey)
        {
            if (!String.IsNullOrEmpty(GetSortExpression(sortKey)))
            {
                string url = Path.GetFileName(this.navigateUrl);

                List<string> query_list = new List<string>();
                foreach (String key in request.QueryString.AllKeys)
                {
                    //排除參數
                    if (key != this.sortDirectionField && key != this.sortExpressionField)
                    {
                        query_list.Add(String.Format("{0}={1}", key, HttpUtility.UrlEncode(request.QueryString[key])));
                    }
                }

                if (query_list.Count > 0)
                {
                    url += "?" + String.Join("&", query_list.ToArray()) + "&";
                }
                else
                {
                    url += "?";
                }

                bool? isAsc = IsAsc(sortKey);
                url += String.Format("{0}={1}&{2}={3}", this.sortExpressionField, sortKey,
                    this.sortDirectionField,
                    isAsc.HasValue ? (isAsc.Value ? "up" : "down") : "down");

                return url;
            }
            else
            {
                throw new Exception("排序找不到對應的 Key 值");
            }

        }

        private string GetSortExpression(string key)
        {
            string result = "";
            if (dicSortExpressionMapp.ContainsKey(key))
            {
                result = dicSortExpressionMapp[key];
            }
            return result;
        }
    }
}
