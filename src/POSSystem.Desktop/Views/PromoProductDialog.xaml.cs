using System.Collections.ObjectModel;
using System.Windows;
using POSSystem.Domain.Models;

namespace POSSystem.Desktop.Views;

public partial class PromoProductDialog : Window
{
    public enum DialogMode { New, Edit }

    public bool IsSaved { get; private set; }
    public bool IsDeleted { get; private set; }

    public ObservableCollection<ProductSummary> Products { get; } = [];

    public int PromoId { get; set; }
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
    public DateTime StartDate
    {
        get => StartDatePicker.SelectedDate ?? DateTime.Today;
        set => StartDatePicker.SelectedDate = value;
    }
    public DateTime EndDate
    {
        get => EndDatePicker.SelectedDate ?? DateTime.Today;
        set => EndDatePicker.SelectedDate = value;
    }
    public bool IsPromoActive => ActiveYes.IsChecked == true;

    public PromoProductDialog(DialogMode mode)
    {
        InitializeComponent();
        TitleText.Text = mode == DialogMode.New ? "New Promotion" : "Edit Promotion";
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

        if (StartDatePicker.SelectedDate == null || EndDatePicker.SelectedDate == null)
        {
            MessageBox.Show("Please select both start and end dates.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (EndDatePicker.SelectedDate < StartDatePicker.SelectedDate)
        {
            MessageBox.Show("End date must be after start date.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsSaved = true;
        DialogResult = true;
        Close();
    }
}
