using SQLite;
using System.Text.Json.Serialization;

namespace StudentApp.Models
{
    public enum NotificationType
    {
        Attendance,  // إشعار حضور/غياب
        Behavior,    // إشعار سلوك
        Report,      // إشعار تقرير
        Message,     // رسالة عامة
        Alert,       // تنبيه
    }

    public class Notification
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [JsonPropertyName("notificationId")]
        public string NotificationId { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("userId")]
        public string UserId { get; set; } // معرف المستخدم المستلم

        [JsonPropertyName("studentId")]
        public string StudentId { get; set; } // معرف الطالب (إن وجد)

        [JsonPropertyName("senderId")]
        public string SenderId { get; set; } // معرف المرسل

        [JsonPropertyName("title")]
        public string Title { get; set; } // عنوان الإشعار

        [JsonPropertyName("message")]
        public string Message { get; set; } // نص الإشعار

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public NotificationType Type { get; set; } // نوع الإشعار

        [JsonPropertyName("relatedId")]
        public string RelatedId { get; set; } // معرف العنصر المرتبط (سلوك، حضور، إلخ)

        [JsonPropertyName("isRead")]
        public bool IsRead { get; set; } // هل تم قراءة الإشعار

        [JsonPropertyName("readAt")]
        public DateTime? ReadAt { get; set; } // وقت قراءة الإشعار

        [JsonPropertyName("isSentToWhatsApp")]
        public bool IsSentToWhatsApp { get; set; } // هل تم إرسال الإشعار عبر واتساب

        [JsonPropertyName("whatsAppSentAt")]
        public DateTime? WhatsAppSentAt { get; set; } // وقت إرسال الإشعار عبر واتساب

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // خصائص محسوبة
        [Ignore]
        [JsonIgnore]
        public string TypeText
        {
            get
            {
                return Type switch
                {
                    NotificationType.Attendance => "حضور/غياب",
                    NotificationType.Behavior => "سلوك",
                    NotificationType.Report => "تقرير",
                    NotificationType.Message => "رسالة",
                    NotificationType.Alert => "تنبيه",
                    _ => "غير معروف"
                };
            }
        }

        [Ignore]
        [JsonIgnore]
        public string TimeText => CreatedAt.ToString("hh:mm tt");

        [Ignore]
        [JsonIgnore]
        public string DateText => CreatedAt.ToString("dd/MM/yyyy");

        [Ignore]
        [JsonIgnore]
        public Color TypeColor
        {
            get
            {
                return Type switch
                {
                    NotificationType.Attendance => Colors.Blue,
                    NotificationType.Behavior => Colors.Orange,
                    NotificationType.Report => Colors.Green,
                    NotificationType.Message => Colors.Purple,
                    NotificationType.Alert => Colors.Red,
                    _ => Colors.Gray
                };
            }
        }

        [Ignore]
        [JsonIgnore]
        public string TypeIcon
        {
            get
            {
                return Type switch
                {
                    NotificationType.Attendance => "attendance_icon.png",
                    NotificationType.Behavior => "behavior_icon.png",
                    NotificationType.Report => "report_icon.png",
                    NotificationType.Message => "message_icon.png",
                    NotificationType.Alert => "alert_icon.png",
                    _ => "notification_icon.png"
                };
            }
        }
    }
}
