using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace VidMaster;

/// <summary>
/// Representing the main application window.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Creates a new <see cref="MainWindow"/> instance.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }

    private void miClose_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void miAbout_Click(object? sender, RoutedEventArgs e)
    {
        // show the about box
        return;
    }

    private void miThemeLight_Click(object? sender, RoutedEventArgs e)
    {
        App.Current?.RequestedThemeVariant = ThemeVariant.Light;
        return;
    }

    private void miThemeDark_Click(object? sender, RoutedEventArgs e)
    {
        App.Current?.RequestedThemeVariant = ThemeVariant.Dark;
        return;
    }

    private void miThemeSystem_Click(object? sender, RoutedEventArgs e)
    {
        App.Current?.RequestedThemeVariant = ThemeVariant.Default;
        return;
    }

    private void btnCancel_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
        return;
    }

    private void btnOk_Click(object? sender, RoutedEventArgs e)
    {
        return;
    }
}