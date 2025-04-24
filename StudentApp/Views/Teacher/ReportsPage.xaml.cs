namespace StudentApp.Views.Teacher;

public partial class ReportsPage : ContentPage
{
    public ReportsPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // تحديث البيانات عند ظهور الصفحة
        if (BindingContext is ViewModels.Teacher.ReportsViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }

    private void ReportTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (BindingContext is ViewModels.Teacher.ReportsViewModel viewModel)
        {
            viewModel.ReportTypeChangedCommand.Execute(null);
        }
    }
}
