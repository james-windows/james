using System.Collections.Generic;
using System.Linq;

namespace Engine.Entity.DirectoryTree
{
    public class DirectoryNode : IDirectoryNode
    {
        public DirectoryNode(string directory)
        {
            Directory = directory;
        }
        public DirectoryNode ParentDirectoryNode { get; set; }

        public List<DirectoryNode> Childrens { get; set; } = new List<DirectoryNode>();

        public string Directory { get; set; }

        /// <summary>
        /// Generates the hole path through traversaling all parents
        /// </summary>
        /// <returns></returns>
        public string GetFullPath() => (ParentDirectoryNode?.GetFullPath() + '\\' + Directory).Trim('\\');

        public bool ComparePath(string path)
        {
            var pointer = this;
            int offset = path.Length - Directory.Length;
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (i - offset < 0)
                {
                    if (pointer.ParentDirectoryNode == null)
                    {
                        return true;
                    }
                    pointer = pointer.ParentDirectoryNode;
                    offset = i - pointer.Directory.Length;
                    if (path[i] != '\\')
                    {
                        return false;
                    }
                    i--;
                }
                if (path[i] != pointer.Directory[i - offset])
                {
                    return false;
                }
            }
            return true;
        }


        public void Destroy()
        {
            if (Childrens.Count == 0)
            {
                ParentDirectoryNode.Childrens.Remove(this);
            }
        }

        /// <summary>
        /// Searchs for the node
        /// </summary>
        /// <param name="path"></param>
        /// <returns>The founded node, otherwise null will be returned</returns>
        public DirectoryNode Find(string path)
        {
            if (path.StartsWith(Directory))
            {
                path = path.Substring(Directory.Length).Trim('\\');
            }
            if (path == "")
            {
                return this;
            }
            var splits = path.Split('\\');
            string currentDirectory = splits.First();
            if (Childrens.All(node => node.Directory != currentDirectory))
            {
                return null;
            }
            return this[currentDirectory].Find(path);
        }

        /// <summary>
        /// Inserts a new string and returns the specific DirectoryNode
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public DirectoryNode Insert(string path)
        {
            string currentDirectory = path.Split('\\').First();
            if (Childrens.ToList().All(node => node.Directory != currentDirectory))
            {
                var newNode = new DirectoryNode(currentDirectory) { ParentDirectoryNode = this };
                Childrens.Add(newNode);
                if (currentDirectory == path)
                {
                    return newNode;
                }
            }
            string nextStep = path.Substring(currentDirectory.Length).TrimStart('\\');
            if (nextStep == "")
            {
                return this[currentDirectory];
            }
            return this[currentDirectory].Insert(nextStep);
        }

        public DirectoryNode this[string directory]
        {
            get
            {
                return Childrens.ToList().First(node => node.Directory == directory);
            }
        }

        public override string ToString() => GetFullPath();

        public IEnumerable<string> GetAllChildrenPaths()
        {
            yield return this.ToString();
            foreach (var directoryNode in Childrens.ToList())
            {
                foreach (var allChildrenPath in directoryNode.GetAllChildrenPaths())
                {
                    yield return allChildrenPath;
                }
            }
        }
    }
}
