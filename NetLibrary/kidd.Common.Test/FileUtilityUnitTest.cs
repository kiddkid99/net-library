using kidd.Common.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gb.Share.Test
{
    [TestClass]
    public class FileUtilityUnitTest
    {
        [TestMethod]
        public void TestGetFileSizeByteFormat()
        {
            //Arrange
            long size = 500;
            string expected = "500 B";

            //Act
            string result = FileUtility.GetFileSizeFormat(size);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGetFileSizeKbFormat()
        {
            //Arrange
            long size = 20000;
            string expected = "19 KB";

            //Act
            string result = FileUtility.GetFileSizeFormat(size);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGetFileSizeMbFormat()
        {
            //Arrange
            long size = 104857600;
            string expected = "100 MB";

            //Act
            string result = FileUtility.GetFileSizeFormat(size);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestGetFileSizeGbFormat()
        {
            //Arrange
            long size = 536870912000;
            string expected = "500 GB";

            //Act
            string result = FileUtility.GetFileSizeFormat(size);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void TestCreateSuffixFilePath()
        {
            //Arrange
            string file = "/folder/date/test.jpg";
            string suffix = "_s";
            string expected = "/folder/date/test_s.jpg";

            //Act
            string result = FileUtility.CreateSuffixFilePath(file, suffix);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
