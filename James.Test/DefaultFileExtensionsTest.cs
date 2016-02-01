using System;
using James.HelperClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace James.Test
{
    [TestClass]
    public class DefaultFileExtensionsTest
    {
        [TestMethod]
        public void CheckIfOnlyOccursOnes()
        {
            bool foundError = false;
            var fileExtensions = DefaultFileExtensions.GetDefault();
            foreach (var fileExtension in fileExtensions)
            {
                foreach (var item in fileExtensions)
                {
                    if (fileExtension.Extension == item.Extension && item != fileExtension)
                    {
                        foundError = true;
                        Console.WriteLine($"{item.Extension} occures at least twice");
                    }
                }
            }
            Assert.IsFalse(foundError);
        }
    }
}
