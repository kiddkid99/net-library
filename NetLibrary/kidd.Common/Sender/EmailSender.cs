using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace kidd.Common.Sender
{
    /// <summary>
    /// 電子郵件發送模組
    /// </summary>
    public class EmailSender
    {
        private Encoding encoding = null;
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        private SmtpClient client = null;
        public SmtpClient SmtpClient
        {
            get
            {
                return client;
            }
        }

        /// <summary>
        /// 使用 web.config 或 app.config 的 <system.net> <mailSettings> 區段設定值初始化
        /// </summary>
        public EmailSender()
        {
            client = new SmtpClient();

            //預設使用 UTF8 編碼
            this.encoding = Encoding.UTF8;
        }

        /// <summary>
        /// 自定義 Section config 組態
        /// </summary>
        /// <param name="section"></param>
        //public EmailSender(SmtpSection section)
        //{
        //    //TODO:未完成
        //    _client = new SmtpClient();
        //    _client.DeliveryMethod = section.DeliveryMethod;
        //    _client.UseDefaultCredentials = section.Network.DefaultCredentials;
        //    if (!_client.UseDefaultCredentials)
        //    {
        //        _client.Credentials = new NetworkCredential(section.Network.UserName, section.Network.Password);
        //    }
        //    _client.EnableSsl = section.Network.EnableSsl;
        //    _client.Host = section.Network.Host;
            
        //}

        public void Send(MailMessage mailMessage)
        {
            try
            {
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 發送郵件
        /// </summary>
        /// <param name="subject">信件主旨</param>
        /// <param name="body">信件內容</param>
        /// <param name="toList">收件者集合</param>
        /// <param name="ccList">附件收件者集合</param>
        /// <param name="attachList">附件集合</param>
        /// <param name="isBodyHtml">主體採用HTML格式</param>
        public void Send(string subject, string body, List<MailAddress> toList, List<MailAddress> ccList, List<Attachment> attachList, bool isBodyHtml)
        {

            MailMessage msg = new MailMessage();
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = isBodyHtml;
            msg.BodyEncoding = this.encoding;


            if (toList != null)
            {
                foreach (var to in toList)
                {
                    msg.To.Add(to);
                }
            }

            if (ccList != null)
            {
                foreach (var cc in ccList)
                {
                    msg.CC.Add(cc);
                }

            }

            if (attachList != null)
            {
                foreach (var attach in attachList)
                {
                    msg.Attachments.Add(attach);
                }

            }

            Send(msg);
        }

        /// <summary>
        /// 發送郵件
        /// </summary>
        /// <param name="subject">信件主旨</param>
        /// <param name="body">信件內容</param>
        /// <param name="to">收件者</param>
        /// <param name="cc">副本收件者</param>
        /// <param name="attachment">附件</param>
        /// <param name="isBodyHtml">主體採用HTML格式</param>
        public void Send(string subject, string body, string to, string cc, Attachment attachment, bool isBodyHtml)
        {
            if (String.IsNullOrEmpty(to))
            {
                throw new Exception("發送對象信箱不得為空");
            }
            List<MailAddress> toList = new List<MailAddress>();
            toList.Add(new MailAddress(to));

            List<MailAddress> ccList = new List<MailAddress>();
            if (!String.IsNullOrEmpty(cc))
            {
                ccList.Add(new MailAddress(cc));
            }

            List<Attachment> attachList = new List<Attachment>();
            if (attachment != null)
            { 
                attachList.Add(attachment);
            }
           
            
          
            Send(subject, body, toList,ccList, attachList, isBodyHtml);
        }
    }
}
