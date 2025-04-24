using StudentApp.Models;
using System.Globalization;

namespace StudentApp.Resources.Converters
{
    public class AttendanceStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AttendanceStatus status)
            {
                return status switch
                {
                    AttendanceStatus.Present => Colors.Green,
                    AttendanceStatus.Absent => Colors.Red,
                    AttendanceStatus.Late => Colors.Orange,
                    AttendanceStatus.Excused => Colors.Blue,
                    AttendanceStatus.EarlyLeave => Colors.Purple,
                    _ => Colors.Gray
                };
            }

            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
