using Manager.Consol.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Consol.Concrete;

public class ConsoleService : IConsoleService
{
    public int? GetIntNumberFromUser(string message)
    {
        if (int.TryParse(GetStringFromUser(message), out int inputUserInt))
        {
            return inputUserInt;
        }
        return null;
    }
    public string GetStringFromUser(string message)
    {
        ConsoleKeyInfo inputKey;
        var inputString = new StringBuilder();
        Console.WriteLine($"{message}\n");
        int customCursorLeft = Console.CursorLeft;
        int customCursorTop = Console.CursorTop;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\nExit Press Escape (Esc)");
        var endBuferPosition = Console.GetCursorPosition();
        Console.ForegroundColor = ConsoleColor.Yellow;

        do
        {
            Console.SetCursorPosition(customCursorLeft, customCursorTop);
            Console.Write(inputString.ToString());

            inputKey = Console.ReadKey(true);
            if (char.IsLetterOrDigit(inputKey.KeyChar))
            {
                inputString.Append(inputKey.KeyChar);
            }
            else if (inputKey.Key == ConsoleKey.Backspace && inputString.Length > 0)
            {
                inputString.Remove(inputString.Length - 1, 1);
                Console.SetCursorPosition(customCursorLeft, customCursorTop);
                Console.Write(inputString.ToString().PadRight(Console.BufferWidth));

            }

            if (inputKey.Key == ConsoleKey.Escape)
            {
                Console.SetCursorPosition(endBuferPosition.Left, endBuferPosition.Top);
                Console.ForegroundColor = ConsoleColor.White;
                return null;
            }
        }
        while (inputKey.Key != ConsoleKey.Enter);

        Console.SetCursorPosition(endBuferPosition.Left, endBuferPosition.Top);
        Console.ForegroundColor = ConsoleColor.White;
        return inputString.ToString();
    }

    public string GetRequiredStringFromUser(string message)
    {
        var startCursorPosition = Console.GetCursorPosition();
        do
        {
            var inputRequiredString = GetStringFromUser(message);
            if (inputRequiredString == string.Empty)
            {
                WriteLineErrorMessage("This field is required");
                if (AnswerYesOrNo("You want to exit? \r\nThe entered data will not be saved"))
                {
                    return null;
                }
            }
            else
            {
                return inputRequiredString;
            }
            Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);

        } while (true);
    }

    public void WriteLineMessage(string message)
    {
        Console.WriteLine(message);
    }

    public bool AnswerYesOrNo(string message)
    {
        var startCursorTop = Console.CursorTop;
        var startCursorLeft = Console.CursorLeft;
        Console.ForegroundColor = ConsoleColor.Magenta;
        WriteLineMessage(message);
        WriteLineMessage("press \"y\" to YES or \"n\" to NO");
        var endCursorTop = Console.CursorTop;

        do
        {
            var inputKey = Console.ReadKey();
            if (inputKey.Key == ConsoleKey.Y)
            {
                Console.SetCursorPosition(startCursorLeft, startCursorTop);
                for (int i = startCursorTop; i <= endCursorTop; i++)
                    Console.WriteLine("{0}", string.Empty.PadRight(Console.BufferWidth));
                Console.SetCursorPosition(startCursorLeft, startCursorTop);
                Console.ForegroundColor = ConsoleColor.White;
                return true;
            }
            else if (inputKey.Key == ConsoleKey.N)
            {
                Console.SetCursorPosition(startCursorLeft, startCursorTop);
                for (int i = startCursorTop; i <= endCursorTop; i++)
                    Console.WriteLine("{0}", string.Empty.PadRight(Console.BufferWidth));
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(startCursorLeft, startCursorTop);
                return false;
            }
        } while (true);
    }

    public void WriteTitle(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{title}\n");
        Console.ForegroundColor = ConsoleColor.White;
    }



    public void WriteLineMessageActionSuccess(string message)
    {
        var startCursorPosition = Console.GetCursorPosition();
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Cyan;
        for (int i = 0; i <= 1; i++)
        {
            Console.WriteLine(message);
            Thread.Sleep(250);
            Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
            Console.Write(string.Empty.PadLeft(Console.BufferWidth));
            Thread.Sleep(250);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        }
        Console.WriteLine(message);
        Console.ForegroundColor = ConsoleColor.White;
        Console.ReadKey();
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.Write(string.Empty.PadLeft(Console.BufferWidth));
        Console.CursorVisible = true;
    }

    public void WriteLineErrorMessage(string errorMessage)
    {
        var startCursorPosition = Console.GetCursorPosition();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(errorMessage);
        Thread.Sleep(250);
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.Write(string.Empty.PadLeft(Console.BufferWidth));
        Thread.Sleep(250);
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.WriteLine(errorMessage);
        Console.ForegroundColor = ConsoleColor.White;
        Console.CursorVisible = false;
        Thread.Sleep(1500);
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.Write(string.Empty.PadLeft(Console.BufferWidth));
        Console.CursorVisible = true;
    }
}
