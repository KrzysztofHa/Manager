using Manager.App;
using Manager.App.Abstract;
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

        if (string.IsNullOrEmpty(userService.GetDisplayUserName()))
        {
            new InitializeUser(userService, consoleService);
        }
        var settings = new Settings();
        MenuActionService actionService = new();
        PlayerManager playerManager = new(actionService, playerService, consoleService, userService);

        var mainMenu = actionService.GetMenuActionsByName("Main");

        while (true)
        {
            consoleService.WriteTitle($"Hello User {userService.GetDisplayUserName()}!");
            for (int i = 0; i < mainMenu.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {mainMenu[i].Name}");
            }
            var operation = consoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    playerManager.PlayerOptionView();
                    break;
                case 2:
                    //Global Ranking
                    break;
                case 3:
                    //Sparing
                    break;
                case 4:
                    //Tournaments
                    break;
                case 5:
                    settings.ChangeSettings();
                    break;
                case 6:
                    operation = null;
                    break;
                default:
                    if (operation != null)
                    {
                        consoleService.WriteLineErrorMessage("Enter a valid operation ID");
                    }
                    break;
            }

            if (operation == null)
            {
                if (consoleService.AnswerYesOrNo("Are you sure you want to Exit?"))
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}