using System;
using System.Windows;
using System.Windows.Input;
using POSSystem.Desktop.ViewModels;

namespace POSSystem.Desktop.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly Window _loginWindow;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string? _errorMessage;
    private bool _isBusy;

    public LoginViewModel(Window loginWindow)
    {
        _loginWindow = loginWindow;
        LoginCommand = new RelayCommand(async () =>
        {
            try
            {
                await LoginAsync();
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Login exception: {ex}");
                ErrorMessage = "An unexpected error occurred. Please try again.";
            }
        }, () => !IsBusy && !string.IsNullOrWhiteSpace(Username));
    }

    public string Username
    {
        get => _username;
        set
        {
            if (SetProperty(ref _username, value))
                CommandManager.InvalidateRequerySuggested();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (SetProperty(ref _password, value))
                CommandManager.InvalidateRequerySuggested();
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
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

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        ErrorMessage = null;
        IsBusy = true;

        try
        {
            // Trim inputs to avoid whitespace issues
            var trimmedUsername = Username?.Trim() ?? string.Empty;
            var trimmedPassword = Password?.Trim() ?? string.Empty;

            Console.WriteLine($"Attempting login for user: '{trimmedUsername}'");

            var result = await AppServices.Auth.LoginAsync(trimmedUsername, trimmedPassword);

            // Log the result for debugging
            Console.WriteLine($"Login result: Success={result.Success}, ErrorMessage={result.ErrorMessage}");

            if (!result.Success)
            {
                // Display the specific error message from the service
                ErrorMessage = result.ErrorMessage ?? "Unable to sign in. Please try again.";
                return;
            }

            // Login successful - open main window
            var shell = new Views.MainWindow();
            shell.Show();
            _loginWindow.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during login: {ex.Message}");
            ErrorMessage = "An unexpected error occurred. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}