//LoginViewModel.cs
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
            catch (Exception)
            {
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
            // ✅ FIX: STRIP WHITESPACE BEFORE SENDING
            var result = await AppServices.Auth.LoginAsync(Username.Trim(), Password.Trim());

            if (!result.Success)
            {
                ErrorMessage = result.ErrorMessage;
                return;
            }

            var shell = new Views.MainWindow();
            shell.Show();
            _loginWindow.Close();
        }
        catch (Exception)
        {
            ErrorMessage = "Unable to sign in. Please try again.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}