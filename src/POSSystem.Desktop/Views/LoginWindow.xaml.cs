using System.Windows;
using System.Windows.Controls;

namespace POSSystem.Desktop.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        DataContext = new ViewModels.LoginViewModel(this);
    }

    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.LoginViewModel vm && sender is PasswordBox box)
            vm.Password = box.Password;
    }
}
