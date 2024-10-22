using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Babel.Converter
{
    [ValueConversion(typeof(ObservableCollection<object>), typeof(bool))]
    public class CollectionToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                try
                {
                    var c = ((ObservableCollection<object>)value).Count;
                    if (c > 0)
                    {
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
