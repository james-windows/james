using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        private List<SearchResult> _searchResults;

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
            e.Handled = true;
            var window = Window.GetWindow(this);
            window?.Hide();
            var index = (int) (e.GetPosition(this).Y/SearchResultElement.RowHeight);
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                _searchResults[index].OpenFolder();
            }
            else
            {
                _searchResults[index].Open();
            }
        }

        public void Search(string str, SearchResult focusedItem = null)
        {
            _lastSearch = str;
            _searchResults = SearchEngine.Instance.Query(str);
            WorkflowManager.Instance.CancelWorkflows();
            if (str.Length >= Math.Max(Config.Instance.StartSearchMinTextLength, 1))
            {
                _searchResults.InsertRange(0, WorkflowManager.Instance.GetKeywordTriggers(str));
            }
            FocusedIndex = (focusedItem != null) ? CalcFocusedItem(focusedItem) : 0;

            _searchResults = _searchResults.Take(10).ToList();
            _searchResultElement.DrawItems(_searchResults, FocusedIndex);
            Dispatcher.BeginInvoke(
                (Action) (() => { _searchResultElement.Height = _searchResults.Count*SearchResultElement.RowHeight; }));
        }

        public int CalcFocusedItem(SearchResult focusedItem)
        {
            for (var i = 0; i < _searchResults.Count; i++)
            {
                if (_searchResults[i].Path == focusedItem.Path)
                {
                    return i;
                }
            }
            return 0;
        }

        public void WorkflowOutput(List<SearchResult> searchResults)
        {
            _searchResults = searchResults;
            Dispatcher.Invoke(() =>
            {
                _searchResultElement.DrawItems(_searchResults, 0);
                Dispatcher.BeginInvoke(
                    (Action)
                        (() => { _searchResultElement.Height = _searchResults.Count*SearchResultElement.RowHeight; }));
            });
        }

        public void MoveUp()
        {
            if (FocusedIndex > 0)
            {
                FocusedIndex--;
                _searchResultElement.DrawItems(_searchResults, FocusedIndex);
            }
        }

        public void MoveDown()
        {
            if (FocusedIndex < _searchResults.Count - 1)
            {
                FocusedIndex++;
                _searchResultElement.DrawItems(_searchResults, FocusedIndex);
            }
        }

        public void Open(KeyEventArgs e)
        {
            e.Handled = true;
            var index = _searchResultElement.CurrentFocus;
            if (index >= 0 && index < _searchResults.Count)
            {
                if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                {
                    _searchResults[index].OpenFolder();
                }
                else
                {
                    _searchResults[index].Open();
                }
            }
        }

        public void IncreasePriority()
        {
            var index = _searchResultElement.CurrentFocus;
            if (index > 0 && index < _searchResults.Count)
            {
                var diff = _searchResults[index - 1].Priority - _searchResults[index].Priority + 1;
                ChangePriority(diff, index);
            }
        }

        public void DecreasePriority()
        {
            var index = _searchResultElement.CurrentFocus;
            if (index >= 0 && index < _searchResults.Count - 1)
            {
                var diff = _searchResults[index].Priority - _searchResults[index + 1].Priority + 1;
                ChangePriority(-diff, index);
            }
        }

        public void ChangePriority(int diff, int index)
        {
            SearchEngine.Instance.IncrementPriority(_searchResults[index].Path, diff);

            Search(_lastSearch, _searchResults[index]);
        }
    }
}