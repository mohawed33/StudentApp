using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentApp.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace StudentApp.ViewModels.Teacher
{
    public partial class ReportsViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private ObservableCollection<Student> _students;

        [ObservableProperty]
        private ObservableCollection<string> _grades;

        [ObservableProperty]
        private ObservableCollection<ReportType> _reportTypes;

        [ObservableProperty]
        private Student _selectedStudent;

        [ObservableProperty]
        private string _selectedGrade;

        [ObservableProperty]
        private ReportType _selectedReportType;

        [ObservableProperty]
        private DateTime _startDate;

        [ObservableProperty]
        private DateTime _endDate;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isGenerating;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private string _searchText;

        [ObservableProperty]
        private bool _includeAttendance;

        [ObservableProperty]
        private bool _includeBehavior;

        [ObservableProperty]
        private bool _includeNotes;

        public ReportsViewModel()
        {
            _databaseService = new DatabaseService();
            
            Students = new ObservableCollection<Student>();
            Grades = new ObservableCollection<string>();
            
            ReportTypes = new ObservableCollection<ReportType>
            {
                ReportType.StudentReport,
                ReportType.ClassReport,
                ReportType.AttendanceReport,
                ReportType.BehaviorReport,
                ReportType.SchoolReport
            };
            
            SelectedReportType = ReportType.StudentReport;
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = DateTime.Now;
            
            IncludeAttendance = true;
            IncludeBehavior = true;
            IncludeNotes = true;
            
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
            }
        }

        [RelayCommand]
        private async Task GenerateReportAsync()
        {
            if (IsGenerating)
                return;

            if (SelectedReportType == ReportType.StudentReport && SelectedStudent == null)
            {
                ErrorMessage = "الرجاء اختيار طالب";
                HasError = true;
                return;
            }

            if (SelectedReportType == ReportType.ClassReport && string.IsNullOrEmpty(SelectedGrade))
            {
                ErrorMessage = "الرجاء اختيار صف";
                HasError = true;
                return;
            }

            if (StartDate > EndDate)
            {
                ErrorMessage = "تاريخ البداية يجب أن يكون قبل تاريخ النهاية";
                HasError = true;
                return;
            }

            if (!IncludeAttendance && !IncludeBehavior && !IncludeNotes)
            {
                ErrorMessage = "الرجاء اختيار محتوى التقرير";
                HasError = true;
                return;
            }

            try
            {
                IsGenerating = true;
                HasError = false;
                ErrorMessage = string.Empty;

                // في الإصدار النهائي، سيتم إنشاء التقرير بصيغة PDF
                // وحفظه في مجلد التنزيلات أو مشاركته

                await Task.Delay(2000); // محاكاة إنشاء التقرير

                string reportName = GetReportName();
                await Shell.Current.DisplayAlert("تم إنشاء التقرير", $"تم إنشاء التقرير {reportName} بنجاح", "موافق");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في إنشاء التقرير: {ex.Message}");
                ErrorMessage = "حدث خطأ أثناء إنشاء التقرير";
                HasError = true;
            }
            finally
            {
                IsGenerating = false;
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
        private void ReportTypeChanged()
        {
            // تحديث واجهة المستخدم حسب نوع التقرير
            OnPropertyChanged(nameof(IsStudentReportSelected));
            OnPropertyChanged(nameof(IsClassReportSelected));
            OnPropertyChanged(nameof(IsAttendanceReportSelected));
            OnPropertyChanged(nameof(IsBehaviorReportSelected));
            OnPropertyChanged(nameof(IsSchoolReportSelected));
        }

        [RelayCommand]
        private async Task SelectStudentAsync(Student student)
        {
            if (student == null)
                return;

            SelectedStudent = student;
        }

        // خصائص مساعدة لعرض/إخفاء عناصر واجهة المستخدم
        public bool IsStudentReportSelected => SelectedReportType == ReportType.StudentReport;
        public bool IsClassReportSelected => SelectedReportType == ReportType.ClassReport;
        public bool IsAttendanceReportSelected => SelectedReportType == ReportType.AttendanceReport;
        public bool IsBehaviorReportSelected => SelectedReportType == ReportType.BehaviorReport;
        public bool IsSchoolReportSelected => SelectedReportType == ReportType.SchoolReport;

        // الحصول على اسم التقرير
        private string GetReportName()
        {
            string reportName = string.Empty;
            
            switch (SelectedReportType)
            {
                case ReportType.StudentReport:
                    reportName = $"تقرير الطالب {SelectedStudent?.Name}";
                    break;
                case ReportType.ClassReport:
                    reportName = $"تقرير الصف {SelectedGrade}";
                    break;
                case ReportType.AttendanceReport:
                    reportName = "تقرير الحضور";
                    break;
                case ReportType.BehaviorReport:
                    reportName = "تقرير السلوك";
                    break;
                case ReportType.SchoolReport:
                    reportName = "تقرير المدرسة";
                    break;
            }
            
            reportName += $" ({StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy})";
            
            return reportName;
        }
    }

    // أنواع التقارير
    public enum ReportType
    {
        StudentReport,   // تقرير طالب
        ClassReport,     // تقرير صف
        AttendanceReport, // تقرير الحضور
        BehaviorReport,  // تقرير السلوك
        SchoolReport     // تقرير المدرسة
    }
}
