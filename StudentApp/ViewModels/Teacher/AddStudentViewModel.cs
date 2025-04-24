using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StudentApp.ViewModels.Teacher
{
    [QueryProperty(nameof(Student), "Student")]
    public partial class AddStudentViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private Student _student;

        [ObservableProperty]
        private ObservableCollection<string> _grades;

        [ObservableProperty]
        private ObservableCollection<string> _sections;

        [ObservableProperty]
        private string _selectedGrade;

        [ObservableProperty]
        private string _selectedSection;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isEditMode;

        [ObservableProperty]
        private string _pageTitle;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _hasError;

        public AddStudentViewModel()
        {
            _databaseService = new DatabaseService();
            
            // تهيئة القوائم
            Grades = new ObservableCollection<string>
            {
                "الصف الأول", "الصف الثاني", "الصف الثالث", "الصف الرابع", "الصف الخامس", "الصف السادس"
            };
            
            Sections = new ObservableCollection<string>
            {
                "أ", "ب", "ج", "د"
            };
            
            // إنشاء طالب جديد
            Student = new Student
            {
                StudentId = Guid.NewGuid().ToString(),
                DateOfBirth = DateTime.Now.AddYears(-7),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            
            IsEditMode = false;
            PageTitle = "إضافة طالب جديد";
        }

        // تعيين الطالب من خلال التنقل (للتعديل)
        public Student Student
        {
            get => _student;
            set
            {
                if (SetProperty(ref _student, value))
                {
                    if (_student != null && !string.IsNullOrEmpty(_student.StudentId))
                    {
                        IsEditMode = true;
                        PageTitle = "تعديل بيانات الطالب";
                        SelectedGrade = _student.Grade;
                        SelectedSection = _student.Section;
                    }
                }
            }
        }

        [RelayCommand]
        private async Task SaveStudentAsync()
        {
            if (string.IsNullOrWhiteSpace(Student.Name))
            {
                ErrorMessage = "الرجاء إدخال اسم الطالب";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Student.SchoolNumber))
            {
                ErrorMessage = "الرجاء إدخال الرقم المدرسي";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedGrade))
            {
                ErrorMessage = "الرجاء اختيار الصف";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedSection))
            {
                ErrorMessage = "الرجاء اختيار الشعبة";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Student.ParentName))
            {
                ErrorMessage = "الرجاء إدخال اسم ولي الأمر";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Student.ParentPhone))
            {
                ErrorMessage = "الرجاء إدخال رقم هاتف ولي الأمر";
                HasError = true;
                return;
            }

            try
            {
                IsLoading = true;
                HasError = false;
                ErrorMessage = string.Empty;

                // تعيين الصف والشعبة
                Student.Grade = SelectedGrade;
                Student.Section = SelectedSection;
                Student.UpdatedAt = DateTime.Now;

                if (IsEditMode)
                {
                    // تحديث بيانات الطالب
                    await _databaseService.UpdateStudentAsync(Student);
                    await Shell.Current.DisplayAlert("تم التحديث", "تم تحديث بيانات الطالب بنجاح", "موافق");
                }
                else
                {
                    // إضافة طالب جديد
                    await _databaseService.AddStudentAsync(Student);
                    await Shell.Current.DisplayAlert("تمت الإضافة", "تم إضافة الطالب بنجاح", "موافق");
                }

                // العودة للصفحة السابقة
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في حفظ بيانات الطالب: {ex.Message}");
                ErrorMessage = "حدث خطأ أثناء حفظ بيانات الطالب";
                HasError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task PickProfileImageAsync()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "اختر صورة الطالب"
                });

                if (result != null)
                {
                    // في الإصدار النهائي، سيتم رفع الصورة إلى Firebase Storage
                    // وتخزين رابط الصورة في Student.ProfileImageUrl
                    
                    // للاختبار، نستخدم مسار الملف المحلي
                    Student.ProfileImageUrl = result.FullPath;
                    OnPropertyChanged(nameof(Student));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في اختيار الصورة: {ex.Message}");
                await Shell.Current.DisplayAlert("خطأ", "حدث خطأ أثناء اختيار الصورة", "موافق");
            }
        }
    }
}
