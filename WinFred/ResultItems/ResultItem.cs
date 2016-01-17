﻿using System;
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
        public abstract string AutoComplete();
    }
}