using Manager.App;
using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Concrete;
using Manager.App.Managers;
using Manager.Consol.Abstract;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using Manager.Helpers;
internal class Program
{
    public static void Main()
    {
        

        
        IPlayerService playerService = new PlayerService();
        IConsoleService consoleService = new ConsoleService();
        IUserService userService = new UserService();

        if (string.IsNullOrEmpty(userService.GetDisplayUserName()) )
        {
            new InitializeUser(userService, consoleService);
        }
        var settings = new Settings();
        
        MenuActionService actionService = new();
        PlayerManager playerManager = new(actionService, playerService, consoleService);

        var mainMenu = actionService.GetMenuActionsByName("Main");
        bool wrongOperation = false;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"\r\n");
            Console.WriteLine($"     Hello User {userService.GetDisplayUserName}!\n");

            if (wrongOperation)
            {
                wrongOperation = false;
                Console.WriteLine("Action You entered does not exist\n");
            }
            else
            {
                Console.WriteLine($"Press number key in Main menu options\n");
            }

            for (int i = 0; i < mainMenu.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {mainMenu[i].Name}");
            }

            Console.WriteLine("\n\n   Press Esc to Exit");
            var operation = Console.ReadKey(true);

            switch (operation.KeyChar)
            {
                case '1':
                    playerManager.PlayerOptionView();
                    break;
                case '2':
                    //Global Ranking
                    break;
                case '3':
                    //Sparing
                    break;
                case '4':
                    //Tournaments
                    break;
                case '5':
                    settings.ChangeSettings();
                    break;
                case '6':
                    Environment.Exit(0);
                    break;
                default:

                    wrongOperation = true;
                    break;
            }

            if (operation.Key == ConsoleKey.Escape)
            {
                Environment.Exit(0);
            }
        }
    }
}