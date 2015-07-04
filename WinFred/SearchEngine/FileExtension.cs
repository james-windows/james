using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFred
{
    public class FileExtension : IComparable<FileExtension>
    {
        public FileExtension()
        {

        }

        public int Priority { get; set; }
        public string Extension { get; set; }
        public FileExtension(string value, int priority)
        {
            Priority = priority;
            Extension = value;
        }

        public int CompareTo(FileExtension other)
        {
            return String.Compare(Extension, other.Extension);
        }

        public override string ToString()
        {
            return Extension + " Wert: " + Priority;
        }
    }
}
