
using Manager;
using Manager.Helpers;
internal class Program
{
    private static void Main(string[] args)
    {
        new LogIn();
        PleyerService pleyerService = new PleyerService();
        MenuActionService actionService = new MenuActionService();
        Initialize(actionService);
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
                    ConsoleKeyInfo country = pleyerService.AddNewPleyerView(actionService);
                    pleyerService.AddNewPleyer(country);
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

    private static MenuActionService Initialize(MenuActionService actionService)
    {
        var menuCountry = new Country();
        actionService.AddNewAction(1, "Add Pleyer", "Main");
        actionService.AddNewAction(2, "Pleyers", "Main");
        actionService.AddNewAction(3, "Remove Pleyer", "Main");
        actionService.AddNewAction(4, "Update Pleyer", "Main");
        actionService.AddNewAction(5, "Tournaments Not implemented", "Main");
        actionService.AddNewAction(6, "Ranking Not implemented ", "Main");
        actionService.AddNewAction(7, "Sparing Not implemented", "Main");

        for (int i = 0; i <= menuCountry.CountryList.Count - 1; i++)
        {
            actionService.AddNewAction(i + 1, menuCountry.CountryList[i], "Country");
        }
        return actionService;
    }
}