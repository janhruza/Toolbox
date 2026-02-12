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
        MenuItemCollection menu = new()
        {
            new MenuItem(id: idx++, text: "Coffee"),
            new MenuItem(id: idx++, text: "Tea"),
            new MenuItem(id: idx++, text: "Pork"),
            new MenuItem(id: idx++, text: "Pasta"),
            new MenuItem(),
            new MenuItem(id: 0, text: "Exit")
        };

        int[] result = ConsoleMenu.Multiselect(menu: menu, header: "SHOPPING LIST");

        if (result.Length == 0)
        {
            Console.WriteLine(value: "No items selected.");
        }

        else
        {
            Console.WriteLine(value: "  ID\tValue");
            Console.WriteLine(value: "  --\t-----");

            MenuItem mi;
            foreach (int id in result)
            {
                mi = menu.Where(predicate: x => x.Id == id).First();
                Console.WriteLine(value: $"  {id.ToString(format: "00")}\t{mi.GetTextWithoutAlt()}");
            }

            Console.WriteLine(value: $"\nTotal: {Terminal.AccentTextStyle}{result.Length}{ANSI.ANSI_RESET} items.");
        }

        return 0;
    }
}