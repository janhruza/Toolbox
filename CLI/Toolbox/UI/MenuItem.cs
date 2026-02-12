using System.Collections.Generic;

namespace Toolbox.UI;

/// <summary>
///     Representing a single terminal menu item.
/// </summary>
public class MenuItem
{
    /// <summary>
    ///     Representing the ID of a separator item.
    /// </summary>
    public const int ID_SEPARATOR = 0x1000;

    /// <summary>
    ///     Representing the ID of an exit/back option.
    ///     This value is reserved.
    /// </summary>
    public const int ID_EXIT = 0x0000;

    /// <summary>
    ///     Representing a pointer to the extra data attached to this object.
    /// </summary>
    public object Extras = new();

    private string sAlt = string.Empty;

    private string sText = string.Empty;

    /// <summary>
    ///     Creates a new <see cref="MenuItem" /> with no parameters.
    ///     No parameters means that this itemm will be initialized as a separator.
    /// </summary>
    public MenuItem()
    {
        this.Id = MenuItem.ID_SEPARATOR;
        this.Text = new string(c: '―', count: Constants.MENU_ITEM_WIDTH); // horizontal box drawing character
    }

    /// <summary>
    ///     Creates a new <see cref="MenuItem" /> with given parameters.
    /// </summary>
    /// <param name="id">Item's ID - used if the item is selected.</param>
    /// <param name="text">Display text.</param>
    public MenuItem(int id, string text)
    {
        this.Id = id;
        this.Text = text;

        // initialize private properties
        this.sText = text;
    }

    /// <summary>
    ///     Creates a new <see cref="MenuItem" /> with given parameters.
    /// </summary>
    /// <param name="id">Item's ID - used if the item is selected.</param>
    /// <param name="text">Display text.</param>
    /// <param name="alt">Alternative item text - aligned to the right side.</param>
    public MenuItem(int id, string text, string alt)
    {
        this.Id = id;
        this.Text = string.Empty;
        this.Update(text: text, alt: alt);
    }

    /// <summary>
    ///     Representing the item ID.
    /// </summary>
    public int Id { get; }

    /// <summary>
    ///     Representing the display text.
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    ///     Updates the content of this <see cref="MenuItem" /> object.
    /// </summary>
    /// <param name="text">New <see cref="Text" /> value.</param>
    /// <param name="alt">New alternative text value.</param>
    public void Update(string text, string alt)
    {
        // trim text if needed
        if (alt.Length > Constants.MENU_ALT_MAX_SIZE) alt = alt[..(Constants.MENU_ALT_MAX_SIZE - 3)] + "...";

        int availableWidth = Constants.MENU_ITEM_WIDTH - alt.Length;
        string paddedText = text.Length > availableWidth
            ? text[..availableWidth]
            : text.PadRight(totalWidth: availableWidth);

        this.Text = paddedText + alt;

        this.sText = text;
        this.sAlt = alt;
    }

    /// <summary>
    ///     Returns only the main text part.
    /// </summary>
    /// <returns>Main text of the item.</returns>
    public string GetTextWithoutAlt()
    {
        return this.sText;
    }

    /// <summary>
    ///     Returns only the alternative text.
    /// </summary>
    /// <returns>Alternative text of the item.</returns>
    public string GetAltText()
    {
        return this.sAlt;
    }
}

/// <summary>
///     Representing a menu item collection.
/// </summary>
public class MenuItemCollection : List<MenuItem>;