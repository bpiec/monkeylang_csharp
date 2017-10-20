using System;

namespace Monkey
{
    internal static class Program
    {
        private static void Main()
        {
            var username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            Console.WriteLine($"Hello {username}! This is the Monkey programming language!");
            Console.WriteLine("Feel free to type in commands.");
            Console.WriteLine();

            var repl = new Repl.Repl();
            repl.Start(Console.In, Console.Out);
        }
    }
}