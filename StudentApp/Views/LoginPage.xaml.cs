namespace StudentApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    // منع العودة للخلف من شاشة تسجيل الدخول
    protected override bool OnBackButtonPressed()
    {
        return true;
    }
}
