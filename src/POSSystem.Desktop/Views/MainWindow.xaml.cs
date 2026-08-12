using System.Windows;
using System.Windows.Controls;
using POSSystem.Desktop.Converters;
using POSSystem.Desktop.ViewModels;

namespace POSSystem.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new ShellViewModel();
    }

    private void NavButton_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button || DataContext is not ShellViewModel shell)
            return;

        var page = button.Tag as string;
        var isActive = string.Equals(shell.CurrentPage, page, StringComparison.OrdinalIgnoreCase);
        button.Style = (Style)FindResource(isActive ? "NavButtonActive" : "NavButton");
    }
}
