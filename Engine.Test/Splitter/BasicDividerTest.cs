using System.Linq;
using Engine.Divider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Test.Splitter
{
    [TestClass]
    public class BasicDividerTest
    {
        [TestMethod]
        public void InitializeObjectTest()
        {
            IDivider divider = new Divider.Divider();
            Assert.IsNotNull(divider);
        }

        [TestMethod]
        public void DivideByUppercaseTest()
        {
            IDivider divider = new Divider.Divider();
            Assert.IsTrue(divider.SplitPath("a").Count() == 1);
            Assert.IsTrue(divider.SplitPath("aa").Count() == 1);
            Assert.IsTrue(divider.SplitPath("aaa").Count() == 1);
            Assert.IsTrue(divider.SplitPath("aBa").Count() == 2);
            Assert.IsTrue(divider.SplitPath("aBBa").Count() == 2);
            Assert.IsTrue(divider.SplitPath("aBaa").Count() == 2);
            Assert.IsTrue(divider.SplitPath("aaB").Count() == 1);
            Assert.IsTrue(divider.SplitPath("Baa").Count() == 1);
            Assert.IsTrue(divider.SplitPath("BaBaBa").Count() == 3);
        }

        [TestMethod]
        public void DivideBySpecialCaseTest()
        {
            IDivider divider = new Divider.Divider();
            Assert.IsTrue(divider.SplitPath("a.").Count() == 1);
            Assert.IsTrue(divider.SplitPath("a.a").Count() == 3);
            Assert.IsTrue(divider.SplitPath("a.aa").Count() == 3);
            Assert.IsTrue(divider.SplitPath(".aa").Count() == 2);
            Assert.IsTrue(divider.SplitPath(".").Count() == 1);
            Assert.IsTrue(divider.SplitPath("._").Count() == 2);
            Assert.IsTrue(divider.SplitPath("hallo.hallo").Count() == 3);
            Assert.IsTrue(divider.SplitPath("hallo .hallo").Count() == 3);
            Assert.IsTrue(divider.SplitPath("hallo hallo").Count() == 2);
            Assert.IsTrue(divider.SplitPath("hallo . hallo").Count() == 3);
            Assert.IsTrue(divider.SplitPath("hallo.hallo.txt").Count() == 4);
            Assert.IsTrue(divider.SplitPath("hallo.hallo.txt.txt").Count() == 5);
            Assert.IsTrue(divider.SplitPath("hallo_._hallo").Count() == 4);
            Assert.IsTrue(divider.SplitPath("hallo@hallo").Count() == 2);
            Assert.IsTrue(divider.SplitPath("@hallo").Count() == 2);
        }

        [TestMethod]
        public void DivideByNumberTest()
        {
            IDivider divider = new Divider.Divider();
            Assert.IsTrue(divider.SplitPath("aaa123aaa").Count() == 3);
            Assert.IsTrue(divider.SplitPath("123").Count() == 1);
            Assert.IsTrue(divider.SplitPath("123aa").Count() == 2);
            Assert.IsTrue(divider.SplitPath("A1A1A1").Count() == 5);
            Assert.IsTrue(divider.SplitPath("123a123").Count() == 3);
            Assert.IsTrue(divider.SplitPath("123.123").Count() == 3);
            Assert.IsTrue(divider.SplitPath("123_123").Count() == 2);
        }

        [TestMethod]
        public void DivideByAllCasesTest()
        {
            IDivider divider = new Divider.Divider();
            Assert.IsTrue(divider.SplitPath("Image Processing, 2nd Edition.pdf").Count() == 7);
            Assert.IsTrue(divider.SplitPath("James.exe - Shortcut.lnk").Count() == 6);
            Assert.IsTrue(divider.SplitPath("VID_20160408_120150.mp4").Count() == 5);
            Assert.IsTrue(divider.SplitPath("Visual Studio 15 Preview(Lightweight Install).lnk").Count() == 8);
            Assert.IsTrue(divider.SplitPath("Visual Studio 15 Preview (Lightweight Install).lnk").Count() == 9);
            Assert.IsTrue(divider.SplitPath("Microsoft® Visual Studio® 2015.lnk").Count() == 6);
            Assert.IsTrue(divider.SplitPath("TCPServer.zip").Count() == 3);
            Assert.IsTrue(divider.SplitPath("appicon.png").Count() == 3);
            Assert.IsTrue(divider.SplitPath("Project Proposal.pdf").Count() == 4);
            Assert.IsTrue(divider.SplitPath("DrawRobot.png").Count() == 4);
            Assert.IsTrue(divider.SplitPath("backup_sysspec.tex").Count() == 4);
            Assert.IsTrue(divider.SplitPath("Planning and Monitoring Sheet.xlsx").Count() == 6);
            Assert.IsTrue(divider.SplitPath("SirDrawALot").Count() == 3);
            Assert.IsTrue(divider.SplitPath("SignalR").Count() == 1);
            Assert.IsTrue(divider.SplitPath("sapgui730").Count() == 2);
            Assert.IsTrue(divider.SplitPath("Markdown-Edit").Count() == 2);
        }
    }
}
