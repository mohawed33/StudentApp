namespace StudentApp.Views.Parent;

public partial class ParentHomePage : ContentPage
{
    public ParentHomePage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        // تحديث البيانات عند ظهور الصفحة
        if (BindingContext is ViewModels.Parent.ParentHomeViewModel viewModel)
        {
            viewModel.LoadDataCommand.Execute(null);
        }
    }
}
