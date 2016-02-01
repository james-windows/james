using System;
using System.Collections.Generic;
using System.Linq;
using James.ResultItems;
using James.Search;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace James.Test
{
    [TestClass]
    public class SearchEngineTest
    {
        #region helper
        public void PrepareTest()
        {
            Config.Instance.ConfigFolderLocation = Environment.CurrentDirectory;
        }

        public void CompareSearchResults(List<ResultItem> one, List<ResultItem> two)
        {
            Assert.IsTrue(one.Count == two.Count, "Length of the two lists should be equal");
            for (int i = 0; i < one.Count; i++)
            {
                Assert.IsTrue(one[i].Subtitle == two[i].Subtitle, $"Path of the {i}.Path are not equal");
                Assert.IsTrue(one[i].Priority == two[i].Priority, $"Priority of the {i}.Path are not equal");
            }
        }

        public List<SearchResultItem> GenerateSearchResults()
        {
            List<SearchResultItem> searchResults = new List<SearchResultItem>
            {
                new SearchResultItem(@"C:\User\Moser\Desktop\firstFile.txt", 10),
                new SearchResultItem(@"C:\User\Moser\Desktop\secondFile.txt", 9),
                new SearchResultItem(@"C:\User\Moser\Desktop\firstExe.exe", 100),
                new SearchResultItem(@"C:\User\Moser\Desktop\SecondExe.exe", 99),
                new SearchResultItem(@"C:\User\Moser\Desktop\tmp\firstFile.txt", 8),
                new SearchResultItem(@"C:\User\Moser\Desktop\tmp\secondFile.txt", 7),
                new SearchResultItem(@"C:\User\Moser\Desktop\tmp\", 80),
                new SearchResultItem(@"C:\User\Moser\Desktop\tmpFolder\", 80)
            };
            return searchResults;
        }
        #endregion

        [TestMethod]
        public void CreatingInstanceTest()
        {
            PrepareTest();
            SearchEngine searchEngine = SearchEngine.Instance;
            Assert.IsNotNull(searchEngine, "Instance should be created!");
        }

        #region Insert
        [TestMethod]
        public void InsertOneTest()
        {
            PrepareTest();
            ResultItem testPath = new SearchResultItem(@"C:\User\Moser\Desktop\firstFile.txt", 0);
            
            SearchEngine.Instance.AddFile(testPath);
            var query = SearchEngine.Instance.Query("first");

            Assert.IsTrue(query.Count == 1, "One result should be returned!");
            Assert.IsTrue(query[0].Subtitle == testPath.Subtitle, "Both paths should match!");
        }

        [TestMethod]
        public void InsertOneTwiceTest()
        {
            PrepareTest();
            ResultItem testPath = new SearchResultItem(@"C:\User\Moser\Desktop\firstFile.txt", 8);

            SearchEngine.Instance.AddFile(testPath);
            testPath.Priority = 10;
            SearchEngine.Instance.AddFile(testPath);
            var query = SearchEngine.Instance.Query("first");
            Assert.IsTrue(query.Count == 1, "One result should be returned!");
            Assert.IsTrue(query[0].Subtitle == testPath.Subtitle, "Both paths should match!");
            Assert.IsTrue(query[0].Priority == testPath.Priority, "Path should be overriden by the higher priority!");
        }

        [TestMethod]
        public void InsertManyTest()
        {
            PrepareTest();
            var searchResults = GenerateSearchResults();
            foreach (var item in searchResults)
            {
                SearchEngine.Instance.AddFile(item);
            }
            var query = SearchEngine.Instance.Query("f");
            var search =
                searchResults.Where(result => result.Subtitle.Split('\\').Last().StartsWith("f"))
                    .OrderBy(result => -result.Priority).ToList().ConvertAll(input => (ResultItem)input); ;

            CompareSearchResults(query, search);
        }
        #endregion

        #region Rename

        [TestMethod]
        public void RenameOneTest()
        {
            PrepareTest();

            InsertManyTest();
            var query = SearchEngine.Instance.Query("first");

            SearchEngine.Instance.RenameFile(query[0].Subtitle, query[0].Subtitle + "rename");
            query[0].Subtitle += "rename";

            var secondQuery = SearchEngine.Instance.Query("first");
            CompareSearchResults(query, secondQuery);
        }

        [TestMethod]
        public void RenameOneFolderTest()
        {
            PrepareTest();

            InsertManyTest();
            var query = SearchEngine.Instance.Query("first");

            SearchEngine.Instance.RenameFile(@"C:\User\Moser\Desktop", @"C:\User\Moser\Documents");
            query =
                query.Select(
                    result =>
                        new SearchResultItem()
                        {
                            Subtitle = result.Subtitle.Replace("Desktop", "Documents"),
                            Priority = result.Priority
                        }).ToList().ConvertAll(input => (ResultItem)input);

            var secondQuery = SearchEngine.Instance.Query("first");
            CompareSearchResults(query, secondQuery);
        }

        [TestMethod]
        public void CheckPriorityAfterRename()
        {
            PrepareTest();

            InsertManyTest();
            SearchEngine.Instance.IncrementPriority("C:\\User\\Moser\\Desktop\\firstExe.exe", 10);
            var query = SearchEngine.Instance.Query("first");
            SearchEngine.Instance.RenameFile(@"C:\User\Moser\Desktop", @"C:\User\Moser\Documents");
            query =
                query.Select(
                    result =>
                        new SearchResultItem()
                        {
                            Subtitle = result.Subtitle.Replace("Desktop", "Documents"),
                            Priority = result.Priority
                        }).ToList().ConvertAll(input => (ResultItem)input);

            var secondQuery = SearchEngine.Instance.Query("first");
            CompareSearchResults(query, secondQuery);
        }
        #endregion

        #region AddPriority
        [TestMethod]
        public void AddPriorityTest()
        {
            InsertManyTest();

            var query = SearchEngine.Instance.Query("f");
            SearchEngine.Instance.IncrementPriority(query[2].Subtitle, 1000);

            var secondQuery = SearchEngine.Instance.Query("f");

            Assert.IsTrue(secondQuery.Count == query.Count, "Count shouldn't be changed");
            Assert.IsTrue(query[2].Subtitle == secondQuery[0].Subtitle);
            Assert.IsTrue(query[2].Priority + 1000 == secondQuery[0].Priority);
        }

        [TestMethod]
        public void DecrementPriorityTest()
        {
            InsertManyTest();

            var query = SearchEngine.Instance.Query("f");
            SearchEngine.Instance.IncrementPriority(query[0].Subtitle, -10);

            var secondQuery = SearchEngine.Instance.Query("f");

            Assert.IsTrue(secondQuery.Count == query.Count, "Count shouldn't be changed");
            Assert.IsTrue(query[0].Subtitle == secondQuery[0].Subtitle);
            Assert.IsTrue(query[0].Priority + -10 == secondQuery[0].Priority);
        }

        [TestMethod]
        public void DecrementPriorityToNegativeTest()
        {
            InsertManyTest();

            var query = SearchEngine.Instance.Query("f");
            SearchEngine.Instance.IncrementPriority(query[0].Subtitle, -1000);

            var secondQuery = SearchEngine.Instance.Query("f");
            query.RemoveAt(0);
            CompareSearchResults(query, secondQuery);
        }
        #endregion

        #region Delete
        [TestMethod]
        public void DeleteOneTest()
        {
            InsertManyTest();
            var query = SearchEngine.Instance.Query("f");
            SearchEngine.Instance.DeletePath(query[0].Subtitle);
            var secondQuery = SearchEngine.Instance.Query("f");

            query.RemoveAt(0);
            CompareSearchResults(query, secondQuery);
        }

        [TestMethod]
        public void DeleteOneWhichDoesntExsistsTest()
        {
            InsertManyTest();
            var query = SearchEngine.Instance.Query("f");
            SearchEngine.Instance.DeletePath(query[0].Subtitle + "something");
            var secondQuery = SearchEngine.Instance.Query("f");
            CompareSearchResults(query, secondQuery);
        }
        #endregion
    }
}
