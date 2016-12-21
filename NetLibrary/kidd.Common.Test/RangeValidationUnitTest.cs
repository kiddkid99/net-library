using kidd.Common.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace gb.Share.Test
{
    [TestClass]
    public class RangeValidationUnitTest
    {
        [TestMethod]
        public void Range__Int32()
        {
            //Arrange
            int min = 0;
            int max = 10;
            int value = 5;

            bool expected = true;

            //Act
            RangeValidation<Int32> range = new RangeValidation<int>(min, max);
            bool result = range.IsRange(value);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Equal__Int32()
        {
            //Arrange
            int single = 10;
            int value = 10;

            bool expected = true;

            //Act
            RangeValidation<Int32> range = new RangeValidation<int>(single);
            bool result = range.IsRange(value);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Range__DateTime()
        {
            //Arrange
            DateTime min = DateTime.Parse("2016/01/01");
            DateTime max = DateTime.Parse("2016/06/01");
            DateTime value = DateTime.Parse("2016/05/31");

            bool expected = true;

            //Act
            RangeValidation<DateTime> range = new RangeValidation<DateTime>(min, max);
            bool result = range.IsRange(value);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void Equal__String()
        {
            //Arrange
            string value = "helloworld";
            string single = "helloworld";

            bool expected = true;

            //Act
            RangeValidation<string> range = new RangeValidation<string>(single);
            bool result = range.IsRange(value);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NotEqual__String()
        {
            //Arrange
            string value = "hello world";
            string single = "hello";

            bool expected = false;

            //Act
            RangeValidation<string> range = new RangeValidation<string>(single);
            bool result = range.IsRange(value);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void Greater__Int32()
        {
            //Arrange
            var value = 10;
            var single = 5;

            bool expected = true;

            //Act
            RangeValidation<Int32> range = new RangeValidation<Int32>(single);
            bool result = range.IsGreater(value);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Less__Int32()
        {
            //Arrange
            var value = 5;
            var single = 10;

            bool expected = true;

            //Act
            RangeValidation<Int32> range = new RangeValidation<Int32>(single);
            bool result = range.IsLess(value);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void Greater__DateTime()
        {
            //Arrange
            var value = DateTime.Now;
            var single = DateTime.Now.AddDays(-10);

            bool expected = true;

            //Act
            RangeValidation<DateTime> range = new RangeValidation<DateTime>(single);
            bool result = range.IsGreater(value);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void Less__DateTime()
        {
            //Arrange
            var value = DateTime.Now.AddDays(-10);
            var single = DateTime.Now;
          
            bool expected = true;

            //Act
            RangeValidation<DateTime> range = new RangeValidation<DateTime>(single);
            bool result = range.IsLess(value);

            //Assert
            Assert.AreEqual(expected, result);
        }


    }
}
