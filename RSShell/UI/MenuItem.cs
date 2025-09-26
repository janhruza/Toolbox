using System.Collections.Generic;

namespace RSShell.UI;

/// <summary>
/// Representing a single terminal menu item.
/// </summary>
public class MenuItem
{
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
