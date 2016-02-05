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
        private string _lastSearch = "";
        public List<ResultItem> results;

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
                results[index].Open(null, Windows.MainWindow.GetInstance().SearchTextBox.Text);
            }
        }

        public void Search(string str, ResultItem focusedItem = null)
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
                FocusedIndex = (focusedItem != null) ? CalcFocusedItem(focusedItem) : 0;

                results = results.Take(10).ToList();
                _searchResultElement.DrawItems(results, FocusedIndex);
                Dispatcher.BeginInvoke(
                    (Action)(() => { _searchResultElement.Height = results.Count * SearchResultElement.RowHeight; }));
            }
        }

        public int CalcFocusedItem(ResultItem focusedItem)
        {
            for (var i = 0; i < results.Count; i++)
            {
                if (results[i].Subtitle == focusedItem.Subtitle)
                {
                    return i;
                }
            }
            return 0;
        }

        public void WorkflowOutput(List<MagicResultItem> searchResults)
        {
            results.RemoveAll(
                item => item is MagicResultItem &&
                    ((MagicResultItem) item).WorkflowComponent.ParentWorkflow == searchResults[0].WorkflowComponent.ParentWorkflow);
            results.InsertRange(0, searchResults);
            results = results.Take(Config.Instance.MaxSearchResults).ToList();
            FocusedIndex = 0;
            Dispatcher.Invoke(() =>
            {
                _searchResultElement.DrawItems(results, 0);
                Dispatcher.BeginInvoke(
                    (Action)
                        (() => { _searchResultElement.Height = results.Count*SearchResultElement.RowHeight; }));
            });
        }

        public void MoveUp()
        {
            if (FocusedIndex > 0)
            {
                FocusedIndex--;
                _searchResultElement.DrawItems(results, FocusedIndex);
            }
        }

        public void MoveDown()
            {
            if (FocusedIndex < results.Count - 1)
            {
                FocusedIndex++;
                _searchResultElement.DrawItems(results, FocusedIndex);
            }
        }

        public void Open(KeyEventArgs e, string input)
        {
            e.Handled = true;
            var index = _searchResultElement.CurrentFocus;
            if (index >= 0 && index < results.Count)
            {
                results[index].Open(e, input);
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
            Search(_lastSearch, results[item]);
        }

        /// <summary>
        /// Asks the current selected ResultItem for the AutoCompletion
        /// </summary>
        /// <returns></returns>
        public string AutoComplete()
        {
            var index = _searchResultElement.CurrentFocus;
            return results[index].AutoComplete();
        }
    }
}