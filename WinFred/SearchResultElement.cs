using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;

namespace James
{
    internal class SearchResultElement : FrameworkElement
    {
        public const int ROW_HEIGHT = 50;
        private const int SMALL_FONT_SIZE = 10;
        private const int LARGE_FONT_SIZE = 18;
        private const int ELEMENT_WIDTH = 700;
        private readonly VisualCollection _children;
        private List<SearchResult> _searchResults;

        public SearchResultElement()
        {
            Config.GetInstance().WindowChangedAccentColor += UpdateAccentColor;
            _children = new VisualCollection(this);
            UpdateAccentColor(this, null);
        }

        public int CurrentFocus { get; private set; }
        public Brush FocusBackgroundBrush { get; set; }
        public Brush FocusForegroundBrush { get; set; }

        private void UpdateAccentColor(object sender, EventArgs e)
        {
            FocusBackgroundBrush = (Brush) ThemeManager.GetResourceFromAppStyle(null, "AccentColorBrush");
            FocusForegroundBrush = (Brush) ThemeManager.GetResourceFromAppStyle(null, "IdealForegroundColorBrush");
        }

        public void DrawItems(List<SearchResult> searchResults, int focusedIndex)
        {
            _searchResults = searchResults;
            Dispatcher.BeginInvoke((Action) (() =>
            {
                _children.Clear();
                CreateBackground();
                FocusIndex(focusedIndex);
                for (var i = 0; i < searchResults.Count; i++)
                {
                    DrawItemAtPos(searchResults[i], i);
                }
            }));
        }

        private void FocusIndex(int index)
        {
            CurrentFocus = index;
            var drawingVisual = new DrawingVisual();
            using (var ctx = drawingVisual.RenderOpen())
            {
                var rect = new Rect(new Point(0, index*ROW_HEIGHT), new Size(ELEMENT_WIDTH, ROW_HEIGHT));
                ctx.DrawRectangle(FocusBackgroundBrush, null, rect);
            }
            _children.Add(drawingVisual);
        }

        private void CreateBackground()
        {
            var drawingVisual = new DrawingVisual();
            using (var ctx = drawingVisual.RenderOpen())
            {
                var rect = new Rect(new Point(0, 0), new Size(ELEMENT_WIDTH, _searchResults.Count*ROW_HEIGHT));
                ctx.DrawRectangle(Brushes.Transparent, null, rect);
            }
            _children.Add(drawingVisual);
        }

        private void DrawItemAtPos(SearchResult sr, int index)
        {
            var drawingVisual = new DrawingVisual();
            using (var ctx = drawingVisual.RenderOpen())
            {
                var isFocused = index == CurrentFocus;
                ctx.DrawText(CreateText(sr.Filename, LARGE_FONT_SIZE, isFocused), new Point(50, index*ROW_HEIGHT));
                ctx.DrawText(CreateText(sr.Path, SMALL_FONT_SIZE, isFocused), new Point(50, index*ROW_HEIGHT + 25));
                ctx.DrawText(CreateText(sr.Priority.ToString(), SMALL_FONT_SIZE, isFocused), new Point(5, index*ROW_HEIGHT));
                ctx.DrawImage(sr.Icon, new Rect(SMALL_FONT_SIZE, SMALL_FONT_SIZE + index*ROW_HEIGHT, 32, 32));
            }
            _children.Add(drawingVisual);
        }

        private FormattedText CreateText(string text, int fontSize, bool focused = false)
        {
            var formattedText = new FormattedText(
                text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                fontSize,
                GetBrush(focused)) {MaxTextWidth = ELEMENT_WIDTH - 30};
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

        protected override int VisualChildrenCount => _children.Count;

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