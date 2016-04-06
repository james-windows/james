using System.Windows;
using System.Windows.Controls;

namespace James.HelperClasses
{
    internal class GridHelper
    {
        #region ColumnDefinition Property

        /// <summary>
        ///     Adds the specified Columns to the Grid
        ///     Example input: a|*|2*|100
        /// </summary>
        public static readonly DependencyProperty ColumnDefinitionProperty =
            DependencyProperty.RegisterAttached(
                "ColumnDefinition", typeof (string), typeof (GridHelper),
                new PropertyMetadata(string.Empty, ColumnCountChanged));

        public static string GetColumnDefinition(DependencyObject obj)
            => (string) obj.GetValue(ColumnDefinitionProperty);

        public static void SetColumnDefinition(DependencyObject obj, string value)
            => obj.SetValue(ColumnDefinitionProperty, value);

        /// <summary>
        /// Redefines the Columndefinitions
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public static void ColumnCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || e.NewValue.ToString().Length == 0)
                return;
            var grid = (Grid) obj;
            grid.ColumnDefinitions.Clear();
            foreach (var item in e.NewValue.ToString().Split('|'))
            {
                if (item == "a")
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition {Width = GridLength.Auto});
                }
                else
                {
                    var width = 1;
                    if (item[item.Length - 1] == '*' &&
                        (item == "*" || int.TryParse(item.Substring(0, item.Length - 1), out width)))
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition
                        {
                            Width = new GridLength(width, GridUnitType.Star)
                        });
                    }
                    else if (int.TryParse(item, out width))
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition {Width = new GridLength(width)});
                    }
                }
            }
        }

        #endregion

        #region RowDefinition Property

        /// <summary>
        ///     Adds the specified Rows to the Grid
        ///     Example input: a|*|2*|100
        /// </summary>
        public static readonly DependencyProperty RowDefinitionProperty =
            DependencyProperty.RegisterAttached(
                "RowDefinition", typeof (string), typeof (GridHelper),
                new PropertyMetadata(string.Empty, RowCountChanged));

        public static string GetRowDefinition(DependencyObject obj) => (string) obj.GetValue(RowDefinitionProperty);

        public static void SetRowDefinition(DependencyObject obj, string value)
            => obj.SetValue(RowDefinitionProperty, value);

        /// <summary>
        /// Redefines the RowDefinitions
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public static void RowCountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || e.NewValue.ToString().Length == 0)
                return;
            var grid = (Grid) obj;
            grid.RowDefinitions.Clear();
            foreach (var item in e.NewValue.ToString().Split('|'))
            {
                if (item == "a")
                {
                    grid.RowDefinitions.Add(new RowDefinition {Height = GridLength.Auto});
                }
                else
                {
                    var height = 1;
                    if (item[item.Length - 1] == '*' &&
                        (item == "*" || int.TryParse(item.Substring(0, item.Length - 1), out height)))
                    {
                        grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(height, GridUnitType.Star)});
                    }
                    else if (int.TryParse(item, out height))
                    {
                        grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(height)});
                    }
                }
            }
        }
        #endregion
    }
}