namespace Engine.Entity.DirectoryTree.Mocking
{
    public class DirectoryNodeMock : IDirectoryNode
    {
        private readonly string _path;

        public DirectoryNodeMock(string path)
        {
            _path = path;
        }

        public DirectoryNodeMock()
        {
            _path = "";
        }

        public string GetFullPath() => _path;
        public void Destroy()
        {

        }

        public bool ComparePath(string path)
        {
            return path == _path;
        }
    }
}
