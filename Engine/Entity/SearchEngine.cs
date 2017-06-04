using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Engine.Collections;
using Engine.Divider;
using Engine.Entity.DirectoryTree;
using Engine.Entity.SearchTree;
using Engine.Helper;

namespace Engine.Entity
{
    public class SearchEngine
    {
        private readonly string _filePath;
#if DEBUG
        public readonly SearchNode _rootSearchNode;
        public readonly DirectoryNode _rootDirectoryNode;
#else
        private readonly SearchNode _rootSearchNode;
        private readonly DirectoryNode _rootDirectoryNode;
#endif
        private readonly IDivider _divider = new Divider.Divider();

        public SearchEngine(string filePath, bool loadIndex = true) : this()
        {
            _filePath = filePath;
            if (loadIndex)
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach( var line in lines)
                {
                    var splits = line.Split(';');
                    Insert(splits[0], int.Parse(splits[1]));
                }
            }
        }

        public SearchEngine(string filePath, int resultListLength, bool loadIndex = true) : this(filePath, loadIndex)
        {
            StaticVariables.ResultListLength = resultListLength;
        }

        public SearchEngine()
        {
            _rootDirectoryNode = new DirectoryNode("");
            _rootSearchNode = new SearchNode() { Label = "?", Results = new MergedResultList() };
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Insert(string path, int priority)
        {
            DirectoryNode directoryNode = _rootDirectoryNode.Insert(path);
            Item item = new Item(directoryNode) { Priority = priority };
            var tmp = new MergeResult();
            foreach (string combination in _divider.SplitPath(path))
            {
                Console.WriteLine(combination);
                _rootSearchNode.Insert(combination.ToLower(), item, ref tmp);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<string> Find(string search)
        {
            SearchNode result = _rootSearchNode.Find(search.ToLower());
            var files = result?.Results?.Items?.Select(item => item.ToString());
            if (files == null) return new List<string>();
            return files;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteFile(string path)
        {
            foreach (string combination in _divider.SplitPath(path))
            {
                _rootSearchNode.Delete(combination.ToLower(), path);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeletePath(string path)
        {
            var root = _rootDirectoryNode.Find(path);
            if (root != null)
            {
                foreach (var itemPath in root.GetAllChildrenPaths().ToList())
                {
                    DeleteFile(itemPath);
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ChangePriority(string path, int delta)
        {
            int priority = FindItemWithPath(path).Priority + delta;
            DeleteFile(path);
            if (priority >= 0)
            {
                Insert(path, priority);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Save(string save = null)
        {
            StreamWriter sw = new StreamWriter(save ?? _filePath);
            foreach (var currentItem in _rootDirectoryNode.GetAllChildrenPaths())
            {
                var firstItem = FindItemWithPath(currentItem);
                if (firstItem != null)
                {
                    sw.WriteLine(firstItem.ToString());
                }
            }
            sw.Flush();
            sw.Close();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private Item FindItemWithPath(string path)
        {
            return _rootSearchNode.Find(path.Trim('\\').Split('\\').Last().ToLower())?.Items.FirstOrDefault(item => item.ComparePath(path));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Rename(string path, string newPath)
        {
            var item = _rootDirectoryNode.Find(path);
            if (item != null)
            {
                var searchNode = FindItemWithPath(path);
                if (searchNode != null)
                {
                    int priority = searchNode.Priority;
                    DeleteFile(path);
                    Insert(newPath, priority);
                }

                if (item.Childrens.Count != 0)
                {
                    var newNode = _rootDirectoryNode.Insert(newPath);
                    var oldParent = item.ParentDirectoryNode;
                    int pos = newNode.ParentDirectoryNode.Childrens.IndexOf(newNode);
                    newNode.ParentDirectoryNode.Childrens[pos] = item;
                    item.ParentDirectoryNode = newNode.ParentDirectoryNode;
                    item.Directory = newNode.Directory;
                    oldParent.Childrens.Remove(item);
                }
            }
        }
    }
}
