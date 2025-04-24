using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StudentApp.ViewModels.Teacher
{
    [QueryProperty(nameof(SelectedStudent), "Student")]
    public partial class BehaviorViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;
        private readonly NotificationService _notificationService;

        [ObservableProperty]
        private ObservableCollection<Student> _students;

        [ObservableProperty]
        private ObservableCollection<string> _grades;

        [ObservableProperty]
        private ObservableCollection<Behavior> _recentBehaviors;

        [ObservableProperty]
        private Student _selectedStudent;

        [ObservableProperty]
        private string _selectedGrade;

        [ObservableProperty]
        private BehaviorType _selectedBehaviorType;

        [ObservableProperty]
        private string _behaviorTitle;

        [ObservableProperty]
        private string _behaviorDescription;

        [ObservableProperty]
        private int _points;

        [ObservableProperty]
        private string _location;

        [ObservableProperty]
        private bool _notifyParent;

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

        [ObservableProperty]
        private string _searchText;

        public BehaviorViewModel()
        {
            _databaseService = new DatabaseService();
            _notificationService = new NotificationService();
            
            Students = new ObservableCollection<Student>();
            Grades = new ObservableCollection<string>();
            RecentBehaviors = new ObservableCollection<Behavior>();
            
            SelectedBehaviorType = BehaviorType.Positive;
            Points = 5;
            NotifyParent = true;
            
            LoadDataAsync();
        }

        // تعيين الطالب المحدد من خلال التنقل
        public Student SelectedStudent
        {
            get => _selectedStudent;
            set
            {
                if (SetProperty(ref _selectedStudent, value) && _selectedStudent != null)
                {
                    LoadStudentBehaviorsAsync(_selectedStudent.StudentId);
                }
            }
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

                // تطبيق البحث إذا كان هناك نص بحث
                if (!string.IsNullOrEmpty(SearchText))
                {
                    students = students.Where(s => 
                        s.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || 
                        s.SchoolNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                foreach (var student in students.OrderBy(s => s.Grade).ThenBy(s => s.Name))
                {
                    Students.Add(student);
                }

                // تحميل آخر سجلات السلوك
                var behaviors = await _databaseService.GetBehaviorByDateAsync(DateTime.Today);
                RecentBehaviors.Clear();
                foreach (var behavior in behaviors.OrderByDescending(b => b.Timestamp).Take(5))
                {
                    RecentBehaviors.Add(behavior);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في تحميل البيانات: {ex.Message}");
                ErrorMessage = "حدث خطأ أثناء تحميل البيانات";
                HasError = true;
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task LoadStudentBehaviorsAsync(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
                return;

            try
            {
                IsLoading = true;
                
                // تحميل سجلات سلوك الطالب
                var behaviors = await _databaseService.GetBehaviorByStudentIdAsync(studentId);
                RecentBehaviors.Clear();
                foreach (var behavior in behaviors.OrderByDescending(b => b.Timestamp).Take(5))
                {
                    RecentBehaviors.Add(behavior);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في تحميل سجلات سلوك الطالب: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task SaveBehaviorAsync()
        {
            if (IsSaving || SelectedStudent == null)
                return;

            if (string.IsNullOrWhiteSpace(BehaviorTitle))
            {
                ErrorMessage = "الرجاء إدخال عنوان السلوك";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(BehaviorDescription))
            {
                ErrorMessage = "الرجاء إدخال وصف السلوك";
                HasError = true;
                return;
            }

            if (Points <= 0)
            {
                ErrorMessage = "الرجاء إدخال عدد النقاط (أكبر من صفر)";
                HasError = true;
                return;
            }

            try
            {
                IsSaving = true;
                HasError = false;
                ErrorMessage = string.Empty;

                // إنشاء سجل السلوك
                var behavior = new Behavior
                {
                    BehaviorId = Guid.NewGuid().ToString(),
                    StudentId = SelectedStudent.StudentId,
                    TeacherId = App.AuthService.CurrentUser?.UserId,
                    Date = DateTime.Today,
                    Timestamp = DateTime.Now,
                    Type = SelectedBehaviorType,
                    Title = BehaviorTitle,
                    Description = BehaviorDescription,
                    Points = Points,
                    Location = Location,
                    Notified = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // حفظ سجل السلوك
                await _databaseService.AddBehaviorAsync(behavior);

                // إرسال إشعار لولي الأمر
                if (NotifyParent)
                {
                    bool notified = await _notificationService.SendBehaviorNotificationAsync(
                        behavior,
                        SelectedStudent,
                        App.AuthService.CurrentUser);

                    if (notified)
                    {
                        behavior.Notified = true;
                        behavior.NotificationTime = DateTime.Now;
                        await _databaseService.UpdateBehaviorAsync(behavior);
                    }
                }

                // إعادة تعيين الحقول
                BehaviorTitle = string.Empty;
                BehaviorDescription = string.Empty;
                Points = SelectedBehaviorType == BehaviorType.Positive ? 5 : 2;
                Location = string.Empty;

                // تحديث قائمة السلوكيات
                await LoadStudentBehaviorsAsync(SelectedStudent.StudentId);

                await Shell.Current.DisplayAlert("تم الحفظ", "تم تسجيل السلوك بنجاح", "موافق");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في حفظ سجل السلوك: {ex.Message}");
                ErrorMessage = "حدث خطأ أثناء حفظ سجل السلوك";
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
        private async Task SearchAsync()
        {
            await LoadDataAsync();
        }

        [RelayCommand]
        private async Task ClearSearchAsync()
        {
            SearchText = string.Empty;
            await LoadDataAsync();
        }

        [RelayCommand]
        private void BehaviorTypeChanged()
        {
            // تعديل عدد النقاط الافتراضي حسب نوع السلوك
            Points = SelectedBehaviorType == BehaviorType.Positive ? 5 : 2;
        }

        [RelayCommand]
        private async Task SelectStudentAsync(Student student)
        {
            if (student == null)
                return;

            SelectedStudent = student;
            await LoadStudentBehaviorsAsync(student.StudentId);
        }
    }
}
