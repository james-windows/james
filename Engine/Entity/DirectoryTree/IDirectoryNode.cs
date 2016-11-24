namespace Engine.Entity.DirectoryTree
{
    public interface IDirectoryNode
    {
        string GetFullPath();

        void Destroy();

        bool ComparePath(string path);
    }
}