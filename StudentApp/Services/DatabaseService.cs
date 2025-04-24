using SQLite;
using StudentApp.Models;

namespace StudentApp.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;
        private bool _isInitialized = false;

        // تهيئة قاعدة البيانات
        private async Task InitAsync()
        {
            if (_isInitialized)
                return;

            // تحديد مسار قاعدة البيانات
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "studentapp.db3");

            // إنشاء اتصال قاعدة البيانات
            _database = new SQLiteAsyncConnection(dbPath);

            // إنشاء الجداول
            await _database.CreateTableAsync<User>();
            await _database.CreateTableAsync<Student>();
            await _database.CreateTableAsync<Attendance>();
            await _database.CreateTableAsync<Behavior>();
            await _database.CreateTableAsync<Notification>();

            // إضافة بيانات تجريبية إذا كانت قاعدة البيانات فارغة
            await SeedDataAsync();

            _isInitialized = true;
        }

        // إضافة بيانات تجريبية
        private async Task SeedDataAsync()
        {
            // التحقق من وجود مستخدمين
            var usersCount = await _database.Table<User>().CountAsync();
            if (usersCount == 0)
            {
                // إضافة مستخدمين تجريبيين
                await _database.InsertAllAsync(new List<User>
                {
                    new User
                    {
                        UserId = "1",
                        Name = "أحمد محمد",
                        Email = "teacher@example.com",
                        Phone = "0501234567",
                        Role = UserRole.Teacher,
                        SchoolId = "school1",
                    },
                    new User
                    {
                        UserId = "2",
                        Name = "محمد علي",
                        Email = "parent@example.com",
                        Phone = "0507654321",
                        Role = UserRole.Parent,
                    }
                });
            }

            // التحقق من وجود طلاب
            var studentsCount = await _database.Table<Student>().CountAsync();
            if (studentsCount == 0)
            {
                // إضافة طلاب تجريبيين
                await _database.InsertAllAsync(new List<Student>
                {
                    new Student
                    {
                        StudentId = "student1",
                        Name = "عبدالله محمد",
                        SchoolNumber = "10001",
                        Grade = "الصف الثالث",
                        Section = "أ",
                        ParentId = "2",
                        ParentName = "محمد علي",
                        ParentPhone = "0507654321",
                        ParentEmail = "parent@example.com",
                        DateOfBirth = new DateTime(2012, 5, 15),
                        Address = "الرياض، حي النزهة",
                    },
                    new Student
                    {
                        StudentId = "student2",
                        Name = "سارة محمد",
                        SchoolNumber = "10002",
                        Grade = "الصف الأول",
                        Section = "ب",
                        ParentId = "2",
                        ParentName = "محمد علي",
                        ParentPhone = "0507654321",
                        ParentEmail = "parent@example.com",
                        DateOfBirth = new DateTime(2014, 8, 20),
                        Address = "الرياض، حي النزهة",
                    }
                });
            }

            // التحقق من وجود سجلات حضور
            var attendanceCount = await _database.Table<Attendance>().CountAsync();
            if (attendanceCount == 0)
            {
                // إضافة سجلات حضور تجريبية
                await _database.InsertAllAsync(new List<Attendance>
                {
                    new Attendance
                    {
                        AttendanceId = "attendance1",
                        StudentId = "student1",
                        TeacherId = "1",
                        Date = DateTime.Today,
                        Timestamp = DateTime.Now.AddHours(-2),
                        Type = AttendanceType.SchoolEntry,
                        Status = AttendanceStatus.Present,
                    },
                    new Attendance
                    {
                        AttendanceId = "attendance2",
                        StudentId = "student2",
                        TeacherId = "1",
                        Date = DateTime.Today,
                        Timestamp = DateTime.Now.AddHours(-2).AddMinutes(15),
                        Type = AttendanceType.SchoolEntry,
                        Status = AttendanceStatus.Late,
                        LateMinutes = 15,
                        Notes = "تأخر بسبب الازدحام المروري",
                    }
                });
            }

            // التحقق من وجود سجلات سلوك
            var behaviorCount = await _database.Table<Behavior>().CountAsync();
            if (behaviorCount == 0)
            {
                // إضافة سجلات سلوك تجريبية
                await _database.InsertAllAsync(new List<Behavior>
                {
                    new Behavior
                    {
                        BehaviorId = "behavior1",
                        StudentId = "student1",
                        TeacherId = "1",
                        Date = DateTime.Today,
                        Timestamp = DateTime.Now.AddHours(-1),
                        Type = BehaviorType.Positive,
                        Title = "مساعدة زميل",
                        Description = "قام بمساعدة زميله في حل الواجب",
                        Points = 5,
                        Location = "الفصل",
                        Notified = true,
                        NotificationTime = DateTime.Now.AddMinutes(-30),
                    },
                    new Behavior
                    {
                        BehaviorId = "behavior2",
                        StudentId = "student2",
                        TeacherId = "1",
                        Date = DateTime.Today,
                        Timestamp = DateTime.Now.AddHours(-1).AddMinutes(30),
                        Type = BehaviorType.Negative,
                        Title = "عدم الانتباه",
                        Description = "لم يكن منتبهاً أثناء الشرح",
                        Points = 2,
                        Location = "الفصل",
                        Notified = true,
                        NotificationTime = DateTime.Now.AddMinutes(-15),
                    }
                });
            }

            // التحقق من وجود إشعارات
            var notificationCount = await _database.Table<Notification>().CountAsync();
            if (notificationCount == 0)
            {
                // إضافة إشعارات تجريبية
                await _database.InsertAllAsync(new List<Notification>
                {
                    new Notification
                    {
                        NotificationId = "notification1",
                        UserId = "2",
                        StudentId = "student1",
                        SenderId = "1",
                        Title = "سلوك إيجابي",
                        Message = "قام عبدالله بمساعدة زميله في حل الواجب",
                        Type = NotificationType.Behavior,
                        RelatedId = "behavior1",
                        IsRead = false,
                        IsSentToWhatsApp = true,
                        WhatsAppSentAt = DateTime.Now.AddMinutes(-30),
                    },
                    new Notification
                    {
                        NotificationId = "notification2",
                        UserId = "2",
                        StudentId = "student2",
                        SenderId = "1",
                        Title = "تأخر في الحضور",
                        Message = "تأخرت سارة في الحضور 15 دقيقة بسبب الازدحام المروري",
                        Type = NotificationType.Attendance,
                        RelatedId = "attendance2",
                        IsRead = true,
                        ReadAt = DateTime.Now.AddMinutes(-10),
                        IsSentToWhatsApp = true,
                        WhatsAppSentAt = DateTime.Now.AddMinutes(-15),
                    }
                });
            }
        }

        #region User Methods

        // الحصول على مستخدم بواسطة المعرف
        public async Task<User> GetUserByIdAsync(string userId)
        {
            await InitAsync();
            return await _database.Table<User>()
                .Where(u => u.UserId == userId)
                .FirstOrDefaultAsync();
        }

        // الحصول على مستخدم بواسطة البريد الإلكتروني
        public async Task<User> GetUserByEmailAsync(string email)
        {
            await InitAsync();
            return await _database.Table<User>()
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        // إضافة مستخدم جديد
        public async Task<int> AddUserAsync(User user)
        {
            await InitAsync();
            return await _database.InsertAsync(user);
        }

        // تحديث بيانات مستخدم
        public async Task<int> UpdateUserAsync(User user)
        {
            await InitAsync();
            return await _database.UpdateAsync(user);
        }

        #endregion

        #region Student Methods

        // الحصول على قائمة الطلاب
        public async Task<List<Student>> GetStudentsAsync()
        {
            await InitAsync();
            return await _database.Table<Student>().ToListAsync();
        }

        // الحصول على قائمة الطلاب لولي أمر معين
        public async Task<List<Student>> GetStudentsByParentIdAsync(string parentId)
        {
            await InitAsync();
            return await _database.Table<Student>()
                .Where(s => s.ParentId == parentId)
                .ToListAsync();
        }

        // الحصول على طالب بواسطة المعرف
        public async Task<Student> GetStudentByIdAsync(string studentId)
        {
            await InitAsync();
            return await _database.Table<Student>()
                .Where(s => s.StudentId == studentId)
                .FirstOrDefaultAsync();
        }

        // إضافة طالب جديد
        public async Task<int> AddStudentAsync(Student student)
        {
            await InitAsync();
            return await _database.InsertAsync(student);
        }

        // تحديث بيانات طالب
        public async Task<int> UpdateStudentAsync(Student student)
        {
            await InitAsync();
            return await _database.UpdateAsync(student);
        }

        // حذف طالب
        public async Task<int> DeleteStudentAsync(Student student)
        {
            await InitAsync();
            return await _database.DeleteAsync(student);
        }

        #endregion

        #region Attendance Methods

        // الحصول على سجل الحضور لطالب معين
        public async Task<List<Attendance>> GetAttendanceByStudentIdAsync(string studentId)
        {
            await InitAsync();
            return await _database.Table<Attendance>()
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.Date)
                .ThenByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        // الحصول على سجل الحضور لتاريخ معين
        public async Task<List<Attendance>> GetAttendanceByDateAsync(DateTime date)
        {
            await InitAsync();
            return await _database.Table<Attendance>()
                .Where(a => a.Date.Date == date.Date)
                .OrderBy(a => a.Timestamp)
                .ToListAsync();
        }

        // الحصول على سجل الحضور لطالب وتاريخ معين
        public async Task<List<Attendance>> GetAttendanceByStudentAndDateAsync(string studentId, DateTime date)
        {
            await InitAsync();
            return await _database.Table<Attendance>()
                .Where(a => a.StudentId == studentId && a.Date.Date == date.Date)
                .OrderBy(a => a.Timestamp)
                .ToListAsync();
        }

        // إضافة سجل حضور جديد
        public async Task<int> AddAttendanceAsync(Attendance attendance)
        {
            await InitAsync();
            return await _database.InsertAsync(attendance);
        }

        // تحديث سجل حضور
        public async Task<int> UpdateAttendanceAsync(Attendance attendance)
        {
            await InitAsync();
            return await _database.UpdateAsync(attendance);
        }

        #endregion

        #region Behavior Methods

        // الحصول على سجل السلوك لطالب معين
        public async Task<List<Behavior>> GetBehaviorByStudentIdAsync(string studentId)
        {
            await InitAsync();
            return await _database.Table<Behavior>()
                .Where(b => b.StudentId == studentId)
                .OrderByDescending(b => b.Date)
                .ThenByDescending(b => b.Timestamp)
                .ToListAsync();
        }

        // الحصول على سجل السلوك لتاريخ معين
        public async Task<List<Behavior>> GetBehaviorByDateAsync(DateTime date)
        {
            await InitAsync();
            return await _database.Table<Behavior>()
                .Where(b => b.Date.Date == date.Date)
                .OrderBy(b => b.Timestamp)
                .ToListAsync();
        }

        // الحصول على سجل السلوك لطالب وتاريخ معين
        public async Task<List<Behavior>> GetBehaviorByStudentAndDateAsync(string studentId, DateTime date)
        {
            await InitAsync();
            return await _database.Table<Behavior>()
                .Where(b => b.StudentId == studentId && b.Date.Date == date.Date)
                .OrderBy(b => b.Timestamp)
                .ToListAsync();
        }

        // إضافة سجل سلوك جديد
        public async Task<int> AddBehaviorAsync(Behavior behavior)
        {
            await InitAsync();
            return await _database.InsertAsync(behavior);
        }

        // تحديث سجل سلوك
        public async Task<int> UpdateBehaviorAsync(Behavior behavior)
        {
            await InitAsync();
            return await _database.UpdateAsync(behavior);
        }

        #endregion

        #region Notification Methods

        // الحصول على الإشعارات لمستخدم معين
        public async Task<List<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            await InitAsync();
            return await _database.Table<Notification>()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // الحصول على الإشعارات غير المقروءة لمستخدم معين
        public async Task<List<Notification>> GetUnreadNotificationsByUserIdAsync(string userId)
        {
            await InitAsync();
            return await _database.Table<Notification>()
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        // إضافة إشعار جديد
        public async Task<int> AddNotificationAsync(Notification notification)
        {
            await InitAsync();
            return await _database.InsertAsync(notification);
        }

        // تحديث إشعار
        public async Task<int> UpdateNotificationAsync(Notification notification)
        {
            await InitAsync();
            return await _database.UpdateAsync(notification);
        }

        // تحديث حالة قراءة الإشعار
        public async Task<int> MarkNotificationAsReadAsync(string notificationId)
        {
            await InitAsync();
            var notification = await _database.Table<Notification>()
                .Where(n => n.NotificationId == notificationId)
                .FirstOrDefaultAsync();

            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.Now;
                notification.UpdatedAt = DateTime.Now;
                return await _database.UpdateAsync(notification);
            }

            return 0;
        }

        #endregion
    }
}
