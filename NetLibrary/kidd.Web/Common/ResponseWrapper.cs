using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace kidd.Web.Common
{
    public class ResponseWrapper
    {
        private HttpResponseBase response;

        public ResponseWrapper(HttpResponse response)
        {
            HttpResponseWrapper wrapper = new HttpResponseWrapper(response);
            this.response = wrapper;
        }

        public ResponseWrapper(HttpResponseBase response)
        {
            this.response = response;
        }

        /// <summary>
        /// 結束請求處理，直接輸出 Javascript Alert 訊息視窗，並回到上一頁( 呼叫 history.go(-1) ) 
        /// </summary>
        /// <param name="message"></param>
        public void AlertMsg(string message)
        {
            AlertMsg(message, "");
        }


        /// <summary>
        /// 結束請求處理，直接輸出 Javascript Alert 訊息視窗，並導向至指定的頁面
        /// </summary>
        /// <param name="message"></param>
        /// <param name="redirect_url"></param>
        public void AlertMsg(string message, string redirect_url)
        {
            try
            {
                this.response.ContentType = "text/html";
                this.response.ContentEncoding = Encoding.UTF8;


                string script = String.Format("<script>alert('{0}');{1}</script>", message,
                    String.IsNullOrEmpty(redirect_url) ? "history.go(-1);" : "location.href='" + redirect_url + "';");

                
                this.response.Write(script);
                this.response.Flush();
                this.response.Close();
                this.response.End();
            }
            catch (ThreadAbortException)
            {
                //呼叫 Reponse.End() ，這會引發 System.Threading.ThreadAbortException 例外發生
                //略過這個例外處理
            } 
        }
    }
}
