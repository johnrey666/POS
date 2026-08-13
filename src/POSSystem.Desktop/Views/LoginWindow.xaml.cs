using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace POSSystem.Desktop.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        DataContext = new ViewModels.LoginViewModel(this);
        UserNameTextBox.Focus();
    }

    // This binds the secure password to your ViewModel
    private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is ViewModels.LoginViewModel vm && sender is PasswordBox box)
            vm.Password = box.Password;
    }

    // Allows you to drag the borderless window
    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }

    // Closes the app entirely when the 'X' is pressed
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}