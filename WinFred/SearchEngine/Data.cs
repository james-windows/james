using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Documents;

namespace WinFred
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
            Document doc = new Document();
            doc.Add(new Field("Id", Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            for (int i = 0; i < FileName.Length - 1 && i != -1; i = FileName.IndexOf(" ", i + 1))
                doc.Add(new Field("FileName", FileName.Substring(i), Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field("Path", Path, Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("Priority", (-Priority).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
