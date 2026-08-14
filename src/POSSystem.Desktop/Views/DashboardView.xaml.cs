using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace POSSystem.Desktop.Views;

public partial class DashboardView : UserControl
{
    public static readonly DependencyProperty CurrentDateTimeProperty =
        DependencyProperty.Register(nameof(CurrentDateTime), typeof(string), typeof(DashboardView));

    public static readonly DependencyProperty GreetingProperty =
        DependencyProperty.Register(nameof(Greeting), typeof(string), typeof(DashboardView));

    public string CurrentDateTime
    {
        get => (string)GetValue(CurrentDateTimeProperty);
        set => SetValue(CurrentDateTimeProperty, value);
    }

    public string Greeting
    {
        get => (string)GetValue(GreetingProperty);
        set => SetValue(GreetingProperty, value);
    }

    private readonly DispatcherTimer _timer;

    public DashboardView()
    {
        InitializeComponent();
        DataContext = this;

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += (_, _) => UpdateClock();
        _timer.Start();

        UpdateClock();
        UpdateGreeting();
    }

    private void UpdateClock()
    {
        CurrentDateTime = DateTime.Now.ToString("dddd, MMMM dd, yyyy  •  h:mm:ss tt");
    }

    private void UpdateGreeting()
    {
        var hour = DateTime.Now.Hour;
        Greeting = hour switch
        {
            < 12 => "Good morning",
            < 17 => "Good afternoon",
            _ => "Good evening"
        };
    }
}