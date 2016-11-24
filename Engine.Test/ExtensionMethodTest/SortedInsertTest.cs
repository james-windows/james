using Engine.Entity.DirectoryTree.Mocking;
using Engine.Entity.SearchTree;
using Engine.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Engine.Test.ExtensionMethodTest
{
    [TestClass]
    public class SortedInsertTest
    {
        [TestMethod]
        public void BasicInsertTest()
        {
            List<int> list = new List<int>();
            list.SortedInsert(2);
            list.SortedInsert(8);
            list.SortedInsert(8);
            list.SortedInsert(10);
            list.SortedInsert(-2);
            list.SortedInsert(4);
            list.SortedInsert(3);
            list.Reverse();
            Assert.IsTrue(list[0] == 10);
            Assert.IsTrue(list[1] == 8);
            Assert.IsTrue(list[2] == 4);
            Assert.IsTrue(list[5] == -2);
        }

        [TestMethod]
        public void BasicInsertItemTest()
        {
            List<Item> list = new List<Item>();
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 2 });
            list.SortedInsert(new Item(new DirectoryNodeMock("differentPaths")) { Priority = 8 });
            list.SortedInsert(new Item(new DirectoryNodeMock("asdf")) { Priority = 8 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 10 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = -2 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 4 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 3 });
            Assert.IsTrue(list[0].Priority == 10);
            Assert.IsTrue(list[1].Priority == 8);
            Assert.IsTrue(list[2].Priority == 8);
            Assert.IsTrue(list[6].Priority == -2);
        }

        [TestMethod]
        public void BasicInsertMaximumLengthTest()
        {
            List<Item> list = new List<Item>();
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 2 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 2 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 2 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 4 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 3 });
            list.SortedInsert(new Item(new DirectoryNodeMock("differentPaths")) { Priority = 8 });
            list.SortedInsert(new Item(new DirectoryNodeMock("asdf")) { Priority = 8 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 10 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = -2 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = -2 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = -2 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = -2 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 4 });
            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = 3 });
            Assert.IsTrue(list[0].Priority == 10);
            Assert.IsTrue(list[1].Priority == 8);
            Assert.IsTrue(list[2].Priority == 8);
            Assert.IsTrue(list[5].Priority == 2);

            list.SortedInsert(new Item(new DirectoryNodeMock()) { Priority = -8 });
            Assert.IsTrue(list[0].Priority == 10);
            Assert.IsTrue(list[1].Priority == 8);
            Assert.IsTrue(list[2].Priority == 8);
            Assert.IsTrue(list[5].Priority == 2);
        }

        [TestMethod]
        public void SortedItemInsertTestWithSamePriority()
        {
            List<Item> list = new List<Item>();
            list.SortedInsert(new Item(new DirectoryNodeMock("asdf")) { Priority = 2 });
            list.SortedInsert(new Item(new DirectoryNodeMock("sdf")) { Priority = 2 });
            list.SortedInsert(new Item(new DirectoryNodeMock("df")) { Priority = 2 });

            list.SortedInsert(new Item(new DirectoryNodeMock("df")) { Priority = 1 });
            list.SortedInsert(new Item(new DirectoryNodeMock("df")) { Priority = 3 });

            Assert.IsTrue(list[0].Path == "df");
            Assert.IsTrue(list[1].Path == "asdf");
            Assert.IsTrue(list[2].Path == "df");
            Assert.IsTrue(list[3].Path == "sdf");
            Assert.IsTrue(list[4].Path == "df");
        }
    }
}
