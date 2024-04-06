using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.Domain.Entity;


namespace Manager.App.Managers
{
    public class PlayerManager
    {
        private readonly MenuActionService _actionService;
        private IService<Player> _playerService;

        public PlayerManager(MenuActionService actionService, IService<Player> playerService)
        {
            _actionService = actionService;
            _playerService = playerService;
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
                        var newplayer = new Player();
                        var countryplayer = new Country();
                        Console.Clear();

                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("Add New player\n");
                            Console.WriteLine("Enter player name:");
                            var playerName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(playerName))
                            {
                                playerName = playerName.ToString().Trim();
                                playerName = playerName.ToLower();
                                for (int i = 0; i <= playerName.Length - 1; i++)
                                {
                                    if (char.IsWhiteSpace(playerName[i]))
                                    {
                                        playerName = playerName.Remove(i);
                                        break;
                                    }
                                }
                                newplayer.Name = playerName[0].ToString().ToUpper();
                                newplayer.Name += playerName.Substring(1);
                                newplayer.Country = countryplayer.CountryList[numberCountry - 1];
                                newplayer.Id = _playerService.GetNextId();
                                _playerService.AddItem(newplayer);
                                return newplayer.Id;
                            }
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

        public int RemovePlayer()
        {
            if (ListOfPlayers())
            {
                Console.WriteLine("\nPlease enter id for player you want remove");
                var playerId = Console.ReadLine();
                int.TryParse(playerId, out int id);
                var playerToRemove = _playerService.Items.FirstOrDefault(p => p.Id == id);
                if (playerToRemove != null)
                {
                    _playerService.RemoveItem(playerToRemove);
                }
                else
                {
                    return 0;
                }
                return playerToRemove.Id;
            }
            else
            {
                return 0;
            }
        }
        public int UpdatePlayer()
        {

            if (ListOfPlayers())
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
                            Console.Clear();
                            Console.WriteLine("Update player\n");
                            Console.WriteLine("Enter player name:");
                            var playerName = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(playerName))
                            {
                                playerName = playerName.ToString().Trim();
                                playerName = playerName.ToLower();
                                for (int i = 0; i <= playerName.Length - 1; i++)
                                {
                                    if (char.IsWhiteSpace(playerName[i]))
                                    {
                                        playerName = playerName.Remove(i);
                                        break;
                                    }
                                }
                                playerToUpdate.Name = playerName[0].ToString().ToUpper();
                                playerToUpdate.Name += playerName.Substring(1);
                                _playerService.UpdateItem(playerToUpdate);
                                return playerToUpdate.Id;
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



        public bool ListOfPlayers()
        {
            if (_playerService.Items.Any())
            {
                Console.Clear();
                Console.WriteLine("List Of players");
                foreach (var player in _playerService.GetAllItem())
                {
                    Console.WriteLine($"{player.Id}. {player.Name} Country: {player.Country}");
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
