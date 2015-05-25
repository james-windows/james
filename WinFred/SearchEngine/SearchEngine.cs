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
using Version = Lucene.Net.Util.Version;

namespace WinFred
{
    class SearchEngine
    {
        private static SearchEngine searchEngine;
        private static object singeltonLock = new object();

        public static SearchEngine GetInstance()
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
            analyzer = new StandardAnalyzer(Version.LUCENE_30);
            //analyzer = new SimpleAnalyzer();
            //analyzer = new WhitespaceAnalyzer();
            //analyzer = new KeywordAnalyzer();
            sort = new Sort(new SortField[] { SortField.FIELD_SCORE, new SortField("Priority", SortField.INT) });
        }

        public void BuildIndex()
        {
            Queue<Data> data = new Queue<Data>();
            DateTime date = DateTime.Now;
            foreach (var item in Config.GetInstance().Paths)
            {
                if (item.IsEnabled)
                {
                    var tmp = (GetFiles(item.Location, "*", item));
                    foreach (var i in tmp)
                    {
                        data.Enqueue(i);
                    }
                }
            }
            Debug.WriteLine((DateTime.Now - date).TotalMilliseconds);
            Debug.WriteLine("Count: " + data.Count);
            int cnt = 0;
            using (IndexWriter writer = new IndexWriter(index, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                while (data.Count > 0)
                {
                    writer.AddDocument(data.Dequeue().GetDocument());
                    cnt++;
                } 
                writer.Optimize();
                writer.Commit();
            }
            Debug.WriteLine("index built! Count: " + cnt);
            Debug.WriteLine((DateTime.Now - date).TotalMilliseconds);
        }
        private List<Data> GetFiles(string path, string pattern, Path folderPath)
        {
            List<Data> data = new List<Data>();
            try
            {
                foreach (string item in System.IO.Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly))
                {
                    int index = folderPath.FileExtensions.BinarySearch(new FileExtension(item.Split('.').Last(), 0));
                    if (index >= 0) //Is file extention found in the folder file extensions
                    {
                        if (folderPath.FileExtensions[index].Priority >= 0)
                        {
                            data.Add(new Data(item, folderPath.FileExtensions[index].Priority + folderPath.Priority));    
                        }
                    }
                    else //else look into the defaultfileextensions
                    {
                        index = Config.GetInstance().DefaultFileExtensions.BinarySearch(new FileExtension(item.Split('.').Last(), 0));
                        if (index >= 0)
                        {
                            data.Add(new Data(item, Config.GetInstance().DefaultFileExtensions[index].Priority + folderPath.Priority));
                        }
                    }
                }
                foreach (String directory in System.IO.Directory.GetDirectories(path))
                {
                    data.AddRange(GetFiles(directory, pattern, folderPath));
                }
                if (data.Count > 0)
                {
                    data.Add(new Data(path) {Priority = 80});
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
                writer.Commit();
            }
        }

        public List<SearchResult> Query(string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Trim().Length < Config.GetInstance().StartSearchMinTextLength)
                return new List<SearchResult>();
            str = str.ToLower();
            //BooleanQuery query = new BooleanQuery();
            //foreach (var item in str.Trim().Split(' '))
            //{
            //    query.Add(new BooleanClause(new PrefixQuery(new Term("FileName", item)), Occur.SHOULD));
            //}

            Query query = new PrefixQuery(new Term("FileName", str.Trim()));
            
            using (IndexReader reader = IndexReader.Open(index, true))
            {
                using (Searcher searcher = new IndexSearcher(reader))
                {
                    TopFieldDocs docs = searcher.Search(query, new QueryWrapperFilter(query), Config.GetInstance().MaxSearchResults, sort);
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
            Data data = new Data(result.Path) { Id = result.Id, Priority = Math.Abs(result.Priority) + 5 };
            using (IndexWriter writer = new IndexWriter(index, analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.UpdateDocument(new Term("Id", result.Id), data.GetDocument());
                writer.Optimize();
                writer.Commit();
            }
        }

        public void DeleteFile(String path)
        {
            using (IndexWriter writer = new IndexWriter(index, analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                Query query = new PrefixQuery(new Term("Path", path.ToLower()));
                writer.DeleteDocuments(query);
                writer.Optimize();
                writer.Commit();
            }
        }

        public void IncrementPriority(String path)
        {
            using (IndexReader reader = IndexReader.Open(index, true))
            {
                Query query = new PrefixQuery(new Term("Path", path.ToLower()));
                using (Searcher searcher = new IndexSearcher(reader))
                {
                    ScoreDoc[] res = searcher.Search(query, new QueryWrapperFilter(query), 10, sort).ScoreDocs;
                    if (res.Length == 0)
                    {
                        return;
                    }
                    ScoreDoc scoreDoc = res[0];
                    Document doc = searcher.Doc(scoreDoc.Doc);
                    SearchResult item = new SearchResult()
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