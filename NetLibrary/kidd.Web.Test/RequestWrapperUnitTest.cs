using kidd.Common.Validation.File;
using kidd.Common.Validation.Custom;
using kidd.Web.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;

namespace gb.Web.Test
{
    [TestClass]
    public class RequestWrapperUnitTest
    {
        [TestMethod]
        public void GetClientIP()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.ServerVariables).Returns(() =>
            {
                NameValueCollection param = new NameValueCollection();
                param.Add("REMOTE_ADDR", "127.0.0.1");
                return param;
            });
            string expected = "127.0.0.1";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            string result = wrapper.GetClientIP();

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void GetAbsoluteApplication()
        {
            //Arrange
            string path = AppDomain.CurrentDomain.BaseDirectory;
            const string virtualDir = "/app";
            AppDomain.CurrentDomain.SetData(".appDomain", "*");
            AppDomain.CurrentDomain.SetData(".appPath", path);
            AppDomain.CurrentDomain.SetData(".appVPath", virtualDir);
            AppDomain.CurrentDomain.SetData(".hostingVirtualPath", virtualDir);
            AppDomain.CurrentDomain.SetData(".hostingInstallDir", HttpRuntime.AspInstallDirectory); 

            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Url).Returns(() =>
            {
                return new Uri("http://www.gameball.com.tw/");
            });

            //var path = new Mock<IVirtualPathUtility>();
            //path.Setup(x => x.ToAbsolute("~")).Returns("/app");
            string expected = "http://www.gameball.com.tw/app";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            string result = wrapper.GetAbsoluteApplication();

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void GetByForm__String()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Form).Returns(() =>
            {
                NameValueCollection param = new NameValueCollection();
                param.Add("name", "kidd");
                return param;
            });
            string expected = "kidd";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            string result = wrapper.GetByForm<string>("name");

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetByQueryString__Int32()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.QueryString).Returns(() =>
            {
                NameValueCollection param = new NameValueCollection();
                param.Add("id", "10");
                return param;
            });
            var expected = 10;

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            var result = wrapper.GetByQueryString<Int32>("id");

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RequiredValidate()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "";
            var field = "使用者";
            var expected = "請輸入使用者";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.RequiredValidate(value, field);
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void RequiredValidateFormat()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "";
            var field = "使用者";
            var expected = "使用者為必填欄位";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.RequiredValidate(value, field, "{0}為必填欄位");
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RequiredValidateFormat_File()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Files[0]).Returns(() =>
            {
                //模擬一個無檔案名稱的上傳檔案
                var mockFile = new Mock<HttpPostedFileBase>();
                mockFile.SetupGet(x => x.FileName).Returns("");
                
                return mockFile.Object;

            });

            var file = request.Object.Files[0];

            var value = file.FileName;
            var field = "上傳檔案";
            var expected = "請選擇上傳檔案";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.RequiredValidate(value, field, "請選擇{0}");
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DataTypeValidate__Int32()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "abc";
            var field = "代號";
            var expected = "代號格式不正確";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.DataTypeValidate<Int32>(value, true, field);
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void DataTypeValidateFormat__Int32()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "abc";
            var field = "代號";
            var expected = "代號錯誤";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.DataTypeValidate<Int32>(value, true, field, "{0}錯誤");
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void DataTypeValidateIgnoreEmpty()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "";
            var field = "代號";
            var expected = 0;

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.DataTypeValidate<Int32>(value, true, field);
            var result = wrapper.ErrorMessageList.Count;

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void EqualValidate__String()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "helloworld";
            var field = "文字";
            var single = "abcdefg";
            var expected = "文字的值必須等於abcdefg";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.EqualValidate(value, single, true, field);
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void EqualValidateFormat__String()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "helloworld";
            var field = "文字";
            var single = "abcdefg";
            var expected = "文字要跟abcdefg一樣";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.EqualValidate(value, single, true, field, "{0}要跟{1}一樣");
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RangeValidate__Int32()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "100";
            var min = 0;
            var max = 50;
            var field = "欄位";
            var expected = "欄位的值必須介於0~50";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.RangeValidate(value, min, max, false, field);
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RangeValidateFormat__Int32()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "100";
            var min = 0;
            var max = 50;
            var field = "欄位";
            var expected = "欄位必須在 0 ~ 50 之間";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.RangeValidate(value, min, max, false, field, "{0}必須在 {1} ~ {2} 之間");
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RangeValidateFormat__DateTime()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = DateTime.Now.ToString();
            var min = DateTime.Parse("2016/01/01");
            var max = DateTime.Parse("2016/02/01");
            var field = "日期";
            var expected = "日期必須在 2016/01/01 ~ 2016/02/01 之間";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.RangeValidate(value, min, max, false, field, "{0}必須在 {1:yyyy/MM/dd} ~ {2:yyyy/MM/dd} 之間");
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RangeValidateFormat__Greater__DateTime()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = DateTime.Now.ToString();
            var single = DateTime.Now.AddDays(10);
            var field = "日期";
            var expected = "日期必須大於" + single.ToString("yyyy/MM/dd");

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.GreaterValidate(value, single, false, field, "{0}必須大於{1:yyyy/MM/dd}");
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Trace.WriteLine(result);
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void RangeValidateFormat__Less__DateTime()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = DateTime.Now.AddDays(10).ToString();
            var single = DateTime.Now;
            var field = "日期";
            var expected = "日期必須小於" + single.ToString("yyyy/MM/dd");

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.LessValidate(value, single, false, field, "{0}必須小於{1:yyyy/MM/dd}");
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Trace.WriteLine(result);
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void FormatValidation__Email()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            var value = "kidd#gameball.com.tw";
            var field = "電子信箱";
            var expected = "電子信箱格式不正確";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.CustomValidate(new EmailFormatValidation(), value, true, field);
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Trace.WriteLine(result);
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void FileExtensionValidation__Invalid__Image()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Files[0]).Returns(() =>
            {
                //模擬一個無檔案名稱的上傳檔案
                var mockFile = new Mock<HttpPostedFileBase>();
                mockFile.SetupGet(x => x.FileName).Returns("test.doc");

                return mockFile.Object;

            });

            var file = request.Object.Files[0];
            var value = file.FileName;
            var field = "上傳檔案";
            var expected = "上傳檔案不支援 .doc 檔案格式";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.FileExtensionValidate(new ImageFileExtensions(), value, true, field);
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Trace.WriteLine(result);
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void FileSizeValidation()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Files[0]).Returns(() =>
            {
                //模擬一個無檔案名稱的上傳檔案
                var mockFile = new Mock<HttpPostedFileBase>();
                mockFile.SetupGet(x => x.FileName).Returns("test.doc");
                mockFile.SetupGet(x => x.ContentLength).Returns(10000);
                return mockFile.Object;

            });

            var file = request.Object.Files[0];
            var value = file.ContentLength;
            var max = 4096;
            var field = "上傳檔案";
            var expected = "上傳檔案超過檔案限制 4 KB";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.FileSizeValidate(value, max, field);
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Trace.WriteLine(result);
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void FileSizeValidation__FormatMessage()
        {
            //Arrange
            var request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.Files[0]).Returns(() =>
            {
                //模擬一個無檔案名稱的上傳檔案
                var mockFile = new Mock<HttpPostedFileBase>();
                mockFile.SetupGet(x => x.FileName).Returns("test.doc");
                mockFile.SetupGet(x => x.ContentLength).Returns(10000);
                return mockFile.Object;

            });

            var file = request.Object.Files[0];
            var value = file.ContentLength;
            var max = 4096;
            var field = "上傳檔案";
            var expected = "上傳檔案超過 4 KB";

            //Act
            RequestWrapper wrapper = new RequestWrapper(request.Object);
            wrapper.FileSizeValidate(value, max, field, "{0}超過 {1}");
            var result = wrapper.GetErrorMessage(ErrorOuputType.Text);

            //Assert
            Trace.WriteLine(result);
            Assert.AreEqual(expected, result);
        }
       

    }
}
