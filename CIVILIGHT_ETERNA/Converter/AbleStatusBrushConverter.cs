
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Babel.Converter
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    public class AbleStatusBrushConverter : IValueConverter 
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (parameter != null && (string)parameter == "1")
            {
                return !(bool)value ? (SolidColorBrush)Application.Current.FindResource("MAINDARKBRUSH") : Brushes.White;
            }
            return (bool)value ? (SolidColorBrush)Application.Current.FindResource("MAINDARKBRUSH") : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
