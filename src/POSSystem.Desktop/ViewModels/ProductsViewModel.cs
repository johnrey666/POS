using System.Collections.ObjectModel;
using System.Windows.Input;
using POSSystem.Domain.Models;

namespace POSSystem.Desktop.ViewModels;

public class ProductsViewModel : ViewModelBase
{
    private string _statusMessage = string.Empty;
    private bool _isBusy;

    public ProductsViewModel()
    {
        Products = [];
        LoadCommand = new RelayCommand(async () => await LoadAsync());

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
}
