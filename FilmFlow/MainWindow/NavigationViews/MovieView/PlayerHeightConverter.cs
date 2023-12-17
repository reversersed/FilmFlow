using System;
using System.Globalization;
using System.Windows.Data;

namespace FilmFlow.MainWindow.NavigationViews.MovieView
{
    [ValueConversion(typeof(double), typeof(double))]
    public class PlayerHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((double)value / 16.0) * 9.0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ((double)value / 9.0) * 16.0;
    }
}
