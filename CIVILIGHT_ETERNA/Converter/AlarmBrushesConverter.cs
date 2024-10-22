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
    [ValueConversion(typeof(Level), typeof(Brush))]
    public class AlarmBrushesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            SolidColorBrush br;
            if (parameter != null && (string)parameter == "2" && (Level)value == Level.Info)
            {
                return (SolidColorBrush)Application.Current.FindResource("TALMINFOBRUSH");
            }
            if (parameter != null && (string)parameter == "1")
            {

                return (Level)value == Level.Info ? (SolidColorBrush)Application.Current.FindResource("MAINBLACKBRUSH") : (SolidColorBrush)Application.Current.FindResource("MAINLIGHTBRUSH");
            }
            switch ((Level)value)
            {
                case Level.Alarm:
                    br = (SolidColorBrush)Application.Current.FindResource("TALMERRBRUSH");
                    break;
                case Level.Warning:
                    br = (SolidColorBrush)Application.Current.FindResource("TALMWARNBRUSH");
                    break;
                case Level.Info:
                    br = (SolidColorBrush)Application.Current.FindResource("MAINLIGHTBRUSH");
                    break;
                default:
                    br = (SolidColorBrush)Application.Current.FindResource("MAINDARKBRUSH");
                    break;
            }
            return br;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
