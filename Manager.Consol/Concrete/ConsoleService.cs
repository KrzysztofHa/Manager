using Manager.Consol.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Consol.Concrete;

public class ConsoleService : IConsoleService
{
    public int? GetIntNumberFromUser(string message)
    {
        if(int.TryParse(GetStringFromUser(message), out int inputUserInt))
        { return inputUserInt; }
         
        return null;        
    }

    public string GetStringFromUser(string message)
    {
        ConsoleKeyInfo inputKey;
        var inputString = new StringBuilder();
        Console.CursorVisible = false;
        do
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{message}:\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(inputString.ToString());
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nExit Press Escape (Esc) or Enter");
            inputKey = Console.ReadKey(true);

            if (char.IsLetterOrDigit(inputKey.KeyChar))
            {
                inputString.Append(inputKey.KeyChar);                
            }
            else if (inputKey.Key == ConsoleKey.Backspace && inputString.Length > 0)
            {
                inputString.Remove(inputString.Length - 1, 1);                
            }

            if (inputKey.Key == ConsoleKey.Escape)
            {
                Console.CursorVisible = true;
                return string.Empty;
            }
        }
        while (inputKey.Key != ConsoleKey.Enter);

        Console.CursorVisible = true;
        return inputString.ToString();
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
        Console.WriteLine($"{title.PadRight(20)}\n");
        Console.ForegroundColor = ConsoleColor.White;
    }
}
