namespace StudentApp.Views.Teacher;

public partial class TeacherHomePage : ContentPage
{
    public TeacherHomePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // تحديث البيانات عند ظهور الصفحة
        if (BindingContext is ViewModels.Teacher.TeacherHomeViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }
}
