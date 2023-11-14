using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FilmFlow.MainWindow.NavigationViews.MovieView
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateReviewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ((DateTime)value).Date.Equals(DateTime.Today) ? (string)FormatDate((DateTime)value) : ((DateTime)value).ToString("dd.MM.yyyy");

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => DateTime.Parse((string)value);
        private object FormatDate(DateTime value)
        {
            TimeSpan diff = DateTime.Now - value;
            if(diff.Hours < 1)
                return String.Format("{0} {1}", diff.Minutes < 1 ? diff.Seconds : diff.Minutes, Application.Current.FindResource(diff.Minutes < 1 ? "SecondAgo" : "MinuteAgo") as string);
            else
                return String.Format("{0} {1}", Application.Current.FindResource("TodayAt") as string, value.ToString("HH:mm"));
        }
    }
}
