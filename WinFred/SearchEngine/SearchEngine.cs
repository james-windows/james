using System;
using System.Collections.Generic;
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

namespace WinFred
{
    class SearchEngine
    {
        public const int SEARCH_RESULTS = 10;
        public const int MIN_SEARCH_LENGTH = 3;

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

        public class Data
        {
            public Data(string path)
            {
                path = path.ToLower();
                Path = path;
                path = path.Replace('-', ' ').Replace(" ", "");
                FileName = path.Substring(path.LastIndexOf('\\') + 1); 
            }

            public int Priority { get; set; }
            public string FileName { get; set; }
            public string Path { get; set; }

            public Document GetDocument()
            {
                Document doc = new Document();
                for (int i = 0; i < FileName.Length - 1; i++)
                    doc.Add(new Field("FileName", FileName.Substring(i), Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field("Path", Path, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("Priority", (-Priority).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
                return doc;
            }
        }

        Directory index;
        Searcher searcher;
        Sort sort;
        Analyzer analyzer;

        private SearchEngine()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+ "\\WinFred";
            index = FSDirectory.Open(path);
            analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            sort = new Sort(new SortField[] { SortField.FIELD_SCORE, new SortField("Priority", SortField.INT) });
        }

        public void BuildIndex()
        {
            List<Data> data = new List<Data>();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Desktop";
            foreach (string file in System.IO.Directory.GetFiles(path, "*", System.IO.SearchOption.AllDirectories))
                data.Add(new Data(file));
            foreach (string dir in System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories))
                data.Add(new Data(dir) { Priority = 1 });
            using (IndexWriter writer = new IndexWriter(index, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (Data a in data)
                    writer.AddDocument(a.GetDocument());
                writer.Optimize();
            }
        }

        public void InitSearch()
        {
            IndexReader reader = IndexReader.Open(index, true);
            searcher = new IndexSearcher(reader);
        }

        public void AddFile(Data data)
        {
            using (IndexWriter writer = new IndexWriter(index, analyzer, false, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                writer.AddDocument(data.GetDocument());
                writer.Optimize();
            }
        }

        public List<string> Query(string str, int limit = SEARCH_RESULTS)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Trim().Length < MIN_SEARCH_LENGTH)
                return new List<string>();
            str = str.Replace('-', ' ').Replace(" ", "").ToLower();
            Query query = new PrefixQuery(new Term("FileName", str.Trim()));
            TopFieldDocs docs = searcher.Search(query, new QueryWrapperFilter(query), limit, sort);
            ScoreDoc[] res = docs.ScoreDocs;
            List<string> match = new List<string>();
            foreach (ScoreDoc scoreDoc in res)
            {
                Document doc = searcher.Doc(scoreDoc.Doc);
                match.Add(doc.Get("Path"));
            }
            return match;
        }
    }

}