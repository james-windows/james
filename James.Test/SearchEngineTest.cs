using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using James.Search;
using James.Workflows.Outputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace James.Test
{
    [TestClass]
    public class SearchEngineTest
    {
        #region helper
        public void PrepareTest()
        {
            Config.Instance.ConfigFolderLocation = "";
        }

        public void CompareSearchResults(List<SearchResult> one, List<SearchResult> two)
        {
            Assert.IsTrue(one.Count == two.Count, "Length of the two lists should be equal");
            for (int i = 0; i < one.Count; i++)
            {
                Assert.IsTrue(one[i].Path == two[i].Path, $"Path of the {i}.Path are not equal");
                Assert.IsTrue(one[i].Priority == two[i].Priority, $"Priority of the {i}.Path are not equal");
            }
        }

        public List<SearchResult> GenerateSearchResults()
        {
            List<SearchResult> searchResults = new List<SearchResult>
            {
                new SearchResult() {Path = @"C:\User\Moser\Desktop\firstFile.txt", Priority = 10},
                new SearchResult() {Path = @"C:\User\Moser\Desktop\secondFile.txt", Priority = 9},
                new SearchResult() {Path = @"C:\User\Moser\Desktop\firstExe.exe", Priority = 100},
                new SearchResult() {Path = @"C:\User\Moser\Desktop\SecondExe.exe", Priority = 99},
                new SearchResult() {Path = @"C:\User\Moser\Desktop\tmp\firstFile.txt", Priority = 8},
                new SearchResult() {Path = @"C:\User\Moser\Desktop\tmp\secondFile.txt", Priority = 7},
                new SearchResult() {Path = @"C:\User\Moser\Desktop\tmp\", Priority = 80},
                new SearchResult() {Path = @"C:\User\Moser\Desktop\tmpFolder\", Priority = 80}
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
            SearchResult testPath = new SearchResult() {Path = @"C:\User\Moser\Desktop\firstFile.txt"};
            
            SearchEngine.Instance.AddFile(testPath);
            var query = SearchEngine.Instance.Query("first");

            Assert.IsTrue(query.Count == 1, "One result should be returned!");
            Assert.IsTrue(query[0].Path == testPath.Path, "Both paths should match!");
        }

        [TestMethod]
        public void InsertOneTwiceTest()
        {
            PrepareTest();
            SearchResult testPath = new SearchResult() { Path = @"C:\User\Moser\Desktop\firstFile.txt", Priority = 8};

            SearchEngine.Instance.AddFile(testPath);
            testPath.Priority = 10;
            SearchEngine.Instance.AddFile(testPath);
            var query = SearchEngine.Instance.Query("first");
            Assert.IsTrue(query.Count == 1, "One result should be returned!");
            Assert.IsTrue(query[0].Path == testPath.Path, "Both paths should match!");
            Assert.IsTrue(query[0].Priority == testPath.Priority, "Path should be overriden by the higher priority!");
        }

        [TestMethod]
        public void InsertManyTest()
        {
            PrepareTest();
            var searchResults = GenerateSearchResults();
            foreach (SearchResult item in searchResults)
            {
                SearchEngine.Instance.AddFile(item);
            }
            var query = SearchEngine.Instance.Query("f");
            var search =
                searchResults.Where(result => result.Path.Split('\\').Last().StartsWith("f"))
                    .OrderBy(result => -result.Priority).ToList();

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

            SearchEngine.Instance.RenameFile(query[0].Path, query[0].Path + "rename");
            query[0].Path += "rename";

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
                        new SearchResult()
                        {
                            Path = result.Path.Replace("Desktop", "Documents"),
                            Priority = result.Priority
                        }).ToList();

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
                        new SearchResult()
                        {
                            Path = result.Path.Replace("Desktop", "Documents"),
                            Priority = result.Priority
                        }).ToList();

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
            SearchEngine.Instance.IncrementPriority(query[2].Path, 1000);

            var secondQuery = SearchEngine.Instance.Query("f");

            Assert.IsTrue(secondQuery.Count == query.Count, "Count shouldn't be changed");
            Assert.IsTrue(query[2].Path == secondQuery[0].Path);
            Assert.IsTrue(query[2].Priority + 1000 == secondQuery[0].Priority);
        }

        [TestMethod]
        public void DecrementPriorityTest()
        {
            InsertManyTest();

            var query = SearchEngine.Instance.Query("f");
            SearchEngine.Instance.IncrementPriority(query[0].Path, -10);

            var secondQuery = SearchEngine.Instance.Query("f");

            Assert.IsTrue(secondQuery.Count == query.Count, "Count shouldn't be changed");
            Assert.IsTrue(query[0].Path == secondQuery[0].Path);
            Assert.IsTrue(query[0].Priority + -10 == secondQuery[0].Priority);
        }

        [TestMethod]
        public void DecrementPriorityToNegativeTest()
        {
            InsertManyTest();

            var query = SearchEngine.Instance.Query("f");
            SearchEngine.Instance.IncrementPriority(query[0].Path, -1000);

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
            SearchEngine.Instance.DeletePath(query[0].Path);
            var secondQuery = SearchEngine.Instance.Query("f");

            query.RemoveAt(0);
            CompareSearchResults(query, secondQuery);
        }

        [TestMethod]
        public void DeleteOneWhichDoesntExsistsTest()
        {
            InsertManyTest();
            var query = SearchEngine.Instance.Query("f");
            SearchEngine.Instance.DeletePath(query[0].Path + "something");
            var secondQuery = SearchEngine.Instance.Query("f");
            CompareSearchResults(query, secondQuery);
        }
        #endregion
    }
}
