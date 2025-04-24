namespace StudentApp.Views.Teacher;

public partial class BehaviorPage : ContentPage
{
    public BehaviorPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // تحديث البيانات عند ظهور الصفحة
        if (BindingContext is ViewModels.Teacher.BehaviorViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }

    private void BehaviorTypePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (BindingContext is ViewModels.Teacher.BehaviorViewModel viewModel)
        {
            viewModel.BehaviorTypeChangedCommand.Execute(null);
        }
    }
}
