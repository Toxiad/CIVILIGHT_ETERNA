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
    [ValueConversion(typeof(string), typeof(string))]
    public class StringSplitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (parameter == null) parameter = "0";
            return ((string)value).Split(';')[int.Parse((string)parameter)];
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
