using StudentApp.Models;
using System.Globalization;

namespace StudentApp.Resources.Converters
{
    public class BehaviorTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BehaviorType type)
            {
                return type switch
                {
                    BehaviorType.Positive => "سلوك إيجابي",
                    BehaviorType.Negative => "سلوك سلبي",
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
                    "سلوك إيجابي" => BehaviorType.Positive,
                    "سلوك سلبي" => BehaviorType.Negative,
                    _ => BehaviorType.Positive
                };
            }

            return BehaviorType.Positive;
        }
    }
}
