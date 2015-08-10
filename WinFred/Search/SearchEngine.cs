using System;
using System.Collections.Generic;
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

        public static SearchEngine GetInstance()
        {
            lock (SingeltonLock)
            {
                return _searchEngine ?? (_searchEngine = new SearchEngine());
            }
        }

        public void BuildIndex()
        {
            var data = new List<Document>();
            Parallel.ForEach(Config.GetInstance().Paths.Where(path => path.IsEnabled), currentPath =>
            {
                var currentFolder = new Folder(currentPath);
                data.AddRange(currentFolder.getItemsToBeIndexed());
            });
            using (var writer = new IndexWriter(_index, _analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                data.ForEach(ElementToBeIndexed => writer.AddDocument(ElementToBeIndexed));
                writer.Optimize();
                writer.Commit();
            }
        }

        public void AddFile(Data data)
        {
            using (var writer = new IndexWriter(_index, _analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.AddDocument(data.GetDocument());
                writer.Optimize();
                writer.Commit();
            }
        }

        internal void RenameFile(string fullPath)
        {
            //todo
        }

        public List<SearchResult> Query(string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Trim().Length < Config.GetInstance().StartSearchMinTextLength)
                return new List<SearchResult>();
            str = str.ToLower();
            Query query = new PrefixQuery(new Term("FileName", str.Trim()));
            using (var reader = IndexReader.Open(_index, true))
            {
                using (Searcher searcher = new IndexSearcher(reader))
                {
                    var docs = searcher.Search(query, new QueryWrapperFilter(query),
                        Config.GetInstance().MaxSearchResults, _sort);
                    var res = docs.ScoreDocs;

                    var match = new List<SearchResult>();
                    foreach (var scoreDoc in res)
                    {
                        var doc = searcher.Doc(scoreDoc.Doc);
                        match.Add(new SearchResult
                        {
                            Id = doc.Get("Id"),
                            Priority = Convert.ToInt32(doc.Get("Priority")),
                            Path = doc.Get("Path")
                        });
                    }
                    return match;
                }
            }
        }

        public void IncrementPriority(SearchResult result)
        {
            var data = new Data(result.Path) {Id = result.Id, Priority = Math.Abs(result.Priority) + 5};
            using (var writer = new IndexWriter(_index, _analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.UpdateDocument(new Term("Id", result.Id), data.GetDocument());
                writer.Optimize();
                writer.Commit();
            }
        }

        public void DeleteFile(string path)
        {
            using (var writer = new IndexWriter(_index, _analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                Query query = new PrefixQuery(new Term("Path", path.ToLower()));
                writer.DeleteDocuments(query);
                writer.Optimize();
                writer.Commit();
            }
        }

        public void IncrementPriority(string path)
        {
            using (var reader = IndexReader.Open(_index, true))
            {
                Query query = new PrefixQuery(new Term("Path", path.ToLower()));
                using (Searcher searcher = new IndexSearcher(reader))
                {
                    var res = searcher.Search(query, new QueryWrapperFilter(query), 10, _sort).ScoreDocs;
                    if (res.Length == 0)
                    {
                        return;
                    }
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
}