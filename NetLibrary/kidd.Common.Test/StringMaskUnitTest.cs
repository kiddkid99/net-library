using kidd.Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gb.Share.Test
{
    [TestClass]
    public class StringMaskUnitTest
    {
        [TestMethod]
        public void TestMask()
        {
            //Arrange
            string text = "0912345678";
            string mask = "1111000111";
            string expected = "0912***678";

            //Act
            string result = StringMask.Mask(text, mask);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMaskRepeat()
        {
            //Arrange
            string text = "0912345678";
            string mask = "100";
            string expected = "0**2**5**8";

            //Act
            string result = StringMask.Mask(text, mask);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMaskIgnore()
        {
            //Arrange
            string text = "0912-345678";
            string mask = "1111000111";
            char[] ignore = { '-' };
            string expected = "0912-**5678";

            //Act
            string result = StringMask.Mask(text, mask, ignore);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMaskIgnoreRepeat()
        {
            //Arrange
            string text = "0912-345678";
            string mask = "1000";
            char[] ignore = { '-' };
            string expected = "0***-***6**";

            //Act
            string result = StringMask.Mask(text, mask, ignore);

            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMaskEmail()
        {
            //Arrange
            string text = "test1234@gmail.com";
            string mask = "1100";
            char[] ignore = { '.', '@' };
            string expected = "te**12**@g**il.*om";

            //Act
            string result = StringMask.Mask(text, mask, ignore);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void TestMaskCustom()
        {
            //Arrange
            string text = "test1234@gmail.com";
            string mask = "1100";
            char[] ignore = { '.', '@' };
            string expected = "te??12??@g??il.?om";

            //Act
            string result = StringMask.Mask(text, mask, '?', ignore);

            //Assert
            Assert.AreEqual(expected, result);
        }

    }
}
