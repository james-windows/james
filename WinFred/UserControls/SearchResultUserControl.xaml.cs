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

        public MainWindow GetParentWindow()
        {
            return Dispatcher.Invoke(() => (MainWindow) Window.GetWindow(this));
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            var window = Window.GetWindow(this);
            window?.Hide();
            var index = (int) (e.GetPosition(this).Y/SearchResultElement.ROW_HEIGHT);
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                _searchResults[index].OpenFolder();
            }
            else
            {
                _searchResults[index].Open();
            }
        }

        public void Search(string str)
        {
            _searchResults = SearchEngine.GetInstance().Query(str);
            WorkflowManager.GetInstance(this).CancelWorkflows();
            if (str.Length >= Config.GetInstance().StartSearchMinTextLength)
            {
                _searchResults.InsertRange(0, WorkflowManager.GetInstance(this).GetKeywordTriggers(str));
            }
            _searchResults = _searchResults.Take(10).ToList();
            FocusedIndex = 0;
            _searchResultElement.DrawItems(_searchResults, FocusedIndex);
            Dispatcher.BeginInvoke(
                (Action) (() => { _searchResultElement.Height = _searchResults.Count*SearchResultElement.ROW_HEIGHT; }));
        }

        public void WorkflowOutput(List<SearchResult> searchResults)
        {
            _searchResults = searchResults;
            Dispatcher.Invoke(() =>
            {
                _searchResultElement.DrawItems(_searchResults, 0);
                Dispatcher.BeginInvoke(
                    (Action)
                        (() => { _searchResultElement.Height = _searchResults.Count*SearchResultElement.ROW_HEIGHT; }));
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
            if (_searchResultElement.CurrentFocus >= 0 && _searchResultElement.CurrentFocus < _searchResults.Count)
            {
                if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                {
                    _searchResults[_searchResultElement.CurrentFocus].OpenFolder();
                }
                else
                {
                    _searchResults[_searchResultElement.CurrentFocus].Open();
                }
            }
        }
    }
}