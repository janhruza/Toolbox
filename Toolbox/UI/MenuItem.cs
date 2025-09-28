using System.Collections.Generic;

namespace Toolbox.UI;

/// <summary>
/// Representing a single terminal menu item.
/// </summary>
public class MenuItem
{
    /// <summary>
    /// Creates a new <see cref="MenuItem"/> with no parameters.
    /// No parameters means that this itemm will be initialized as a separator.
    /// </summary>
    public MenuItem()
    {
        this.Id = 0x1000;
        this.Text = new string('―', Constants.MENU_ITEM_WIDTH); // horizontal box drawing character
    }

    /// <summary>
    /// Creates a new <see cref="MenuItem"/> with given parameters.
    /// </summary>
    /// <param name="id">Item's ID - used if the item is selected.</param>
    /// <param name="text">Display text.</param>
    public MenuItem(int id, string text)
    {
        this.Id = id;
        this.Text = text;
    }

    /// <summary>
    /// Creates a new <see cref="MenuItem"/> with given parameters.
    /// </summary>
    /// <param name="id">Item's ID - used if the item is selected.</param>
    /// <param name="text">Display text.</param>
    /// <param name="alt">Alternative item text - aligned to the right side.</param>
    public MenuItem(int id, string text, string alt)
    {
        this.Id = id;

        // trim text if needed
        if (alt.Length > Constants.MENU_ALT_MAX_SIZE)
        {
            alt = alt.Substring(0, Constants.MENU_ALT_MAX_SIZE - 3) + "...";
        }

        int availableWidth = Constants.MENU_ITEM_WIDTH - alt.Length;
        string paddedText = text.Length > availableWidth
            ? text.Substring(0, availableWidth)
            : text.PadRight(availableWidth);

        this.Text = paddedText + alt;
    }

    /// <summary>
    /// Representing the item ID.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Representing the display text.
    /// </summary>
    public string Text { get; }
}

/// <summary>
/// Representing a menu item collection.
/// </summary>
public class MenuItemCollection : List<MenuItem>;
