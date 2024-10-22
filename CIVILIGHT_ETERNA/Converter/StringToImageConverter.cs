using EquipmentLogExport.BabelSystem.Alarm;
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
    [ValueConversion(typeof(string), typeof(ImageSource))]
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            return (ImageSource)Application.Current.FindResource((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
