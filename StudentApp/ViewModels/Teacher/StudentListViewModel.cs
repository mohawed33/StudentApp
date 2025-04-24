using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StudentApp.ViewModels.Teacher
{
    public partial class StudentListViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Student> _students;

        [ObservableProperty]
        private ObservableCollection<string> _grades;

        [ObservableProperty]
        private string _selectedGrade;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isRefreshing;

        [ObservableProperty]
        private bool _isFiltering;

        [ObservableProperty]
        private int _totalStudents;

        public StudentListViewModel()
        {
            _databaseService = new DatabaseService();
            Students = new ObservableCollection<Student>();
            Grades = new ObservableCollection<string>();
            
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

                TotalStudents = Students.Count;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في تحميل الطلاب: {ex.Message}");
                await Shell.Current.DisplayAlert("خطأ", "حدث خطأ أثناء تحميل قائمة الطلاب", "موافق");
            }
            finally
            {
                IsLoading = false;
                IsRefreshing = false;
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
        private async Task NavigateToStudentDetailsAsync(Student student)
        {
            if (student == null)
                return;

            var navigationParameter = new Dictionary<string, object>
            {
                { "Student", student }
            };

            await Shell.Current.GoToAsync($"StudentDetailsPage", navigationParameter);
        }

        [RelayCommand]
        private async Task NavigateToAddStudentAsync()
        {
            await Shell.Current.GoToAsync("AddStudentPage");
        }
    }
}
