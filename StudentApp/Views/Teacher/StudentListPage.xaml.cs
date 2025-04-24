namespace StudentApp.Views.Teacher;

public partial class StudentListPage : ContentPage
{
    public StudentListPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // تحديث البيانات عند ظهور الصفحة
        if (BindingContext is ViewModels.Teacher.StudentListViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }
}
