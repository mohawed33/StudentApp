using StudentApp.Models;
using System.Diagnostics;

namespace StudentApp.Services
{
    public class NotificationService
    {
        private readonly DatabaseService _databaseService;
        private readonly WhatsAppService _whatsAppService;

        public NotificationService()
        {
            _databaseService = new DatabaseService();
            _whatsAppService = new WhatsAppService();
        }

        // إرسال إشعار
        public async Task<bool> SendNotificationAsync(
            string userId,
            string title,
            string message,
            NotificationType type,
            string studentId = null,
            string senderId = null,
            string relatedId = null,
            string phoneNumber = null)
        {
            try
            {
                // إنشاء الإشعار
                var notification = new Notification
                {
                    UserId = userId,
                    Title = title,
                    Message = message,
                    Type = type,
                    StudentId = studentId,
                    SenderId = senderId,
                    RelatedId = relatedId,
                    IsRead = false,
                    IsSentToWhatsApp = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // حفظ الإشعار في قاعدة البيانات
                await _databaseService.AddNotificationAsync(notification);

                // إرسال إشعار محلي
                await SendLocalNotificationAsync(title, message);

                // إرسال إشعار عبر واتساب إذا كان رقم الهاتف متوفرًا
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    bool whatsappSent = await _whatsAppService.SendMessageAsync(phoneNumber, $"{title}\n{message}");
                    
                    if (whatsappSent)
                    {
                        notification.IsSentToWhatsApp = true;
                        notification.WhatsAppSentAt = DateTime.Now;
                        notification.UpdatedAt = DateTime.Now;
                        await _databaseService.UpdateNotificationAsync(notification);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في إرسال الإشعار: {ex.Message}");
                return false;
            }
        }

        // إرسال إشعار محلي
        private async Task SendLocalNotificationAsync(string title, string message)
        {
            try
            {
                // في الإصدار النهائي، سيتم استخدام Plugin.LocalNotification
                // لإرسال إشعارات محلية على الجهاز
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في إرسال الإشعار المحلي: {ex.Message}");
            }
        }

        // الحصول على إشعارات المستخدم
        public async Task<List<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await _databaseService.GetNotificationsByUserIdAsync(userId);
        }

        // الحصول على الإشعارات غير المقروءة للمستخدم
        public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _databaseService.GetUnreadNotificationsByUserIdAsync(userId);
        }

        // تحديث حالة قراءة الإشعار
        public async Task<bool> MarkNotificationAsReadAsync(string notificationId)
        {
            int result = await _databaseService.MarkNotificationAsReadAsync(notificationId);
            return result > 0;
        }

        // إرسال إشعار حضور
        public async Task<bool> SendAttendanceNotificationAsync(
            Attendance attendance,
            Student student,
            User teacher)
        {
            if (student == null || string.IsNullOrEmpty(student.ParentId))
                return false;

            string title = "إشعار حضور";
            string message = string.Empty;

            switch (attendance.Status)
            {
                case AttendanceStatus.Present:
                    message = $"تم تسجيل حضور {student.Name} إلى {attendance.TypeText} في تمام الساعة {attendance.TimeText}";
                    break;
                case AttendanceStatus.Absent:
                    message = $"تم تسجيل غياب {student.Name} عن {attendance.TypeText} اليوم {attendance.DateText}";
                    break;
                case AttendanceStatus.Late:
                    message = $"تم تسجيل تأخر {student.Name} عن {attendance.TypeText} بمقدار {attendance.LateMinutes} دقيقة";
                    break;
                case AttendanceStatus.Excused:
                    message = $"تم تسجيل غياب {student.Name} بعذر عن {attendance.TypeText} اليوم {attendance.DateText}";
                    break;
                case AttendanceStatus.EarlyLeave:
                    message = $"تم تسجيل خروج {student.Name} مبكراً من {attendance.TypeText} في تمام الساعة {attendance.TimeText}";
                    break;
            }

            if (!string.IsNullOrEmpty(attendance.Notes))
            {
                message += $"\nملاحظات: {attendance.Notes}";
            }

            return await SendNotificationAsync(
                student.ParentId,
                title,
                message,
                NotificationType.Attendance,
                student.StudentId,
                teacher?.UserId,
                attendance.AttendanceId,
                student.ParentPhone);
        }

        // إرسال إشعار سلوك
        public async Task<bool> SendBehaviorNotificationAsync(
            Behavior behavior,
            Student student,
            User teacher)
        {
            if (student == null || string.IsNullOrEmpty(student.ParentId))
                return false;

            string title = behavior.Type == BehaviorType.Positive ? "سلوك إيجابي" : "سلوك سلبي";
            string message = $"{behavior.Title}: {behavior.Description}";
            
            if (behavior.Points != 0)
            {
                message += $"\nالنقاط: {behavior.PointsText}";
            }

            if (!string.IsNullOrEmpty(behavior.Location))
            {
                message += $"\nالمكان: {behavior.Location}";
            }

            message += $"\nالتاريخ: {behavior.DateText} الساعة {behavior.TimeText}";

            return await SendNotificationAsync(
                student.ParentId,
                title,
                message,
                NotificationType.Behavior,
                student.StudentId,
                teacher?.UserId,
                behavior.BehaviorId,
                student.ParentPhone);
        }
    }
}
