
using Manager;
internal class Program
{
    private static void Main(string[] args)
    {
        //Przywitanie
        // Logowanie
        //Dostanie możliwość wyboru
        //// 
        ////  Edycja danych 
        /// Turniej
        /// Sparing

        new LogIn();
        PleyerService ListOfPleyers = new PleyerService();

        MenuActionService actionService = new MenuActionService();
        Initialize(actionService);
        var mainMenu = actionService.GetMenuActionsByName("Main");
        bool noOption = false;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Hello User {LogIn.UserName}!\n\n");

            if (noOption)
            {
                noOption = false;
                Console.WriteLine("Action You entered does not exist\n");
            }
            else
            {
                Console.WriteLine($"Pres number key in menu options\n");
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
                    ListOfPleyers.AddNewPleyerView();
                    break;
                case '2':
                    ListOfPleyers.ListOfPleyersView();
                    break;
                case '3':
                    break;
                case '4':
                    break;
                case '5':
                    break;
                default:

                    noOption = true;
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
        actionService.AddNewAction(1, "Add Pleyer", "Main");
        actionService.AddNewAction(2, "Pleyers", "Main");
        actionService.AddNewAction(3, "Turnaments", "Main");
        actionService.AddNewAction(4, "Sparing", "Main");
        actionService.AddNewAction(5, "Edit Data", "Main");
        actionService.AddNewAction(6, "Ranking", "Main");



        return actionService;
    }
}