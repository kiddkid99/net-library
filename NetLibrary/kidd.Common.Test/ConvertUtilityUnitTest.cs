using kidd.Common.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace gb.Share.Test
{
    [TestClass]
    public class ConvertUtilityUnitTest
    {
        [TestMethod]
        public void TestConvertInt32()
        {
            //Arrange
            var test = "123";
            int expected = 123;

            //Act
            var result = ConvertUtility.ConvertType<Int32>(test);

            //Assert
            Assert.AreEqual<Int32>(expected, result);
        }

        [TestMethod]
        public void TestConvertDateTime()
        {
            //Arrange
            string test = "2016/12/31";
            DateTime expected = new DateTime(2016, 12, 31);

            //Act
            var result = ConvertUtility.ConvertType<DateTime>(test);

            //Assert
            Assert.AreEqual<DateTime>(expected, result);
        }

        [TestMethod]
        public void TestConvertDateTimeFormat()
        {
            //Arrange
            DateTime now = DateTime.Now;
            string test = now.ToString("yyyy/MM/dd HH:mm:ss");
            DateTime expected = DateTime.Parse(test);

            //Act
            var result = ConvertUtility.ConvertType<DateTime>(test);

            //Assert
            Assert.AreEqual<DateTime>(expected, result);
        }

        [TestMethod]
        public void TestConvertGuid()
        {
            //Arrange
            var test = "c4a1d8ee-92eb-4880-9099-e30c5f6ce4f9";
            var expected = Guid.Parse(test);

            //Act
            var result = ConvertUtility.ConvertType<Guid>(test);

            //Assert
            Assert.AreEqual<Guid>(expected, result);
        }


        [TestMethod]
        public void TestConvertNullableInt32()
        {
            //Arrange
            var test = "123";
            Int32? expected = 123;

            //Act
            var result = ConvertUtility.ConvertType<Nullable<Int32>>(test);

            //Assert
            Assert.AreEqual<Nullable<Int32>>(expected, result);
        }

        [TestMethod]
        public void TestConvertNullable()
        {
            //Arrange
            var test = "";
            DateTime? expected = null;

            //Act
            var result = ConvertUtility.ConvertType<DateTime?>(test);

            //Assert
            Assert.AreEqual<DateTime?>(expected, result);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void TestConvertException()
        {
            //轉換不合法的字串
            //Arrange
            var test = "aaa";
      
            //Act
            var result = ConvertUtility.ConvertType<Int32>(test);

            //no Assert
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestConvertFormatException()
        {
            //轉換錯誤日期字串
            //Arrange
            var test = "1111";

            //Act
            var result = ConvertUtility.ConvertType<DateTime>(test);

            //no Assert
        }

        [TestMethod]
        public void TestConvertNotSupported()
        {
            //轉換 FileUtility 類別
            //Arrange
            var test = "";
            FileUtility expected = null;

            //Act
            var result = ConvertUtility.ConvertType<FileUtility>(test);

            //no Assert
            Assert.AreEqual<FileUtility>(expected, result);
        }


        [TestMethod]
        public void TestConvertDouble()
        {
            //Arrange
            var test = "3.14159";
            double expected = 3.14159; ;

            //Act
            var result = ConvertUtility.ConvertType<Double>(test);

            //Assert
            Assert.AreEqual<Double>(expected, result);
        }

        [TestMethod]
        public void TestConvertBoolean()
        {
            //Arrange
            var test = "true";
            var expected = true; ;

            //Act
            var result = ConvertUtility.ConvertType<Boolean>(test);

            //Assert
            Assert.AreEqual<Boolean>(expected, result);
        }


        [TestMethod]
        public void TestIsInt32()
        {
            //Arrange
            var test = "123";
            var expected = true; ;

            //Act
            var result = ConvertUtility.IsType<Int32>(test);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestConvertDefaultInt32()
        {
            //Arrange
            var test = "aaa";
            var expected = 0; ;

            //Act
            var result = ConvertUtility.ConvertType<Int32>(test, true);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestConvertDefaultDateTime()
        {
            //Arrange
            var test = "aaa";
            DateTime expected = new DateTime();

            //Act
            var result = ConvertUtility.ConvertType<DateTime>(test, true);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void TestConvertDefaultDouble()
        {
            //Arrange
            var test = "aaa";
            var expected = 0;

            //Act
            var result = ConvertUtility.ConvertType<Double>(test, true);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
