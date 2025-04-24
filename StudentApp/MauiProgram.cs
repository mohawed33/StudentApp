using Microsoft.Extensions.Logging;
using StudentApp.Resources.Converters;
using StudentApp.Services;
using StudentApp.ViewModels;
using StudentApp.Views;

namespace StudentApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("Cairo-Regular.ttf", "CairoRegular");
                fonts.AddFont("Cairo-Bold.ttf", "CairoBold");
                fonts.AddFont("Cairo-SemiBold.ttf", "CairoSemiBold");
            });

        // تسجيل الخدمات
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<WhatsAppService>();

        // تسجيل نماذج العرض
        builder.Services.AddTransient<LoginViewModel>();

        // تسجيل الصفحات
        builder.Services.AddTransient<LoginPage>();

        // إضافة المحولات العامة
        builder.Services.AddSingleton<EnumToBoolConverter>();
        builder.Services.AddSingleton<InverseBoolConverter>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
