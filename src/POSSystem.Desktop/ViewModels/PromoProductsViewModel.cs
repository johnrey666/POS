using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using POSSystem.Domain.Models;

namespace POSSystem.Desktop.ViewModels;

public class PromoProductsViewModel : ViewModelBase
{
    private string _statusMessage = string.Empty;
    private bool _isBusy;
    private PromoProductItem? _selectedPromoProduct;
    private DateTime _promoStartDate = DateTime.Today;
    private DateTime _promoEndDate = DateTime.Today.AddMonths(1);
    private int _currentPage = 1;
    private int _totalPages;
    private int _totalItems;
    private readonly List<PromoProductItem> _allItems = [];

    public PromoProductsViewModel()
    {
        PromoProducts = [];

        LoadCommand = new RelayCommand(async () => await LoadAsync());
        NewPromoCommand = new RelayCommand(OpenNewPromo);
        EditPromoCommand = new RelayCommand(obj =>
        {
            if (obj is PromoProductItem item)
                OpenEditPromo(item);
        });
        DeletePromoCommand = new RelayCommand(obj =>
        {
            if (obj is PromoProductItem item)
                DeletePromo(item);
        });
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

    public ObservableCollection<PromoProductItem> PromoProducts { get; }

    public PromoProductItem? SelectedPromoProduct
    {
        get => _selectedPromoProduct;
        set => SetProperty(ref _selectedPromoProduct, value);
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

    public DateTime PromoStartDate
    {
        get => _promoStartDate;
        set => SetProperty(ref _promoStartDate, value);
    }

    public DateTime PromoEndDate
    {
        get => _promoEndDate;
        set => SetProperty(ref _promoEndDate, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
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
    public ICommand NewPromoCommand { get; }
    public ICommand EditPromoCommand { get; }
    public ICommand DeletePromoCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }

    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = "Loading promotional products...";

        try
        {
            var items = await AppServices.ProductManagement.GetPromoProductsAsync();
            _allItems.Clear();
            _allItems.AddRange(items);
            TotalItems = _allItems.Count;
            CurrentPage = 1;
            ApplyPaging();
            StatusMessage = $"Loaded {TotalItems} promotional products.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
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

        PromoProducts.Clear();
        foreach (var item in pageItems)
            PromoProducts.Add(item);

        CommandManager.InvalidateRequerySuggested();
    }

    private void ChangePage(int delta)
    {
        CurrentPage += delta;
        ApplyPaging();
    }

    private async void OpenNewPromo()
    {
        var dialog = new Views.PromoProductDialog(Views.PromoProductDialog.DialogMode.New);
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
                await AppServices.ProductManagement.CreatePromoProductAsync(dialog.ProductId, dialog.StartDate, dialog.EndDate);
                StatusMessage = "Promotional product created successfully.";
                _ = LoadAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error creating promo: {ex.Message}";
            }
        }
    }

    private async void OpenEditPromo(PromoProductItem item)
    {
        var dialog = new Views.PromoProductDialog(Views.PromoProductDialog.DialogMode.Edit);
        dialog.PromoId = item.Id;
        dialog.ProductId = item.ProductId;
        dialog.ProductName = item.ProductName;
        dialog.StartDate = item.StartDate;
        dialog.EndDate = item.EndDate;

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

        if (dialog.ShowDialog() == true)
        {
            if (dialog.IsDeleted)
            {
                try
                {
                    await AppServices.ProductManagement.DeletePromoProductAsync(item.Id);
                    StatusMessage = $"Promotional product '{item.ProductName}' deleted.";
                    _ = LoadAsync();
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error deleting: {ex.Message}";
                }
            }
            else if (dialog.IsSaved)
            {
                try
                {
                    await AppServices.ProductManagement.UpdatePromoProductAsync(item.Id, dialog.ProductId, dialog.StartDate, dialog.EndDate);
                    StatusMessage = $"Promotional product '{item.ProductName}' updated.";
                    _ = LoadAsync();
                }
                catch (Exception ex)
                {
                    StatusMessage = $"Error updating: {ex.Message}";
                }
            }
        }
    }

    private void DeletePromo(PromoProductItem item)
    {
        var result = MessageBox.Show(
            $"Delete promotional product '{item.ProductName}'?",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            AppServices.ProductManagement.DeletePromoProductAsync(item.Id).Wait();
            StatusMessage = $"Promotional product '{item.ProductName}' deleted.";
            _ = LoadAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error deleting: {ex.Message}";
        }
    }
}
