using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

using static Toolbox.ANSI;

namespace Toolbox.UI;

/// <summary>
/// Representing the terminal menu handler class.
/// </summary>
public static class ConsoleMenu
{
    /// <summary>
    /// Representing a value when the menu is escaped using the ESC key.
    /// </summary>
    public const int KEY_ESCAPE = -1;

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
        DrawMenu(items, idx, string.Empty, string.Empty);
        return;
    }

    // Definujeme statickou proměnnou mimo metodu, aby si aplikace pamatovala, kde menu začíná
    private static int _menuStartLine = -1;

    /// <summary>
    /// Draws the console menu to the screen.
    /// </summary>
    /// <param name="items">Menu items collection.</param>
    /// <param name="idx">Currently selected menu item (index).</param>
    /// <param name="header">Menu header text.</param>
    /// <remarks>
    /// This method will only draws the console menu and no user input is required.
    /// </remarks>
    public static void DrawMenu(MenuItemCollection items, int idx, string header)
    {
        DrawMenu(items, idx, header, string.Empty);
    }

    /// <summary>
    /// Draws the console menu to the screen.
    /// </summary>
    /// <param name="items">Menu items collection.</param>
    /// <param name="idx">Currently selected menu item (index).</param>
    /// <param name="header">Menu header text.</param>
    /// <param name="helpText">Additional help text.</param>
    /// <remarks>
    /// This method will only draws the console menu and no user input is required.
    /// </remarks>
    public static void DrawMenu(MenuItemCollection items, int idx, string header, string helpText)
    {
        // the screen was probably cleared, adjust the start position
        if (Console.CursorTop < _menuStartLine)
        {
            _menuStartLine = -1;
        }

        if (_menuStartLine == -1)
        {
            _menuStartLine = Console.CursorTop;
        }
        else
        {
            Console.SetCursorPosition(0, _menuStartLine);
        }

        int reservedSpace = 10;
        int pageSize = Math.Max(5, Console.WindowHeight - (_menuStartLine + reservedSpace));
        if (items.Count < pageSize) pageSize = items.Count;

        int startIdx = Math.Max(0, Math.Min(idx - pageSize / 2, items.Count - pageSize));
        int endIdx = Math.Min(startIdx + pageSize, items.Count);

        if (string.IsNullOrWhiteSpace(header)) header = "MENU";

        string leftOffset = "    "; // 4 spaces
        int maxWidth = Console.WindowWidth - 1;

        void WriteCleanLine(string formattedText)
        {
            Console.Write(formattedText);
            // removal of the ANSI to calculate the actual width
            string plainText = System.Text.RegularExpressions.Regex.Replace(formattedText, @"\e\[[0-9;]*m", "");
            int paddingCount = maxWidth - plainText.Length;
            if (paddingCount > 0) Console.Write(new string(' ', paddingCount));
            Console.WriteLine();
        }

        // --- Drawing ---

        // Header
        WriteCleanLine($"  {leftOffset}{Terminal.AccentTextStyle}{header}{ANSI.ANSI_RESET}");

        // Top separator (right under the menu header) - horizontal line
        string topSeparator = startIdx > 0
            ? "▲ ... more above ..."
            : new string('―', Constants.MENU_ITEM_WIDTH);
        WriteCleanLine($"{leftOffset}  {topSeparator}");

        // Menu items itself
        for (int x = startIdx; x < endIdx; x++)
        {
            // text left-padding by 2 places
            string innerText = $"  {items[x].Text.PadRight(Constants.MENU_ITEM_WIDTH)}  ";
            string line = (x == idx)
                ? $"{leftOffset}{Terminal.AccentHighlightStyle}{innerText}{ANSI_RESET}"
                : $"{leftOffset}\e[0m{innerText}{ANSI_RESET}";

            WriteCleanLine(line);
        }

        // The bottom separator - horizontal line
        string bottomSeparator = endIdx < items.Count
            ? "▼ ... more bellow ..."
            : new string('-', Constants.MENU_ITEM_WIDTH);
        WriteCleanLine($"{leftOffset}  {bottomSeparator}");

        WriteCleanLine("");
        string infoText = $"{leftOffset}Use arrows to navigate (item {idx + 1} out of {items.Count})";
        WriteCleanLine($"{Terminal.Colors.GrayText}{infoText}{ANSI_RESET}");

        // --- Pre-Logging Events ---

        int logRow;

        if (string.IsNullOrWhiteSpace(helpText) == false)
        {
            // adjust the log line position
            logRow = _menuStartLine + pageSize + 8;

            // draw the jelper text
            WriteCleanLine($"{Terminal.Colors.GrayText}{leftOffset}{helpText}{ANSI_RESET}");
        }

        else
        {
            logRow = _menuStartLine + pageSize + 6;
        }

        // --- Logs ---
        if (logRow < Console.BufferHeight)
        {
            Console.SetCursorPosition(0, logRow);
            Terminal.WriteLastLogEntry();
            int currentPos = Console.CursorLeft;
            if (currentPos < maxWidth) Console.Write(new string(' ', maxWidth - currentPos));
            Console.CursorLeft = 0; // quick fix
        }
    }

    /// <summary>
    /// Handles the menu item selection action.
    /// </summary>
    /// <param name="items">Menu item collection.</param>
    /// <returns>The ID of the selected menu item.</returns>
    public static int SelectMenu(MenuItemCollection items)
    {
        return SelectMenu(items, string.Empty);
    }

    /// <summary>
    /// Handles the menu item selection action.
    /// </summary>
    /// <param name="items">Menu item collection.</param>
    /// <param name="header">Menu header text.</param>
    /// <returns>The ID of the selected menu item.</returns>
    public static int SelectMenu(MenuItemCollection items, string header)
    {
        try
        {
            int top, left, index, ntop, nleft, selected;
            index = 0;
            top = Console.CursorTop;
            left = Console.CursorLeft;

            Console.CursorVisible = false;
            while (true)
            {
                DrawMenu(items, index, header);
                var key = Console.ReadKey(true).Key;
                (nleft, ntop) = Console.GetCursorPosition();

                switch (key)
                {
                    case ConsoleKey.Escape:
                        return KEY_ESCAPE;

                    case ConsoleKey.UpArrow:
                        index = (index > 0 && index < items.Count) ? --index : items.Count - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        index = (index < items.Count - 1) ? ++index : 0;
                        break;

                    case ConsoleKey.Enter:
                        selected = items[index].Id;

                        if (selected == MenuItem.ID_SEPARATOR)
                        {
                            // ignore selection of separator
                            break;
                        }

                        goto Exit;

                    default:
                        break;
                }

                Console.SetCursorPosition(left, top);
            }

        Exit:
            Console.CursorVisible = true;
            Console.SetCursorPosition(nleft, ntop);
            return selected;
        }

        catch (Exception ex)
        {
            Log.Exception(ex);
            return 0xDEAD;
        }

        finally
        {
            Console.CursorVisible = true;
        }
    }

    /// <summary>
    /// Allows users to select multiple menu items at once by selecting them.
    /// </summary>
    /// <param name="menu">List of available options.</param>
    /// <param name="header">Specifies the caption of the menu.</param>
    /// <param name="helpText">Additional helper text.</param>
    /// <returns>Array of the selected menu item indexes. Items are returned in the order of selection.</returns>
    /// <remarks>
    /// This method overwrites the content of the menu items' <see cref="MenuItem.Text"/>
    /// property using the <see cref="MenuItem.Update(string, string)"/>
    /// method to indicate selected/unselected items.
    /// </remarks>
    public static int[] Multiselect(MenuItemCollection menu, string header, string helpText)
    {
        // check for empty menu
        if (menu.Count == 0) return [];

        const string ALT_SELECTED = "[*]", ALT_DESELECTED = "[ ]";

        // the list of selected items
        List<int> items = [];

        // helper function to (de)select item(s)
        void SelectItem(int selectedId)
        {
            MenuItem mi = menu.Where(x => x.Id == selectedId).First();

            // update the state of the item
            if (items.Contains(selectedId) == true)
            {
                items.Remove(selectedId);
                mi.Update(mi.GetTextWithoutAlt(), ALT_DESELECTED);
            }

            else
            {
                items.Add(selectedId);
                mi.Update(mi.GetTextWithoutAlt(), ALT_SELECTED);
            }
        }

        try
        {
            int top, left, index, ntop, nleft, selected;
            index = 0;
            top = Console.CursorTop;
            left = Console.CursorLeft;

            // prepare all selectable items
            foreach (MenuItem mi in menu.Where(x => x.Id != MenuItem.ID_EXIT && x.Id != MenuItem.ID_SEPARATOR))
            {
                mi.Update(mi.GetTextWithoutAlt(), ALT_DESELECTED);
            }

            Console.CursorVisible = false;
            while (true)
            {
                DrawMenu(menu, index, header, helpText);
                var key = Console.ReadKey(true).Key;
                (nleft, ntop) = Console.GetCursorPosition();

                switch (key)
                {
                    case ConsoleKey.Escape:
                        goto Exit;

                    case ConsoleKey.UpArrow:
                        index = (index > 0 && index < menu.Count) ? --index : menu.Count - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        index = (index < menu.Count - 1) ? ++index : 0;
                        break;

                    case ConsoleKey.Enter:
                        {
                            selected = menu[index].Id;

                            switch (selected)
                            {
                                case MenuItem.ID_SEPARATOR:
                                    break;

                                case MenuItem.ID_EXIT:
                                    goto Exit;

                                default:
                                    SelectItem(selected);
                                    break;
                            }
                        }
                        break;

                    // select/deselect the menu item
                    case ConsoleKey.Spacebar:
                        selected = menu[index].Id;

                        if (selected == MenuItem.ID_SEPARATOR || selected == MenuItem.ID_EXIT)
                        {
                            // ignore selection of separator
                            break;
                        }

                        // (de)select the target item
                        SelectItem(selected);

                        break;

                    default:
                        break;
                }

                Console.SetCursorPosition(left, top);
            }

        Exit:
            Console.CursorVisible = true;
            Console.SetCursorPosition(nleft, ntop);
            return items.ToArray();
        }

        catch (Exception ex)
        {
            Log.Exception(ex);
            return [];
        }

        finally
        {
            Console.CursorVisible = true;
        }
    }

    /// <summary>
    /// Allows users to select multiple menu items at once by selecting them.
    /// </summary>
    /// <param name="menu">List of available options.</param>
    /// <returns>Array of the selected menu item indexes. Items are returned in the order of selection.</returns>
    /// <remarks>
    /// This method overwrites the content of the menu items' <see cref="MenuItem.Text"/>
    /// property using the <see cref="MenuItem.Update(string, string)"/>
    /// method to indicate selected/unselected items.
    /// </remarks>
    public static int[] Multiselect(MenuItemCollection menu)
    {
        return Multiselect(menu, string.Empty, "Use spacebar to select/deselect items.");
    }

    /// <summary>
    /// Allows users to select multiple menu items at once by selecting them.
    /// </summary>
    /// <param name="menu">List of available options.</param>
    /// <param name="header">Specifies the caption of the menu.</param>
    /// <returns>Array of the selected menu item indexes. Items are returned in the order of selection.</returns>
    /// <remarks>
    /// This method overwrites the content of the menu items' <see cref="MenuItem.Text"/>
    /// property using the <see cref="MenuItem.Update(string, string)"/>
    /// method to indicate selected/unselected items.
    /// </remarks>
    public static int[] Multiselect(MenuItemCollection menu, string header)
    {
        return Multiselect(menu, header, "Use spacebar/enter to select/deselect items.");
    }
}
