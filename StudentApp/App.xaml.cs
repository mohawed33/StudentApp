using StudentApp.Services;
using StudentApp.Views;

namespace StudentApp;

public partial class App : Application
{
    public static AuthService AuthService { get; private set; }
    public static DatabaseService DatabaseService { get; private set; }
    public static NotificationService NotificationService { get; private set; }

    public App()
    {
        InitializeComponent();

        // تهيئة الخدمات
        AuthService = new AuthService();
        DatabaseService = new DatabaseService();
        NotificationService = new NotificationService();

        // تعيين الصفحة الرئيسية
        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        // التحقق من حالة تسجيل الدخول عند بدء التطبيق
        Task.Run(async () =>
        {
            bool isLoggedIn = await AuthService.IsLoggedInAsync();
            if (!isLoggedIn)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Shell.Current.GoToAsync("//LoginPage");
                });
            }
        });
    }

    protected override void OnSleep()
    {
        // حفظ الحالة عند دخول التطبيق في وضع السكون
    }

    protected override void OnResume()
    {
        // استعادة الحالة عند استئناف التطبيق
    }
}
