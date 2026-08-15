using System.Windows;
using System.Windows.Controls;
using POSSystem.Desktop.ViewModels;

namespace POSSystem.Desktop.Views;

public partial class MainWindow : Window
{
    private ShellViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        DataContext = new ShellViewModel();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _viewModel = DataContext as ShellViewModel;
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            UpdateNavStyles(_viewModel.CurrentPage);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ShellViewModel.CurrentPage) && _viewModel != null)
        {
            UpdateNavStyles(_viewModel.CurrentPage);
        }
    }

    private void UpdateNavStyles(string currentPage)
    {
        var header = FindName("HeaderNavPanel") as StackPanel;
        if (header == null) return;

        foreach (UIElement child in header.Children)
        {
            if (child is Button btn && btn.Tag is string page)
            {
                var isActive = string.Equals(currentPage, page, StringComparison.OrdinalIgnoreCase);
                
                if (isActive)
                    btn.Style = (Style)FindResource("HeaderNavButtonActive");
                else
                    btn.Style = (Style)FindResource("HeaderNavButton");
            }
        }
    }
}
