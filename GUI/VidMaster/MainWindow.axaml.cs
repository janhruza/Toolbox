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

    private async Task RefreshUI()
    {
        if (Downloader.Exists() == false)
        {
            miCheckForUpdates.IsEnabled = false;
            await new DlgDownloaderNotFound().ShowDialog(this);
        }

        miCheckForUpdates.IsEnabled = true;
        return;
    }

    private async void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        await RefreshUI();
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
            SetStatusMessage("Source or destination is empty.");
            return;
        }

        string format = string.Empty;

        if (cbxFormats.SelectedItem is ComboBoxItem cbi)
        {
            if (cbi.Tag is string formatId)
            {
                // get the selected format
                format = formatId;
            }
        }

        SetStatusMessage("Download in progress...");
        int ecode = await Downloader.Download(src, dest, format);
        SetStatusMessage(string.Empty);

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
            SetStatusMessage("Can't pick folder.");
        }

        return;
    }

    private void txtUrl_TextChanged(object? sender, TextChangedEventArgs e)
    {
        bool value = string.IsNullOrWhiteSpace(this.txtUrl.Text) == false;
        this.btnOk.IsEnabled = value;
        this.btnRefreshFormats.IsEnabled = value;
        return;
    }

    private async Task<bool> RefreshFormats()
    {
        string src = this.txtUrl.Text ?? string.Empty;
        if (string.IsNullOrWhiteSpace(src))
        {
            return false;
        }

        // clear old formats
        this.cbxFormats.Items.Clear();

        // fetch the list of available formats
        List<FormatInfo> formats = await Downloader.GetAvailableFormats(src);
        if (formats.Count == 0)
        {
            return false;
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

        // insert the 'default' quality item
        ComboBoxItem cbiDefault = new ComboBoxItem
        {
            Content = "Default",
            Tag = string.Empty
        };

        cbxFormats.Items.Insert(0, cbiDefault);

        // add the  'best quality' format item
        ComboBoxItem cbiBest = new ComboBoxItem
        {
            Content = "Overall Best Quality",
            Tag = "bestvideo+bestaudio/best"
        };

        _ = cbxFormats.Items.Add(cbiBest);
        this.cbxFormats.SelectedIndex = 0;

        return true;
    }

    private async Task RefreshFormatsWrapper()
    {
        SetStatusMessage("Listing available formats...");
        string msg = await RefreshFormats() == true ? string.Empty : "Unable to retrieve the list of available formats.";
        SetStatusMessage(msg);

        if (string.IsNullOrWhiteSpace(msg) == false)
        {
            await DlgMessageBox.Show(this, "Unable to retrieve the list of available formats. Please make sure you entered a valid URL address and try again.", "No formats available");
        }
    }

    private async void btnRefreshFormats_Click(object? sender, RoutedEventArgs e)
    {
        await RefreshFormatsWrapper();
    }

    private async void miCheckForUpdates_Click(object? sender, RoutedEventArgs e)
    {
        SetStatusMessage("Checking for updates");
        bool result = await Downloader.CheckForUpdates();
        SetStatusMessage(result == true ? string.Empty : "Update failed.");
    }

    private async void miRefresh_Click(object? sender, RoutedEventArgs e)
    {
        await RefreshUI();
    }
}