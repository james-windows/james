using System;
using Lucene.Net.Documents;

namespace James
{
    public class Data
    {
        public Data(string path)
        {
            Id = Guid.NewGuid().ToString();
            Path = path.ToLower();
            FileName = path.Substring(path.LastIndexOf('\\') + 1);
        }

        public Data(string path, int priority)
        {
            Id = Guid.NewGuid().ToString();
            Path = path.ToLower();
            FileName = path.Substring(path.LastIndexOf('\\') + 1);
            Priority = Convert.ToInt32(priority + Math.Max(5 - Math.Sqrt(FileName.Length), 0));
        }

        public string Id { get; set; }
        public int Priority { get; set; }
        public string FileName { get; set; }
        public string Path { get; set; }

        public Document GetDocument()
        {
            var doc = new Document();
            doc.Add(new Field("Id", Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            foreach (var item in FileName.Split(' '))
            {
                doc.Add(new Field("FileName", item, Field.Store.YES, Field.Index.ANALYZED));
            }
            doc.Add(new Field("Path", Path, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
            doc.Add(new Field("Priority", Priority.ToString(), Field.Store.YES, Field.Index.ANALYZED_NO_NORMS));
            return doc;
        }

        public override string ToString() => Path;
    }
}