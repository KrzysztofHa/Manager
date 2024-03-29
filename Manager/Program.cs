using Manager.App.Concrete;
using Manager.App.Managers;
using Manager.Helpers;
internal class Program
{
    private static void Main(string[] args)
    {
        new LogIn();
        MenuActionService actionService = new MenuActionService();
        PleyerManager pleyerManager = new PleyerManager(actionService);
        var mainMenu = actionService.GetMenuActionsByName("Main");
        bool noOperation = false;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Hello User {LogIn.UserName}!\n\n");

            if (noOperation)
            {
                noOperation = false;
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
                    var newId = pleyerManager.AddNewPleyer();
                    break;
                case '2':
                    pleyerService.ListOfPleyersView();
                    break;
                case '3':
                    pleyerService.ListOfPleyersView();
                    var removeId = pleyerService.RemovePleyerView();
                    pleyerService.RemovePleyer(removeId);
                    pleyerService.ListOfPleyersView();
                    break;
                case '4':
                    pleyerService.ListOfPleyersView();
                    var updateId = pleyerService.UpdatePleyerView();
                    pleyerService.UpdatePleyer(updateId);
                    pleyerService.ListOfPleyersView();
                    break;
                case '5':
                    break;
                case '6':
                    break;
                default:

                    noOperation = true;
                    break;
            }

            if (operation.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }


}