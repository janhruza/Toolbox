using System;

using Toolbox;
using Toolbox.UI;

namespace Algebrax;

/// <summary>
/// Main program class.
/// </summary>
public class Program : IApplication
{
    /// <inheritdoc/>
    public static Version Version => new Version(1, 0, 0, 0);

    /// <inheritdoc/>
    public static void DisplayBanner()
    {
        return;
    }

    /// <inheritdoc/>
    public static void LoadConfig()
    {
        return;
    }

    /// <inheritdoc/>
    public static void PostExitCleanup()
    {
        return;
    }

    private static MenuItemCollection CreateMainMenu()
    {
        int idx = 1;
        MenuItemCollection menuItems = new MenuItemCollection
        {
            new MenuItem(idx++, "QUADRATIC EQUATION"),
            new MenuItem(),
            new MenuItem(MenuItem.ID_EXIT, "EXIT")
        };

        return menuItems;
    }

    private static int Main(string[] args)
    {
        DisplayBanner();
        MenuItemCollection menuItems = CreateMainMenu();

        bool bExit = false;
        while (!bExit)
        {
            Console.Clear();
            int idx = ConsoleMenu.SelectMenu(menuItems);
            Console.Clear();

            switch (idx)
            {
                case 0:
                    bExit = true;
                    break;

                case 1:
                    _ = Actions.QuadraticEquation();
                    Terminal.Pause();
                    break;

                default:
                    Console.WriteLine("Invalid action.");
                    Terminal.Pause();
                    break;
            }
        }

        PostExitCleanup();
        return 0;
    }
}
