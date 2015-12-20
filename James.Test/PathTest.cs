using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using James.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace James.Test
{
    [TestClass]
    public class PathTest
    {

        private Path CreateBasicPath()
        {
            var fileExtensions = new Dictionary<string, int>();
            fileExtensions["txt"] = 20;
            fileExtensions["exe"] = 40;
            fileExtensions["pdf"] = 30;
            fileExtensions["md5"] = 20;
            return new Path() { Location = @"C:\Users\Moser", Priority = 20, IsDefaultConfigurationEnabled = true, FileExtensions = fileExtensions};
        }

        [TestMethod]
        public void BasicCalcPriorityTest()
        {
            var path = CreateBasicPath();
            Assert.IsTrue(path.GetPathPriority(@"C:\Users\Moser\asdf.txt") == 40, "should be 40");
            Assert.IsTrue(path.GetPathPriority(@"C:\Users\Moser\asdf.exe") == 60, "should be 60");
            Assert.IsTrue(path.GetPathPriority(@"C:\Users\Moser\asdf.asdf") == -1, "should be -1");
        }

        [TestMethod]
        public void FallbackDefaultPriorityTest()
        {
            var path = CreateBasicPath();
            Config.Instance.DefaultFileExtensions["testEnding"] = 10;
            Assert.IsTrue(path.GetPathPriority(@"C:\Users\Moser\asdf.txt") == 40, "should be 40");
            Assert.IsTrue(path.GetPathPriority(@"C:\Users\Moser\asdf.exe") == 60, "should be 60");
            Assert.IsTrue(path.GetPathPriority(@"C:\Users\Moser\asdf.testEnding") == 10, "should be -1");
        }
    }
}
