using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Manager.Consol.Concrete;

public static class ConsoleService
{
    public static string SizeWindowAllert = "The application window is too small, \n\rwhich makes it difficult to use. "
                + "\n\rChange the size of the application window, \n\rminimum width 120 and height 30 " +
                "\n\ror you can close the window exit the application \n\rand the changes made may not be saved.\n\r";

    public static bool CheckAndSetSizeWindow()
    {
        if (Console.WindowHeight < 30 || Console.WindowWidth < 120)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Clear();
            Console.WriteLine(SizeWindowAllert);
            var startCursorPosition = Console.GetCursorPosition();
            Console.CursorVisible = false;
            do
            {
                Thread.Sleep(200);
                if (startCursorPosition.Left + 5 >= Console.BufferWidth || startCursorPosition.Top + 5 >= Console.BufferHeight)
                {
                    Console.WriteLine("Window is Too small exit aplication");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Press Any Key...");
                    Console.ReadKey();
                    System.Environment.Exit(0);
                }
                Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
                Console.Write(string.Empty.PadRight(Console.BufferWidth));
                Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
                Console.ForegroundColor = ConsoleColor.Red;
                if (Console.WindowHeight >= 30)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"Height: {Console.WindowHeight}");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.Write($"Height: {Console.WindowHeight}");
                }

                if (Console.WindowWidth >= 120)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write($"  Width: {Console.WindowWidth}");
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"  Width: {Console.WindowWidth}");
                }

                if (Console.WindowHeight >= 30 && Console.WindowWidth >= 120)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top + 3);
                    Console.Write("The window size is just right." +
                        "\n\rYou can continue working with the application. \n\rPress Any Key...\n\r And Have fun!");
                    Console.ReadKey();
                }
            } while (Console.WindowHeight < 30 || Console.WindowWidth < 120);

            Console.Write(string.Empty.PadRight(Console.BufferWidth));
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = true;
            return false;
        }

        return true;
    }

    public static int? GetIntNumberFromUser(string message, string messageBack = " ")
    {
        if (!CheckAndSetSizeWindow())
        {
            return null;
        }

        if (int.TryParse(GetStringFromUser(message, messageBack), out int inputUserInt))
        {
            return inputUserInt;
        }
        return null;
    }

    public static string GetStringFromUser(string message, string messageBack = " ")
    {
        if (!CheckAndSetSizeWindow())
        {
            return null;
        }
        ConsoleKeyInfo inputKey;
        var inputString = new StringBuilder();
        Console.WriteLine($"{message}\n\r");
        int customCursorLeft = Console.CursorLeft;
        int customCursorTop = Console.CursorTop;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n\rExit Press Escape (Esc)");
        var endBuferPosition = Console.GetCursorPosition();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(messageBack);

        do
        {
            Console.SetCursorPosition(customCursorLeft, customCursorTop);
            Console.Write(inputString.ToString());

            inputKey = Console.ReadKey(true);

            if (char.IsLetterOrDigit(inputKey.KeyChar))
            {
                if (string.IsNullOrEmpty(inputString.ToString()))
                {
                    var upperInputKey = inputKey.KeyChar.ToString().ToUpper();
                    inputString.Append(upperInputKey);
                }
                else
                {
                    inputString.Append(inputKey.KeyChar);
                }
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

    public static string GetRequiredStringFromUser(string message, string messageBack = " ")
    {
        if (!CheckAndSetSizeWindow())
        {
            return null;
        }

        var startCursorPosition = Console.GetCursorPosition();

        do
        {
            var inputRequiredString = GetStringFromUser(message, messageBack);
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

    public static void WriteLineMessage(string message)
    {
        if (!CheckAndSetSizeWindow())
        {
            return;
        }

        Console.WriteLine(message);
    }

    public static bool AnswerYesOrNo(string message)
    {
        CheckAndSetSizeWindow();
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

    public static void WriteTitle(string title)
    {
        CheckAndSetSizeWindow();
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{title}\n");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void WriteLineMessageActionSuccess(string message)
    {
        CheckAndSetSizeWindow();
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

    public static void WriteLineErrorMessage(string errorMessage)
    {
        CheckAndSetSizeWindow();
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Red;
        var startCursorPosition = Console.GetCursorPosition();
        Console.WriteLine(errorMessage);
        Thread.Sleep(250);
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.WriteLine(string.Empty.PadLeft(Console.BufferWidth));
        Thread.Sleep(250);
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.WriteLine(errorMessage);
        Thread.Sleep(250);
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.WriteLine(string.Empty.PadLeft(Console.BufferWidth));
        Thread.Sleep(250);
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.WriteLine(errorMessage);
        Thread.Sleep(1300);
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.WriteLine(string.Empty.PadLeft(Console.BufferWidth));
        while (Console.KeyAvailable == true)
        {
            Console.ReadKey(true);
        }
        Console.CursorVisible = true;
    }

    public static ConsoleKeyInfo GetKeyFromUser(string messageBack = " ")
    {
        CheckAndSetSizeWindow();

        var startCursorPosition = Console.GetCursorPosition();
        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n\r" + messageBack);
        Console.SetCursorPosition(startCursorPosition.Left, startCursorPosition.Top);
        Console.WriteLine(string.Empty.PadLeft(Console.BufferWidth));
        Console.CursorVisible = false;
        return Console.ReadKey(true);
    }

    public static void WriteMessage(string message)
    {
        Console.Write(message);
    }
}