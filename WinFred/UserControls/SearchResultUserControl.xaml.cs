using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using James.HelperClasses;
using James.ResultItems;
using James.Search;
using James.Workflows;

namespace James.UserControls
{
    /// <summary>
    ///     Interaction logic for SearchResultUserControl.xaml
    /// </summary>
    public partial class SearchResultUserControl : UserControl
    {
        private readonly SearchResultElement _searchResultElement;
        private string _lastSearch = "";
        private List<ResultItem> _results;

        public SearchResultUserControl()
        {
            InitializeComponent();
            _searchResultElement = new SearchResultElement
            {
                Width = 700,
                Cursor = Cursors.Hand
            };
            Grid.Children.Add(_searchResultElement);
            _searchResultElement.MouseLeftButtonDown += MouseClick;
        }

        public int FocusedIndex { get; private set; }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            lock (this)
            {
                e.Handled = true;
                var window = Window.GetWindow(this);
                window?.Hide();
                var index = (int) (e.GetPosition(this).Y/SearchResultElement.RowHeight);
                _results[index].Open(null, Windows.MainWindow.GetInstance().SearchTextBox.Text);
            }
        }

        public void Search(string str, ResultItem focusedItem = null)
        {
            lock (this)
            {
                _lastSearch = str;
                _results = SearchEngine.Instance.Query(str);
                WorkflowManager.Instance.CancelWorkflows();
                if (str.Length >= Math.Max(Config.Instance.StartSearchMinTextLength, 1))
                {
                    _results.InsertRange(0, WorkflowManager.Instance.GetKeywordTriggers(str));
                }
                FocusedIndex = (focusedItem != null) ? CalcFocusedItem(focusedItem) : 0;

                _results = _results.Take(10).ToList();
                _searchResultElement.DrawItems(_results, FocusedIndex);
                Dispatcher.BeginInvoke(
                    (Action)(() => { _searchResultElement.Height = _results.Count * SearchResultElement.RowHeight; }));
            }
        }

        public int CalcFocusedItem(ResultItem focusedItem)
        {
            for (var i = 0; i < _results.Count; i++)
            {
                if (_results[i].Subtitle == focusedItem.Subtitle)
                {
                    return i;
                }
            }
            return 0;
        }

        public void WorkflowOutput(List<ResultItem> searchResults)
        {
            _results = searchResults;
            FocusedIndex = 0;
            Dispatcher.Invoke(() =>
            {
                _searchResultElement.DrawItems(_results, 0);
                Dispatcher.BeginInvoke(
                    (Action)
                        (() => { _searchResultElement.Height = _results.Count*SearchResultElement.RowHeight; }));
            });
        }

        public void MoveUp()
        {
            if (FocusedIndex > 0)
            {
                FocusedIndex--;
                _searchResultElement.DrawItems(_results, FocusedIndex);
            }
        }

        public void MoveDown()
            {
            if (FocusedIndex < _results.Count - 1)
            {
                FocusedIndex++;
                _searchResultElement.DrawItems(_results, FocusedIndex);
            }
        }

        public void Open(KeyEventArgs e, string input)
        {
            e.Handled = true;
            var index = _searchResultElement.CurrentFocus;
            if (index >= 0 && index < _results.Count)
            {
                _results[index].Open(e, input);
            }
        }

        public void IncreasePriority()
        {
            var index = _searchResultElement.CurrentFocus;
            if (index > 0 && index < _results.Count)
            {
                var diff = _results[index - 1].Priority - _results[index].Priority + 1;
                ChangePriority(diff, index);
            }
        }

        public void DecreasePriority()
        {
            var index = _searchResultElement.CurrentFocus;
            if (index >= 0 && index < _results.Count - 1)
            {
                var diff = _results[index].Priority - _results[index + 1].Priority + 1;
                ChangePriority(-diff, index);
            }
        }

        public void ChangePriority(int diff, int index)
        {
            SearchEngine.Instance.IncrementPriority(_results[index].Subtitle, diff);

            Search(_lastSearch, _results[index]);
        }

        public string AutoComplete()
        {
            var index = _searchResultElement.CurrentFocus;
            return _results[index].AutoComplete();
        }
    }
}