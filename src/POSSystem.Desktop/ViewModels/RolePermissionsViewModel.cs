using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using POSSystem.Domain.Models;

namespace POSSystem.Desktop.ViewModels;

public class RolePermissionsViewModel : ViewModelBase
{
    private RoleSummary? _selectedRole;
    private string? _statusMessage;
    private string? _errorMessage;
    private bool _isBusy;
    private int _currentPage = 1;
    private int _totalPages;
    private int _totalItems;
    private readonly List<PermissionRowViewModel> _allItems = [];

    public RolePermissionsViewModel()
    {
        Roles = [];
        Permissions = [];
        SaveCommand = new RelayCommand(async () => await SaveAsync(), CanSave);
        LoadCommand = new RelayCommand(async () => await LoadRolesAsync());
        PreviousPageCommand = new RelayCommand(() => ChangePage(-1), () => CurrentPage > 1);
        NextPageCommand = new RelayCommand(() => ChangePage(1), () => CurrentPage < TotalPages);

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

    public int CurrentPage
    {
        get => _currentPage;
        set => SetProperty(ref _currentPage, value);
    }

    public int TotalPages
    {
        get => _totalPages;
        set => SetProperty(ref _totalPages, value);
    }

    public int TotalItems
    {
        get => _totalItems;
        set => SetProperty(ref _totalItems, value);
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
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }

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
            _allItems.Clear();
            TotalItems = 0;
            CurrentPage = 1;
            TotalPages = 0;
            Permissions.Clear();
            return;
        }

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var items = await AppServices.PermissionAdmin.GetRolePermissionsAsync(SelectedRole.Id);
            _allItems.Clear();
            foreach (var item in items)
                _allItems.Add(new PermissionRowViewModel(item));

            TotalItems = _allItems.Count;
            CurrentPage = 1;
            ApplyPaging();
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

    private void ApplyPaging()
    {
        const int pageSize = 15;
        TotalPages = (int)Math.Ceiling((double)TotalItems / pageSize);
        if (CurrentPage > TotalPages) CurrentPage = TotalPages;
        if (CurrentPage < 1) CurrentPage = 1;

        var pageItems = _allItems
            .Skip((CurrentPage - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        Permissions.Clear();
        foreach (var item in pageItems)
            Permissions.Add(item);

        CommandManager.InvalidateRequerySuggested();
    }

    private void ChangePage(int delta)
    {
        CurrentPage += delta;
        ApplyPaging();
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
