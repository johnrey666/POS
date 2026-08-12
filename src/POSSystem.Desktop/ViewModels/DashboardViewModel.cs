using System.Reflection;

namespace POSSystem.Desktop.ViewModels;

public class DashboardViewModel : ViewModelBase
{
    public string Title => "Dashboard";

    public string Message =>
        $"Signed in as {AppServices.Auth.CurrentUser?.FullName} ({AppServices.Auth.CurrentUser?.RoleName}). " +
        "Phase 2 complete — login, roles, and permissions are active.";

    public string DatabaseStatus => App.DatabaseStatus;

    public string VersionDisplay =>
        $"v{Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0"}";
}
