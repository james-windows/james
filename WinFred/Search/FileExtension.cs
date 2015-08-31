using System;

namespace WinFred
{
    public class FileExtension : IComparable<FileExtension>
    {
        public FileExtension()
        {
        }

        public FileExtension(string value, int priority)
        {
            Priority = priority;
            Extension = value;
        }

        public int Priority { get; set; }
        public string Extension { get; set; }
        public int CompareTo(FileExtension other) => string.Compare(Extension, other.Extension);

        public override string ToString()
        {
            return Extension + " Wert: " + Priority;
        }
    }
}