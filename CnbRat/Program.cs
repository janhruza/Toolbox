using System;

internal class Program
{
    private static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            Console.Error.WriteLine("Arguments are disabled.");
            return 1;
        }

        return 0;
    }
}