using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StudentApp.ViewModels.Parent
{
    public partial class ParentHomeViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<Student> _children;

        [ObservableProperty]
        private ObservableCollection<Attendance> _todayAttendance;

        [ObservableProperty]
        private ObservableCollection<Behavior> _recentBehaviors;

        [ObservableProperty]
        private ObservableCollection<Notification> _notifications;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private string _parentName;

        [ObservableProperty]
        private string _currentDate;

        [ObservableProperty]
        private int _unreadNotifications;

        public ParentHomeViewModel()
        {
            _databaseService = new DatabaseService();
            _notificationService = new NotificationService();
            
            Children = new ObservableCollection<Student>();
            TodayAttendance = new ObservableCollection<Attendance>();
            RecentBehaviors = new ObservableCollection<Behavior>();
            Notifications = new ObservableCollection<Notification>();
            
            CurrentDate = DateTime.Now.ToString("dddd, dd MMMM yyyy", new System.Globalization.CultureInfo("ar-SA"));
            ParentName = App.AuthService.CurrentUser?.Name ?? "ولي الأمر";
            
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

                // تحميل الأبناء
                if (App.AuthService.CurrentUser != null)
                {
                    var children = await _databaseService.GetStudentsByParentIdAsync(App.AuthService.CurrentUser.UserId);
                    Children.Clear();
                    foreach (var child in children)
                    {
                        Children.Add(child);
                    }

                    // تحميل سجلات الحضور لليوم
                    TodayAttendance.Clear();
                    foreach (var child in Children)
                    {
                        var attendanceRecords = await _databaseService.GetAttendanceByStudentAndDateAsync(child.StudentId, DateTime.Today);
                        foreach (var attendance in attendanceRecords)
                        {
                            TodayAttendance.Add(attendance);
                        }
                    }

                    // تحميل سجلات السلوك الأخيرة
                    RecentBehaviors.Clear();
                    foreach (var child in Children)
                    {
                        var behaviorRecords = await _databaseService.GetBehaviorByStudentIdAsync(child.StudentId);
                        foreach (var behavior in behaviorRecords.OrderByDescending(b => b.Timestamp).Take(3))
                        {
                            RecentBehaviors.Add(behavior);
                        }
                    }

                    // تحميل الإشعارات
                    var notifications = await _notificationService.GetUserNotificationsAsync(App.AuthService.CurrentUser.UserId);
                    Notifications.Clear();
                    foreach (var notification in notifications.OrderByDescending(n => n.CreatedAt).Take(5))
                    {
                        Notifications.Add(notification);
                    }

                    // حساب عدد الإشعارات غير المقروءة
                    var unreadNotifications = await _notificationService.GetUnreadNotificationsAsync(App.AuthService.CurrentUser.UserId);
                    UnreadNotifications = unreadNotifications.Count;
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
        private async Task NavigateToChildrenAsync()
        {
            await Shell.Current.GoToAsync("//ParentTabs/ChildrenPage");
        }

        [RelayCommand]
        private async Task NavigateToAttendanceHistoryAsync()
        {
            await Shell.Current.GoToAsync("//ParentTabs/AttendanceHistoryPage");
        }

        [RelayCommand]
        private async Task NavigateToBehaviorHistoryAsync()
        {
            await Shell.Current.GoToAsync("//ParentTabs/BehaviorHistoryPage");
        }

        [RelayCommand]
        private async Task NavigateToReportsAsync()
        {
            await Shell.Current.GoToAsync("//ParentTabs/ParentReportsPage");
        }

        [RelayCommand]
        private async Task ViewChildDetailsAsync(Student child)
        {
            if (child == null)
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Student", child }
            };

            await Shell.Current.GoToAsync("StudentProfilePage", navigationParameter);
        }

        [RelayCommand]
        private async Task MarkAllNotificationsAsReadAsync()
        {
            try
            {
                IsLoading = true;

                foreach (var notification in Notifications.Where(n => !n.IsRead))
                {
                    await _notificationService.MarkNotificationAsReadAsync(notification.NotificationId);
                }

                UnreadNotifications = 0;
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في تحديث حالة الإشعارات: {ex.Message}");
                await Shell.Current.DisplayAlert("خطأ", "حدث خطأ أثناء تحديث حالة الإشعارات", "موافق");
            }
            finally
            {
                IsLoading = false;
            }
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
