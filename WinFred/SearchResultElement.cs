using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro;

namespace WinFred
{
    class SearchResultElement : FrameworkElement
    {
        public const int ROW_HEIGHT = 50;
        private readonly VisualCollection _children;
        private List<SearchResult> _searchResults;

        public int CurrentFocus { get; private set; }

        public Brush FocusBackgroundBrush { get; set; }
        public Brush FocusForegroundBrush { get; set; }

        public SearchResultElement()
        {
            Config.GetInstance().WindowChangedAccentColor += UpdateAccentColor;
            _children = new VisualCollection(this);
            UpdateAccentColor(this, null);
        }

        private void UpdateAccentColor(object sender, EventArgs e)
        {
            FocusBackgroundBrush = (Brush)ThemeManager.GetResourceFromAppStyle(null, "AccentColorBrush");
            FocusForegroundBrush = (Brush)ThemeManager.GetResourceFromAppStyle(null, "IdealForegroundColorBrush");
        }

        public void DrawItems(List<SearchResult> searchResults, int focusedIndex)
        {
            this._searchResults = searchResults;
            Dispatcher.BeginInvoke((Action)(() =>
            {
                _children.Clear();
                CreateBackground();
                FocusIndex(focusedIndex);
                for (int i = 0; i < searchResults.Count; i++)
                {
                    DrawItemAtPos(searchResults[i], i);
                }
            }));
        }

        private void FocusIndex(int index)
        {
            CurrentFocus = index;
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext ctx = drawingVisual.RenderOpen())
            {
                Rect rect = new Rect(new Point(0, index * ROW_HEIGHT), new Size(700, ROW_HEIGHT));
                ctx.DrawRectangle(FocusBackgroundBrush, null, rect);
            }
            _children.Add(drawingVisual);
        }

        private void CreateBackground()
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext ctx = drawingVisual.RenderOpen())
            {
                Rect rect = new Rect(new Point(0, 0), new Size(700, _searchResults.Count * ROW_HEIGHT));
                ctx.DrawRectangle(Brushes.Transparent, null, rect);
            }
            _children.Add(drawingVisual);
        }

        private void DrawItemAtPos(SearchResult sr, int index)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext ctx = drawingVisual.RenderOpen())
            {
                bool isFocused = index == CurrentFocus;
                ctx.DrawText(CreateText(sr.Filename, 18, isFocused), new Point(30, index * ROW_HEIGHT));
                ctx.DrawText(CreateText(sr.Path, 10, isFocused), new Point(30, index * ROW_HEIGHT + 25));
                ctx.DrawText(CreateText(sr.Priority.ToString(), 10, isFocused), new Point(5, index * ROW_HEIGHT));
            }
            _children.Add(drawingVisual);
        }

        private FormattedText CreateText(string text, int fontSize, bool focused = false)
        {
            
            FormattedText formattedText = new FormattedText(
                text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                fontSize,
                GetBrush(focused)) {MaxTextWidth = 700 - 30};
            return formattedText;
        }

        private Brush GetBrush(bool focused)
        {
            Brush tmpBrush = Brushes.Black;
            if (!Config.GetInstance().IsBaseLight)
            {
                tmpBrush = Brushes.White;
            }
            if (focused)
            {
                tmpBrush = FocusForegroundBrush;
            }
            return tmpBrush;
        }

        #region necessary for FrameworkElement
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
        #endregion
    }
}
