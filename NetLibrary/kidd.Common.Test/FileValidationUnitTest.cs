using kidd.Common.Validation.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gb.Share.Test
{
    [TestClass]
    public class FileValidationUnitTest
    {
        [TestMethod]
        public void Image_Valid()
        {
            //Arrange
            string test = "test.jpg";
            bool expected = true;

            //Act
            IFileExtensions extension = new ImageFileExtensions();
            FileValidation validate = new FileValidation();
            bool result = validate.IsExtension(test, extension);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void Image_Invalid()
        {
            //Arrange
            string test = "test.doc";
            bool expected = false;

            //Act
            IFileExtensions extension = new ImageFileExtensions();
            FileValidation validate = new FileValidation();
            bool result = validate.IsExtension(test, extension);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void ExceedSize()
        {
            //Arrange
            int test = 4000;
            int max = 2048;
            bool expected = true;

            //Act
            FileValidation validate = new FileValidation();
            bool result = validate.ExceedSize(test, max);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
