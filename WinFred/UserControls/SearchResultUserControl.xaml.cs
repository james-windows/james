using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public List<ResultItem> results;
        private string _lastSearch;

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
        private ResultItem _focusedItem;

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            lock (this)
            {
                e.Handled = true;
                Window.GetWindow(this)?.Hide();
                var index = (int) (e.GetPosition(this).Y/SearchResultElement.RowHeight);
                results[index].Open(new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Enter), MyWindows.MainWindow.GetInstance().SearchTextBox.Text, false);
            }
        }

        /// <summary>
        /// Starts a new search, calls the searchengine and merges it with the matching KeywordTriggers
        /// </summary>
        /// <param name="str"></param>
        public void Search(string str)
        {
            lock (this)
            {
                _lastSearch = str;
                results = SearchEngine.Instance.Query(str);
                WorkflowManager.Instance.CancelWorkflows();
                if (str.Length >= Math.Max(Config.Instance.StartSearchMinTextLength, 1))
                {
                    results.InsertRange(0, WorkflowManager.Instance.GetKeywordTriggers(str));
                }
                FocusedIndex = CalcFocusedItem();

                results = results.Take(Config.Instance.MaxSearchResults).ToList();
                _searchResultElement.DrawItems(results, FocusedIndex, out _focusedItem);
                Dispatcher.BeginInvoke(
                    (Action)(() => { _searchResultElement.Height = results.Count * SearchResultElement.RowHeight; }));
            }
        }

        /// <summary>
        /// Tries to restore the last selection
        /// </summary>
        /// <returns></returns>
        public int CalcFocusedItem()
        {
            int pos = results.IndexOf(_focusedItem);
            return pos != -1 ? pos : 0;
        }

        /// <summary>
        /// Displays the results from a MagicOutput
        /// </summary>
        /// <param name="searchResults"></param>
        public void WorkflowOutput(List<MagicResultItem> searchResults)
        {
            lock (this)
            {
                results.RemoveAll(
                item => item is MagicResultItem &&
                    ((MagicResultItem)item).WorkflowComponent.ParentWorkflow == searchResults[0].WorkflowComponent.ParentWorkflow);
                results.InsertRange(0, searchResults);
                results = results.Take(Config.Instance.MaxSearchResults).ToList();
                FocusedIndex = 0;
                Dispatcher.Invoke(() =>
                {
                    _searchResultElement.DrawItems(results, 0, out _focusedItem);
                    Dispatcher.BeginInvoke(
                        (Action)
                            (() => { _searchResultElement.Height = results.Count * SearchResultElement.RowHeight; }));
                });
            }
        }

        /// <summary>
        /// Moves current selection up
        /// </summary>
        public void MoveUp()
        {
            FocusedIndex += results.Count - 1;
            FocusedIndex %= results.Count;
            _searchResultElement.DrawItems(results, FocusedIndex, out _focusedItem);
        }

        /// <summary>
        /// Moves current selection down
        /// </summary>
        public void MoveDown()
        {
            FocusedIndex = ++FocusedIndex % results.Count;
            _searchResultElement.DrawItems(results, FocusedIndex, out _focusedItem);
        }

        /// <summary>
        /// Opens current selection
        /// </summary>
        /// <param name="e"></param>
        /// <param name="input"></param>
        public void Open(KeyEventArgs e, string input, bool showFileProperties = false)
        {
            e.Handled = true;
            var index = _searchResultElement.CurrentFocus;
            if (index >= 0 && index < results.Count)
            {
                results[index].Open(e, input, showFileProperties);
            }
        }

        /// <summary>
        /// Increases the priority of the selected element
        /// If it's on the first position it adds 10 to the priority
        /// </summary>
        public void IncreasePriority()
        {
            var index = _searchResultElement.CurrentFocus;
            if (index >= 0 && index < results.Count)
            {
                int diff;
                if (results[index] is SearchResultItem)
                {
                    if (index == 0 || results[index - 1] is MagicResultItem)
                    {
                        diff = 10;
                    }
                    else
                    {
                        diff = results[index - 1].Priority - results[index].Priority + 1;
                    }
                    ChangePriority(diff, index);
                }
            }
        }

        /// <summary>
        /// Decreases the priority of the selected element
        /// If it's on the last position it decreases the priority by -10
        /// </summary>
        public void DecreasePriority()
        {
            var index = _searchResultElement.CurrentFocus;
            if (index >= 0 && index < results.Count)
            {
                int diff;
                if (results[index] is SearchResultItem)
                {
                    if (index == results.Count - 1 || results[index + 1] is MagicResultItem)
                    {
                        diff = -10;
                    }
                    else
                    {
                        diff = results[index + 1].Priority - results[index].Priority - 1;
                    }
                    ChangePriority(diff, index);
                }
            }
        }

        /// <summary>
        /// Changes the priority of the given file by the providen diff
        /// </summary>
        /// <param name="diff"></param>
        /// <param name="item"></param>
        public void ChangePriority(int diff, int item)
        {
            SearchEngine.Instance.IncrementPriority(results[item].Subtitle, diff);
            _focusedItem = results[item];
            Search(_lastSearch);
        }

        /// <summary>
        /// Asks the current selected ResultItem for the AutoCompletion
        /// </summary>
        /// <returns></returns>
        public string AutoComplete()
        {
            var index = _searchResultElement.CurrentFocus;
            return results != null && index < results.Count ? results[index].AutoComplete(): string.Empty;
        }
    }
}