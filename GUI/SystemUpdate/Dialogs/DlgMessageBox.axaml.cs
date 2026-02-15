using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SystemUpdate.Dialogs;

/// <summary>
/// Representing the about dialog.
/// </summary>
public partial class DlgMessageBox : Window
{
    /// <summary>
    /// Creates a new <see cref="DlgMessageBox"/> instance.
    /// </summary>
    private DlgMessageBox(string message, string caption)
    {
        this.InitializeComponent();
        this.Title = caption;
        this.tbMessage.Text = message;
    }

    private void BtnClose_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    /// <summary>
    /// Shows the message box.
    /// </summary>
    /// <param name="owner">Message box owner window.</param>
    /// <param name="message">A message to be shown.</param>
    /// <param name="caption">Caption of the message.</param>
    public static async Task Show(Window owner, string message, string caption)
    {
        DlgMessageBox msgBox = new DlgMessageBox(message, caption);
        await msgBox.ShowDialog(owner);
        return;
    }
}