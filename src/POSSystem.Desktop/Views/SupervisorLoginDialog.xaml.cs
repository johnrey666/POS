using System.Windows;

namespace POSSystem.Desktop.Views;

public partial class SupervisorLoginDialog : Window
{
    public string Username => UsernameBox.Text.Trim();
    public string Password => PasswordBox.Password;

    public SupervisorLoginDialog(string actionDescription)
    {
        InitializeComponent();
        ActionText.Text = actionDescription;
        UsernameBox.Focus();
    }

    private void Approve_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorText.Text = "Username and password are required.";
            ErrorText.Visibility = Visibility.Visible;
            return;
        }

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}