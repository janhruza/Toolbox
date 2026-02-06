using System;
using System.Linq;

using Toolbox;
using Toolbox.UI;

namespace ToolboxTest;

internal class Program
{
    private static int Main(string[] args)
    {
        int idx = 1;
        MenuItemCollection menu = new MenuItemCollection
        {
            new MenuItem(idx++, "Coffee"),
            new MenuItem(idx++, "Tea"),
            new MenuItem(idx++, "Pork"),
            new MenuItem(idx++, "Pasta"),
            new MenuItem(),
            new MenuItem(0, "Exit")
        };

        int[] result = ConsoleMenu.Multiselect(menu, "SHOPPING LIST");

        if (result.Length == 0)
        {
            Console.WriteLine("No items selected.");
        }

        else
        {
            Console.WriteLine($"  ID\tValue");
            Console.WriteLine($"  --\t-----");

            MenuItem mi;
            foreach (int id in result)
            {
                mi = menu.Where(x => x.Id == id).First();
                Console.WriteLine($"  {id.ToString("00")}\t{mi.GetTextWithoutAlt()}");
            }

            Console.WriteLine($"\nTotal: {Terminal.AccentTextStyle}{result.Length}{ANSI.ANSI_RESET} items.");
        }

        return 0;
    }
}
