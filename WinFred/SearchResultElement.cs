using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using MahApps.Metro;

namespace WinFred
{
    class SearchResultElement : FrameworkElement
    {
        public const int ROW_HEIGHT = 50;
        private VisualCollection _children;
        private int _currentFocus;

        public int CurrentFocus
        {
            get { return _currentFocus; }
        }

        public Brush FocusBackgroundBrush { get; set; } = (Brush) ThemeManager.GetResourceFromAppStyle(null, "AccentColorBrush");
        public Brush FocusForegroundBrush { get; set; } = (Brush)ThemeManager.GetResourceFromAppStyle(null, "IdealForegroundColorBrush");

        public SearchResultElement()
        {
            Config.GetInstance().WindowChangedAccentColor += SearchResultElement_WindowChangedAccentColor;
            _children = new VisualCollection(this);

            //this.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(MyVisualHost_MouseLeftButtonUp);
        }

        private void SearchResultElement_WindowChangedAccentColor(object sender, EventArgs e)
        {
            FocusBackgroundBrush = (Brush)ThemeManager.GetResourceFromAppStyle(null, "AccentColorBrush");
            FocusForegroundBrush = (Brush)ThemeManager.GetResourceFromAppStyle(null, "IdealForegroundColorBrush");
        }

        public void DrawItems(List<SearchResult> searchResults, int focusedIndex)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                _children.Clear();
                focusIndex(focusedIndex);
                for (int i = 0; i < searchResults.Count; i++)
                {
                    DrawItemAtPos(searchResults[i], i);
                }
            }));
        }

        private void focusIndex(int index)
        {
            _currentFocus = index;
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext ctx = drawingVisual.RenderOpen())
            {
                Rect rect = new Rect(new Point(0, index * ROW_HEIGHT), new Size(700, ROW_HEIGHT));
                ctx.DrawRectangle(
                    FocusBackgroundBrush,
                    null,
                    rect);
            }
            _children.Add(drawingVisual);
        }

        private void DrawItemAtPos(SearchResult sr, int index)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext ctx = drawingVisual.RenderOpen())
            {
                ctx.DrawText(CreateText(sr.Filename, 18, index == _currentFocus), new Point(30, index * ROW_HEIGHT));
                ctx.DrawText(CreateText(sr.Path, 10, index == _currentFocus), new Point(30, index * ROW_HEIGHT + 25));
                ctx.DrawText(CreateText(sr.Priority.ToString(), 10, index == _currentFocus), new Point(5, index * ROW_HEIGHT));
            }
            _children.Add(drawingVisual);
        }

        private FormattedText CreateText(string text, int fontSize, bool focused = false)
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
            FormattedText formattedText = new FormattedText(
                    text,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    fontSize,
                    tmpBrush);
            formattedText.MaxTextWidth = 700 - 30;
            return formattedText;
        }

        // Provide a required override for the VisualChildrenCount property.
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        // Provide a required override for the GetVisualChild method.
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }
    }
}
