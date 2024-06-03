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
                    AddOrUpdatePlayer();
                    break;
                case 4:
                    RemovePlayerView();
                    break;
                case 5:
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
    public int AddOrUpdatePlayer()
    {
        bool isUpdatePlayer = false;
        Player updatePlayer = new Player();
        string title = string.Empty;
        _consoleService.WriteTitle("Add Update Player");
        if (_consoleService.AnswerYesOrNo("You want to edit player?"))
        {
            title = "Update Player";
            isUpdatePlayer = true;
            updatePlayer = _playerService.GetItemById(SearchPlayer());
            if (updatePlayer == null)
            {
                return -1;
            }
        }
        else
        {
            title = "Add New Player";
        }
        string[] property = ["First Name", "Last Name", "City", "Country"];

        string updateString;
        foreach (var propertyItem in property)
        {
            _consoleService.WriteTitle(title);
            if (propertyItem != "Country")
            {
                if (isUpdatePlayer)
                {
                    updateString = _consoleService.GetStringFromUser(propertyItem);
                }
                else
                {
                    updateString = _consoleService.GetRequiredStringFromUser(propertyItem);
                    if (string.IsNullOrEmpty(updateString))
                    {
                        return -1;
                    }
                }

                if (updateString == string.Empty)
                {
                    continue;
                }
                else
                {
                    if (propertyItem == "First Name")
                    {
                        updatePlayer.FirstName = updateString;
                    }
                    else if (propertyItem == "Last Name")
                    {
                        updatePlayer.LastName = updateString;
                    }
                    else if (propertyItem == "City")
                    {
                        updatePlayer.City = updateString;
                    }
                }
            }
            else
            {
                var countryListMenu = _actionService.GetMenuActionsByName("Country");
                do
                {
                    _consoleService.WriteTitle("Update Player");
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
                            updatePlayer.Country = countryName.Name;
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

                } while (updatePlayer.Country == null);
            }

        }
        if (isUpdatePlayer)
        {
            updatePlayer.ModifiedById = _userService.GetIdActiveUser();
            updatePlayer.ModifiedDateTime = DateTime.Now;
            _playerService.SaveList();            
        }
        else
        {
            updatePlayer.CreatedById = _userService.GetIdActiveUser();
            updatePlayer.CreatedDateTime = DateTime.Now;
            _playerService.AddItem(updatePlayer);
            _playerService.SaveList();
        }
        _consoleService.WriteLineMessage($"{updatePlayer.Id,-5}{updatePlayer.FirstName,-15}{updatePlayer.LastName,-15}" +
                      $"{updatePlayer.City,-15}{updatePlayer.Country}");
        _consoleService.WriteLineMessageActionSuccess($"Data of player has been update");

        return updatePlayer.Id;
    }
    public bool ListOfActivePlayersView()
    {
        var activePlayer = _playerService.ListOfActivePlayers();
        if (activePlayer.Any())
        {
            _consoleService.WriteTitle($"{"ID",-5}{"First Name",-15}{"Last Name",-15}{"City",-15}{"Country",-15}");
            foreach (var player in activePlayer)
            {
                _consoleService.WriteLineMessage($"{player.Id,-5}{player.FirstName,-15}{player.LastName,-15}" +
                      $"{player.City,-15}{player.Country,-15}");
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
        StringBuilder inputString = new StringBuilder();
        List<Player> findPlayer = _playerService.SearchPlayer("1");
        int IdSelectedPlayer = 0;
        do
        {
            _consoleService.WriteTitle("Search Player");

            if (findPlayer.Any())
            {
                _consoleService.WriteTitle($"{"ID",-5}{"First Name",-15}{"Last Name",-15}{"City",-15}{"Country",-15}");

                foreach (var player in findPlayer)
                {
                    var result = findPlayer.IndexOf(player) == IdSelectedPlayer ? "<= Select Press Enter" : string.Empty;
                    _consoleService.WriteLineMessage($"{player.Id,-5}{player.FirstName,-15}{player.LastName,-15}" +
                      $"{player.City,-15}{player.Country} {result}");

                }
            }
            else
            {
                IdSelectedPlayer = 0;
                _consoleService.WriteLineErrorMessage("Not Find Player");
            }
            _consoleService.WriteLineMessage("---------------\r\n" + inputString.ToString());
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
            else if (keyFromUser.Key == ConsoleKey.DownArrow && IdSelectedPlayer < findPlayer.Count - 1)
            {
                IdSelectedPlayer++;
            }
            else if (keyFromUser.Key == ConsoleKey.UpArrow && IdSelectedPlayer > 0)
            {
                IdSelectedPlayer--;
            }

            if (keyFromUser.Key == ConsoleKey.Enter && findPlayer.Any())
            {
                var findPlayerToSelect = findPlayer.First(p => findPlayer.IndexOf(p) == IdSelectedPlayer);
                _consoleService.WriteLineMessage($"{"ID",-5}{"First Name",-15}{"Last Name",-15}{"City",-15}{"Country",-15}");
                _consoleService.WriteLineMessage($"{findPlayerToSelect.Id,-5}{findPlayerToSelect.FirstName,-15}" +
                    $"{findPlayerToSelect.LastName,-15}{findPlayerToSelect.City,-15}{findPlayerToSelect.Country,-15}");

                if (_consoleService.AnswerYesOrNo("Selected Player"))
                {
                    return findPlayerToSelect.Id;
                }
            }

            if (keyFromUser.Key == ConsoleKey.Escape)
            {
                break;
            }

            if (inputString.ToString() != string.Empty)
            {

                var findPlayerTemp = _playerService.SearchPlayer(inputString.ToString());
                if (findPlayerTemp.Any())
                {
                    findPlayer.Clear();
                    findPlayer.AddRange(findPlayerTemp);
                }
                else
                {
                    inputString.Remove(inputString.Length - 1, 1);
                    _consoleService.WriteLineErrorMessage("No entry found !!");
                }
            }
        } while (true);

        return -1;
    }
}
