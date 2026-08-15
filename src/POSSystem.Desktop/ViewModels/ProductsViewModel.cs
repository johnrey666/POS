using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using POSSystem.Domain.Models;

namespace POSSystem.Desktop.ViewModels;

public class ProductsViewModel : ViewModelBase
{
    private string _statusMessage = string.Empty;
    private bool _isBusy;
    private ProductManagementItem? _selectedProduct;
    private int _currentPage = 1;
    private int _totalPages;
    private int _totalItems;
    private readonly List<ProductManagementItem> _allItems = [];

    public ProductsViewModel()
    {
        Products = [];

        LoadCommand = new RelayCommand(async () => await LoadAsync());
        NewProductCommand = new RelayCommand(OpenNewProduct);
        ViewProductCommand = new RelayCommand(obj =>
        {
            if (obj is ProductManagementItem product)
                OpenViewProduct(product);
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

    public ObservableCollection<ProductManagementItem> Products { get; }

    public ProductManagementItem? SelectedProduct
    {
        get => _selectedProduct;
        set => SetProperty(ref _selectedProduct, value);
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
    public ICommand NewProductCommand { get; }
    public ICommand ViewProductCommand { get; }
    public ICommand PreviousPageCommand { get; }
    public ICommand NextPageCommand { get; }

    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = "Loading products...";

        try
        {
            var items = await AppServices.ProductManagement.GetProductsAsync();
            _allItems.Clear();
            _allItems.AddRange(items);
            TotalItems = _allItems.Count;
            CurrentPage = 1;
            ApplyPaging();
            StatusMessage = $"Loaded {TotalItems} products.";
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

        Products.Clear();
        foreach (var item in pageItems)
            Products.Add(item);

        CommandManager.InvalidateRequerySuggested();
    }

    private void ChangePage(int delta)
    {
        CurrentPage += delta;
        ApplyPaging();
    }

 private void OpenNewProduct()
{
    var dialog = new Views.ProductDialog(Views.ProductDialog.DialogMode.New);

    // Safe owner assignment
    var activeWindow = Application.Current.Windows
        .OfType<Window>()
        .FirstOrDefault(w => w.IsActive);

    if (activeWindow != null && !ReferenceEquals(activeWindow, dialog))
        dialog.Owner = activeWindow;
    else if (Application.Current.MainWindow != null && !ReferenceEquals(Application.Current.MainWindow, dialog))
        dialog.Owner = Application.Current.MainWindow;

    if (dialog.ShowDialog() == true && dialog.IsSaved)
    {
        StatusMessage = "Product created successfully.";
        _ = LoadAsync();
    }
}

private void OpenViewProduct(ProductManagementItem product)
{
    var dialog = new Views.ProductDialog(Views.ProductDialog.DialogMode.View);

    // Safe owner assignment
    var activeWindow = Application.Current.Windows
        .OfType<Window>()
        .FirstOrDefault(w => w.IsActive);

    if (activeWindow != null && !ReferenceEquals(activeWindow, dialog))
        dialog.Owner = activeWindow;
    else if (Application.Current.MainWindow != null && !ReferenceEquals(Application.Current.MainWindow, dialog))
        dialog.Owner = Application.Current.MainWindow;

    // Pre-fill
    dialog.ProductIdBox.Text = product.Id.ToString();
    dialog.NameBox.Text = product.Name;
    dialog.PriceBox.Text = product.SellingPrice.ToString("N2");

    if (dialog.ShowDialog() == true)
    {
        if (dialog.IsDeleted)
        {
            StatusMessage = $"Product '{product.Name}' deleted.";
            _ = LoadAsync();
        }
        else if (dialog.IsSaved)
        {
            StatusMessage = $"Product '{product.Name}' updated.";
            _ = LoadAsync();
        }
    }
}
}