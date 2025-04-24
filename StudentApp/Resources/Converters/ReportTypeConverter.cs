using StudentApp.ViewModels.Teacher;
using System.Globalization;

namespace StudentApp.Resources.Converters
{
    public class ReportTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ReportType type)
            {
                return type switch
                {
                    ReportType.StudentReport => "تقرير طالب",
                    ReportType.ClassReport => "تقرير صف",
                    ReportType.AttendanceReport => "تقرير الحضور",
                    ReportType.BehaviorReport => "تقرير السلوك",
                    ReportType.SchoolReport => "تقرير المدرسة",
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
                    "تقرير طالب" => ReportType.StudentReport,
                    "تقرير صف" => ReportType.ClassReport,
                    "تقرير الحضور" => ReportType.AttendanceReport,
                    "تقرير السلوك" => ReportType.BehaviorReport,
                    "تقرير المدرسة" => ReportType.SchoolReport,
                    _ => ReportType.StudentReport
                };
            }

            return ReportType.StudentReport;
        }
    }
}
