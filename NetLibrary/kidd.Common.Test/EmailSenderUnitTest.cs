using kidd.Common.Sender;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Net.Mail;

namespace gb.Share.Test
{
    [TestClass]
    public class EmailSenderUnitTest
    {
        private string to = System.Configuration.ConfigurationManager.AppSettings["email_send"];


        [TestMethod]
        public void TestSend()
        {
            //Arrange
            EmailSender client = new EmailSender();
            string subject = "Email Sender Unit Test";
            string body = "Email Sender Unit Test";
            Exception expected = null;

            //Act
            Exception result = null;
            try
            {
                client.Send(subject, body, to, null, null, true);
            }
            catch (Exception ex)
            {
                result = ex;
            }
           
            //Assert
            Assert.AreEqual(expected, result);


           
        }

        [TestMethod]
        public void TestSendWithCC()
        {
            //Arrange
            EmailSender client = new EmailSender();
            string cc = to;
            string subject = "Email Sender Unit Test CC";
            string body = "Email Sender Unit Test CC";
            Exception expected = null;

            //Act
            Exception result = null;
            try
            {
                client.Send(subject, body, to, cc, null, true);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            //Assert
            Assert.AreEqual(expected, result);

        }

        [TestMethod]
        public void TestSendWithAttachment()
        {
            //Arrange
            EmailSender client = new EmailSender();
            string subject = "Email Sender Unit Test Attachment";
            string body = "Email Sender Unit Test Attachment";

            MemoryStream ms = new MemoryStream();
            string content = "Email Sender 測試附件";
            Attachment attach = Attachment.CreateAttachmentFromString(content, "test.txt");
            Exception expected = null;

            //Act
            Exception result = null;
            try
            {
                client.Send(subject, body, to, null, attach, true);
            }
            catch (Exception ex)
            {
                result = ex;
            }

            //Assert
            Assert.AreEqual(expected, result);

        }


        [TestMethod]
        public void TestSendNoTo()
        {
            //Arrange
            EmailSender client = new EmailSender();
            string to = "";
            string subject = "Email Sender Unit Test";
            string body = "Email Sender Unit Test";

            string expected = "發送對象信箱不得為空";

            //Act
            string result = "";
            try
            {
                client.Send(subject, body, to, null, null, true);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            //Assert
            Assert.AreEqual(expected, result);

        }
    }
}
