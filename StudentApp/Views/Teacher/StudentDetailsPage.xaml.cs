namespace StudentApp.Views.Teacher;

public partial class StudentDetailsPage : ContentPage
{
    public StudentDetailsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // تحديث البيانات عند ظهور الصفحة
        if (BindingContext is ViewModels.Teacher.StudentDetailsViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }
}
