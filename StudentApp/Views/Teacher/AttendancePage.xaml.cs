namespace StudentApp.Views.Teacher;

public partial class AttendancePage : ContentPage
{
    public AttendancePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // تحديث البيانات عند ظهور الصفحة
        if (BindingContext is ViewModels.Teacher.AttendanceViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }

    private void DatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (BindingContext is ViewModels.Teacher.AttendanceViewModel viewModel)
        {
            viewModel.DateChangedCommand.Execute(null);
        }
    }

    private void AttendanceTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (BindingContext is ViewModels.Teacher.AttendanceViewModel viewModel)
        {
            viewModel.AttendanceTypeChangedCommand.Execute(null);
        }
    }
}
