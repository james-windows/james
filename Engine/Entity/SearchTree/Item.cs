using System;
using Engine.Entity.DirectoryTree;

namespace Engine.Entity.SearchTree
{
    public class Item : IComparable<Item>
    {
        public Item(IDirectoryNode path)
        {
            _path = path;
        }

        public int Priority { get; set; }

        private readonly IDirectoryNode _path;

        public string Path => _path.GetFullPath();

        public int CompareTo(Item other)
        {
            if (other.Priority != Priority) return other.Priority - Priority;
            return Path.CompareTo(other.Path);
        }

        public bool ComparePath(string path) => _path.ComparePath(path);

        public bool ComparePath(Item item) => item._path == _path;

        public override string ToString() => $"{Path};{Priority}";

        public void Delete()
        {
            _path.Destroy();
        }

        ~Item()
        {
            _path.Destroy();
        }
    }
}
