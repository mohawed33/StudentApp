using SQLite;
using System.Text.Json.Serialization;

namespace StudentApp.Models
{
    public enum AttendanceType
    {
        SchoolEntry, // دخول المدرسة
        SchoolExit,  // خروج من المدرسة
        ClassEntry,  // دخول الحصة
        ClassExit,   // خروج من الحصة
    }

    public enum AttendanceStatus
    {
        Present,    // حاضر
        Absent,     // غائب
        Late,       // متأخر
        Excused,    // غياب بعذر
        EarlyLeave, // خروج مبكر
    }

    public class Attendance
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonPropertyName("attendanceId")]
        public string AttendanceId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("studentId")]
        public string StudentId { get; set; }

        [JsonPropertyName("teacherId")]
        public string TeacherId { get; set; }

        [JsonPropertyName("date")]
        public DateTime Date { get; set; } // تاريخ التسجيل

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } // وقت التسجيل الدقيق

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendanceType Type { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendanceStatus Status { get; set; }

        [JsonPropertyName("className")]
        public string ClassName { get; set; } // اسم المادة (للحصص فقط)

        [JsonPropertyName("notes")]
        public string Notes { get; set; } // ملاحظات

        [JsonPropertyName("lateMinutes")]
        public int? LateMinutes { get; set; } // عدد دقائق التأخير (إذا كان متأخرًا)

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // خصائص محسوبة
        [Ignore]
        [JsonIgnore]
        public string StatusText
        {
            get
            {
                return Status switch
                {
                    AttendanceStatus.Present => "حاضر",
                    AttendanceStatus.Absent => "غائب",
                    AttendanceStatus.Late => "متأخر",
                    AttendanceStatus.Excused => "غياب بعذر",
                    AttendanceStatus.EarlyLeave => "خروج مبكر",
                    _ => "غير معروف"
                };
            }
        }

        [Ignore]
        [JsonIgnore]
        public string TypeText
        {
            get
            {
                return Type switch
                {
                    AttendanceType.SchoolEntry => "دخول المدرسة",
                    AttendanceType.SchoolExit => "خروج من المدرسة",
                    AttendanceType.ClassEntry => "دخول الحصة",
                    AttendanceType.ClassExit => "خروج من الحصة",
                    _ => "غير معروف"
                };
            }
        }

        [Ignore]
        [JsonIgnore]
        public string TimeText => Timestamp.ToString("hh:mm tt");

        [Ignore]
        [JsonIgnore]
        public string DateText => Date.ToString("dd/MM/yyyy");
    }
}
