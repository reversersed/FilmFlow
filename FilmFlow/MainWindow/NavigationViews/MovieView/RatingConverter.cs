using System;
using System.Globalization;
using System.Windows.Data;

namespace FilmFlow.MainWindow.NavigationViews.MovieView
{
    [ValueConversion(typeof(float), typeof(int))]
    public class RatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)Math.Round(20 * (float)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (float)value/20.0;
    }
}
