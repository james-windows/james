using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using WinFred.Search.IndexGeneration;
using LuceneDirectory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace WinFred.Search
{
    internal class SearchEngine
    {
        public delegate void ChangedBuildingIndexProgressEventHandler(object sender, ProgressChangedEventArgs e);

        private static SearchEngine _searchEngine;
        private static readonly object SingeltonLock = new object();
        private readonly Analyzer _analyzer;
        private readonly LuceneDirectory _index;
        private readonly Sort _sort;

        private SearchEngine()
        {
            _index = FSDirectory.Open(Config.GetInstance().ConfigFolderLocation + "\\Index");
            _analyzer = new StandardAnalyzer(Version.LUCENE_30);
            _sort = new Sort(new SortField("Priority", SortField.INT, true));
        }

        public event ChangedBuildingIndexProgressEventHandler ChangedBuildingIndexProgress;

        public static SearchEngine GetInstance()
        {
            lock (SingeltonLock)
            {
                return _searchEngine ?? (_searchEngine = new SearchEngine());
            }
        }

        public void BuildIndex()
        {
            var data = GetFilesToBeIndexed();
            WriteFileToIndex(data);
        }

        private List<Document> GetFilesToBeIndexed()
        {
            var data = new List<Document>();
            Parallel.ForEach(Config.GetInstance().Paths.Where(path => path.IsEnabled), currentPath =>
            {
                var currentFolder = new Folder(currentPath);
                data.AddRange(currentFolder.getItemsToBeIndexed());
            });
            return data;
        }

        private void WriteFileToIndex(List<Document> data)
        {
            var lastProgress = -1;
            using (var writer = new IndexWriter(_index, _analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                for (var i = 0; i < data.Count; i++)
                {
                    ChangeProgress(i, data.Count, ref lastProgress);
                    writer.AddDocument(data[i]);
                }
                ChangedBuildingIndexProgress?.Invoke(this, new ProgressChangedEventArgs(100, null));
                writer.Optimize();
                writer.Commit();
            }
        }

        private void ChangeProgress(int position, int total, ref int lastProgress)
        {
            if (lastProgress != CalcProgress(position, total))
            {
                lastProgress = CalcProgress(position, total);
                ChangedBuildingIndexProgress?.Invoke(this, new ProgressChangedEventArgs(lastProgress, null));
            }
        }

        private static int CalcProgress(int position, int total) => (int) (((double) position)/total*100);

        public void AddFile(Data data)
        {
            try
            { 
                using (var writer = new IndexWriter(_index, _analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    writer.AddDocument(data.GetDocument());
                    writer.Optimize();
                    writer.Commit();
                }
            }
            catch (FileNotFoundException e) { }
    }

        internal void RenameFile(string oldPath, string newPath)
        {
            try
            { 
                using(var reader = IndexReader.Open(_index, true))
                {
                    Query query = new PrefixQuery(new Term("Path", oldPath.ToLower()));
                    using (Searcher searcher = new IndexSearcher(reader))
                    {
                        var res = searcher.Search(query, new QueryWrapperFilter(query), 10, _sort).ScoreDocs;
                        if (res.Length != 0)
                        {
                            var scoreDoc = res[0];
                            var doc = searcher.Doc(scoreDoc.Doc);
                            var item = new SearchResult
                            {
                                Id = doc.Get("Id"),
                                Priority = Convert.ToInt32(doc.Get("Priority")),
                                Path = newPath
                            };
                            IncrementPriority(item);
                        }
                    }
                }
            }
            catch (FileNotFoundException e) { }
        }

        public List<SearchResult> Query(string str)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str) || str.Trim().Length < Config.GetInstance().StartSearchMinTextLength)
                    return new List<SearchResult>();
                Query query = new PrefixQuery(new Term("FileName", str.ToLower().Trim()));
                using (var reader = IndexReader.Open(_index, true))
                {
                    using (Searcher searcher = new IndexSearcher(reader))
                    {
                        var docs = searcher.Search(query, new QueryWrapperFilter(query),
                            Config.GetInstance().MaxSearchResults, _sort);
                        return
                            docs.ScoreDocs.Select(scoreDoc => searcher.Doc(scoreDoc.Doc)).Select(doc => new SearchResult
                            {
                                Id = doc.Get("Id"),
                                Priority = Convert.ToInt32(doc.Get("Priority")),
                                Path = doc.Get("Path")
                            }).ToList();
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                return new List<SearchResult> ();
            }
        }

        public void DeleteFile(string path)
        {
            try
            { 
                using (var writer = new IndexWriter(_index, _analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    Query query = new PrefixQuery(new Term("Path", path.ToLower()));
                    writer.DeleteDocuments(query);
                    writer.Optimize();
                    writer.Commit();
                }
            }
            catch (FileNotFoundException e) { }
}

        public void IncrementPriority(SearchResult result)
        {
            try
            { 

                var data = new Data(result.Path) { Id = result.Id, Priority = Math.Abs(result.Priority) + 5 };
                using (var writer = new IndexWriter(_index, _analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    writer.UpdateDocument(new Term("Id", result.Id), data.GetDocument());
                    writer.Optimize();
                    writer.Commit();
                }
            }
            catch (FileNotFoundException e) { }
}

        public void IncrementPriority(string path)
        {
            try
            {
                using (var reader = IndexReader.Open(_index, true))
                {
                    Query query = new PrefixQuery(new Term("Path", path.ToLower()));
                    using (Searcher searcher = new IndexSearcher(reader))
                    {
                        var res = searcher.Search(query, new QueryWrapperFilter(query), 10, _sort).ScoreDocs;
                        if (res.Length != 0)
                        {
                            var scoreDoc = res[0];
                            var doc = searcher.Doc(scoreDoc.Doc);
                            var item = new SearchResult
                            {
                                Id = doc.Get("Id"),
                                Priority = Convert.ToInt32(doc.Get("Priority")) + 5,
                                Path = doc.Get("Path")
                            };
                            IncrementPriority(item);
                        }
                    }
                }
            }
            catch (FileNotFoundException e) { }
        }
    }
}