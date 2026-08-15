using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using POSSystem.Domain.Models;

namespace POSSystem.Desktop.ViewModels;

public class BranchProductListingViewModel : ViewModelBase
{
    private string _statusMessage = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private BranchSummary? _selectedBranch;
    private int _currentPage = 1;
    private int _totalPages;
    private int _totalItems;
    private readonly List<BranchProductListingItem> _allItems = [];

    public BranchProductListingViewModel()
    {
        Branches = [];
        BranchProductListings = [];

        LoadCommand = new RelayCommand(async () => await LoadAsync());
        AddProductCommand = new RelayCommand(OpenAddProduct);
        SavePriceCommand = new RelayCommand(async () => await SavePricesAsync());
        PreviousPageCommand = new RelayCommand(() => ChangePage(-1), () => CurrentPage > 1);
        NextPageCommand = new RelayCommand(() => ChangePage(1), () => CurrentPage < TotalPages);

        try
        {
            _ = LoadAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Initialization error: {ex.Message}";
        }
    }

    public ObservableCollection<BranchSummary> Branches { get; }
    public ObservableCollection<BranchProductListingItem> BranchProductListings { get; }

    public BranchSummary? SelectedBranch
    {
        get => _selectedBranch;
        set
        {
            if (SetProperty(ref _selectedBranch, value))
                _ = LoadBranchListingsAsync();
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

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public string ErrorMessage
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

    public ICommand LoadCommand { get; }
    public ICommand AddProductCommand { get; }
    public ICommand SavePriceCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }

    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = "Loading branches...";

        try
        {
            var branches = await AppServices.Branches.GetBranchesAsync();
            Branches.Clear();
            foreach (var branch in branches)
                Branches.Add(branch);

            StatusMessage = "Select a branch to view products.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading branches: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadBranchListingsAsync()
    {
        if (SelectedBranch == null)
        {
            _allItems.Clear();
            TotalItems = 0;
            CurrentPage = 1;
            TotalPages = 0;
            BranchProductListings.Clear();
            return;
        }

        IsBusy = true;
        StatusMessage = $"Loading products for {SelectedBranch.Name}...";

        try
        {
            var items = await AppServices.ProductManagement.GetBranchProductListingsAsync(SelectedBranch.Id);
            _allItems.Clear();
            _allItems.AddRange(items);
            TotalItems = _allItems.Count;
            CurrentPage = 1;
            ApplyPaging();
            StatusMessage = $"Loaded {TotalItems} products for {SelectedBranch.Name}.";
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading branch products: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
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

        BranchProductListings.Clear();
        foreach (var item in pageItems)
            BranchProductListings.Add(item);

        CommandManager.InvalidateRequerySuggested();
    }

    private void ChangePage(int delta)
    {
        CurrentPage += delta;
        ApplyPaging();
    }

    private async void OpenAddProduct()
    {
        if (SelectedBranch == null)
        {
            ErrorMessage = "Please select a branch first.";
            return;
        }

        var dialog = new Views.BranchProductDialog(Views.BranchProductDialog.DialogMode.Add);
        var activeWindow = Application.Current.Windows
            .OfType<Window>()
            .FirstOrDefault(w => w.IsActive);

        if (activeWindow != null && !ReferenceEquals(activeWindow, dialog))
            dialog.Owner = activeWindow;
        else if (Application.Current.MainWindow != null && !ReferenceEquals(Application.Current.MainWindow, dialog))
            dialog.Owner = Application.Current.MainWindow;

        try
        {
            var products = await AppServices.ProductManagement.GetProductsAsync();
            foreach (var p in products)
                dialog.Products.Add(new ProductSummary { Id = p.Id, Name = p.Name, Barcode = p.Barcode, SellingPrice = p.SellingPrice, CategoryId = p.CategoryId });
        }
        catch { }

        if (dialog.ShowDialog() == true && dialog.IsSaved)
        {
            try
            {
                await AppServices.ProductManagement.AddBranchProductAsync(dialog.ProductId, SelectedBranch.Id, dialog.BranchPrice);
                StatusMessage = "Product added to branch successfully.";
                _ = LoadBranchListingsAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error adding product: {ex.Message}";
            }
        }
    }

    private async Task SavePricesAsync()
    {
        if (SelectedBranch == null)
        {
            ErrorMessage = "Please select a branch first.";
            return;
        }

        IsBusy = true;
        StatusMessage = "Saving prices...";
        ErrorMessage = string.Empty;

        try
        {
            foreach (var item in BranchProductListings.Where(i => i.IsActive))
            {
                await AppServices.ProductManagement.UpdateBranchProductPriceAsync(
                    item.ProductId, item.BranchId, item.BranchPrice);
            }

            StatusMessage = "Prices saved successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving prices: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
