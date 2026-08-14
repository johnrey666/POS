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

    private async Task LoadAsync()
    {
        IsBusy = true;
        StatusMessage = "Loading products...";

        try
        {
            var items = await AppServices.ProductManagement.GetProductsAsync();
            Products.Clear();
            foreach (var item in items)
                Products.Add(item);

            StatusMessage = $"Loaded {Products.Count} products.";
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