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
        // Unsubscribe from old ViewModel if exists
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        // Set new ViewModel and subscribe to changes
        _viewModel = DataContext as ShellViewModel;
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            // Force highlight update immediately on startup
            UpdateNavStyles(_viewModel.CurrentPage);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Only react when the CurrentPage changes
        if (e.PropertyName == nameof(ShellViewModel.CurrentPage) && _viewModel != null)
        {
            UpdateNavStyles(_viewModel.CurrentPage);
        }
    }

    private void UpdateNavStyles(string currentPage)
    {
        if (NavLinksStackPanel == null) return;

        // Loop through every button in the center navigation panel
        foreach (UIElement child in NavLinksStackPanel.Children)
        {
            if (child is Button btn)
            {
                var page = btn.Tag as string;
                var isActive = string.Equals(currentPage, page, StringComparison.OrdinalIgnoreCase);
                
                // Apply the correct style
                btn.Style = (Style)FindResource(isActive ? "TopNavLinkActive" : "TopNavLink");
            }
        }
    }
}