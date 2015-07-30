using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WinFred.Search.IndexGeneration
{
    class Folder
    {
        Path folder;
        public Folder(Path folder)
        {
            this.folder = folder;
        }

        internal IEnumerable<Document> getItemsToBeIndexed(String currentPath = "")
        {
            List<Document> data = new List<Document>();
            try
            {
                data.AddRange(getItemsInCurrentScope(folder.Location + currentPath));
                foreach (String directory in Directory.GetDirectories(folder.Location + currentPath))
                {
                    data.AddRange(getItemsToBeIndexed(directory.Replace(folder.Location, "")));
                }
            }
            catch (UnauthorizedAccessException) { }
            data.Add((new Data(folder.Location + currentPath) { Priority = 80 }).GetDocument());
            return data;
        }

        private IEnumerable<Document> getItemsInCurrentScope(String currentPath)
        {
            List<Document> data = new List<Document>();
            foreach (string filePath in Directory.GetFiles(currentPath))
            {
                data.AddRange(GetFileIfItShouldBeTraced(filePath));
            }
            return data;
        }
        private IEnumerable<Document> GetFileIfItShouldBeTraced(String filePath)
        {
            List<Document> data = new List<Document>();
            int priority = GetFilePriority(filePath);
            if(priority > 0)
            {
                data.Add((new Data(filePath, priority + folder.Priority)).GetDocument());
            }
            return data;
        }

        private int GetFilePriority(String filePath)
        {
            int FileExtension = CalculatePriorityByFileExtensions(filePath, folder.FileExtensions);
            int DefaultFileExtension = CalculatePriorityByFileExtensions(filePath, Config.GetInstance().DefaultFileExtensions);

            return Math.Max(FileExtension, DefaultFileExtension) + folder.Priority;
        }

        private int CalculatePriorityByFileExtensions(string filePath, List<FileExtension> fileExtensions)
        {
            int wasFound = fileExtensions.BinarySearch(new FileExtension(filePath.Split('.').Last(), 0));
            int priority = wasFound;
            if (priority >= 0)
            {
                priority = fileExtensions[priority].Priority;
            }
            return priority;
        }
    }
}
