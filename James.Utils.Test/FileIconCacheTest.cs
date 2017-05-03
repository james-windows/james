using System;
using System.IO;
using James.HelperClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace James.Utils.Test
{
    [TestClass]
    public class FileIconCacheTest
    {
        [TestMethod]
        public void BasicFileExtensionCacheTest()
        {
            Assert.IsNull(FileIconCache.Instance.GetFileIcon(""));
            Assert.IsNotNull(FileIconCache.Instance.GetFileIcon("C:\\Windows"));
            // ReSharper disable once EqualExpressionComparison
            Assert.AreEqual(FileIconCache.Instance.GetFileIcon("C:\\Windows"), FileIconCache.Instance.GetFileIcon("C:\\Windows"));
            Assert.AreEqual(FileIconCache.Instance.GetFileIcon("C:\\Windows"), FileIconCache.Instance.GetFileIcon("C:\\Windows\\System32"));
        }

        private static string _testFilePath = "D:\\WinFred\\Project\\james\\Engine.Test\\bin\\Debug\\files2.txt";
        [TestMethod]
        public void ManyFileExtensionCacheTest()
        {
            var lines = File.ReadAllLines(_testFilePath);
            foreach (var line in lines)
            {
                FileIconCache.Instance.GetFileIcon(line.Split(';')[0]);
            }
            int tmp = FileIconCache.Instance.CachedIconsCount;
            foreach (var line in lines)
            {
                FileIconCache.Instance.GetFileIcon(line.Split(';')[0]);
            }
            Assert.AreEqual(tmp, FileIconCache.Instance.CachedIconsCount);
        }
    }
}
