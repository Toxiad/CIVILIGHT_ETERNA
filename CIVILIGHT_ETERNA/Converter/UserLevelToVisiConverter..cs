
using System;
using System.Windows;
using System.Windows.Data;
using Toxiad.IO.Standar.Module;

namespace Babel.Converter
{
    [ValueConversion(typeof(int), typeof(Visibility))]
    public class UserLevelToVisiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            UserLevel limit = UserLevel.Operator;
            if (parameter != null)
            {
                limit = (UserLevel)int.Parse((string)parameter);
            }
            return limit.HasFlag((UserLevel)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
