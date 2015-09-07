using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;

namespace James.Search.IndexGeneration
{
    internal class Folder
    {
        private readonly Path _folder;

        public Folder(Path folder)
        {
            _folder = folder;
        }

        internal IEnumerable<Document> GetItemsToBeIndexed(string currentPath = "")
        {
            var data = new List<Document>();
            try
            {
                data.AddRange(GetItemsInCurrentScope(_folder.Location + currentPath));
                foreach (var directory in Directory.GetDirectories(_folder.Location + currentPath))
                {
                    data.AddRange(GetItemsToBeIndexed(directory.Replace(_folder.Location, "")));
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            data.Add(
                (new Data(_folder.Location + currentPath)
                {
                    Priority = Config.GetInstance().DefaultFolderPriority + _folder.Priority
                }).GetDocument());
            return data;
        }

        private IEnumerable<Document> GetItemsInCurrentScope(string currentPath)
        {
            var data = new List<Document>();
            foreach (var filePath in Directory.GetFiles(currentPath))
            {
                data.AddRange(GetFileIfItShouldBeTraced(filePath));
            }
            return data;
        }

        private IEnumerable<Document> GetFileIfItShouldBeTraced(string filePath)
        {
            var data = new List<Document>();
            var priority = _folder.GetFilePriority(filePath);
            if (priority > 0)
            {
                data.Add((new Data(filePath, priority)).GetDocument());
            }
            return data;
        }
    }
}