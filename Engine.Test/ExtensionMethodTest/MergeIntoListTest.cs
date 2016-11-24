using System.Collections.Generic;
using Engine.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Test.ExtensionMethodTest
{
    [TestClass]
    public class MergeIntoListTest
    {
        [TestMethod]
        public void EasyMergeTest()
        {
            List<int> first = new List<int> { 1, 2, 3 };
            List<int> second = new List<int> { 4, -1, 2 };
            List<int> third = new List<int> { 6, 3, 4 };

            first.MergeIntoList(second);
            Assert.IsTrue(first.Count == 5);
            Assert.IsTrue(first[0] == -1);

            first.MergeIntoList(third);
            Assert.IsTrue(first.Count == 6);
            Assert.IsTrue(first[0] == -1);
            Assert.IsTrue(first[5] == 6);
        }

        [TestMethod]
        public void LimitMergeTest()
        {
            List<int> first = new List<int> { 1, 2, 3 };
            List<int> second = new List<int> { 4, -1, 2 };
            List<int> third = new List<int> { 6, 3, 4 };
            first.MergeIntoList(second);
            first.MergeIntoList(third);
            first.MergeIntoList(new List<int> { 2, 3, 4, 5 });

            Assert.IsTrue(first.Count == 7);
            Assert.IsTrue(first[4] == 4);
            Assert.IsTrue(first[3] == 3);
            Assert.IsTrue(first[1] == 1);
            Assert.IsTrue(first[0] == -1);
        }
    }
}
