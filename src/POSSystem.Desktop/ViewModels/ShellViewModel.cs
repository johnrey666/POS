using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using POSSystem.Domain.Security;
using POSSystem.Desktop.ViewModels;

namespace POSSystem.Desktop.ViewModels;

public class ShellViewModel : ViewModelBase
{
    private object? _currentView;
    private string _currentPage = "dashboard";
    private bool _isDarkMode;

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
        ToggleDarkModeCommand = new RelayCommand(ToggleDarkMode);

        // Load saved theme preference (optional – default to Light)
        IsDarkMode = false; // or read from settings

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

    public bool IsDarkMode
    {
        get => _isDarkMode;
        set
        {
            if (SetProperty(ref _isDarkMode, value))
                ApplyTheme(value);
        }
    }

    public RelayCommand NavigateDashboardCommand { get; }
    public RelayCommand NavigatePosCommand { get; }
    public RelayCommand NavigateProductsCommand { get; }
    public RelayCommand NavigatePermissionsCommand { get; }
    public RelayCommand LogoutCommand { get; }
    public RelayCommand ToggleDarkModeCommand { get; }

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

    private void ToggleDarkMode()
    {
        IsDarkMode = !IsDarkMode;
    }

    // THE CRITICAL FIX IS HERE
    private void ApplyTheme(bool dark)
    {
        var mergedDictionaries = Application.Current.Resources.MergedDictionaries;

        // 1. Find and remove the currently loaded color dictionary
        var currentColorDict = mergedDictionaries.FirstOrDefault(d => 
            d.Source.ToString().Contains("Colors.xaml") || 
            d.Source.ToString().Contains("Dark.xaml"));

        if (currentColorDict != null)
            mergedDictionaries.Remove(currentColorDict);

        // 2. Find and remove the currently loaded styles dictionary
        var currentStyleDict = mergedDictionaries.FirstOrDefault(d => 
            d.Source.ToString().Contains("Styles.xaml"));

        if (currentStyleDict != null)
            mergedDictionaries.Remove(currentStyleDict);

        // 3. Load the correct color dictionary (Colors.xaml for Light, Dark.xaml for Dark)
        string colorPath = dark 
            ? "/POSSystem.Desktop;component/Themes/Dark.xaml" 
            : "/POSSystem.Desktop;component/Themes/Colors.xaml";

        mergedDictionaries.Add(new ResourceDictionary 
        { 
            Source = new Uri(colorPath, UriKind.Relative) 
        });

        // 4. ALWAYS reload the Styles.xaml dictionary, or all your button/card styling will vanish!
        mergedDictionaries.Add(new ResourceDictionary 
        { 
            Source = new Uri("/POSSystem.Desktop;component/Themes/Styles.xaml", UriKind.Relative) 
        });
    }
}