using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using POSSystem.Infrastructure.Data;
using POSSystem.Domain.Models;

namespace POSSystem.Desktop.ViewModels;

public class PosViewModel : ViewModelBase
{
    private CategorySummary? _selectedCategory;
    private string _searchText = string.Empty;
    private string? _statusMessage;
    private bool _isBusy;

    public PosViewModel()
    {
        Categories = [];
        Products = [];
        Cart = [];
        RecentSales = [];

        var currentUser = AppServices.Auth.CurrentUser;
        BranchName = currentUser?.BranchName ?? "Main Branch";
        TerminalName = currentUser?.TerminalName ?? "Terminal 01";

        AddProductCommand = new RelayCommand(obj =>
        {
            if (obj is ProductSummary product)
                AddProductToCart(product);
        });

        IncreaseQuantityCommand = new RelayCommand(obj =>
        {
            if (obj is CartLineViewModel line)
                IncreaseQuantity(line);
        });

        DecreaseQuantityCommand = new RelayCommand(obj =>
        {
            if (obj is CartLineViewModel line)
                DecreaseQuantity(line);
        });

        RemoveFromCartCommand = new RelayCommand(obj =>
        {
            if (obj is CartLineViewModel line)
                RemoveFromCart(line);
        });

        ClearCartCommand = new RelayCommand(ClearCart);
        CheckoutCommand = new RelayCommand(Checkout, () => Cart.Count > 0 && !IsBusy);
        HoldSaleCommand = new RelayCommand(HoldSale, () => Cart.Count > 0 && !IsBusy);

        try
        {
            _ = LoadAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Initialization error: {ex.Message}";
        }
    }

    public ObservableCollection<CategorySummary> Categories { get; }
    public ObservableCollection<ProductSummary> Products { get; }
    public ObservableCollection<CartLineViewModel> Cart { get; }
    public ObservableCollection<RecentSaleSummary> RecentSales { get; }

    public CategorySummary? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
                _ = LoadProductsAsync();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
                _ = LoadProductsAsync();
        }
    }

    public string BranchName { get; }
    public string TerminalName { get; }

    public string? StatusMessage
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
            {
                CommandManager.InvalidateRequerySuggested();
                OnPropertyChanged(nameof(Total));
            }
        }
    }

    // Tax removed
    public decimal Total => Cart.Sum(item => item.LineTotal);

    public ICommand AddProductCommand { get; }
    public ICommand IncreaseQuantityCommand { get; }
    public ICommand DecreaseQuantityCommand { get; }
    public ICommand RemoveFromCartCommand { get; }
    public ICommand ClearCartCommand { get; }
    public ICommand CheckoutCommand { get; }
    public ICommand HoldSaleCommand { get; }

    private async Task LoadAsync()
    {
        await LoadCategoriesAsync();
        await LoadProductsAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var categories = await AppServices.ProductCatalog.GetCategoriesAsync();
            Categories.Clear();
            foreach (var category in categories)
                Categories.Add(category);

            // Start with All Items
            SelectedCategory = null;
        }
        catch (Exception)
        {
            StatusMessage = "Unable to load product categories.";
        }
    }

    private async Task LoadProductsAsync()
    {
        try
        {
            var products = await AppServices.ProductCatalog.GetProductsAsync(
                SelectedCategory?.Id,
                SearchText,
                default);

            Products.Clear();
            foreach (var product in products)
                Products.Add(product);
        }
        catch (Exception)
        {
            StatusMessage = "Unable to load products.";
        }
    }

    private void AddProductToCart(ProductSummary? product)
    {
        if (product is null)
            return;

        var existing = Cart.FirstOrDefault(item => item.ProductId == product.Id);
        if (existing is null)
        {
            Cart.Add(new CartLineViewModel(product.Id, product.Name, product.SellingPrice, product.Barcode ?? string.Empty));
        }
        else
        {
            existing.Quantity += 1;
            existing.NotifyChanged();
        }

        StatusMessage = $"Added {product.Name} to cart.";
        OnPropertyChanged(nameof(Total));
        CommandManager.InvalidateRequerySuggested();
    }

    private void IncreaseQuantity(CartLineViewModel? line)
    {
        if (line is null)
            return;

        line.Quantity += 1;
        line.NotifyChanged();
        OnPropertyChanged(nameof(Total));
    }

    private void DecreaseQuantity(CartLineViewModel? line)
    {
        if (line is null)
            return;

        // Require supervisor approval for ANY quantity decrease
        if (!RequestSupervisorAccess("Decrease item quantity"))
            return;

        if (line.Quantity <= 1)
        {
            Cart.Remove(line);
        }
        else
        {
            line.Quantity -= 1;
            line.NotifyChanged();
        }

        OnPropertyChanged(nameof(Total));
    }

    private void RemoveFromCart(CartLineViewModel? line)
    {
        if (line is null)
            return;

        if (!RequestSupervisorAccess("Remove item from cart"))
            return;

        Cart.Remove(line);
        OnPropertyChanged(nameof(Total));
    }

    private void ClearCart()
    {
        if (Cart.Count == 0)
            return;

        if (!RequestSupervisorAccess("Clear entire cart"))
            return;

        Cart.Clear();
        StatusMessage = "Cart cleared.";
        OnPropertyChanged(nameof(Total));
    }

    private void Checkout()
    {
        if (Cart.Count == 0)
            return;

        try
        {
            using var context = DatabaseBootstrap.CreateContext();
            var receiptNumber = $"POS-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";

            foreach (var line in Cart.ToList())
            {
                var product = context.Products.FirstOrDefault(p => p.Id == line.ProductId)
                    ?? throw new InvalidOperationException($"Product '{line.ProductName}' was not found.");

                if (product.StockQuantity < line.Quantity)
                    throw new InvalidOperationException($"Not enough stock for {product.Name}. Only {product.StockQuantity} left.");

                product.StockQuantity -= line.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }

            context.SaveChanges();

            RecentSales.Insert(0, new RecentSaleSummary
            {
                ReceiptNumber = receiptNumber,
                ItemCount = Cart.Count,
                Total = Total,
                CreatedAt = DateTime.Now
            });

            StatusMessage = $"Sale complete — receipt {receiptNumber}. Total {Total:C}.";
            Cart.Clear();
            OnPropertyChanged(nameof(Total));
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void HoldSale()
    {
        if (Cart.Count == 0)
            return;

        var receiptNumber = $"HOLD-{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
        RecentSales.Insert(0, new RecentSaleSummary
        {
            ReceiptNumber = receiptNumber,
            ItemCount = Cart.Count,
            Total = Total,
            CreatedAt = DateTime.Now,
            Status = "Held"
        });

        StatusMessage = $"Sale held for later — {receiptNumber}.";
        Cart.Clear();
        OnPropertyChanged(nameof(Total));
    }

    /// <summary>
    /// Shows a single dialog with Username + Password.
    /// Returns true only if credentials are valid.
    /// </summary>
    private bool RequestSupervisorAccess(string actionDescription)
{
    var dialog = new Views.SupervisorLoginDialog(actionDescription);

    // Safely set the owner (prevents "Cannot set Owner property to itself")
    var activeWindow = Application.Current.Windows
        .OfType<Window>()
        .FirstOrDefault(w => w.IsActive);

    if (activeWindow != null && !ReferenceEquals(activeWindow, dialog))
    {
        dialog.Owner = activeWindow;
    }
    else if (Application.Current.MainWindow != null && 
             !ReferenceEquals(Application.Current.MainWindow, dialog))
    {
        dialog.Owner = Application.Current.MainWindow;
    }

    var result = dialog.ShowDialog();
    if (result != true)
        return false;

    try
    {
        var authResult = AppServices.Auth
            .LoginAsync(dialog.Username, dialog.Password)
            .GetAwaiter()
            .GetResult();

        if (authResult is null || !authResult.Success)
        {
            MessageBox.Show(
                authResult?.ErrorMessage ?? "Invalid supervisor credentials.",
                "Access Denied",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return false;
        }

        // Optional role check (uncomment when ready)
        // if (authResult.User?.Role is not ("Supervisor" or "Manager" or "Admin"))
        // {
        //     MessageBox.Show("This account does not have supervisor privileges.",
        //         "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
        //     AppServices.Auth.Logout();
        //     return false;
        // }

        // Log out supervisor so the original cashier stays logged in
        AppServices.Auth.Logout();

        return true;
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Authentication error: {ex.Message}", "Error",
            MessageBoxButton.OK, MessageBoxImage.Error);
        return false;
    }
}
}

public sealed class RecentSaleSummary
{
    public string ReceiptNumber { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Completed";
}

public class CartLineViewModel : ViewModelBase
{
    public CartLineViewModel(int productId, string productName, decimal unitPrice, string barcode)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Barcode = barcode;
        Quantity = 1;
    }

    public int ProductId { get; }
    public string ProductName { get; }
    public string Barcode { get; }
    public decimal UnitPrice { get; }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
                OnPropertyChanged(nameof(LineTotal));
        }
    }

    public decimal LineTotal => Quantity * UnitPrice;

    public void NotifyChanged()
    {
        OnPropertyChanged(nameof(Quantity));
        OnPropertyChanged(nameof(LineTotal));
    }
}