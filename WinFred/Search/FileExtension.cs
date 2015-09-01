using System;
using static System.String;

namespace WinFred.Search
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
        public int CompareTo(FileExtension other) => CompareOrdinal(Extension, other.Extension);

        public override string ToString() => Extension + " Wert: " + Priority;
    }
}