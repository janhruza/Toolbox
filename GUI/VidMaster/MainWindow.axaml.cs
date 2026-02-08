using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

using System.Threading.Tasks;

using VidMaster.Core;

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

        Config cfg = new Config();
        txtFolder.Text = cfg.SaveLocation;
    }

    private async void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        if (Downloader.Exists() == false)
        {
            await new DlgDownloaderNotFound().ShowDialog(this);
        }

        return;
    }

    private void miClose_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async void miAbout_Click(object? sender, RoutedEventArgs e)
    {
        // show the about box
        var dlg = new Avalonia.Dialogs.AboutAvaloniaDialog
        {
            CanResize = false,
            CanMaximize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        await dlg.ShowDialog(this);
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

    private void miGetDownloader_Click(object? sender, RoutedEventArgs e)
    {
        Downloader.GetDownloader();
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

    private async void btnChooseFolder_Click(object? sender, RoutedEventArgs e)
    {
        if (StorageProvider.CanPickFolder)
        {
            var folders = await StorageProvider.OpenFolderPickerAsync(new Avalonia.Platform.Storage.FolderPickerOpenOptions
            {
                AllowMultiple = false,
                Title = "Select destination folder"
            });

            if (folders.Count == 1)
            {
                txtFolder.Text = folders[0].Path.AbsolutePath;
            }
        }

        else
        {
            // cant pick folder
        }

        return;
    }
}