using System;
using System.Windows.Input;
using System.Windows.Media;

namespace James.ResultItems
{
    public abstract class ResultItem : IComparable<ResultItem>
    {
        public int Priority { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public virtual ImageSource Icon { get; set; }

        public int CompareTo(ResultItem other) => Priority - other.Priority;

        public abstract void Open(KeyEventArgs e, string search);

        public override string ToString() => Title + ", " + Priority;

        /// <summary>
        /// Returns the string to be inserted, when the user wants to autocomplete
        /// </summary>
        /// <returns></returns>
        public abstract string AutoComplete();
    }
}