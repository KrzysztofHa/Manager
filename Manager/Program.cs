using Manager.App;
using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Concrete;
using Manager.App.Managers;
using Manager.Domain.Entity;
using Manager.Helpers;
internal class Program
{
    private static void Main()
    {
        new LogIn();
        MenuActionService actionService = new();
        IService<Player> playerService = new PlayerService();
        PlayerManager playerManager = new(actionService, playerService);

        var mainMenu = actionService.GetMenuActionsByName("Main");
        bool wrongOperation = false;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Hello User {LogIn.UserName}!\n\n");

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
                Console.WriteLine($"{mainMenu[i].Id}. {mainMenu[i].Name}");
            }

            Console.WriteLine("\n\n\n\n       Press Esc to Exit");
            var operation = Console.ReadKey(true);

            switch (operation.KeyChar)
            {
                case '1':
                    playerManager.PlayerOptionView();                    
                    break;
                case '2':
                    playerManager.ListOfPlayers();
                    Console.ReadKey();
                    break;
                case '3':
                    var removeId = playerManager.RemovePlayer();
                    break;
                case '4':
                    var updateId = playerManager.UpdatePlayer();
                    break;
                case '5':
                    break;
                case '6':
                    break;
                default:

                    wrongOperation = true;
                    break;
            }

            if (operation.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }
}