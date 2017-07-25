using System;
using System.IO;
using System.Linq;
using Engine.Entity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Engine.Test.SearchEngineTest
{
    [TestClass]
    public class SearchEngineTest
    {
        private string _testFile = "D:\\WinFred\\Project\\james\\Engine.Test\\bin\\Debug\\files.txt";
        [TestMethod]
        public void FirstInsertTest()
        {
            SearchEngine engine = new SearchEngine();
            engine.Insert("asdf", 1);
            engine.Insert("D:\\Winfred\\james-windows\\James.sln", 10);
            Assert.IsTrue(engine.Find("Jam").First() == "D:\\Winfred\\james-windows\\James.sln;10");

            engine.Insert(@"C:\Users\moser\Desktop\reading\College_Physics-OP.pdf", 100);
            Assert.IsTrue(engine.Find(".pdf").First() == @"C:\Users\moser\Desktop\reading\College_Physics-OP.pdf;100");
            Assert.IsTrue(engine.Find("pdf").First() == @"C:\Users\moser\Desktop\reading\College_Physics-OP.pdf;100");
            Assert.IsTrue(engine.Find("Coll").First() == @"C:\Users\moser\Desktop\reading\College_Physics-OP.pdf;100");
            Assert.IsTrue(engine.Find("Phys").First() == @"C:\Users\moser\Desktop\reading\College_Physics-OP.pdf;100");

        }

        [TestMethod]
        public void RealisticInsertTest()
        {
            SearchEngine engine = new SearchEngine();
            Random ran = new Random();
            string directory = "C:\\Windows";
            foreach (var path in Directory.GetFiles(directory))
            {
                engine.Insert(path, Math.Abs(ran.Next()));
            }
            Assert.IsTrue(engine.Find("Wind").Count() == 2);
        }

        [TestMethod]
        public void RealisticInsertAndQueryManyTest()
        {
            SearchEngine engine = new SearchEngine();
            var lines = File.ReadAllLines(_testFile).Select(x => x.Split(';')).Select(x => new Tuple<string, int, string>(x[0], int.Parse(x[1]), x[0].Split('\\').Last()));
            DateTime start = DateTime.Now;
            foreach(var line in lines)
            {
                engine.Insert(line.Item1, line.Item2);
            }
            Console.WriteLine((DateTime.Now - start).TotalMilliseconds);
            start = DateTime.Now;
            for (int i = 0; i < 100; i++)
            {
                foreach (var line in lines)
                {
                    engine.Find(line.Item3);
                }
            }
            Console.WriteLine((DateTime.Now - start).TotalMilliseconds);
        }

        [TestMethod]
        public void RealisticInsertManyTest()
        {
            SearchEngine engine = new SearchEngine();
            StreamReader reader = new StreamReader(_testFile);
            while (!reader.EndOfStream)
            {
                var splits = reader.ReadLine().Split(';');
                engine.Insert(splits[0], int.Parse(splits[1]));
            }
        }

        [TestMethod]
        public void ProveOrderOfResults()
        {
            SearchEngine engine = new SearchEngine();
            InsertSamples(ref engine);

            var results = engine.Find("win").ToArray();
            Assert.IsTrue(results[0] == "D:\\Folder1\\Winfred3;90");
            Assert.IsTrue(results.Length == 4);
        }

        private void InsertSamples(ref SearchEngine engine)
        {
            engine.Insert("D:\\WinFred", 1);
            engine.Insert("D:\\WinFred\\winfred.txt", 11);
            engine.Insert("D:\\WinFredTest", 22);
            engine.Insert("D:\\WinFred\\test.txt", 33);
            engine.Insert("D:\\WinFred\\config.xml", 44);
            engine.Insert("D:\\WinFred\\Asdf", 55);
            engine.Insert("D:\\WinFred\\index.html", 66);
            engine.Insert("D:\\WinFred\\program.cs", 77);
            engine.Insert("D:\\WinFred\\project.txt", 88);
            engine.Insert("D:\\WinFred\\readme.md", 80);
            engine.Insert("D:\\Folder1\\Folder2", 100);
            engine.Insert("D:\\Folder1\\Winfred3", 90);
        }

        [TestMethod]
        public void SimpleDeleteTest()
        {
            SearchEngine engine = new SearchEngine();
            #region input
            engine.Insert(@"D:\WinFred\Project\james-windows\James.sln", 67);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngine", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\AssemblyInfo.cpp", 17);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\ReadMe.txt", 47);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\SearchEngineWrapper.cpp", 17);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\SearchEngineWrapper.h", 16);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\Stdafx.cpp", 18);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\Stdafx.h", 18);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\app.ico", 27);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\resource.h", 17);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\x64", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\x64\Release", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngineWrapper\x64\Release\SearchEngineWrapper.log", 14);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngine\main.cpp", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngine\shared", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngine\shared\SearchAlgorithm", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngine\shared\SearchAlgorithm\search-engine.cpp", 17);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngine\x64", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngine\x64\Release", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\SearchEngine\x64\Release\SearchEngine.log", 14);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\ApiListener.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\App.xaml.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Config.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Enumerations", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Enumerations\AccentColorTypes.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\CustomExtensionMethods.cs", 18);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\DefaultFileExtensions.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\DefaultPaths.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\GridHelper.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\IconHelper.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\MouseHelper.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\PathHelper.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\RegistryHelper.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\SerializationHelper.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\HelperClasses\WindowHelper.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Properties", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Properties\Annotations.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Properties\AssemblyInfo.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Properties\Resources.Designer.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Properties\Settings.Designer.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Resources", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Resources\About.png", 27);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Resources\folder.ico", 26);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Resources\logo.png", 28);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Resources\logo2.ico", 26);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Resources\logo2.png", 27);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Resources\workflow-bg.jpg", 27);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\ResultItems", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\ResultItems\MagicResultItem.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\ResultItems\ResultItem.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\ResultItems\SearchResultItem.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Search", 80);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\SearchResultElement.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Search\DeleteEvent.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Search\FileExtension.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Search\MyFileWatcher.cs", 19);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Search\Path.cs", 21);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Search\SearchEngine.cs", 20);
            engine.Insert(@"D:\WinFred\Project\james-windows\WinFred\Shortcut", 80);
            #endregion

            Assert.IsTrue(engine.Find("MagicResultItem").Count() == 1);
            engine.DeleteFile(@"D:\WinFred\Project\james-windows\WinFred\ResultItems\main.cpp");
            Assert.IsTrue(engine.Find("MagicResultItem").Count() == 1);
            engine.DeleteFile(@"D:\WinFred\Project\james-windows\WinFred\ResultItems\MagicResultItem.cs");
            Assert.IsTrue(!engine.Find("MagicResultItem").Any());
        }

        [TestMethod]
        public void RealisticInsertAndDeleteManyTest()
        {
            SearchEngine engine = new SearchEngine(_testFile);
            StreamReader reader = new StreamReader(_testFile);

            while (!reader.EndOfStream)
            {
                var splits = reader.ReadLine().Split(';');
                engine.DeleteFile(splits[0]);
            }
            
            Assert.IsTrue(!engine.Find("?").Any());
            for (int i = 'A'; i <= 'Z'; i++)
            {
                Assert.IsTrue(!engine.Find(((char)i).ToString()).Any());
            }
        }

        [TestMethod]
        public void SpecificTestForTestingPurpose()
        {
            SearchEngine engine = new SearchEngine();
            engine.Insert("C:\\Users\\moser\\OneDrive\\Schule\\4BHIF\\DBI\\apex\\%C3%9Cbung 1", 6);
            engine.DeletePath("C:\\Users\\moser\\OneDrive\\Schule\\4BHIF\\DBI\\apex\\%C3%9Cbung 1");
            Assert.IsTrue(!engine.Find("X").Any());
        }

        [TestMethod]
        public void SimpleChangePriorityTest()
        {
            SearchEngine engine = new SearchEngine();
            InsertSamples(ref engine);

            var results = engine.Find("win").ToArray();
            Assert.IsTrue(results[0] == "D:\\Folder1\\Winfred3;90");
            Assert.IsTrue(results.Length == 4);

            engine.ChangePriority(@"D:\WinFred\index.html", 30);

            var results2 = engine.Find("ind").ToArray();
            Assert.IsTrue(results2[0] == "D:\\WinFred\\index.html;96");

            engine.ChangePriority(@"D:\WinFred\index.html", -30);
            Assert.IsTrue(engine.Find("ind").ToArray()[0] == "D:\\WinFred\\index.html;66");
        }

        private readonly string _testFilePath = "D:\\WinFred\\Project\\james\\Engine.Test\\bin\\Debug\\files2.txt";
        [TestMethod]
        public void RealisticInsertAndSaveManyTest()
        {
            SearchEngine engine = new SearchEngine(_testFile);
            engine.Save(_testFilePath);
            Assert.IsTrue(File.ReadAllLines(_testFilePath).Length == 23754);
        }
        [TestMethod]
        public void RealisticInsertDeletePathAndSaveManyTest()
        {
            SearchEngine engine = new SearchEngine(_testFile);
            engine.DeletePath(@"C:\Users\moser\Dropbox\SirDrawALot Presentation\");
            engine.Save(_testFilePath);
            Assert.IsTrue(File.ReadAllLines(_testFilePath).Length == 23754 - 47);
        }

        [TestMethod]
        public void BasicRenameFileTest()
        {
            SearchEngine engine = new SearchEngine();
            InsertSamples(ref engine);
            engine.Rename("D:\\WinFred\\test.txt", "D:\\WinFred\\asdf.txt");
            Assert.IsTrue(engine.Find("asdf").Skip(1).First().Split(';')[1] == "33");
            engine.Rename("D:\\WinFred\\asdf.txt", "D:\\moser.txt");
            Assert.IsTrue(engine.Find("moser").First() == "D:\\moser.txt;33");
            Assert.IsTrue(engine.Find("asdf").Count() == 1);
        }
    }
}
