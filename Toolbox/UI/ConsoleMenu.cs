using System;

using static Toolbox.ANSI;

namespace Toolbox.UI;

/// <summary>
/// Representing the terminal menu handler class.
/// </summary>
public static class ConsoleMenu
{
    /// <summary>
    /// Draws the console menu to the screen.
    /// </summary>
    /// <param name="items">Menu items collection.</param>
    /// <param name="idx">Currently selected menu item (index).</param>
    /// <remarks>
    /// This method will only draws the console menu and no user input is required.
    /// </remarks>
    public static void DrawMenu(MenuItemCollection items, int idx)
    {
        for (int x = 0; x < items.Count; x++)
        {
            Console.Write($"\t");

            if (x == idx)
            {
                // item is selected
                Console.WriteLine($"{Terminal.AccentHighlightStyle}  {items[x].Text,-Constants.MENU_ITEM_WIDTH}  {ANSI_RESET}");
            }

            else
            {
                // item is not selected
                Console.WriteLine($"\e[0m  {items[x].Text,-Constants.MENU_ITEM_WIDTH}  {ANSI_RESET}");
            }
        }

        Console.WriteLine($"\n\t\e[38;5;243mUse arrow up/down to navigate.{ANSI_RESET}");

        // write last log entry to the screen
        Terminal.WriteLastLogEntry();
        return;
    }

    /// <summary>
    /// Handles the menu item selection action.
    /// </summary>
    /// <param name="items">Menu item collection.</param>
    /// <returns>The ID of the selected menu item.</returns>
    public static int SelectMenu(MenuItemCollection items)
    {
        int top, left, index, ntop, nleft, selected;
        index = 0;
        top = Console.CursorTop;
        left = Console.CursorLeft;

        while (true)
        {
            DrawMenu(items, index);
            var key = Console.ReadKey(true).Key;
            (nleft, ntop) = Console.GetCursorPosition();

            switch (key)
            {
                case ConsoleKey.Escape:
                    return -1;

                case ConsoleKey.UpArrow:
                    index = (index > 0 && index < items.Count) ? --index : items.Count - 1;
                    break;

                case ConsoleKey.DownArrow:
                    index = (index < items.Count - 1) ? ++index : 0;
                    break;

                case ConsoleKey.Enter:
                    selected = items[index].Id;
                    goto Exit;

                default:
                    break;
            }

            Console.SetCursorPosition(left, top);
        }

    Exit:
        Console.SetCursorPosition(nleft, ntop);
        return selected;
    }
}
