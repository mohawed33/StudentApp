using StudentApp.Models;
using System.Globalization;

namespace StudentApp.Resources.Converters
{
    public class AttendanceTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is AttendanceType type)
            {
                return type switch
                {
                    AttendanceType.SchoolEntry => "دخول المدرسة",
                    AttendanceType.SchoolExit => "خروج من المدرسة",
                    AttendanceType.ClassEntry => "دخول الحصة",
                    AttendanceType.ClassExit => "خروج من الحصة",
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
                    "دخول المدرسة" => AttendanceType.SchoolEntry,
                    "خروج من المدرسة" => AttendanceType.SchoolExit,
                    "دخول الحصة" => AttendanceType.ClassEntry,
                    "خروج من الحصة" => AttendanceType.ClassExit,
                    _ => AttendanceType.SchoolEntry
                };
            }

            return AttendanceType.SchoolEntry;
        }
    }
}
