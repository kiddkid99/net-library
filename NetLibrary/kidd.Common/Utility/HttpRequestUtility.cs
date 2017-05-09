using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace kidd.Common.Utility
{
    public static class HttpRequestUtility
    {
        /// <summary>
        /// 使用 HTTP GET 發出請求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetHttpRequest(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string result = reader.ReadToEnd();
                    return result;
                }
            }
        }

        /// <summary>
        /// 使用 HTTP POST 表單請求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string PostHttpRequest(string uri, NameValueCollection param, string contentType = "application/x-www-form-urlencoded")
        {
            List<string> param_list = new System.Collections.Generic.List<string>();
            foreach (string key in param)
            {
                param_list.Add(key + "=" + param[key]);
            }

            string post_param = String.Join("&", param_list.ToArray());

            return PostHttpRequest(uri, post_param);
        }

        /// <summary>
        /// 使用 HTTP POST
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string PostHttpRequest(string uri, string body, string contentType = "application/x-www-form-urlencoded")
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Post;
            request.KeepAlive = true;
            request.ContentType = contentType;


            byte[] post_data = System.Text.Encoding.UTF8.GetBytes(body);

            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(post_data, 0, post_data.Length);
            }


            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    string result = sr.ReadToEnd();
                    return result;
                }
            }
        }
    }
}
