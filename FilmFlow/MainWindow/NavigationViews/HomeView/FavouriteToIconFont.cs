using FontAwesome.Sharp;
using System;
using System.Globalization;
using System.Windows.Data;

namespace FilmFlow.MainWindow.NavigationViews.HomeView
{
    [ValueConversion(typeof(bool), typeof(IconFont))]
    public class FavouriteToIconFont : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? IconFont.Solid : IconFont.Regular;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
