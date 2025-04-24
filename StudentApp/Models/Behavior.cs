using SQLite;
using System.Text.Json.Serialization;

namespace StudentApp.Models
{
    public enum BehaviorType
    {
        Positive, // سلوك إيجابي
        Negative, // سلوك سلبي
    }

    public class Behavior
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonPropertyName("behaviorId")]
        public string BehaviorId { get; set; } = Guid.NewGuid().ToString();

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
        public BehaviorType Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } // عنوان السلوك

        [JsonPropertyName("description")]
        public string Description { get; set; } // وصف السلوك

        [JsonPropertyName("points")]
        public int Points { get; set; } // النقاط (موجبة للإيجابي، سالبة للسلبي)

        [JsonPropertyName("mediaUrls")]
        public List<string> MediaUrls { get; set; } = new List<string>(); // روابط الصور أو التسجيلات الصوتية

        [JsonPropertyName("location")]
        public string Location { get; set; } // مكان حدوث السلوك (الفصل، الملعب، إلخ)

        [JsonPropertyName("notified")]
        public bool Notified { get; set; } // هل تم إشعار ولي الأمر

        [JsonPropertyName("notificationTime")]
        public DateTime? NotificationTime { get; set; } // وقت الإشعار

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // خصائص محسوبة
        [Ignore]
        [JsonIgnore]
        public string TypeText => Type == BehaviorType.Positive ? "إيجابي" : "سلبي";

        [Ignore]
        [JsonIgnore]
        public string TimeText => Timestamp.ToString("hh:mm tt");

        [Ignore]
        [JsonIgnore]
        public string DateText => Date.ToString("dd/MM/yyyy");

        [Ignore]
        [JsonIgnore]
        public string PointsText => Type == BehaviorType.Positive ? $"+{Points}" : $"-{Points}";

        [Ignore]
        [JsonIgnore]
        public string NotifiedText => Notified ? "تم الإشعار" : "لم يتم الإشعار";

        [Ignore]
        [JsonIgnore]
        public Color TypeColor => Type == BehaviorType.Positive ? Colors.Green : Colors.Red;
    }
}
