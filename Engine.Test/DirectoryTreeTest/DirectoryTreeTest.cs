using Engine.Entity.DirectoryTree;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Test.DirectoryTreeTest
{
    [TestClass]
    public class DirectoryTreeTest
    {
        [TestMethod]
        public void BasicInsertAndFindTest()
        {
            DirectoryNode root = new DirectoryNode("");
            TestPath("D:\\WinFred\\Project\\james-windows", root);
            TestPath("D:\\WinFred\\Project\\james-windows\\James.sln", root);
            TestPath("D:\\WinFred\\Project\\james-test\\James.sln", root);
            TestPath("D:\\WinFred\\Project\\james-test\\James.s.ln", root);
            var result = root.Find("D:\\WinFred\\Project");
            Assert.IsTrue(result.Childrens.Count == 2);
        }

        private void TestPath(string path, DirectoryNode root)
        {
            var result = root.Insert(path);
            Assert.IsTrue(result.GetFullPath() == path, "Both paths should match");
            Assert.IsTrue(result.ComparePath(path), "Both paths should match");
        }

        [TestMethod]
        public void ReturnNullOnNotFoundTest()
        {
            DirectoryNode root = new DirectoryNode("");
            Assert.IsNull(root.Find("D:\\Winfred"));

            TestPath("D:\\WinFred\\Project\\james-windows", root);
            TestPath("D:\\WinFred\\Project\\james-windows\\James.sln", root);
            TestPath("D:\\WinFred\\Project\\james-test\\James.sln", root);
            TestPath("D:\\WinFred\\Project\\james-test\\James.s.ln", root);

            Assert.IsNull(root.Find("D:\\Winfred2"));
        }

        [TestMethod]
        public void ComparePathTest()
        {
            DirectoryNode root = new DirectoryNode("");
            root.Insert("D:\\WinFred\\Project\\james-windows");
            Assert.IsTrue(root.Find("D:\\WinFred\\Project\\james-windows").ComparePath("D:\\WinFred\\Project\\james-windows"));
        }
    }
}
