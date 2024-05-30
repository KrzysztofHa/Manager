using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Abstract;
using Manager.Domain.Entity;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace Manager.App.Managers;

public class PlayerManager
{
    private readonly MenuActionService _actionService;
    private readonly IPlayerService _playerService;
    private readonly IConsoleService _consoleService;
    private readonly IUserService _userService;

    public PlayerManager(MenuActionService actionService, IPlayerService playerService, IConsoleService consoleService, IUserService userService)
    {
        _actionService = actionService;
        _playerService = playerService;
        _consoleService = consoleService;
        _userService = userService;
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
                    _consoleService.WriteLineMessageActionSuccess("Press Any Key");
                    break;
                case 2:
                    SearchPlayer();
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
                break;
            }
        }
    }
    public int AddNewPlayer()
    {
        TypeInfo infoPlayer = typeof(Player).GetTypeInfo();
        IEnumerable<PropertyInfo> propertyListPlayer = infoPlayer.DeclaredProperties;
        Player newPlayer = new Player();
        foreach (PropertyInfo property in propertyListPlayer)
        {
            _consoleService.WriteTitle("Add Player");
            var stringQuery = from p in property.Name
                              where Char.IsUpper(p)
                              select p;

            string namePropertyToView = string.Empty;

            foreach (char findUpperLeterr in stringQuery)
            {
                if (property.Name.IndexOf(findUpperLeterr) != 0)
                {
                    namePropertyToView = property.Name.Insert(property.Name.IndexOf(findUpperLeterr), " ").ToString();
                }
                else
                {
                    namePropertyToView = property.Name;
                }
            }
            var inputString = _consoleService.GetRequiredStringFromUser($"Enter {namePropertyToView}");

            if (inputString == null)
            {
                return 0;
            }

            if (property.Name == "FirstName")
            {
                newPlayer.FirstName = inputString;
            }
            else if (property.Name == "LastName")
            {
                newPlayer.LastName = inputString;
            }
            else if (property.Name == "City")
            {
                newPlayer.City = inputString;
            }
            else if (property.Name == "Country")
            {
                var countryListMenu = _actionService.GetMenuActionsByName(property.Name);
                do
                {
                    _consoleService.WriteTitle("Add Player");
                    for (int i = 0; i < countryListMenu.Count; i++)
                    {
                        _consoleService.WriteLineMessage($"{i + 1}. {countryListMenu[i].Name}");
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
                            return -1;
                        }
                    }
                    else
                    {
                        _consoleService.WriteLineErrorMessage($"No option nr: " + inputInt);
                    }

                } while (newPlayer.Country == null);
            }
        }

        newPlayer.CreatedById = _userService.GetIdActiveUser();
        _playerService.AddItem(newPlayer);
        _playerService.SaveList();
        _consoleService.WriteLineMessage($"ID: {newPlayer.Id}. Name: {newPlayer.FirstName} Last Name: {newPlayer.LastName}   " +
                      $"Country: {newPlayer.Country} City: {newPlayer.City}");
        _consoleService.WriteLineMessageActionSuccess("New player has been added");
        return newPlayer.Id;
    }
    public int RemovePlayerView()
    {
        if (ListOfActivePlayersView())
        {
            var playerId = _consoleService.GetIntNumberFromUser("\nPlease enter ID for player you want remove");
            var playerToRemove = _playerService.GetAllItem().FirstOrDefault(p => p.Id == playerId);
            if (playerToRemove != null && playerId != null)
            {
                _playerService.RemoveItem(playerToRemove);
                _consoleService.WriteLineMessageActionSuccess($"Remove Player {playerToRemove.FirstName} Success");
                _playerService.SaveList();
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
            var idPlayerToUpdate = _consoleService.GetIntNumberFromUser("Please enter id for player you want update");
            Player playerToUpdate = new Player();
            if (idPlayerToUpdate != null)
            {
                playerToUpdate = _playerService.GetAllItem().FirstOrDefault(p => p.Id == idPlayerToUpdate);
            }
            else
            {
                _consoleService.WriteLineMessage("Entered Player ID not exist");
                return -1;
            }

            TypeInfo infoPlayer = typeof(Player).GetTypeInfo();
            IEnumerable<PropertyInfo> propertyListPlayer = infoPlayer.DeclaredProperties;
            _consoleService.WriteTitle("Update Player");
            var valueToChang = string.Empty;
            foreach (PropertyInfo property in propertyListPlayer)
            {
                if (property.Name == "FirstName")
                {
                    valueToChang = playerToUpdate.FirstName;
                }
                else if (property.Name == "LastName")
                {
                    valueToChang = playerToUpdate.LastName;
                }
                else if (property.Name == "City")
                {
                    valueToChang = playerToUpdate.City;
                }
                else if (property.Name == "Country")
                {
                    valueToChang = playerToUpdate.Country;
                }

                var stringQuery = from p in property.Name
                                  where Char.IsUpper(p)
                                  select p;

                var namePropertyToView = string.Empty;

                foreach (char findUpperLeterr in stringQuery)
                {
                    if (property.Name.IndexOf(findUpperLeterr) != 0)
                    {
                        namePropertyToView = property.Name.Insert(property.Name.IndexOf(findUpperLeterr), " ").ToString();
                    }
                    else
                    {
                        namePropertyToView = property.Name;
                    }
                }

                _consoleService.WriteLineMessage($"{propertyListPlayer.ToList().IndexOf(property) + 1}. {namePropertyToView}: {valueToChang}");
            }

            var option = _consoleService.GetIntNumberFromUser("Select option");
            if (option != null)
            {
                var namePropertyToUpdate = propertyListPlayer.FirstOrDefault(p => p.Name.IndexOf(p.Name) + 1 == option).Name;

                if (namePropertyToUpdate != null)
                {
                    var updateString = _consoleService.GetRequiredStringFromUser($"{namePropertyToUpdate}:");
                    if (updateString == null)
                    {
                        return 0;
                    }
                    foreach (PropertyInfo property in propertyListPlayer)
                    {
                        if (property.Name == "FirstName")
                        {
                            playerToUpdate.FirstName = updateString;
                        }
                        else if (property.Name == "LastName")
                        {
                            playerToUpdate.LastName = updateString;
                        }
                        else if (property.Name == "City")
                        {
                            playerToUpdate.City = updateString;
                        }
                        else if (property.Name != "Country")
                        {
                            var countryListMenu = _actionService.GetMenuActionsByName(property.Name);
                            do
                            {
                                _consoleService.WriteTitle("Add Player");
                                for (int i = 0; i < countryListMenu.Count; i++)
                                {
                                    _consoleService.WriteLineMessage($"{i + 1}. {countryListMenu[i].Name}");
                                }

                                var inputInt = _consoleService.GetIntNumberFromUser("Country");
                                if (inputInt <= countryListMenu.Count)
                                {
                                    var countryName = countryListMenu.FirstOrDefault(p => p.Name == countryListMenu[(int)inputInt - 1].Name);
                                    if (countryName != null && inputInt != null && !countryName.Name.Contains("Exit"))
                                    {
                                        playerToUpdate.Country = countryName.Name;
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

                            } while (playerToUpdate.Country == null);
                        }
                    }
                }
            }
            else
            {
                return -1;
            }
            playerToUpdate.ModifiedById = _userService.GetIdActiveUser();
            playerToUpdate.ModifiedDateTime = DateTime.Now;
            _playerService.SaveList();
            _consoleService.WriteLineMessage($"ID: {playerToUpdate.Id}. First Name: {playerToUpdate.FirstName}  Last Name:  {playerToUpdate.LastName}   " +
                      $"Country: {playerToUpdate.Country} City: {playerToUpdate.City}");
            _consoleService.WriteLineMessageActionSuccess($"Data of player has been update");
            return playerToUpdate.Id;
        }

        return 0;
    }
    public bool ListOfActivePlayersView()
    {


        var activePlayer = _playerService.ListOfActivePlayers();
        if (activePlayer.Any())
        {
            _consoleService.WriteTitle("List Of Players");
            foreach (var player in activePlayer)
            {
                _consoleService.WriteLineMessage($"ID: {player.Id}. First Name: {player.FirstName} Last Name: {player.LastName}   " +
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

    public int SearchPlayer()
    {
        List<Player> findPlayer = new List<Player>();
        TypeInfo infoPlayer = typeof(Player).GetTypeInfo();
        IEnumerable<PropertyInfo> propertyListPlayer = infoPlayer.DeclaredProperties;
        Player newPlayer = new Player();
        _consoleService.WriteTitle("Search Player");
        foreach (PropertyInfo property in propertyListPlayer)
        {
            var stringQuery = from p in property.Name
                              where Char.IsUpper(p)
                              select p;
            var namePropertyToView = string.Empty;

            foreach (char findUpperLeterr in stringQuery)
            {
                if (property.Name.IndexOf(findUpperLeterr) != 0)
                {
                    namePropertyToView = property.Name.Insert(property.Name.IndexOf(findUpperLeterr), " ").ToString();
                }
                else
                {
                    namePropertyToView = property.Name;
                }
            }
            _consoleService.WriteLineMessage($"{propertyListPlayer.ToList().IndexOf(property) + 1}. {namePropertyToView}");
        }

        var option = _consoleService.GetIntNumberFromUser("Select search option");
        if (option != null)
        {
            var namePropertyPlayer = propertyListPlayer.FirstOrDefault(p => p.Name.IndexOf(p.Name) + 1 == option).Name;

            if (namePropertyPlayer != null)
            {
                foreach (PropertyInfo property in propertyListPlayer)
                {
                    if (property.Name == "FirstName")
                    {
                        var inputString = new StringBuilder();
                        do
                        {
                            _consoleService.WriteTitle("Search Player");

                            if (findPlayer.Any())
                            {
                                foreach (var player in findPlayer)
                                {
                                    _consoleService.WriteLineMessage($"ID: {player.Id}.First Name: {player.FirstName} Last Name: {player.LastName}   " +
                                          $"Country: {player.Country} City: {player.City}");
                                }

                                var findPlayerToSelect = findPlayer.First();

                                _consoleService.WriteLineMessage($"---------------\r\n" +
                                    $"Select => ID: {findPlayerToSelect.Id}. First Name: {findPlayerToSelect.FirstName} " +
                                    $"Last Name: {findPlayerToSelect.LastName}   " +
                                         $"Country: {findPlayerToSelect.Country} City: {findPlayerToSelect.City}\r\n" +
                                         $"Press Enter\r\n------------------");
                            }
                            else
                            {
                                _consoleService.WriteLineErrorMessage("Not Find Player");
                            }
                            _consoleService.WriteLineMessage(inputString.ToString());
                            _consoleService.WriteLineMessage("Enter string");

                            var keyFromUser = _consoleService.GetKeyFromUser();

                            if (char.IsLetterOrDigit(keyFromUser.KeyChar))
                            {
                                inputString.Append(keyFromUser.KeyChar);
                            }
                            else if (keyFromUser.Key == ConsoleKey.Backspace && inputString.Length > 0)
                            {
                                inputString.Remove(inputString.Length - 1, 1);
                            }

                            if (keyFromUser.Key == ConsoleKey.Enter)
                            {
                                var findPlayerToview = findPlayer.First();

                                _consoleService.WriteLineMessage($"ID: {findPlayerToview.Id}." +
                                    $"First Name: {findPlayerToview.FirstName} Last Name: {findPlayerToview.LastName}   " +
                                $"Country: {findPlayerToview.Country} City: {findPlayerToview.City}");

                                if (_consoleService.AnswerYesOrNo("Selected Player"))
                                {
                                    break;
                                }
                            }

                            if (keyFromUser.Key == ConsoleKey.Escape)
                            {
                                break;
                            }

                            findPlayer.Clear();
                            if (inputString.ToString() != string.Empty)
                            {
                                findPlayer = _playerService.ListOfActivePlayers().FindAll(p => p.FirstName.Contains(inputString.ToString()))
                                    .OrderBy(i => i.FirstName).ToList();
                            }

                        } while (true);

                    }
                    else if (property.Name == "LastName")
                    {

                    }
                    else if (property.Name == "City")
                    {

                    }
                    else if (property.Name != "Country")
                    {
                        var countryListMenu = _actionService.GetMenuActionsByName(property.Name);
                        do
                        {
                            _consoleService.WriteTitle("Add Player");
                            for (int i = 0; i < countryListMenu.Count; i++)
                            {
                                _consoleService.WriteLineMessage($"{i + 1}. {countryListMenu[i].Name}");
                            }

                            var inputInt = _consoleService.GetIntNumberFromUser("Country");
                            if (inputInt <= countryListMenu.Count)
                            {
                                var countryName = countryListMenu.FirstOrDefault(p => p.Name == countryListMenu[(int)inputInt - 1].Name);
                                if (countryName != null && inputInt != null && !countryName.Name.Contains("Exit"))
                                {
                                    ;
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

                        } while (property.Name != namePropertyPlayer);
                    }
                }
            }
            return -1;
        }
        return findPlayer.First().Id;
    }
}
