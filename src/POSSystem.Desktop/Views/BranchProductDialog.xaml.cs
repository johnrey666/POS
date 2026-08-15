using System.Collections.ObjectModel;
using System.Windows;
using POSSystem.Domain.Models;

namespace POSSystem.Desktop.Views;

public partial class BranchProductDialog : Window
{
    public enum DialogMode { Add, Edit }

    public bool IsSaved { get; private set; }
    public bool IsDeleted { get; private set; }

    public ObservableCollection<ProductSummary> Products { get; } = [];

    public int ProductId
    {
        get => (int)ProductCombo.SelectedValue;
        set => ProductCombo.SelectedValue = value;
    }
    public string ProductName
    {
        get => ProductCombo.Text;
        set => ProductCombo.Text = value;
    }
    public decimal BranchPrice
    {
        get
        {
            return decimal.TryParse(PriceBox.Text, out var price) ? price : 0;
        }
        set => PriceBox.Text = value.ToString("N2");
    }

    public BranchProductDialog(DialogMode mode)
    {
        InitializeComponent();
        TitleText.Text = mode == DialogMode.Add ? "Add Product to Branch" : "Edit Branch Product";
        ProductCombo.ItemsSource = Products;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (ProductCombo.SelectedValue == null)
        {
            MessageBox.Show("Please select a product.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!decimal.TryParse(PriceBox.Text, out var price) || price < 0)
        {
            MessageBox.Show("Please enter a valid price.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsSaved = true;
        DialogResult = true;
        Close();
    }
}
