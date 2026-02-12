using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace VidMaster;

/// <summary>
///     Representing a simple message box.
/// </summary>
public class DlgMessageBox : Window
{
    /// <summary>
    ///     Representing a simple message box.
    /// </summary>
    /// <param name="message">The message to be shown.</param>
    /// <param name="caption">Message box caption text.</param>
    public DlgMessageBox(string message, string caption)
    {
        InitializeComponent();
        this.Title = caption;
        this.tbMessage.Text = message;
    }

    private async Task BeepAsync()
    {
        Console.Beep();
    }

    private async void Window_Loaded(object? sender, RoutedEventArgs e)
    {
        await this.BeepAsync();
    }

    private void btnOk_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    /// <summary>
    ///     Shows a basic message box.
    /// </summary>
    /// <param name="owner">Message box owner.</param>
    /// <param name="message">The message to be shown.</param>
    /// <param name="title">Message box caption text.</param>
    /// <returns>Awaitable task with no return value.</returns>
    public static async Task Show(Window owner, string message, string title)
    {
        DlgMessageBox msgBox = new(message: message, caption: title);
        await msgBox.ShowDialog(owner: owner);
    }
}