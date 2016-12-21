using kidd.Common.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace gb.Share.Test
{
    [TestClass]
    public class RandomUtilityUnitTest
    {
        [TestMethod]
        public void TestGetNumber()
        {
            //Arrange
            int number = 100;
            int expected_min = 0;
            int exprected_max = 100;

            //Act
            int result = RandomUtility.GetNumber(number);

            //Assert
            Trace.WriteLine("產生結果：" + result);
            Assert.IsTrue(expected_min <= result && exprected_max >= result);
        }


        [TestMethod]
        public void TestGetString()
        {
            //Arrange
            int number = 5;

            //Act
            string result = RandomUtility.GetString(number);

            //Assert
            Trace.WriteLine("產生結果：" + result);
            Assert.IsFalse(ConvertUtility.IsType<Int32>(result));
        }
    
    }
}
