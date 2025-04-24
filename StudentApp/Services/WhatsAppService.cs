using System.Diagnostics;

namespace StudentApp.Services
{
    public class WhatsAppService
    {
        // إرسال رسالة عبر واتساب
        public async Task<bool> SendMessageAsync(string phoneNumber, string message)
        {
            try
            {
                // تنسيق رقم الهاتف
                string formattedPhone = FormatPhoneNumber(phoneNumber);

                // إنشاء رابط واتساب
                string whatsappUrl = $"https://wa.me/{formattedPhone}?text={Uri.EscapeDataString(message)}";

                // فتح الرابط في المتصفح أو تطبيق واتساب
                await Browser.Default.OpenAsync(new Uri(whatsappUrl), BrowserLaunchMode.External);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في إرسال رسالة واتساب: {ex.Message}");
                return false;
            }
        }

        // تنسيق رقم الهاتف للاستخدام مع واتساب
        private string FormatPhoneNumber(string phone)
        {
            // إزالة أي أحرف غير رقمية
            string cleaned = new string(phone.Where(char.IsDigit).ToArray());

            // إضافة رمز الدولة إذا لم يكن موجودًا (966 للسعودية)
            if (cleaned.StartsWith("0"))
            {
                cleaned = "966" + cleaned.Substring(1);
            }
            else if (!cleaned.StartsWith("966"))
            {
                cleaned = "966" + cleaned;
            }

            return cleaned;
        }
    }
}
