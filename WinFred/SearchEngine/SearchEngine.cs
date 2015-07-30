using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using WinFred.Search.IndexGeneration;
using LuceneDirectory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace WinFred.Search
{
    class SearchEngine
    {
        private static SearchEngine searchEngine;
        private static object singeltonLock = new object();
        LuceneDirectory index;
        Sort sort;
        Analyzer analyzer;

        public static SearchEngine GetInstance()
        {
            lock (singeltonLock)
            {
                if (searchEngine == null)
                    searchEngine = new SearchEngine();
                return searchEngine;
            }
        }

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
            List<Document> data = new List<Document>();
            foreach(Path currentPath in Config.GetInstance().Paths)
            {
                Folder currentFolder = new Folder(currentPath);
                data.AddRange(currentFolder.getItemsToBeIndexed());
            }

            using (IndexWriter writer = new IndexWriter(index, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                data.ForEach(ElementToBeIndexed => writer.AddDocument(ElementToBeIndexed));
                writer.Optimize();
                writer.Commit();
            }
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