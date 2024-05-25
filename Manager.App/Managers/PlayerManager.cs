using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.Consol.Abstract;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;

namespace Manager.App.Managers;

public class PlayerManager
{
    private readonly MenuActionService _actionService;
    private readonly IPlayerService _playerService;
    private readonly IConsoleService _consoleService;

    public PlayerManager(MenuActionService actionService, IPlayerService playerService, IConsoleService consoleService)
    {
        _actionService = actionService;
        _playerService = playerService;
        _consoleService = consoleService;
    }

    public void PlayerOptionView()
    {
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Players");
        while (true)
        {
            _consoleService.WriteTitle("Manage Players");
            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                _consoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = _consoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    ListOfActivePlayersView();
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
                    RemovePlayerView();
                    break;
                case 6:
                    operation = null;
                    break;
                default:
                    if (operation != null)
                    {
                        _consoleService.WriteLineErrorMessage("Enter a valid operation ID");
                    }
                    break;
            }


            if (operation == null)
            {
                Console.Clear();
                break;
            }
        }
    }
    public int AddNewPlayer()
    {
        _consoleService.WriteTitle("Add Player");
        TypeInfo infoPlayer = typeof(Player).GetTypeInfo();
        IEnumerable<PropertyInfo> propertyListPlayer = infoPlayer.DeclaredProperties;
        Player newPlayer = new Player();
        foreach (PropertyInfo property in propertyListPlayer)
        {
            if (property.Name != "Country")
            {
                _consoleService.WriteTitle("Add Player " + property.Name);
                var inputString = _consoleService.GetRequiredStringFromUser("Enter " + property.Name);

                if (inputString == null)
                {
                    return 0;
                }

                if (property.Name == "Name")
                {
                    newPlayer.Name = inputString;
                }
                else if (property.Name == "LastName")
                {
                    newPlayer.LastName = inputString;
                }
                else if (property.Name == "City")
                {
                    newPlayer.City = inputString;
                }
            }
            else
            {
                var countryListMenu = _actionService.GetMenuActionsByName(property.Name);
                do
                {
                    for (int i = 0; i < countryListMenu.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {countryListMenu[i].Name}");
                    }

                    var inputInt = _consoleService.GetIntNumberFromUser("Country");
                    if (inputInt <= countryListMenu.Count)
                    {
                        var countryName = countryListMenu.FirstOrDefault(p => p.Name == countryListMenu[(int)inputInt - 1].Name);
                        if (countryName != null && inputInt != null && !countryName.Name.Contains("Exit"))
                        {
                            newPlayer.Country = countryName.Name;
                        }
                        else if (countryName.Name.Contains("Exit"))
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        _consoleService.WriteLineErrorMessage($"No option nr: " + inputInt);
                    }

                } while (newPlayer.Country == null);
            }
        }

        _playerService.AddItem(newPlayer);
        _playerService.SaveList();
        return newPlayer.Id;
    }
    
    public int RemovePlayerView()
    {
        if (ListOfActivePlayersView())
        {
            Console.WriteLine("\nPlease enter id for player you want remove");
            var playerId = Console.ReadLine();
            int.TryParse(playerId, out int id);
            var playerToRemove = _playerService.GetAllItem().FirstOrDefault(p => p.Id == id);
            if (playerToRemove != null)
            {
                _playerService.RemoveItem(playerToRemove);
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
        if (ListOfActivePlayersView())
        {
            Console.WriteLine("\nPlease enter id for player you want update");
            var playerId = Console.ReadLine();
            if (int.TryParse(playerId, out int id))
            {
                var playerToUpdate = _playerService.GetAllItem().FirstOrDefault(p => p.Id == id);
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
                            Console.WriteLine($"Actual Country {playerToUpdate.Country}\n");

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

                            Console.WriteLine("\n\n\n\n       Press Q to back in Main menu");
                            var operation = Console.ReadKey();
                            int.TryParse(operation.KeyChar.ToString(), out int numberCountry);

                            if (numberCountry <= countryList.Count && numberCountry >= 0)
                            {
                                if (numberCountry != 0)
                                {
                                    var countryplayer = new Country();
                                    playerToUpdate.Country = countryplayer.CountryList[numberCountry - 1];
                                }

                                Console.Clear();
                                Console.WriteLine("Update player\n");
                                Console.WriteLine($"Actual Name {playerToUpdate.Name}\n");
                                Console.WriteLine("Enter player name:");

                                var playerName = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(playerName))
                                {
                                    playerName = playerName.Trim();
                                    playerName.Replace(playerName.First(), playerName.ToUpper().First());
                                    playerToUpdate.Name = playerName;
                                }
                                Console.Clear();
                                Console.WriteLine("  Update Player\n");
                                Console.WriteLine($" Actual last name {playerToUpdate.LastName}\n");
                                Console.Write("Enter player last name:\n");
                                var playerLastName = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(playerLastName))
                                {
                                    playerLastName.Trim();
                                    playerLastName.Replace(playerLastName.First(), playerLastName.ToUpper().First());
                                    playerToUpdate.LastName = playerLastName;
                                }
                                Console.Clear();
                                Console.WriteLine("  Update Player\n");
                                Console.WriteLine($" Actual City {playerToUpdate.City}\n");
                                Console.Write("Enter player City:\n");
                                var playerCity = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(playerCity))
                                {
                                    playerCity.Trim();
                                    playerCity.Replace(playerCity.First(), playerCity.ToUpper().First());
                                    playerToUpdate.City = playerCity;
                                }

                                _playerService.UpdateItem(playerToUpdate);
                                return playerToUpdate.Id;
                            }
                            noCountry = true;
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
    public bool ListOfActivePlayersView()
    {


        var activePlayer = _playerService.ListOfActivePlayers();
        if (activePlayer.Any())
        {
            _consoleService.WriteTitle("List Of Players");
            foreach (var player in activePlayer)
            {
                _consoleService.WriteLineMessage($"ID: {player.Id}. Name: {player.Name} Last Name: {player.LastName}   " +
                      $"Country: {player.Country} City: {player.City}");
            }
            return true;
        }
        else
        {
            _consoleService.WriteLineErrorMessage("Empty List");
            return false;
        }
    }


}
