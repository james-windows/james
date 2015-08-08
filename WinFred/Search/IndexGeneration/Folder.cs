using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinFred.Search.IndexGeneration
{
    class Folder
    {
        readonly Path _folder;
        public Folder(Path folder)
        {
            this._folder = folder;
        }

        internal IEnumerable<Document> getItemsToBeIndexed(string currentPath = "")
        {
            List<Document> data = new List<Document>();
            try
            {
                data.AddRange(getItemsInCurrentScope(_folder.Location + currentPath));
                foreach (String directory in Directory.GetDirectories(_folder.Location + currentPath))
                {
                    data.AddRange(getItemsToBeIndexed(directory.Replace(_folder.Location, "")));
                }
            }
            catch (UnauthorizedAccessException) { }
            data.Add((new Data(_folder.Location + currentPath) { Priority = 80 }).GetDocument());
            return data;
        }

        private IEnumerable<Document> getItemsInCurrentScope(string currentPath)
        {
            List<Document> data = new List<Document>();
            foreach (string filePath in Directory.GetFiles(currentPath))
            {
                data.AddRange(GetFileIfItShouldBeTraced(filePath));
            }
            return data;
        }
        private IEnumerable<Document> GetFileIfItShouldBeTraced(string filePath)
        {
            List<Document> data = new List<Document>();
            int priority = _folder.GetFilePriority(filePath);
            if(priority > 0)
            {
                data.Add((new Data(filePath, priority)).GetDocument());
            }
            return data;
        }
    }
}
