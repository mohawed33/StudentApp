using StudentApp.Views;
using StudentApp.Views.Teacher;
using StudentApp.Views.Parent;

namespace StudentApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // تسجيل المسارات للتنقل
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        
        // مسارات المعلم
        Routing.RegisterRoute(nameof(TeacherHomePage), typeof(TeacherHomePage));
        Routing.RegisterRoute(nameof(StudentListPage), typeof(StudentListPage));
        Routing.RegisterRoute(nameof(StudentDetailsPage), typeof(StudentDetailsPage));
        Routing.RegisterRoute(nameof(AddStudentPage), typeof(AddStudentPage));
        Routing.RegisterRoute(nameof(AttendancePage), typeof(AttendancePage));
        Routing.RegisterRoute(nameof(BehaviorPage), typeof(BehaviorPage));
        Routing.RegisterRoute(nameof(ReportsPage), typeof(ReportsPage));
        
        // مسارات ولي الأمر
        Routing.RegisterRoute(nameof(ParentHomePage), typeof(ParentHomePage));
        Routing.RegisterRoute(nameof(ChildrenPage), typeof(ChildrenPage));
        Routing.RegisterRoute(nameof(AttendanceHistoryPage), typeof(AttendanceHistoryPage));
        Routing.RegisterRoute(nameof(BehaviorHistoryPage), typeof(BehaviorHistoryPage));
        Routing.RegisterRoute(nameof(ParentReportsPage), typeof(ParentReportsPage));
    }

    // تبديل عرض القوائم حسب نوع المستخدم
    public void SwitchToTeacherView()
    {
        TeacherTabBar.IsVisible = true;
        ParentTabBar.IsVisible = false;
        Current.GoToAsync("//TeacherTabs/TeacherHomePage");
    }

    public void SwitchToParentView()
    {
        TeacherTabBar.IsVisible = false;
        ParentTabBar.IsVisible = true;
        Current.GoToAsync("//ParentTabs/ParentHomePage");
    }
}
