using StudentApp.Models;
using System.Globalization;

namespace StudentApp.Resources.Converters
{
    public class AttendanceStatusVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AttendanceStatus status)
            {
                return status == AttendanceStatus.Late || 
                       status == AttendanceStatus.Absent || 
                       status == AttendanceStatus.Excused || 
                       status == AttendanceStatus.EarlyLeave;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LateStatusVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AttendanceStatus status)
            {
                return status == AttendanceStatus.Late;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
