using System.Globalization;

namespace StudentApp.Resources.Converters
{
    public class SelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Colors.Transparent;

            return value.ToString() == parameter.ToString() ? Colors.LightBlue : Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedItemTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Colors.Black;

            return value.ToString() == parameter.ToString() ? Colors.Blue : Colors.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
