using System.Windows;
using System.Windows.Controls;
using POSSystem.Domain.Security;
using POSSystem.Desktop.ViewModels;

namespace POSSystem.Desktop.ViewModels;

public class ShellViewModel : ViewModelBase
{
    private object? _currentView;
    private string _currentPage = "dashboard";

    public ShellViewModel()
    {
        var user = AppServices.Auth.CurrentUser
            ?? throw new InvalidOperationException("No authenticated user.");

        WelcomeName = user.FullName;
        RoleName = user.RoleName;

        CanViewDashboard = user.HasPermission(PermissionCodes.DashboardView);
        CanAccessPos = user.HasPermission(PermissionCodes.PosAccess);
        CanViewReports = user.HasPermission(PermissionCodes.ReportsView);
        CanManagePermissions = user.HasPermission(PermissionCodes.PermissionsManage);

        NavigateDashboardCommand = new RelayCommand(() => Navigate("dashboard", new DashboardViewModel()));
        NavigatePosCommand = new RelayCommand(() => Navigate("pos", new PosViewModel()));
        NavigateProductsCommand = new RelayCommand(() => Navigate("products", new ProductsViewModel()));
        NavigatePermissionsCommand = new RelayCommand(() => Navigate("permissions", new RolePermissionsViewModel()));
        LogoutCommand = new RelayCommand(Logout);

        if (CanAccessPos)
            Navigate("pos", new PosViewModel());
        else if (CanViewDashboard)
            Navigate("dashboard", new DashboardViewModel());
        else if (CanManagePermissions)
            Navigate("permissions", new RolePermissionsViewModel());
        else
            CurrentView = new DashboardViewModel();
    }

    public string WelcomeName { get; }
    public string RoleName { get; }

    public bool CanViewDashboard { get; }
    public bool CanAccessPos { get; }
    public bool CanViewReports { get; }
    public bool CanManagePermissions { get; }

    public string CurrentPage
    {
        get => _currentPage;
        private set => SetProperty(ref _currentPage, value);
    }

    public object? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public RelayCommand NavigateDashboardCommand { get; }
    public RelayCommand NavigatePosCommand { get; }
    public RelayCommand NavigateProductsCommand { get; }
    public RelayCommand NavigatePermissionsCommand { get; }
    public RelayCommand LogoutCommand { get; }

    private void Navigate(string page, object view)
    {
        CurrentPage = page;
        CurrentView = view;
    }

    private void Logout()
    {
        AppServices.Auth.Logout();
        var login = new Views.LoginWindow();
        login.Show();

        foreach (Window window in Application.Current.Windows)
        {
            if (window is Views.MainWindow)
            {
                window.Close();
                break;
            }
        }
    }
}
