using kidd.Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace gb.Share.Test
{
    [TestClass]
    public class StringFilterUnitTest
    {
        [TestMethod]
        public void TestRemoveHtml()
        {
            //Arrange
            string text = @"<h1>Title</h1><div><p>Hello world.</p></div>";
            string expected = "TitleHello world.";

            //Act
            string result = StringFilter.RemoveHtml(text);

            //Assert
            Assert.AreEqual(expected, result);
        }


        [TestMethod]
        public void TestRemoveSqlStatement()
        {
            //Arrange
            string text = @"DELETE from member;truncate table menu;";
            string expected = " from membertruncate  menu";

            //Act
            string result = StringFilter.RemoveSqlStatement(text);

            //Assert
            Assert.AreEqual(expected, result);
        }
    }
}
