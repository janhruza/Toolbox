using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VidMaster.Core;

namespace VidMaster;

/// <summary>
/// Representing the main application window.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Sets the status message.
    /// </summary>
    /// <param name="message">The new message tobe shown.</param>
    /// <remarks>
    /// You can set the <paramref name="message"/> value to <see cref="string.Empty"/> in order to hide the status message box.
    /// </remarks>
    public void SetStatusMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message) == true)
        {
            this.tbMessage.Text = string.Empty;
            this.tbMessage.IsVisible = false;
            return;
        }

        else
        {
            this.tbMessage.Text = message;
            this.tbMessage.IsVisible = true;
            return;
        }
    }

    /// <summary>
    /// Creates a new <see cref="MainWindow"/> instance.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        Config cfg = new Config();
        this.txtFolder.Text = cfg.SaveLocation;
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
        Close();
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
        _ = (App.Current?.RequestedThemeVariant = ThemeVariant.Light);
        return;
    }

    private void miThemeDark_Click(object? sender, RoutedEventArgs e)
    {
        _ = (App.Current?.RequestedThemeVariant = ThemeVariant.Dark);
        return;
    }

    private void miThemeSystem_Click(object? sender, RoutedEventArgs e)
    {
        _ = (App.Current?.RequestedThemeVariant = ThemeVariant.Default);
        return;
    }

    private void miGetDownloader_Click(object? sender, RoutedEventArgs e)
    {
        Downloader.GetDownloader();
        return;
    }

    private void btnCancel_Click(object? sender, RoutedEventArgs e)
    {
        Close();
        return;
    }

    private async void btnOk_Click(object? sender, RoutedEventArgs e)
    {
        string src = this.txtUrl.Text ?? string.Empty;
        string dest = this.txtFolder.Text ?? string.Empty;

        if (src == string.Empty || dest == string.Empty)
        {
            // can't download
            return;
        }

        int ecode = await Downloader.Download(src, dest);
        if (ecode != 0)
        {
            await DlgMessageBox.Show(this, $"Unable to download your media. Error code {ecode}.", "Download Error");
        }

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
                this.txtFolder.Text = folders[0].Path.AbsolutePath;
            }
        }

        else
        {
            // cant pick folder
        }

        return;
    }

    private void txtUrl_TextChanged(object? sender, TextChangedEventArgs e)
    {
        this.btnOk.IsEnabled = string.IsNullOrWhiteSpace(this.txtUrl.Text) == false;
        return;
    }

    private async Task RefreshFormats()
    {
        string src = this.txtUrl.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(src))
        {
            await DlgMessageBox.Show(this, "No media URL provided.", "Refresh Formats");
            return;
        }

        // clear old formats
        this.cbxFormats.Items.Clear();

        // fetch the list of available formats
        List<FormatInfo> formats = await Downloader.GetAvailableFormats(src);
        if (formats.Count == 0)
        {
            await DlgMessageBox.Show(this, "No formats available.", "Refresh Formats");
            return;
        }

        // update the list of formats in the UI
        foreach (FormatInfo format in formats)
        {
            ComboBoxItem cbi = new ComboBoxItem
            {
                Content = format,
                Tag = format.FormatId
            };

            _ = this.cbxFormats.Items.Add(cbi);
        }

        if (this.cbxFormats.Items.Any())
        {
            // select the first item (if any)
            this.cbxFormats.SelectedIndex = 0;
        }

        return;
    }

    private async void btnRefreshFormats_Click(object? sender, RoutedEventArgs e)
    {
        SetStatusMessage("Listing available formats...");
        await RefreshFormats();
        SetStatusMessage(string.Empty);
    }
}