using Toolbox.UI;

namespace ToolboxTest;

internal class Program
{
    static int Main(string[] args)
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
            foreach (int id in result)
            {
                Console.WriteLine($"Item {id} selected.");
            }
        }

        return 0;
    }
}
