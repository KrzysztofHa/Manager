using Manager.Consol.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Consol.Concrete;

public class ConsoleService : IConsoleService
{
    public int GetIntNumberFromUser(string message)
    {

        throw new NotImplementedException();
    }

    public string GetStringFromUser(string message)
    {
        ConsoleKeyInfo cki;

        do
        {
            Console.WriteLine("\nPress a key to display; press the 'x' key to quit.");

            // Your code could perform some useful task in the following loop. However,
            // for the sake of this example we'll merely pause for a quarter second.

            while (Console.KeyAvailable == false)
                Thread.Sleep(250); // Loop until input is entered.

            cki = Console.ReadKey(true);
            Console.WriteLine("You pressed the '{0}' key.", cki.Key);
        } while (cki.Key != ConsoleKey.X);

        throw new NotImplementedException();
    }

    public void WriteErrorMesage(string errorMessage)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(errorMessage);
        Console.ForegroundColor = ConsoleColor.White;
    }

    public void WriteMesage(string message)
    {
        Console.WriteLine(message);
    }

    public void WriteTitle(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("      " + title);
        Console.ForegroundColor = ConsoleColor.White;
    }
}
