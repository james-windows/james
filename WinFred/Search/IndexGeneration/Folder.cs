using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Documents;

namespace WinFred.Search.IndexGeneration
{
    internal class Folder
    {
        private readonly Path _folder;

        public Folder(Path folder)
        {
            _folder = folder;
        }

        internal IEnumerable<Document> getItemsToBeIndexed(string currentPath = "")
        {
            var data = new List<Document>();
            try
            {
                data.AddRange(getItemsInCurrentScope(_folder.Location + currentPath));
                foreach (var directory in Directory.GetDirectories(_folder.Location + currentPath))
                {
                    data.AddRange(getItemsToBeIndexed(directory.Replace(_folder.Location, "")));
                }
            }
            catch (UnauthorizedAccessException)
            {
            }
            data.Add(
                (new Data(_folder.Location + currentPath)
                {
                    Priority = Config.GetInstance().FolderPriority + _folder.Priority
                }).GetDocument());
            return data;
        }

        private IEnumerable<Document> getItemsInCurrentScope(string currentPath)
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