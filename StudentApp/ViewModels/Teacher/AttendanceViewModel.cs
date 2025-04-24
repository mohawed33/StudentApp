using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StudentApp.ViewModels.Teacher
{
    public partial class AttendanceViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<Student> _students;

        [ObservableProperty]
        private ObservableCollection<string> _grades;

        [ObservableProperty]
        private ObservableCollection<AttendanceRecord> _attendanceRecords;

        [ObservableProperty]
        private string _selectedGrade;

        [ObservableProperty]
        private DateTime _selectedDate;

        [ObservableProperty]
        private AttendanceType _selectedAttendanceType;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isSaving;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _hasError;

        public AttendanceViewModel()
        {
            _databaseService = new DatabaseService();
            _notificationService = new NotificationService();
            
            Students = new ObservableCollection<Student>();
            Grades = new ObservableCollection<string>();
            AttendanceRecords = new ObservableCollection<AttendanceRecord>();
            
            SelectedDate = DateTime.Today;
            SelectedAttendanceType = AttendanceType.SchoolEntry;
            
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
                HasError = false;
                ErrorMessage = string.Empty;

                // تحميل الطلاب
                var students = await _databaseService.GetStudentsAsync();
                Students.Clear();
                
                // استخراج الصفوف الفريدة
                var uniqueGrades = students.Select(s => s.Grade).Distinct().OrderBy(g => g).ToList();
                Grades.Clear();
                Grades.Add("جميع الصفوف");
                foreach (var grade in uniqueGrades)
                {
                    Grades.Add(grade);
                }

                // تطبيق التصفية إذا كان هناك صف محدد
                if (!string.IsNullOrEmpty(SelectedGrade) && SelectedGrade != "جميع الصفوف")
                {
                    students = students.Where(s => s.Grade == SelectedGrade).ToList();
                }

                foreach (var student in students.OrderBy(s => s.Grade).ThenBy(s => s.Name))
                {
                    Students.Add(student);
                }

                // تحميل سجلات الحضور الحالية لليوم والنوع المحدد
                var existingAttendance = await _databaseService.GetAttendanceByDateAsync(SelectedDate);
                existingAttendance = existingAttendance.Where(a => a.Type == SelectedAttendanceType).ToList();

                // إنشاء سجلات الحضور
                AttendanceRecords.Clear();
                foreach (var student in Students)
                {
                    var existingRecord = existingAttendance.FirstOrDefault(a => a.StudentId == student.StudentId);
                    
                    var record = new AttendanceRecord
                    {
                        Student = student,
                        Attendance = existingRecord ?? new Attendance
                        {
                            StudentId = student.StudentId,
                            TeacherId = App.AuthService.CurrentUser?.UserId,
                            Date = SelectedDate,
                            Timestamp = DateTime.Now,
                            Type = SelectedAttendanceType,
                            Status = AttendanceStatus.Present,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        IsExisting = existingRecord != null
                    };
                    
                    AttendanceRecords.Add(record);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في تحميل بيانات الحضور: {ex.Message}");
                ErrorMessage = "حدث خطأ أثناء تحميل بيانات الحضور";
                HasError = true;
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task SaveAttendanceAsync()
        {
            if (IsSaving)
                return;

            try
            {
                IsSaving = true;
                HasError = false;
                ErrorMessage = string.Empty;

                int savedCount = 0;
                int notifiedCount = 0;

                foreach (var record in AttendanceRecords)
                {
                    // تحديث وقت التسجيل
                    record.Attendance.Timestamp = DateTime.Now;
                    record.Attendance.UpdatedAt = DateTime.Now;

                    if (record.IsExisting)
                    {
                        // تحديث سجل موجود
                        await _databaseService.UpdateAttendanceAsync(record.Attendance);
                    }
                    else
                    {
                        // إضافة سجل جديد
                        await _databaseService.AddAttendanceAsync(record.Attendance);
                        record.IsExisting = true;
                    }

                    savedCount++;

                    // إرسال إشعار لولي الأمر إذا كان الطالب غائب أو متأخر
                    if (record.Attendance.Status == AttendanceStatus.Absent || 
                        record.Attendance.Status == AttendanceStatus.Late)
                    {
                        bool notified = await _notificationService.SendAttendanceNotificationAsync(
                            record.Attendance,
                            record.Student,
                            App.AuthService.CurrentUser);

                        if (notified)
                        {
                            notifiedCount++;
                        }
                    }
                }

                await Shell.Current.DisplayAlert(
                    "تم الحفظ", 
                    $"تم حفظ {savedCount} سجل حضور بنجاح\nتم إرسال {notifiedCount} إشعار لأولياء الأمور", 
                    "موافق");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في حفظ بيانات الحضور: {ex.Message}");
                ErrorMessage = "حدث خطأ أثناء حفظ بيانات الحضور";
                HasError = true;
            }
            finally
            {
                IsSaving = false;
            }
        }

        [RelayCommand]
        private async Task FilterByGradeAsync(string grade)
        {
            SelectedGrade = grade;
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task DateChangedAsync()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task AttendanceTypeChangedAsync()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        private void SetAllStatus(AttendanceStatus status)
        {
            foreach (var record in AttendanceRecords)
            {
                record.Attendance.Status = status;
            }
        }
    }

    // فئة مساعدة لعرض سجلات الحضور
    public class AttendanceRecord : ObservableObject
    {
        [ObservableProperty]
        private Student _student;

        [ObservableProperty]
        private Attendance _attendance;

        [ObservableProperty]
        private bool _isExisting;
    }
}
