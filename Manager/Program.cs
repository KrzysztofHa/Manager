using Manager.App;
using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers;
using Manager.Consol.Concrete;
using Manager.Helpers;
internal class Program
{
    public static void Main()
    {
        IPlayerService playerService = new PlayerService();
        IUserService userService = new UserService();

        if (string.IsNullOrEmpty(userService.GetDisplayUserName()))
        {
            new InitializeUser(userService);
        }
        var settings = new Settings();
        MenuActionService actionService = new();
        PlayerManager playerManager = new(actionService, playerService, userService);
        SparringManager sparringManager = new(actionService, playerManager, userService, playerService);        
        TurnamentsManager turnamentsManager = new(actionService, playerManager, userService, playerService);
        
        

        var mainMenu = actionService.GetMenuActionsByName("Main");

        while (true)
        {
            ConsoleService.WriteTitle($"Hello User {userService.GetDisplayUserName()}!");
            for (int i = 0; i < mainMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {mainMenu[i].Name}");
            }
            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    playerManager.PlayerOptionView();
                    break;
                case 2:
                    //Global Ranking
                    break;
                case 3:
                    sparringManager.SparringOptionView();
                    break;
                case 4:
                    turnamentsManager.SparringOptionView();
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
                        ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                    }
                    break;
            }

            if (operation == null)
            {
                if (ConsoleService.AnswerYesOrNo("Are you sure you want to Exit?"))
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}