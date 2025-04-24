using StudentApp.Models;
using System.Globalization;

namespace StudentApp.Resources.Converters
{
    public class AttendanceStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AttendanceStatus status)
            {
                return status switch
                {
                    AttendanceStatus.Present => "حاضر",
                    AttendanceStatus.Absent => "غائب",
                    AttendanceStatus.Late => "متأخر",
                    AttendanceStatus.Excused => "غياب بعذر",
                    AttendanceStatus.EarlyLeave => "خروج مبكر",
                    _ => "غير معروف"
                };
            }

            return "غير معروف";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str switch
                {
                    "حاضر" => AttendanceStatus.Present,
                    "غائب" => AttendanceStatus.Absent,
                    "متأخر" => AttendanceStatus.Late,
                    "غياب بعذر" => AttendanceStatus.Excused,
                    "خروج مبكر" => AttendanceStatus.EarlyLeave,
                    _ => AttendanceStatus.Present
                };
            }

            return AttendanceStatus.Present;
        }
    }
}
