using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.Domain.Entity;
using Manager.Infrastructure.Abstract;
using Manager.Infrastructure.Common;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Manager.App.Managers
{
    public class PlayerManager
    {
        private readonly MenuActionService _actionService;
        private readonly IService<Player> _playerService;
        private readonly IBaseService<Player> _baseService = new BaseOperationService<Player>();
        public PlayerManager(MenuActionService actionService, IService<Player> playerService)
        {
            _actionService = actionService;
            _playerService = playerService;
            _playerService.Items = _baseService.ListOfElements;
        }

        public void PlayerOptionView()
        {
            var optionPlayerMenu = _actionService.GetMenuActionsByName("Players");
            while (true)
            {
                Console.Clear();
                for (int i = 0; i < optionPlayerMenu.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {optionPlayerMenu[i].Name}");
                }

                Console.WriteLine("\n\n    Back To Main Menu  Press Esc or Q ");
                Console.WriteLine("     Enter Option");



                var option = Console.ReadKey();

                if (int.TryParse(option.KeyChar.ToString(), out int operation))
                {
                    switch (operation)
                    {
                        case 1:
                            ListOfPlayersView();
                            Console.ReadKey();
                            break;
                        case 2:
                            break;
                        case 3:
                            AddNewPlayer();
                            break;
                        case 4:
                            UpdatePlayer();
                            break;
                        case 5:
                            RemovePlayer();
                            break;
                        case 6:
                            break;
                        default:

                            break;
                    }
                }

                if (option.Key == ConsoleKey.Q || option.Key == ConsoleKey.Escape)
                {
                    _baseService.SaveListToBase();
                    break;
                }
            }
        }
        public int AddNewPlayer()
        {
            var countryList = _actionService.GetMenuActionsByName("Country");
            bool noCountry = false;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("  Add New player\n");

                if (noCountry)
                {
                    noCountry = false;
                    Console.WriteLine("Action You entered does not exist\n");
                }
                else
                {
                    Console.WriteLine($"\n    Enter Country\n\n\nPress number key in menu options\n");
                }

                for (int i = 0; i < countryList.Count; i++)
                {
                    Console.WriteLine($"{countryList[i].Id}. {countryList[i].Name}");
                }

                Console.WriteLine("\n\n\n\n       Press q to back in Main menu");
                var operation = Console.ReadKey();
                int.TryParse(operation.KeyChar.ToString(), out int numberCountry);

                if (numberCountry <= countryList.Count && numberCountry > 0)
                {
                    if (operation.Key != ConsoleKey.Q)
                    {
                        var newPlayer = new Player();
                        var countryPlayer = new Country();
                        newPlayer.Country = countryPlayer.CountryList[numberCountry - 1];

                        Console.Clear();
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("Add New player\n");
                            Console.WriteLine("Enter player name:");
                            var playerName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(playerName))
                            {
                                playerName = playerName.Trim();
                                playerName.Replace(playerName.First(), playerName.ToUpper().First());
                                newPlayer.Name = playerName;
                                break;
                            }
                        }
                        while (true)
                        {
                            Console.Clear();
                            Console.Write("Enter player last name:\n");
                            var playerLastName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(playerLastName))
                            {
                                playerLastName.Trim();
                                playerLastName.Replace(playerLastName.First(), playerLastName.ToUpper().First());
                                newPlayer.LastName = playerLastName;
                                break;
                            }
                        }
                        while (true)
                        {
                            Console.Clear();
                            Console.Write("Enter player City:\n");
                            var playerCity = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(playerCity))

                            {
                                playerCity.Trim();
                                playerCity.Replace(playerCity.First(), playerCity.ToUpper().First());
                                newPlayer.City = playerCity;
                                break;
                            }
                        }
                        if (_playerService.Items.Any())
                        {
                            newPlayer.Id = _playerService.Items.Count + 1;
                        }
                        else
                        {
                            newPlayer.Id = 1;
                        }
                        newPlayer.Active = true;
                        return _playerService.AddItem(newPlayer);

                    }
                    else
                    {
                        return 0;
                    }
                }

                if (operation.Key == ConsoleKey.Q)
                {
                    return 0;
                }
            }
        }

        public int RemovePlayer()
        {
            if (ListOfPlayersView())
            {
                Console.WriteLine("\nPlease enter id for player you want remove");
                var playerId = Console.ReadLine();
                int.TryParse(playerId, out int id);
                var playerToRemove = _playerService.Items.FirstOrDefault(p => p.Id == id);
                if (playerToRemove != null)
                {
                    playerToRemove.Active = false;                    
                }       
                
                return playerToRemove.Id;
            }
            else
            {
                return -1;
            }
        }
        public int UpdatePlayer()
        {

            if (ListOfPlayersView())
            {
                Console.WriteLine("\nPlease enter id for player you want update");
                var playerId = Console.ReadLine();
                if (int.TryParse(playerId, out int id))
                {
                    var playerToUpdate = _playerService.Items.FirstOrDefault(p => p.Id == id);
                    if (playerToUpdate != null)
                    {
                        while (true)
                        {
                            var countryList = _actionService.GetMenuActionsByName("Country");
                            bool noCountry = false;
                            while (true)
                            {
                                Console.Clear();
                                Console.WriteLine("  Update Player\n");

                                if (noCountry)
                                {
                                    noCountry = false;
                                    Console.WriteLine("Action You entered does not exist\n");
                                }
                                else
                                {
                                    Console.WriteLine($"\n    Enter Country\n\n\nPress number key in menu options\n");
                                }

                                Console.WriteLine($"0. Skip changing country");
                                for (int i = 0; i < countryList.Count; i++)
                                {
                                    Console.WriteLine($"{countryList[i].Id}. {countryList[i].Name}");
                                }

                                Console.WriteLine("\n\n\n\n       Press q to back in Main menu");
                                var operation = Console.ReadKey();
                                int.TryParse(operation.KeyChar.ToString(), out int numberCountry);

                                if (numberCountry <= countryList.Count && numberCountry >= 0)
                                {
                                    if (operation.Key != ConsoleKey.Q)
                                    {
                                        var countryplayer = new Country();

                                        Console.Clear();
                                        Console.WriteLine("Update player\n");
                                        Console.WriteLine("Enter player name:");
                                        var playerName = Console.ReadLine();
                                        if (!string.IsNullOrWhiteSpace(playerName))
                                        {
                                            playerName = playerName.Trim();
                                            playerName.Replace(playerName.First(), playerName.ToUpper().First());
                                            playerToUpdate.Name = playerName[0].ToString().ToUpper();
                                            playerToUpdate.Name += playerName.Substring(1);
                                            
                                            
                                        }
                                       

                                        if (numberCountry != 0)
                                        {
                                            playerToUpdate.Country = countryplayer.CountryList[numberCountry - 1];
                                            _playerService.UpdateItem(playerToUpdate);
                                            return playerToUpdate.Id;
                                        }
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                }
                                if (operation.Key == ConsoleKey.Q)
                                {
                                    return 0;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Player no found");
                        Console.WriteLine("Press Any key");
                        Console.ReadKey();
                        return 0;
                    }
                }

                Console.Clear();
                Console.WriteLine("Player no found");
                Console.WriteLine("Press Any key");
                Console.ReadKey();
                return 0;
            }
            else
            {
                return 0;
            }
        }
        public bool ListOfPlayersView()
        {
            if (_playerService.Items.Any())
            {

                Console.Clear();
                Console.WriteLine("List Of players");
                var activePlayer = _playerService.Items.Any(p => p.Active == true);
                var ordinalNumber = 0;
                foreach (var player in _playerService.GetAllItem())
                {
                    
                    Console.WriteLine($"{ordinalNumber++}. {player.Name} Country: {player.Country}");

                }
                return true;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Empty List");
                return false;
            }
        }
    }
}
