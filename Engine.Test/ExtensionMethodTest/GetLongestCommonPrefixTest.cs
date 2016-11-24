using Engine.Entity;
using Engine.Entity.SearchTree;
using Engine.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Test.ExtensionMethodTest
{
    [TestClass]
    public class GetLongestCommonPrefixTest
    {
        [TestMethod]
        public void BasicTest()
        {
            int longestCommonPrefix;
            Assert.AreEqual("hallo".GetLongestCommonPrefix("hallohallo", out longestCommonPrefix), TraversalOptions.MoveDown);
            Assert.AreEqual(5, longestCommonPrefix);

            Assert.AreEqual("h".GetLongestCommonPrefix("h", out longestCommonPrefix), TraversalOptions.Found);
            Assert.AreEqual(1, longestCommonPrefix);

            Assert.AreEqual("ha".GetLongestCommonPrefix("h", out longestCommonPrefix), TraversalOptions.Split);
            Assert.AreEqual(1, longestCommonPrefix);

            Assert.AreEqual("ha".GetLongestCommonPrefix("ha", out longestCommonPrefix), TraversalOptions.Found);
            Assert.AreEqual(2, longestCommonPrefix);
        }
    }
}
