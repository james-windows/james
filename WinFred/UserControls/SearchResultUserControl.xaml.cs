using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinFred.Search;

namespace WinFred.UserControls
{
    /// <summary>
    /// Interaction logic for SearchResultUserControl.xaml
    /// </summary>
    public partial class SearchResultUserControl : UserControl
    {
        private SearchResultElement searchResultElement;
        private List<SearchResult> searchResults;
        public SearchResultUserControl()
        {
            InitializeComponent();
            searchResultElement = new SearchResultElement();
            searchResultElement.Width = 700;
            Grid.Children.Add(searchResultElement);
        }

        public int FocusedIndex{get; private set;}

        public void Search(string str)
        {
            DateTime tmp = DateTime.Now;
            searchResults = SearchEngine.GetInstance().Query(str);
            FocusedIndex = 0;
            searchResultElement.DrawItems(searchResults, FocusedIndex);
            Dispatcher.BeginInvoke((Action)(() =>
            {
                searchResultElement.Height = searchResults.Count * SearchResultElement.ROW_HEIGHT;
            }));

            Debug.WriteLine((DateTime.Now - tmp).TotalMilliseconds);
        }

        public void MoveUp()
        {
            if (FocusedIndex > 0)
            {
                FocusedIndex--;
                searchResultElement.DrawItems(searchResults, FocusedIndex);
            }
        }

        public void MoveDown()
        {
            if (FocusedIndex < searchResults.Count - 1)
            {
                FocusedIndex++;
                searchResultElement.DrawItems(searchResults, FocusedIndex);
            }
        }

        public void Open(KeyEventArgs e)
        {
            if (searchResultElement.Currentfocus >= 0 && searchResultElement.Currentfocus < searchResults.Count)
            {
                if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                {
                    searchResults[searchResultElement.Currentfocus].OpenFolder();
                }
                else
                {
                    searchResults[searchResultElement.Currentfocus].Open();
                }
            }
        }
    }
}
