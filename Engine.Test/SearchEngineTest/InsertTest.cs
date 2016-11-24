using Engine.Collections;
using Engine.Entity.DirectoryTree.Mocking;
using Engine.Entity.SearchTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Test.SearchEngineTest
{
    [TestClass]
    public class InsertTest
    {
        [TestMethod]
        public void BasicInsertTest()
        {
            SearchNode root = new SearchNode() { Label = "chrome", Results = new MergedResultList() };
            var item = new Item(new DirectoryNodeMock("chrome.exe")) { Priority = 2 };
            root.Items.Add(item);
            root.Results.Add(item);
            MergeResult result = new MergeResult();
            root.Insert(".exe", item, ref result);
            root.Insert("google chrome", item, ref result);
            root.Insert("chrome", item, ref result);
            root.Insert("firefox", new Item(new DirectoryNodeMock("firefox.exe")) { Priority = 3 }, ref result);
            root.Insert("filezilla", new Item(new DirectoryNodeMock("filezilla.exe")) { Priority = 4 }, ref result);
            Assert.IsTrue(root.Find("fi").ResultItems.Count == 2);
            Assert.IsTrue(root.Find(".exe").ResultItems[0] == item);
            Assert.IsTrue(root.Find("google chrome").ResultItems[0] == item);
            Assert.IsTrue(root.Find("chrome").ResultItems[0] == item);
        }

        [TestMethod]
        public void InsertSameItem()
        {
            SearchNode root = new SearchNode() { Label = "chrome", Results = new MergedResultList() };
            var item = new Item(new DirectoryNodeMock("chrome.exe")) { Priority = 2 };
            root.Items.Add(item);
            MergeResult result = new MergeResult();
            for (int i = 0; i < 11; i++)
            {
                root.Insert("asdf", item, ref result);
            }
            Assert.IsTrue(root.Find("asdf").ResultItems.Count == 1);
            Assert.IsTrue(root.Find("asdf").Items.Count == 1);
        }
    }
}
