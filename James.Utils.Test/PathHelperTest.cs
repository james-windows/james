using System.IO;
using James.HelperClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace James.Utils.Test
{
    [TestClass]
    public class PathHelperTest
    {
        [TestMethod]
        public void BasicPathHelperFileExtensionTest()
        {
            Assert.IsTrue("txt" == PathHelper.GetFileExtension("C:\\tmp\\asdf.txt"));
            Assert.IsNull(PathHelper.GetFileExtension("C:\\tmp\\asdf"));
            Assert.IsNull(PathHelper.GetFileExtension("asdf"));
            Assert.IsNull(PathHelper.GetFileExtension("C:\\tmp\\asdf.asdf\\asdf"));
            Assert.IsNull(PathHelper.GetFileExtension("n"));
            Assert.IsNull(PathHelper.GetFileExtension(""));
            Assert.IsNull(PathHelper.GetFileExtension(null));
            Assert.IsTrue("t" == PathHelper.GetFileExtension("C:\\tmp\\asdf.t"));
            Assert.IsTrue("json" == PathHelper.GetFileExtension("C:\\tmp\\asdf.json"));
            Assert.IsTrue("json" == PathHelper.GetFileExtension("C:\\tmp.asdf\\asdf.json"));
            Assert.IsTrue("xml" == PathHelper.GetFileExtension("C:\\tmp\\asdf.txt.json.xml"));
        }

        private string _testFilePath = "D:\\WinFred\\Project\\james\\Engine.Test\\bin\\Debug\\files2.txt";

        [TestMethod]
        public void MultiplePathHelperFileExtensionTest()
        {
            var lines = File.ReadAllLines(_testFilePath);
            for (int i = 0; i < 1000; i++)
            {
                foreach (var line in lines)
                {
                    PathHelper.GetFileExtension(line.Split(';')[0]);
                }
            }
        }

        [TestMethod]
        public void BasicPathHelperGetFilenameTest()
        {
            Assert.IsTrue("asdf.txt" == PathHelper.GetFilename("C:\\tmp\\asdf.txt"));
            Assert.IsTrue("asdf.txt.txt" == PathHelper.GetFilename("C:\\tmp\\asdf.txt.txt"));
            Assert.IsTrue("asdf" == PathHelper.GetFilename("C:\\tmp\\asdf"));
            Assert.IsTrue("txt" == PathHelper.GetFilename("txt"));
        }

        [TestMethod]
        public void MultiplePathHelperGetFilenameTest()
        {
            var lines = File.ReadAllLines(_testFilePath);
            for (int i = 0; i < 1000; i++)
            {
                foreach (var line in lines)
                {
                    PathHelper.GetFilename(line.Split(';')[0]);
                }
            }
        }

        [TestMethod]
        public void BasicPathHelperGetFolderPathTest()
        {
            Assert.IsTrue("C:\\tmp" == PathHelper.GetFolderPath("C:\\tmp\\asdf.txt"));
            Assert.IsTrue("C:\\tmp\\asdf" == PathHelper.GetFolderPath("C:\\tmp\\asdf\\asdf.txt.txt"));
            Assert.IsNull(PathHelper.GetFolderPath("txt"));
        }

        [TestMethod]
        public void MultiplePathHelperGetFolderPathTest()
        {
            var lines = File.ReadAllLines(_testFilePath);
            for (int i = 0; i < 1000; i++)
            {
                foreach (var line in lines)
                {
                    PathHelper.GetFolderPath(line.Split(';')[0]);
                }
            }
        }

        [TestMethod]
        public void BasicPathHelperGetFullPathOfExeTest()
        {
            Assert.IsTrue(@"C:\WINDOWS\system32\notepad.exe" == PathHelper.GetFullPathOfExe("notepad.exe"));
            Assert.IsNull(PathHelper.GetFullPathOfExe("asdf123"));
        }

        [TestMethod]
        public void BasicGetLocationOfJamesTest()
        {
            Assert.IsTrue(PathHelper.GetLocationOfJames().Length > 10);
        }
    }
}
