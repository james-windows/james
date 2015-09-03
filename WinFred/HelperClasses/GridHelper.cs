using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ListBox = System.Windows.Forms.ListBox;

namespace WinFred.HelperClasses
{
    class GridHelper
    {
        #region ColumnDefinition Property

        /// <summary>
        /// Adds the specified Columns to the Grid
        /// Example input: a|*|2*|100
        /// </summary>
        public static readonly DependencyProperty ColumnDefinitionProperty =
            DependencyProperty.RegisterAttached(
                "ColumnDefinition", typeof(string), typeof(GridHelper),
                new PropertyMetadata(string.Empty, ColumnCountChanged));

        public static string GetColumnDefinition(DependencyObject obj)
        {
            return (string)obj.GetValue(ColumnDefinitionProperty);
        }

        public static void SetColumnDefinition(DependencyObject obj, string value)
        {
            obj.SetValue(ColumnDefinitionProperty, value);
        }

        public static void ColumnCountChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || e.NewValue.ToString().Length == 0)
                return;

            Grid grid = (Grid)obj;
            grid.ColumnDefinitions.Clear();

            foreach (var item in e.NewValue.ToString().Split('|'))
            {
                if (item == "a")
                {
                    grid.ColumnDefinitions.Add(
                        new ColumnDefinition() {Width = GridLength.Auto});
                }
                else
                {
                    int width = 1;
                    if (item[item.Length - 1] == '*' && (item == "*" || int.TryParse(item.Substring(0, item.Length - 1), out width)))
                    {
                        grid.ColumnDefinitions.Add(
                            new ColumnDefinition() {Width = new GridLength(width, GridUnitType.Star)});
                    }
                    else if (int.TryParse(item, out width))
                    {
                        grid.ColumnDefinitions.Add(
                            new ColumnDefinition() {Width = new GridLength(width)});
                    }
                }
            }
        }

        #endregion

        #region RowDefinition Property

        /// <summary>
        /// Adds the specified Rows to the Grid
        /// Example input: a|*|2*|100
        /// </summary>
        public static readonly DependencyProperty RowDefinitionProperty =
            DependencyProperty.RegisterAttached(
                "RowDefinition", typeof(string), typeof(GridHelper),
                new PropertyMetadata(string.Empty, RowCountChanged));

        public static string GetRowDefinition(DependencyObject obj)
        {
            return (string)obj.GetValue(RowDefinitionProperty);
        }

        public static void SetRowDefinition(DependencyObject obj, string value)
        {
            obj.SetValue(RowDefinitionProperty, value);
        }

        public static void RowCountChanged(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!(obj is Grid) || e.NewValue.ToString().Length == 0)
                return;

            Grid grid = (Grid)obj;
            grid.RowDefinitions.Clear();

            foreach (var item in e.NewValue.ToString().Split('|'))
            {
                if (item == "a")
                {
                    grid.RowDefinitions.Add(
                        new RowDefinition() {Height = GridLength.Auto});
                }
                else
                {
                    int height = 1;
                    if (item[item.Length - 1] == '*' && (item == "*" || int.TryParse(item.Substring(0, item.Length - 1), out height)))
                    {
                        grid.RowDefinitions.Add(
                            new RowDefinition() {Height = new GridLength(height, GridUnitType.Star)});
                    }
                    else if (int.TryParse(item, out height))
                    {
                        grid.RowDefinitions.Add(
                            new RowDefinition() {Height = new GridLength(height)});
                    }
                }
            }
        }

        #endregion
    }
}
