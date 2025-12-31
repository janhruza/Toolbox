using System.Collections.Generic;
using System.Runtime.CompilerServices;

using static System.Net.Mime.MediaTypeNames;

namespace Toolbox.UI;

/// <summary>
/// Representing a single terminal menu item.
/// </summary>
public class MenuItem
{
    /// <summary>
    /// Representing the ID of a separator item.
    /// </summary>
    public const int ID_SEPARATOR = 0x1000;

    /// <summary>
    /// Creates a new <see cref="MenuItem"/> with no parameters.
    /// No parameters means that this itemm will be initialized as a separator.
    /// </summary>
    public MenuItem()
    {
        this.Id = ID_SEPARATOR;
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

        // initialize private properties
        sText = text;
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
        this.Text = string.Empty;
        this.Update(text, alt);
    }

    /// <summary>
    /// Representing the item ID.
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Representing the display text.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// Representing a pointer to the extra data attached to this object.
    /// </summary>
    public object Extras = new object();

    private string sText = string.Empty;
    private string sAlt = string.Empty;

    /// <summary>
    /// Updates the content of this <see cref="MenuItem"/> object.
    /// </summary>
    /// <param name="text">New <see cref="Text"/> value.</param>
    /// <param name="alt">New alternative text value.</param>
    public void Update(string text, string alt)
    {
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

        sText = text;
        sAlt = alt;
        return;
    }

    /// <summary>
    /// Returns only the main text part.
    /// </summary>
    /// <returns>Main text of the item.</returns>
    public string GetTextWithoutAlt()
    {
        return sText;
    }

    /// <summary>
    /// Returns only the alternative text.
    /// </summary>
    /// <returns>Alternative text of the item.</returns>
    public string GetAltText()
    {
        return sAlt;
    }
}

/// <summary>
/// Representing a menu item collection.
/// </summary>
public class MenuItemCollection : List<MenuItem>;
