using System;
using System.Windows.Data;
using System.Windows.Media;

namespace MKSS.APP.UIBiaoDing
{
    // 定义转换器
    [ValueConversion(typeof(string), typeof(SolidColorBrush))]
    public class DataColorConverterForGround : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || value.ToString() == "")
                return new SolidColorBrush(Colors.Black);
            bool HEGE = (bool)value;
            if (!HEGE)
            {
                try
                {
                    return new SolidColorBrush(Colors.White);
                }
                catch
                { throw; }
            }
            return new SolidColorBrush(Colors.Black); //new SolidColorBrush(Color.FromRgb(23,152,111));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }

    }

}
