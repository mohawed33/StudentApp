using StudentApp.Models;
using System.Text.Json;

namespace StudentApp.Services
{
    public class AuthService
    {
        private User _currentUser;
        private readonly string _userKey = "current_user";

        public User CurrentUser => _currentUser;
        public bool IsLoggedIn => _currentUser != null;
        public bool IsTeacher => _currentUser?.Role == UserRole.Teacher || _currentUser?.Role == UserRole.Admin;
        public bool IsParent => _currentUser?.Role == UserRole.Parent;
        public bool IsAdmin => _currentUser?.Role == UserRole.Admin;

        public AuthService()
        {
            // محاولة استعادة المستخدم المخزن
            LoadUserFromPreferences();
        }

        // تسجيل الدخول
        public async Task<bool> LoginAsync(string username, string password, UserRole role)
        {
            try
            {
                // في الإصدار النهائي، سيتم استبدال هذا بطلب Firebase Auth
                await Task.Delay(1000); // محاكاة طلب الشبكة

                // بيانات المستخدم الوهمية للاختبار
                if (role == UserRole.Teacher && username == "teacher" && password == "password")
                {
                    _currentUser = new User
                    {
                        UserId = "1",
                        Name = "أحمد محمد",
                        Email = "teacher@example.com",
                        Phone = "0501234567",
                        Role = UserRole.Teacher,
                        SchoolId = "school1",
                        IsAuthenticated = true,
                        Token = "sample_token_teacher"
                    };
                }
                else if (role == UserRole.Parent && username == "parent" && password == "password")
                {
                    _currentUser = new User
                    {
                        UserId = "2",
                        Name = "محمد علي",
                        Email = "parent@example.com",
                        Phone = "0507654321",
                        Role = UserRole.Parent,
                        StudentIds = new List<string> { "student1", "student2" },
                        IsAuthenticated = true,
                        Token = "sample_token_parent"
                    };
                }
                else
                {
                    return false;
                }

                // حفظ بيانات المستخدم
                await SaveUserToPreferencesAsync();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تسجيل الدخول: {ex.Message}");
                return false;
            }
        }

        // تسجيل الخروج
        public async Task LogoutAsync()
        {
            _currentUser = null;
            await SecureStorage.Default.SetAsync(_userKey, string.Empty);
            Preferences.Remove(_userKey);
        }

        // التحقق من حالة تسجيل الدخول
        public async Task<bool> IsLoggedInAsync()
        {
            if (_currentUser != null)
            {
                return true;
            }

            // محاولة استعادة المستخدم المخزن
            return await LoadUserFromPreferencesAsync();
        }

        // حفظ بيانات المستخدم
        private async Task SaveUserToPreferencesAsync()
        {
            if (_currentUser == null) return;

            try
            {
                string userJson = JsonSerializer.Serialize(_currentUser);
                await SecureStorage.Default.SetAsync(_userKey, userJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في حفظ بيانات المستخدم: {ex.Message}");
            }
        }

        // استعادة بيانات المستخدم
        private void LoadUserFromPreferences()
        {
            try
            {
                string userJson = Preferences.Get(_userKey, string.Empty);
                if (!string.IsNullOrEmpty(userJson))
                {
                    _currentUser = JsonSerializer.Deserialize<User>(userJson);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في استعادة بيانات المستخدم: {ex.Message}");
            }
        }

        // استعادة بيانات المستخدم بشكل غير متزامن
        private async Task<bool> LoadUserFromPreferencesAsync()
        {
            try
            {
                string userJson = await SecureStorage.Default.GetAsync(_userKey);
                if (!string.IsNullOrEmpty(userJson))
                {
                    _currentUser = JsonSerializer.Deserialize<User>(userJson);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في استعادة بيانات المستخدم: {ex.Message}");
            }

            return false;
        }
    }
}
