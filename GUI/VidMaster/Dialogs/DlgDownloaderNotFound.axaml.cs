using Avalonia.Controls;
using Avalonia.Interactivity;
using VidMaster.Core;

namespace VidMaster;

/// <summary>
///     Representing the downloader not found error box.
/// </summary>
public class DlgDownloaderNotFound : Window
{
    /// <summary>
    ///     Creates a new <see cref="DlgDownloaderNotFound" /> instance.
    /// </summary>
    public DlgDownloaderNotFound()
    {
        InitializeComponent();
    }

    private void btnCancel_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void btnDownloadNow_Click(object? sender, RoutedEventArgs e)
    {
        Downloader.GetDownloader();
        this.Close();
    }
}