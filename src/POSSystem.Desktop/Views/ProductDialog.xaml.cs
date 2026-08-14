using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace POSSystem.Desktop.Views;

public partial class ProductDialog : Window
{
    public enum DialogMode { View, Edit, New }

    public DialogMode Mode { get; private set; }
    public bool IsSaved { get; private set; }
    public bool IsDeleted { get; private set; }

    public ProductDialog(DialogMode mode)
    {
        InitializeComponent();
        Mode = mode;
        ApplyMode();
    }

    private void ApplyMode()
    {
        switch (Mode)
        {
            case DialogMode.New:
                TitleText.Text = "New Product";
                ProductIdBox.Text = "(Auto generated)";
                ActivePanel.Visibility = Visibility.Collapsed;
                ViewButtons.Visibility = Visibility.Collapsed;
                EditButtons.Visibility = Visibility.Visible;
                SetReadOnly(false);
                break;

            case DialogMode.View:
                TitleText.Text = "View Product";
                ActivePanel.Visibility = Visibility.Visible;
                ViewButtons.Visibility = Visibility.Visible;
                EditButtons.Visibility = Visibility.Collapsed;
                SetReadOnly(true);
                break;

            case DialogMode.Edit:
                TitleText.Text = "Edit Product";
                ActivePanel.Visibility = Visibility.Visible;
                ViewButtons.Visibility = Visibility.Collapsed;
                EditButtons.Visibility = Visibility.Visible;
                SetReadOnly(false);
                break;
        }
    }

    private void SetReadOnly(bool readOnly)
    {
        NameBox.IsReadOnly = readOnly;
        PriceBox.IsReadOnly = readOnly;
        UomBox.IsEnabled = !readOnly;
        BranchesList.IsEnabled = !readOnly;
        UploadImageButton.IsEnabled = !readOnly;
        ActiveYes.IsEnabled = !readOnly;
        ActiveNo.IsEnabled = !readOnly;
    }

    private void UploadImage_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
            Title = "Select Product Image"
        };

        if (dialog.ShowDialog() == true)
        {
            // TODO: handle image path
            MessageBox.Show($"Selected: {dialog.FileName}", "Image", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void AddRawMaterial_Click(object sender, RoutedEventArgs e)
    {
        var row = new Grid { Margin = new Thickness(0, 0, 0, 8) };
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(12) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(8) });
        row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        var nameBox = new TextBox
        {
            Height = 34,
            Padding = new Thickness(8, 0, 8, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            Tag = "Name"
        };
        Grid.SetColumn(nameBox, 0);

        var uomBox = new TextBox
        {
            Height = 34,
            Padding = new Thickness(8, 0, 8, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            Tag = "Uom"
        };
        Grid.SetColumn(uomBox, 2);

        var qtyBox = new TextBox
        {
            Height = 34,
            Padding = new Thickness(8, 0, 8, 0),
            VerticalContentAlignment = VerticalAlignment.Center,
            Tag = "Qty"
        };
        Grid.SetColumn(qtyBox, 4);

        var removeBtn = new Button
        {
            Content = "✕",
            Width = 30,
            Height = 30,
            Background = System.Windows.Media.Brushes.Transparent,
            BorderThickness = new Thickness(0),
            Cursor = System.Windows.Input.Cursors.Hand,
            Foreground = System.Windows.Media.Brushes.Red
        };
        removeBtn.Click += (s, args) => RawMaterialsPanel.Children.Remove(row);
        Grid.SetColumn(removeBtn, 6);

        row.Children.Add(nameBox);
        row.Children.Add(uomBox);
        row.Children.Add(qtyBox);
        row.Children.Add(removeBtn);

        RawMaterialsPanel.Children.Add(row);
    }

    private void Edit_Click(object sender, RoutedEventArgs e)
    {
        Mode = DialogMode.Edit;
        ApplyMode();
    }

    private void Delete_Click(object sender, RoutedEventArgs e)
    {
        var confirm = MessageBox.Show("Are you sure you want to delete this product?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.Yes)
        {
            IsDeleted = true;
            DialogResult = true;
            Close();
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameBox.Text))
        {
            MessageBox.Show("Product Name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        IsSaved = true;
        DialogResult = true;
        Close();
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}