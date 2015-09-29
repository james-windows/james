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

        internal IEnumerable<SearchResult> GetItemsToBeIndexed(string currentPath = "")
        {
            var data = new List<SearchResult>();
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
            if (_folder.IndexFolders && data.Count > 0)
            {
                data.Add(new SearchResult()
                {
                    Path = _folder.Location + currentPath,
                    Priority = Config.GetInstance().DefaultFolderPriority + _folder.Priority
                });
            }
            return data;
        }

        private IEnumerable<SearchResult> GetItemsInCurrentScope(string currentPath)
        {
            var data = new List<SearchResult>();
            foreach (var filePath in Directory.GetFiles(currentPath))
            {
                data.AddRange(GetFileIfItShouldBeTraced(filePath));
            }
            return data;
        }

        private IEnumerable<SearchResult> GetFileIfItShouldBeTraced(string filePath)
        {
            var data = new List<SearchResult>();
            var priority = _folder.GetFilePriority(filePath);
            if (priority > 0)
            {
                data.Add(new SearchResult(){Path = filePath, Priority = priority});
            }
            return data;
        }
    }
}