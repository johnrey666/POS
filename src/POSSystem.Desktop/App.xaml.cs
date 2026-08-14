using System.Windows;
using System.Windows.Threading;
using POSSystem.Desktop.Views;

namespace POSSystem.Desktop;

public partial class App : Application
{
    public static bool DatabaseReady { get; private set; }

    public static string DatabaseStatus { get; private set; } = "Checking database...";

    protected override async void OnStartup(StartupEventArgs e)
    {
        DispatcherUnhandledException += OnDispatcherUnhandledException;

        var result = await Infrastructure.Data.DatabaseBootstrap.InitializeAsync();
        DatabaseReady = result.Success;
        DatabaseStatus = result.Message;

        if (!DatabaseReady)
        {
            MessageBox.Show(
                DatabaseStatus,
                "POS System — Database Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown();
            return;
        }

        base.OnStartup(e);

        var login = new LoginWindow();
        login.Show();
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        MessageBox.Show(
            e.Exception.ToString(),
            "Unhandled Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        e.Handled = true;
    }
}