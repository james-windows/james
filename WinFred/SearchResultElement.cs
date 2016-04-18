using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using James.ResultItems;
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
        private List<ResultItem> _searchResults;

        public SearchResultElement()
        {
            Config.Instance.WindowChangedAccentColor += UpdateAccentColor;
            _children = new VisualCollection(this);
            UpdateAccentColor(this, null);
        }

        public int CurrentFocus { get; private set; }
        public Brush FocusBackgroundBrush { get; set; }
        public Brush FocusForegroundBrush { get; set; }

        /// <summary>
        /// Dynamically reloads the accent colors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateAccentColor(object sender, EventArgs e)
        {
            FocusBackgroundBrush = (Brush) ThemeManager.GetResourceFromAppStyle(null, "AccentColorBrush");
            FocusForegroundBrush = (Brush) ThemeManager.GetResourceFromAppStyle(null, "IdealForegroundColorBrush");
        }

        /// <summary>
        /// Draws the providen searchResults items and focused the correct item
        /// </summary>
        /// <param name="searchResults"></param>
        /// <param name="focusedIndex"></param>
        /// <param name="focusedItem"></param>
        public void DrawItems(List<ResultItem> searchResults, int focusedIndex, out ResultItem focusedItem)
        {
            focusedItem = searchResults.ElementAtOrDefault(focusedIndex);
            _searchResults = searchResults;
            Dispatcher.BeginInvoke((Action) (() =>
            {
                _children.Clear();
                CurrentFocus = focusedIndex;
                for (var i = 0; i < searchResults.Count; i++)
                {
                    DrawBackground(i, focusedIndex == i? FocusBackgroundBrush : Brushes.Transparent);
                    DrawItemAtPos(searchResults[i], i);
                }
            }));
        }

        /// <summary>
        /// Draws the background of the item
        /// </summary>
        /// <param name="index"></param>
        private void DrawBackground(int index, Brush backgroundBrush)
        {
            var drawingVisual = new DrawingVisual();
            using (var ctx = drawingVisual.RenderOpen())
            {
                var rect = new Rect(new Point(0, index*RowHeight), new Size(ElementWidth, RowHeight));
                ctx.DrawRectangle(backgroundBrush, null, rect);
            }
            _children.Add(drawingVisual);
        }

        /// <summary>
        /// Draws resultitem on a specific index
        /// </summary>
        /// <param name="resultItem"></param>
        /// <param name="index"></param>
        private void DrawItemAtPos(ResultItem resultItem, int index)
        {
            var drawingVisual = new DrawingVisual();
            using (var ctx = drawingVisual.RenderOpen())
            {
                var isFocused = index == CurrentFocus;
                ctx.DrawText(CreateText(resultItem.Title, LargeFontSize, isFocused), new Point(50, index*RowHeight));
                ctx.DrawText(CreateText(resultItem.Subtitle, SmallFontSize, isFocused),
                    new Point(50, index*RowHeight + 25));
                if (Config.Instance.DisplayPriorities && resultItem is SearchResultItem)
                {
                    ctx.DrawText(CreateText(resultItem.Priority.ToString(), SmallFontSize, isFocused), new Point(5, index*RowHeight));
                    if (Config.Instance.DisplayFileIcons)
                    {
                        ctx.DrawImage(resultItem.Icon, new Rect(SmallFontSize, SmallFontSize + index*RowHeight, 32, 32));
                    }
                }
                else
                {
                    ctx.DrawImage(resultItem.Icon, new Rect(SmallFontSize/2, SmallFontSize/2 + index*RowHeight, 40, 40));
                }
            }
            _children.Add(drawingVisual);
        }

        /// <summary>
        /// Creates the text and setts some configurations
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <param name="focused"></param>
        /// <returns></returns>
        private FormattedText CreateText(string text, int fontSize, bool focused = false)
        {
            var formattedText = new FormattedText(
                text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                fontSize,
                GetBrush(focused)) {MaxTextWidth = ElementWidth - 30};
            formattedText.MaxTextWidth = ElementWidth - 50;
            formattedText.MaxTextHeight = LargeFontSize + 10;
            return formattedText;
        }

        /// <summary>
        /// Checks the user defined color from the config IsBaseLight?
        /// </summary>
        /// <param name="focused"></param>
        /// <returns></returns>
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
            if (index< 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
        #endregion
    }
}