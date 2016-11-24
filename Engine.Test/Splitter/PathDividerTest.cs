using System.Linq;
using Engine.Divider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Test.Splitter
{
    [TestClass]
    public class PathDividerTest
    {
        [TestMethod]
        public void InitializeObjectTest()
        {
            IDivider divider = new Divider.Divider();
            Assert.IsNotNull(divider);
        }

        [TestMethod]
        public void BasicDividePathTest()
        {
            IDivider divider = new Divider.Divider();
            Assert.IsTrue(divider.SplitPath("D:\\WinFred\\logins.txt").Count() == 3);
            Assert.IsTrue(divider.SplitPath("C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe").Count() == 3);
            Assert.IsTrue(divider.SplitPath(@"C:\Users\moser\OneDrive\Schule\5BHIF\M\Geometrie\Beispiel 36.pdf").Count() == 4);
            Assert.IsTrue(
                divider.SplitPath(@"C:\Users\moser\OneDrive\Schule\5BHIF\M\Wiederholung Differential- und Integralrechnung\Kapitel 5 und 6 Differential- und Integralrechnung.pdf")
                .Count() == 9);
        }

        [TestMethod]
        public void AdvancedDividerTest()
        {
            IDivider divider = new Divider.Divider();
            var results = divider.SplitPath(@"C:\Users\moser\Desktop\reading\College_Physics-OP.pdf");
            string[] test = { "College_Physics-OP.pdf", "Physics-OP.pdf", "OP.pdf", "pdf", ".pdf" };

            Assert.IsTrue(results.Count() == test.Length);
            foreach (var item in results)
            {
                Assert.IsTrue(test.Any(s => s == item));
            }
        }
    }
}
