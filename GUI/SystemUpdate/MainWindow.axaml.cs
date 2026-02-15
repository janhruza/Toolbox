using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Timers;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SystemUpdate.Core;
using SystemUpdate.Core.PackageManagers;
using SystemUpdate.Dialogs;

namespace SystemUpdate;

/// <summary>
/// Representing the main application window.
/// </summary>
public partial class MainWindow : Window
{
    #region OP codes

    private const string OP_SYNC = "sync";
    private const string OP_UPDATE = "update";
    private const string OP_INSTALL = "install";
    private const string OP_REMOVE = "remove";
    
    #endregion
    
    // PacMan is default
    private PackageManager _manager = new Pacman();

    private MenuItem[] managerItems = new MenuItem[3];
    private void UnselectAll()
    {
        foreach (MenuItem item in managerItems)
        {
            item.IsSelected = false;
        }
        
        return;
    }
    
    internal static MainWindow? Instance { get; private set; }
    
    /// <summary>
    /// Creates a new <see cref="MainWindow"/> instance.
    /// </summary>
    public MainWindow()
    {
        this.InitializeComponent();
        MainWindow.Instance = this;

        managerItems[0] = this.miPacMan;
        managerItems[1] = this.miWinGet;
        managerItems[2] = this.miAPT;
        
        this._timer.Interval = TimeSpan.FromMilliseconds(100);
        this._timer.Tick += _timer_Elapsed;
    }

    private readonly DispatcherTimer _timer = new DispatcherTimer();
    private readonly Stopwatch _sw = new Stopwatch();

    private void _timer_Elapsed(object? sender, EventArgs e)
    {
        this.tbTime.Text = $"{this._sw.Elapsed.Hours:00}:{this._sw.Elapsed.Minutes:00}:{this._sw.Elapsed.Seconds:00}";
    }
    
    /// <summary>
    /// Starts the measuring timer.
    /// </summary>
    private void StartTimer()
    {
        this._timer.Start();
        this._sw.Start();
    }

    /// <summary>
    /// Stops the measuring timer.
    /// </summary>
    private void StopTimer()
    {
        this._sw.Stop();
        this._timer.Stop();
    }

    /// <summary>
    /// Clears the timer.
    /// </summary>
    private void ClearTimer()
    {
        this._sw.Reset();
        this.tbTime.Text = "00:00:00";
    }
    
    /// <summary>
    /// Sets the status text message.
    /// </summary>
    /// <param name="text">The text to be displayed.</param>
    private void SetStatusText(string text)
    {
        this.tbStatus.Text = text;
        return;
    }
    
    private void MiClose_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private async void MiAbout_OnClick(object? sender, RoutedEventArgs e)
    {
        // TODO show the about box

        await DlgMessageBox.Show(this, "Simple system update utility. It provides basic wrapper over WinGet, PacMan and APT package managers.", "About SystemUpdate");
        return;
    }

    private string GetSelectedOperation()
    {
        if (this.cbxAction.SelectedItem is null) return string.Empty;
        if (this.cbxAction.SelectedItem is ComboBoxItem item)
        {
            if (item.Tag is string tag) return tag;
            else return string.Empty;
        }

        else
        {
            return string.Empty;
        }
    }
    
    private async void BtnPerform_OnClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // clear the timer first
            this.ClearTimer();
            
            // check for the root access
            if (!Program.IsRoot())
            {
                SetStatusText("Root access required.");
                return;
            }
            
            string op = this.GetSelectedOperation();
            if (string.IsNullOrWhiteSpace(op) == true)
            {
                SetStatusText("Invalid operation selected.");
                Console.Beep();
                return;
            }
        
            // start the timer when the operation is ready to execute
            this.StartTimer();

            bool result;
        
            switch (op)
            {
                case MainWindow.OP_SYNC:
                    result = await this._manager.Sync();
                    break;
            
                case MainWindow.OP_UPDATE:
                    result = await this._manager.Update();
                    break;
            
                default:
                    result = false;
                    break;
            }
        
            // stop the timer when the operation finished
            this.StopTimer();
        
            // display the result
            if (result == true)
            {
                this.SetStatusText("Operation completed successfully.");
            }

            else
            {
                this.SetStatusText("Operation failed.");
            }
        
            Console.Beep();
            return;
        }
        catch (Exception ex)
        {
            SetStatusText(ex.Message);
            Console.Beep();
        }
    }

    private void LoadPM(PackageManager pm, MenuItem item)
    {
        UnselectAll();
        _manager = pm;
        item.IsSelected = true;
        this.lblPackageManager.Content = this._manager.Name;
        return;
    }
    
    private void MiPacMan_OnClick(object? sender, RoutedEventArgs e)
    {
        this.LoadPM(new Pacman(), this.miPacMan);
    }

    private void MiWinGet_OnClick(object? sender, RoutedEventArgs e)
    {
        this.LoadPM(new WinGet(), this.miWinGet);
    }

    private void MiAPT_OnClick(object? sender, RoutedEventArgs e)
    {
        this.LoadPM(new APT(), this.miAPT);
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        LoadPM(new Pacman(), this.miPacMan);
        SetStatusText("Window loaded.");
    }

    private void CbxAction_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (this.cbxAction is null || this.cbxAction.SelectedItem is null) return;
        if (this.cbxAction.SelectedItem is ComboBoxItem item)
        {
            if (item.Tag is string tag)
            {
                if (tag == MainWindow.OP_INSTALL || tag == MainWindow.OP_REMOVE)
                {
                    // show the package input
                    this.lblPackages.IsVisible = true;
                    this.txtPackages.IsVisible = true;
                }

                else
                {
                    this.lblPackages.IsVisible = false;
                    this.txtPackages.IsVisible = false;
                }
            }
        }
    }
}