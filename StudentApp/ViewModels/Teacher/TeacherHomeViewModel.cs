using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StudentApp.ViewModels.Teacher
{
    public partial class TeacherHomeViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<Student> _students;

        [ObservableProperty]
        private ObservableCollection<Attendance> _todayAttendance;

        [ObservableProperty]
        private ObservableCollection<Behavior> _recentBehaviors;

        [ObservableProperty]
        private ObservableCollection<Notification> _notifications;

        [ObservableProperty]
        private int _totalStudents;

        [ObservableProperty]
        private int _presentStudents;

        [ObservableProperty]
        private int _absentStudents;

        [ObservableProperty]
        private int _lateStudents;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _teacherName;

        [ObservableProperty]
        private string _currentDate;

        public TeacherHomeViewModel()
        {
            _databaseService = new DatabaseService();
            _notificationService = new NotificationService();
            
            Students = new ObservableCollection<Student>();
            TodayAttendance = new ObservableCollection<Attendance>();
            RecentBehaviors = new ObservableCollection<Behavior>();
            Notifications = new ObservableCollection<Notification>();
            
            CurrentDate = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("ar-SA"));
            TeacherName = App.AuthService.CurrentUser?.Name ?? "المعلم";
            
            LoadDataAsync();
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            if (IsLoading)
                return;

            try
            {
                IsLoading = true;
                IsRefreshing = true;

                // تحميل الطلاب
                var students = await _databaseService.GetStudentsAsync();
                Students.Clear();
                foreach (var student in students)
                {
                    Students.Add(student);
                }
                TotalStudents = Students.Count;

                // تحميل سجلات الحضور لليوم
                var attendanceRecords = await _databaseService.GetAttendanceByDateAsync(DateTime.Today);
                TodayAttendance.Clear();
                foreach (var attendance in attendanceRecords)
                {
                    TodayAttendance.Add(attendance);
                }

                // حساب إحصائيات الحضور
                PresentStudents = TodayAttendance.Count(a => a.Status == AttendanceStatus.Present);
                AbsentStudents = TodayAttendance.Count(a => a.Status == AttendanceStatus.Absent);
                LateStudents = TodayAttendance.Count(a => a.Status == AttendanceStatus.Late);

                // تحميل سجلات السلوك الأخيرة
                var behaviors = await _databaseService.GetBehaviorByDateAsync(DateTime.Today);
                RecentBehaviors.Clear();
                foreach (var behavior in behaviors.OrderByDescending(b => b.Timestamp).Take(5))
                {
                    RecentBehaviors.Add(behavior);
                }

                // تحميل الإشعارات
                if (App.AuthService.CurrentUser != null)
                {
                    var notifications = await _notificationService.GetUserNotificationsAsync(App.AuthService.CurrentUser.UserId);
                    Notifications.Clear();
                    foreach (var notification in notifications.OrderByDescending(n => n.CreatedAt).Take(5))
                    {
                        Notifications.Add(notification);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في تحميل البيانات: {ex.Message}");
                await Shell.Current.DisplayAlert("خطأ", "حدث خطأ أثناء تحميل البيانات", "موافق");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task NavigateToStudentListAsync()
        {
            await Shell.Current.GoToAsync("//TeacherTabs/StudentListPage");
        }

        [RelayCommand]
        private async Task NavigateToAttendanceAsync()
        {
            await Shell.Current.GoToAsync("//TeacherTabs/AttendancePage");
        }

        [RelayCommand]
        private async Task NavigateToBehaviorAsync()
        {
            await Shell.Current.GoToAsync("//TeacherTabs/BehaviorPage");
        }

        [RelayCommand]
        private async Task NavigateToReportsAsync()
        {
            await Shell.Current.GoToAsync("//TeacherTabs/ReportsPage");
        }

        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert("تسجيل الخروج", "هل أنت متأكد من رغبتك في تسجيل الخروج؟", "نعم", "لا");
            
            if (confirm)
            {
                await App.AuthService.LogoutAsync();
                await Shell.Current.GoToAsync("//LoginPage");
            }
        }
    }
}
