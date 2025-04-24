using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StudentApp.ViewModels.Teacher
{
    [QueryProperty(nameof(Student), "Student")]
    public partial class StudentDetailsViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private Student _student;

        [ObservableProperty]
        private ObservableCollection<Attendance> _attendanceRecords;

        [ObservableProperty]
        private ObservableCollection<Behavior> _behaviorRecords;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private int _positivePoints;

        [ObservableProperty]
        private int _negativePoints;

        [ObservableProperty]
        private int _totalPoints;

        [ObservableProperty]
        private int _presentDays;

        [ObservableProperty]
        private int _absentDays;

        [ObservableProperty]
        private int _lateDays;

        public StudentDetailsViewModel()
        {
            _databaseService = new DatabaseService();
            _notificationService = new NotificationService();
            
            AttendanceRecords = new ObservableCollection<Attendance>();
            BehaviorRecords = new ObservableCollection<Behavior>();
        }

        // تعيين الطالب من خلال التنقل
        public Student Student
        {
            get => _student;
            set
            {
                if (SetProperty(ref _student, value))
                {
                    LoadDataAsync().ConfigureAwait(false);
                }
            }
        }

        [RelayCommand]
        private async Task LoadDataAsync()
        {
            if (IsLoading || Student == null)
                return;

            try
            {
                IsLoading = true;
                IsRefreshing = true;

                // تحميل سجلات الحضور
                var attendanceRecords = await _databaseService.GetAttendanceByStudentIdAsync(Student.StudentId);
                AttendanceRecords.Clear();
                foreach (var attendance in attendanceRecords.OrderByDescending(a => a.Date).ThenByDescending(a => a.Timestamp))
                {
                    AttendanceRecords.Add(attendance);
                }

                // حساب إحصائيات الحضور
                PresentDays = AttendanceRecords.Count(a => a.Status == AttendanceStatus.Present && a.Type == AttendanceType.SchoolEntry);
                AbsentDays = AttendanceRecords.Count(a => a.Status == AttendanceStatus.Absent && a.Type == AttendanceType.SchoolEntry);
                LateDays = AttendanceRecords.Count(a => a.Status == AttendanceStatus.Late && a.Type == AttendanceType.SchoolEntry);

                // تحميل سجلات السلوك
                var behaviorRecords = await _databaseService.GetBehaviorByStudentIdAsync(Student.StudentId);
                BehaviorRecords.Clear();
                foreach (var behavior in behaviorRecords.OrderByDescending(b => b.Date).ThenByDescending(b => b.Timestamp))
                {
                    BehaviorRecords.Add(behavior);
                }

                // حساب إحصائيات السلوك
                PositivePoints = BehaviorRecords.Where(b => b.Type == BehaviorType.Positive).Sum(b => b.Points);
                NegativePoints = BehaviorRecords.Where(b => b.Type == BehaviorType.Negative).Sum(b => b.Points);
                TotalPoints = PositivePoints - NegativePoints;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في تحميل بيانات الطالب: {ex.Message}");
                await Shell.Current.DisplayAlert("خطأ", "حدث خطأ أثناء تحميل بيانات الطالب", "موافق");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task EditStudentAsync()
        {
            if (Student == null)
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Student", Student }
            };

            await Shell.Current.GoToAsync("EditStudentPage", navigationParameter);
        }

        [RelayCommand]
        private async Task RecordAttendanceAsync()
        {
            if (Student == null)
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Student", Student }
            };

            await Shell.Current.GoToAsync("RecordAttendancePage", navigationParameter);
        }

        [RelayCommand]
        private async Task RecordBehaviorAsync()
        {
            if (Student == null)
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Student", Student }
            };

            await Shell.Current.GoToAsync("RecordBehaviorPage", navigationParameter);
        }

        [RelayCommand]
        private async Task ContactParentAsync()
        {
            if (Student == null || string.IsNullOrEmpty(Student.ParentPhone))
                return;

            string action = await Shell.Current.DisplayActionSheet(
                "التواصل مع ولي الأمر", 
                "إلغاء", 
                null, 
                "اتصال", 
                "رسالة نصية", 
                "واتساب");

            switch (action)
            {
                case "اتصال":
                    PhoneDialer.Default.Open(Student.ParentPhone);
                    break;
                case "رسالة نصية":
                    await Sms.Default.ComposeAsync(new SmsMessage("", new[] { Student.ParentPhone }));
                    break;
                case "واتساب":
                    var whatsAppService = new WhatsAppService();
                    await whatsAppService.SendMessageAsync(Student.ParentPhone, "");
                    break;
            }
        }

        [RelayCommand]
        private async Task DeleteStudentAsync()
        {
            if (Student == null)
                return;

            bool confirm = await Shell.Current.DisplayAlert(
                "حذف الطالب", 
                $"هل أنت متأكد من حذف الطالب {Student.Name}؟ لا يمكن التراجع عن هذا الإجراء.", 
                "حذف", 
                "إلغاء");

            if (confirm)
            {
                try
                {
                    IsLoading = true;
                    await _databaseService.DeleteStudentAsync(Student);
                    await Shell.Current.DisplayAlert("تم الحذف", "تم حذف الطالب بنجاح", "موافق");
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"خطأ في حذف الطالب: {ex.Message}");
                    await Shell.Current.DisplayAlert("خطأ", "حدث خطأ أثناء حذف الطالب", "موافق");
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }
    }
}
