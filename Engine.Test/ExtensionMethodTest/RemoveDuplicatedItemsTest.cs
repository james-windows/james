using System.Collections.Generic;
using Engine.Entity.DirectoryTree.Mocking;
using Engine.Entity.SearchTree;
using Engine.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Test.ExtensionMethodTest
{
    [TestClass]
    public class RemoveDuplicatedItemsTest
    {
        [TestMethod]
        public void BasicRemoveDuplicatedItemsTest()
        {
            List<Item> items = new List<Item>();
            items.Add(new Item(new DirectoryNodeMock("asdf")) { Priority = 20 });
            items.Add(new Item(new DirectoryNodeMock("asdf")) { Priority = 20 });
            items.Add(new Item(new DirectoryNodeMock("asdf")) { Priority = 20 });
            items.Add(new Item(new DirectoryNodeMock("aba")) { Priority = 15 });
            items.Add(new Item(new DirectoryNodeMock("aba")) { Priority = 14 });
            items.Add(new Item(new DirectoryNodeMock("asdf")) { Priority = 14 });
            items.Add(new Item(new DirectoryNodeMock("aba")) { Priority = 14 });
            items.Add(new Item(new DirectoryNodeMock("asdf")) { Priority = 4 });
            items.Add(new Item(new DirectoryNodeMock("asdf")) { Priority = 4 });
            items.Add(new Item(new DirectoryNodeMock("a")) { Priority = 4 });
            items.Add(new Item(new DirectoryNodeMock("asdf")) { Priority = 4 });

            items.RemoveDuplicatedItems();

            Assert.IsTrue(items.Count == 6);
        }
    }
}
