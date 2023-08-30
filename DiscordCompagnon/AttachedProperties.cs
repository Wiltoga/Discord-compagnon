using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DiscordCompagnon
{
    internal static class Ext
    {
        public static readonly DependencyProperty ColumnWidthProperty = DependencyProperty.RegisterAttached("ColumnWidth", typeof(double), typeof(Ext), new PropertyMetadata(0d, columnWidthChanged));
        public static readonly DependencyProperty RowHeightProperty = DependencyProperty.RegisterAttached("RowHeight", typeof(double), typeof(Ext), new PropertyMetadata(0d, rowHeightChanged));
        public static readonly DependencyProperty SimpleCornerProperty = DependencyProperty.RegisterAttached("SimpleCorner", typeof(double), typeof(Ext), new PropertyMetadata(0d, simpleCornerChanged));

        public static double GetColumnWidth(ColumnDefinition d) => (double)d.GetValue(ColumnWidthProperty);

        public static double GetRowHeight(RowDefinition d) => (double)d.GetValue(RowHeightProperty);

        public static double GetSimpleCorner(Border d) => (double)d.GetValue(SimpleCornerProperty);

        public static void SetColumnWidth(ColumnDefinition d, double value) => d.SetValue(ColumnWidthProperty, value);

        public static void SetRowHeight(RowDefinition d, double value) => d.SetValue(RowHeightProperty, value);

        public static void SetSimpleCorner(Border d, double value) => d.SetValue(SimpleCornerProperty, value);

        private static void columnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var column = (ColumnDefinition)d;
            var value = (double)e.NewValue;
            column.Width = new GridLength(value);
        }

        private static void rowHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var row = (RowDefinition)d;
            var value = (double)e.NewValue;
            row.Height = new GridLength(value);
        }

        private static void simpleCornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var border = (Border)d;
            var value = (double)e.NewValue;
            border.CornerRadius = new CornerRadius(value);
        }
    }
}