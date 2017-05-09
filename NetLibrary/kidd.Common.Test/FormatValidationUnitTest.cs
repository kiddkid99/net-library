using kidd.Common.Validation.Custom;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gb.Share.Test
{
    [TestClass]
    public class FormatValidationUnitTest
    {
        [TestMethod]
        public void TestMobileValid()
        {
            //Arrange
            string test = "0912345678";

            bool expected = true;

            //Act
            ICustomValidation format = new MobileFormatValidation();
            bool result = format.Validate(test);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMobileInvalid()
        {
            //Arrange
            string test = "123893427932";

            bool expected = false;

            //Act
            ICustomValidation format = new MobileFormatValidation();
            bool result = format.Validate(test);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEmailValid()
        {
            //Arrange
            string test = "kidd@gameball.com.tw";

            bool expected = true;

            //Act
            ICustomValidation format = new EmailFormatValidation();
            bool result = format.Validate(test);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestEmailInvalid()
        {
            //Arrange
            string test = "kidd!dssd@gmail.com.tw?";

            bool expected = false;

            //Act
            ICustomValidation format = new EmailFormatValidation();
            bool result = format.Validate(test);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTelValid()
        {
            //Arrange
            string[] array = { "02-2550-9196", "(02)25509196", "0225509196", "02-25509196", "(02)2550-9196"};

            bool expected = true;

            //Act
            ICustomValidation format = new TelephoneFormatValidation();
            bool result = true;
            foreach (var value in array)
            {
                if (!format.Validate(value))
                {
                    result = false;
                    break;
                }
            }
            

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestTelInvalid()
        {
            //Arrange
            string[] array = { "abcdefg", "123456778" };

            bool expected = true;

            //Act
            ICustomValidation format = new TelephoneFormatValidation();
            bool result = true;
            foreach (var value in array)
            {
                if (format.Validate(value))
                {
                    result = false;
                    break;
                }
            }


            //Assert
            Assert.AreEqual(expected, result);
        }




        [TestMethod]
        public void TestIdValid()
        {
            //Arrange
            string test = "A172219232";

            bool expected = true;

            //Act
            ICustomValidation format = new IdFormatValidation();
            bool result = format.Validate(test);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestIdInvalid()
        {
            //Arrange
            string test = "A111111111";

            bool expected = false;

            //Act
            ICustomValidation format = new IdFormatValidation();
            bool result = format.Validate(test);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
