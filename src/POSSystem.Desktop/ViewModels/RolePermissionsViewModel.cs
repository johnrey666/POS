using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using POSSystem.Domain.Services;

namespace POSSystem.Desktop.ViewModels;

public class RolePermissionsViewModel : ViewModelBase
{
    private RoleSummary? _selectedRole;
    private string? _statusMessage;
    private string? _errorMessage;
    private bool _isBusy;

    public RolePermissionsViewModel()
    {
        Roles = [];
        Permissions = [];
        SaveCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
        LoadCommand = new RelayCommand(async () => await LoadRolesAsync());

        try
        {
            _ = LoadRolesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Initialization error: {ex.Message}";
        }
    }

    public ObservableCollection<RoleSummary> Roles { get; }
    public ObservableCollection<PermissionRowViewModel> Permissions { get; }

    public RoleSummary? SelectedRole
    {
        get => _selectedRole;
        set
        {
            if (SetProperty(ref _selectedRole, value))
                _ = LoadPermissionsForRoleAsync();
        }
    }

    public string? StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
                CommandManager.InvalidateRequerySuggested();
        }
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand LoadCommand { get; }

    private bool CanSave() => !IsBusy && SelectedRole is not null && Permissions.Count > 0;

    private async Task LoadRolesAsync()
    {
        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var roles = await AppServices.PermissionAdmin.GetRolesAsync();
            Roles.Clear();
            foreach (var role in roles)
                Roles.Add(role);

            SelectedRole = Roles.FirstOrDefault();
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load roles.";
        }
        finally
        {
            IsBusy = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private async Task LoadPermissionsForRoleAsync()
    {
        if (SelectedRole is null)
        {
            Permissions.Clear();
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var items = await AppServices.PermissionAdmin.GetRolePermissionsAsync(SelectedRole.Id);
            Permissions.Clear();
            foreach (var item in items)
                Permissions.Add(new PermissionRowViewModel(item));
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to load permissions.";
        }
        finally
        {
            IsBusy = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private async Task SaveAsync()
    {
        if (SelectedRole is null)
            return;

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var enabledIds = Permissions
                .Where(p => p.IsEnabled)
                .Select(p => p.PermissionId)
                .ToList();

            await AppServices.PermissionAdmin.SaveRolePermissionsAsync(SelectedRole.Id, enabledIds);
            StatusMessage = $"Permissions saved for {SelectedRole.Name}.";
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to save permissions.";
        }
        finally
        {
            IsBusy = false;
            CommandManager.InvalidateRequerySuggested();
        }
    }
}

public class PermissionRowViewModel : ViewModelBase
{
    private bool _isEnabled;

    public PermissionRowViewModel(RolePermissionItem item)
    {
        PermissionId = item.PermissionId;
        Code = item.Code;
        Name = item.Name;
        Category = item.Category;
        _isEnabled = item.IsEnabled;
    }

    public int PermissionId { get; }
    public string Code { get; }
    public string Name { get; }
    public string Category { get; }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }
}
