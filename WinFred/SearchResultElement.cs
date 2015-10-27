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
        public const int RowHeight = 50;
        private const int SmallFontSize = 10;
        private const int LargeFontSize = 18;
        private const int ElementWidth = 700;
        private readonly VisualCollection _children;
        private List<SearchResult> _searchResults;

        public SearchResultElement()
        {
            Config.Instance.WindowChangedAccentColor += UpdateAccentColor;
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
                var rect = new Rect(new Point(0, index*RowHeight), new Size(ElementWidth, RowHeight));
                ctx.DrawRectangle(FocusBackgroundBrush, null, rect);
            }
            _children.Add(drawingVisual);
        }

        private void DrawItemAtPos(SearchResult sr, int index)
        {
            var drawingVisual = new DrawingVisual();
            using (var ctx = drawingVisual.RenderOpen())
            {
                var isFocused = index == CurrentFocus;
                ctx.DrawText(CreateText(sr.Filename, LargeFontSize, isFocused), new Point(50, index*RowHeight));
                ctx.DrawText(CreateText(sr.Path, SmallFontSize, isFocused), new Point(50, index*RowHeight + 25));
                ctx.DrawText(CreateText(sr.Priority.ToString(), SmallFontSize, isFocused),
                    new Point(5, index*RowHeight));
                ctx.DrawImage(sr.Icon, new Rect(SmallFontSize, SmallFontSize + index*RowHeight, 32, 32));
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
                GetBrush(focused)) {MaxTextWidth = ElementWidth - 30};
            return formattedText;
        }

        private Brush GetBrush(bool focused)
        {
            Brush tmpBrush = Brushes.Black;
            if (focused)
            {
                tmpBrush = FocusForegroundBrush;
            }
            else if (!Config.Instance.IsBaseLight)
            {
                tmpBrush = Brushes.White;
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