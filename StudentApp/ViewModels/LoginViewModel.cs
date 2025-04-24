using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentApp.Models;
using System.Diagnostics;

namespace StudentApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private UserRole _selectedRole = UserRole.Teacher;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private string _errorMessage;

        [ObservableProperty]
        private bool _hasError;

        // أمر تسجيل الدخول
        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "الرجاء إدخال اسم المستخدم";
                HasError = true;
                return;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "الرجاء إدخال كلمة المرور";
                HasError = true;
                return;
            }

            try
            {
                IsLoading = true;
                HasError = false;
                ErrorMessage = string.Empty;

                bool success = await App.AuthService.LoginAsync(Username, Password, SelectedRole);

                if (success)
                {
                    // التنقل إلى الصفحة المناسبة حسب نوع المستخدم
                    if (App.AuthService.IsTeacher)
                    {
                        // التنقل إلى صفحة المعلم
                        await Shell.Current.GoToAsync("//TeacherTabs/TeacherHomePage");
                        ((AppShell)Shell.Current).SwitchToTeacherView();
                    }
                    else if (App.AuthService.IsParent)
                    {
                        // التنقل إلى صفحة ولي الأمر
                        await Shell.Current.GoToAsync("//ParentTabs/ParentHomePage");
                        ((AppShell)Shell.Current).SwitchToParentView();
                    }
                }
                else
                {
                    ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"خطأ في تسجيل الدخول: {ex.Message}");
                ErrorMessage = "حدث خطأ أثناء تسجيل الدخول";
                HasError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        // أمر تبديل نوع المستخدم
        [RelayCommand]
        private void ToggleRole()
        {
            SelectedRole = SelectedRole == UserRole.Teacher ? UserRole.Parent : UserRole.Teacher;
        }
    }
}
