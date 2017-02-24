using System;
using System.Windows.Input;
using System.Windows.Media;

namespace James.ResultItems
{
    public abstract class ResultItem : IComparable<ResultItem>, IEquatable<ResultItem>
    {
        public int Priority { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public virtual ImageSource Icon { get; set; }

        public int CompareTo(ResultItem other) => Priority - other.Priority;

        public abstract void Open(KeyEventArgs e, string search, bool showFileProperties);

        public bool Equals(ResultItem other) => other.Title == Title && other.Subtitle == Subtitle;

        public override string ToString() => Subtitle + ", " + Priority;

        /// <summary>
        /// Returns the string to be inserted, when the user wants to autocomplete
        /// </summary>
        /// <returns></returns>
        public abstract string AutoComplete();
    }
}