using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Dialogs;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using VidMaster.Core;

namespace VidMaster;

/// <summary>
///     Representing the main application window.
/// </summary>
public class MainWindow : Window
{
    /// <summary>
    ///     Creates a new <see cref="MainWindow" /> instance.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        Config cfg = new();
        this.txtFolder.Text = cfg.SaveLocation;
    }

    /// <summary>
    ///     Sets the status message.
    /// </summary>
    /// <param name="message">The new message tobe shown.</param>
    /// <remarks>
    ///     You can set the <paramref name="message" /> value to <see cref="string.Empty" /> in order to hide the status
    ///     message box.
    /// </remarks>
    public void SetStatusMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(value: message))
        {
            this.tbMessage.Text = string.Empty;
            this.tbMessage.IsVisible = false;
        }

        else
        {
            this.tbMessage.Text = message;
            this.tbMessage.IsVisible = true;
        }
    }

    private async Task RefreshUI()
    {
        if (!Downloader.Exists())
        {
            this.miCheckForUpdates.IsEnabled = false;
            await new DlgDownloaderNotFound().ShowDialog(owner: this);
        }

        this.miCheckForUpdates.IsEnabled = true;
    }

    private async void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        await this.RefreshUI();
    }

    private void miClose_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async void miAbout_Click(object? sender, RoutedEventArgs e)
    {
        // show the about box
        AboutAvaloniaDialog dlg = new()
        {
            CanResize = false,
            CanMaximize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        await dlg.ShowDialog(owner: this);
    }

    private void miThemeLight_Click(object? sender, RoutedEventArgs e)
    {
        _ = Application.Current?.RequestedThemeVariant = ThemeVariant.Light;
    }

    private void miThemeDark_Click(object? sender, RoutedEventArgs e)
    {
        _ = Application.Current?.RequestedThemeVariant = ThemeVariant.Dark;
    }

    private void miThemeSystem_Click(object? sender, RoutedEventArgs e)
    {
        _ = Application.Current?.RequestedThemeVariant = ThemeVariant.Default;
    }

    private void miGetDownloader_Click(object? sender, RoutedEventArgs e)
    {
        Downloader.GetDownloader();
    }

    private void btnCancel_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async void btnOk_Click(object? sender, RoutedEventArgs e)
    {
        string? src = this.txtUrl.Text ?? string.Empty;
        string? dest = this.txtFolder.Text ?? string.Empty;

        if (src == string.Empty || dest == string.Empty)
        {
            // can't download
            this.SetStatusMessage(message: "Source or destination is empty.");
            return;
        }

        string format = string.Empty;

        if (this.cbxFormats.SelectedItem is ComboBoxItem cbi)
            if (cbi.Tag is string formatId)
                // get the selected format
                format = formatId;

        this.SetStatusMessage(message: "Download in progress...");
        int ecode = await Downloader.Download(url: src, destination: dest, format: format);
        this.SetStatusMessage(message: string.Empty);

        if (ecode != 0)
            await DlgMessageBox.Show(owner: this, message: $"Unable to download your media. Error code {ecode}.",
                title: "Download Error");
    }

    private async void btnChooseFolder_Click(object? sender, RoutedEventArgs e)
    {
        if (this.StorageProvider.CanPickFolder)
        {
            IReadOnlyList<IStorageFolder> folders = await this.StorageProvider.OpenFolderPickerAsync(
                options: new FolderPickerOpenOptions
                {
                    AllowMultiple = false,
                    Title = "Select destination folder"
                });

            if (folders.Count == 1) this.txtFolder.Text = folders[index: 0].Path.AbsolutePath;
        }

        else
        {
            // cant pick folder
            this.SetStatusMessage(message: "Can't pick folder.");
        }
    }

    private void txtUrl_TextChanged(object? sender, TextChangedEventArgs e)
    {
        bool value = !string.IsNullOrWhiteSpace(value: this.txtUrl.Text);
        this.btnOk.IsEnabled = value;
        this.btnRefreshFormats.IsEnabled = value;
    }

    private async Task<bool> RefreshFormats()
    {
        string? src = this.txtUrl.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value: src)) return false;

        // clear old formats
        this.cbxFormats.Items.Clear();

        // fetch the list of available formats
        List<FormatInfo> formats = await Downloader.GetAvailableFormats(mediaUrl: src);
        if (formats.Count == 0) return false;

        // update the list of formats in the UI
        foreach (FormatInfo format in formats)
        {
            ComboBoxItem cbi = new()
            {
                Content = format,
                Tag = format.FormatId
            };

            _ = this.cbxFormats.Items.Add(cbi);
        }

        // insert the 'default' quality item
        ComboBoxItem cbiDefault = new()
        {
            Content = "Default",
            Tag = string.Empty
        };

        this.cbxFormats.Items.Insert(0, cbiDefault);

        // add the  'best quality' format item
        ComboBoxItem cbiBest = new()
        {
            Content = "Overall Best Quality",
            Tag = "bestvideo+bestaudio/best"
        };

        _ = this.cbxFormats.Items.Add(cbiBest);
        this.cbxFormats.SelectedIndex = 0;

        return true;
    }

    private async Task RefreshFormatsWrapper()
    {
        this.SetStatusMessage(message: "Listing available formats...");
        string msg = await this.RefreshFormats() ? string.Empty : "Unable to retrieve the list of available formats.";
        this.SetStatusMessage(message: msg);

        if (!string.IsNullOrWhiteSpace(value: msg))
            await DlgMessageBox.Show(owner: this,
                message:
                "Unable to retrieve the list of available formats. Please make sure you entered a valid URL address and try again.",
                title: "No formats available");
    }

    private async void btnRefreshFormats_Click(object? sender, RoutedEventArgs e)
    {
        await this.RefreshFormatsWrapper();
    }

    private async void miCheckForUpdates_Click(object? sender, RoutedEventArgs e)
    {
        this.SetStatusMessage(message: "Checking for updates");
        bool result = await Downloader.CheckForUpdates();
        this.SetStatusMessage(message: result ? string.Empty : "Update failed.");
    }

    private async void miRefresh_Click(object? sender, RoutedEventArgs e)
    {
        await this.RefreshUI();
    }
}