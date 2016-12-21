using kidd.Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gb.Share.Test
{
    [TestClass]
    public class StringEncryptUnitTest
    {
        [TestMethod]
        public void TestMd5Success()
        {
            //Arrange
            string text = "1234";
            string expected = "81dc9bdb52d04dc20036dbd8313ed055";
           
            //Act
            string result = StringEncrypt.Md5Encrypt(text);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDesEncrypt()
        {
            //Arrange
            string key = "12345678";
            string iv = "87654321";
            string text = "hello world";
            string expected = "f66U/RqLiA2NVFTdjfMMQA==";

            //Act
            string result = StringEncrypt.DesEncryptBase64(text, key, iv);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDesDecrypt()
        {
            //Arrange
            string key = "12345678";
            string iv = "87654321";
            string text = "f66U/RqLiA2NVFTdjfMMQA==";
            string expected = "hello world";

            //Act
            string result = StringEncrypt.DesDecryptBase64(text, key, iv);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [ExpectedException(typeof(DecryptExcepion))]
        public void TestDesException()
        {
            //Arrange
            string key = "12345";
            string iv = "87654";
            string text = "f66U/RqLiA2NVFTdjfMMQA==";

            //Act
            string result = StringEncrypt.DesDecryptBase64(text, key, iv);

            
            //no assert
        }
    }
}
