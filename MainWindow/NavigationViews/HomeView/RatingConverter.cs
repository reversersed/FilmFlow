﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FilmFlow.MainWindow.NavigationViews.HomeView
{
    [ValueConversion(typeof(float), typeof(int))]
    public class RatingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)Math.Round(20 * (float)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (float)value/20.0;
    }
}
