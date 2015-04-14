using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;

namespace WinFred
{
    class SearchEngine
    {
        private static SearchEngine searchEngine;
        private static object singeltonLock = new object();

        public  static SearchEngine GetInstance()
        {
            lock (singeltonLock)
            {
                if (searchEngine == null)
                    searchEngine = new SearchEngine();
                return searchEngine;
            }
        }

        Directory index;
        Sort sort;
        Analyzer analyzer;

        private SearchEngine()
        {
            index = FSDirectory.Open(Config.GetInstance().ConfigFolderLocation + "\\Index");
            analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            sort = new Sort(new SortField[] { SortField.FIELD_SCORE, new SortField("Priority", SortField.INT) });
        }

        public void BuildIndex()
        {
            List<Data> data = new List<Data>();
            DateTime date = DateTime.Now;
            foreach (var item in Config.GetInstance().Paths)
            {
                data.AddRange(GetFiles(item.Location, "*"));
            }
            Debug.WriteLine((DateTime.Now - date).TotalMilliseconds);
            using (IndexWriter writer = new IndexWriter(index, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (Data a in data)
                    writer.AddDocument(a.GetDocument());
                writer.Optimize();
            }
            Trace.WriteLine("index built! Count: " + data.Count);
        }
        private List<Data> GetFiles(string path, string pattern)
        {
            List<Data> data = new List<Data>();
            try
            {
                foreach (string item in System.IO.Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly))
                {
                    //data.Add(new Data(item));
                    int index = Config.GetInstance().DefaultFileExtensions.BinarySearch(new FileExtension(item.Split('.').Last(), 0));
                    if (index >= 0)
                    {
                        data.Add(new Data(item, Config.GetInstance().DefaultFileExtensions[index].Priority));
                    }
                }
                foreach (var directory in System.IO.Directory.GetDirectories(path))
                    data.AddRange(GetFiles(directory, pattern));
                if (data.Count > 0)
                {
                    data.Add(new Data(path) {Priority = 1});
                }            
            }
            catch (UnauthorizedAccessException) { }
            return data;
        }

        public void AddFile(Data data)
        {
            using (IndexWriter writer = new IndexWriter(index, analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.AddDocument(data.GetDocument());
                writer.Optimize();
            }
        }

        public List<SearchResult> Query(string str)
        {
            int limit = Config.GetInstance().MaxSearchResults;
            if (string.IsNullOrWhiteSpace(str) || str.Trim().Length < Config.GetInstance().StartSearchMinTextLength)
                return new List<SearchResult>();
            str = str.Replace('-', ' ').Replace(" ", "").ToLower();
            Query query = new PrefixQuery(new Term("FileName", str.Trim()));
            using (IndexReader reader = IndexReader.Open(index, true))
            {
                using (Searcher searcher = new IndexSearcher(reader))
                {
                    TopFieldDocs docs = searcher.Search(query, new QueryWrapperFilter(query), limit, sort);
                    ScoreDoc[] res = docs.ScoreDocs;
                    List<SearchResult> match = new List<SearchResult>();
                    foreach (ScoreDoc scoreDoc in res)
                    {
                        Document doc = searcher.Doc(scoreDoc.Doc);
                        match.Add(new SearchResult()
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
            Data data = new Data(result.Path) { Id = result.Id, Priority = result.Priority + 1 };
            using (IndexWriter writer = new IndexWriter(index, analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.UpdateDocument(new Term("Id", result.Id), data.GetDocument());
                writer.Commit();
            }
        }

    }

}