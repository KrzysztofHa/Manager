using Manager.App.Abstract;
using Manager.App.Common;
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
        var playerId = SearchPlayer($"Remove Player\r\n Search and Select Player To Remove");
        if (playerId != -1)
        {
            var playerToRemove = _playerService.GetAllItem().FirstOrDefault(p => p.Id == playerId);
            if (playerToRemove != null)
            {
                playerToRemove.ModifiedById = _userService.GetIdActiveUser();
                _playerService.RemoveItem(playerToRemove);
                _consoleService.WriteLineMessageActionSuccess($"Remove PLayer {playerToRemove.Id} {playerToRemove.FirstName}" +
                    $" {playerToRemove.FirstName} Success !!");
                _playerService.SaveList();
                return playerToRemove.Id;
            }
        }
        return -1;
    }
    public int AddOrUpdatePlayer()
    {

        bool isUpdatePlayer = false;
        Player updatePlayer = new Player();
        Address playerAddress = new Address();
        string title = string.Empty;
        _consoleService.WriteTitle("Add Or Update Player\r\nSelect Option--------");
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
                        playerAddress.City = updateString;
                    }
                }
            }
            else
            {
                var countryListMenu = _actionService.GetMenuActionsByName("Country");
                do
                {
                    _consoleService.WriteTitle(title);
                    for (int i = 0; i < countryListMenu.Count; i++)
                    {
                        _consoleService.WriteLineMessage($"{i + 1}. {countryListMenu[i].Name}");
                    }

                    var inputInt = _consoleService.GetIntNumberFromUser("Country");
                    if (inputInt <= countryListMenu.Count)
                    {
                        var countryName = countryListMenu.FirstOrDefault(p => p.Name == countryListMenu[(int)inputInt - 1].Name);
                        if (countryName != null && !countryName.Name.Contains("Exit"))
                        {
                            playerAddress.Country = countryName.Name;
                        }
                        else if (countryName.Name.Contains("Exit"))
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        if (inputInt == null && isUpdatePlayer)
                        {
                            if (_consoleService.AnswerYesOrNo("You want to exit? \r\nThe entered data will not be saved"))
                            {
                                return -1;
                            }
                        }
                        _consoleService.WriteLineErrorMessage($"No option nr: " + inputInt);
                    }

                } while (playerAddress.Country == null);
            }

        }
        if (isUpdatePlayer)
        {
            updatePlayer.ModifiedById = _userService.GetIdActiveUser();
            _playerService.UpdateItem(_playerService.AddPlayerAddress(updatePlayer, playerAddress));
            _playerService.SaveList();
        }
        else
        {
            updatePlayer.CreatedById = _userService.GetIdActiveUser();
            ;
            _playerService.AddItem(_playerService.AddPlayerAddress(updatePlayer, playerAddress));
            _playerService.SaveList();
        }
        _consoleService.WriteLineMessage(_playerService.GetPlayerDetailView(updatePlayer));
        _consoleService.WriteLineMessageActionSuccess($"Data of player has been update");

        return updatePlayer.Id;
    }
    public bool ListOfActivePlayersView()
    {
        var activePlayer = _playerService.ListOfActivePlayers();
        if (activePlayer.Any())
        {
            _consoleService.WriteTitle($"{"ID|",-5}{"First Name|",+15}{"Last Name|",+15}" +
                    $"{"Street|",+15}{"Number|",+10}{"City|",+15}{"Country|",+15}{"zip|",+10}");
            foreach (var player in activePlayer)
            {
                _consoleService.WriteLineMessage(_playerService.GetPlayerDetailView(player));
            }
            return true;
        }
        else
        {
            _consoleService.WriteLineErrorMessage("Empty List");
            return false;
        }
    }
    public int SearchPlayer(string title = "")
    {
        StringBuilder inputString = new StringBuilder();
        List<Player> findPlayer = _playerService.SearchPlayer("1");
        List<Player> findPlayerTemp = new List<Player>();
        var address = new Address();
        int IdSelectedPlayer = 0;
        title = string.IsNullOrWhiteSpace(title) ? "Search Player" : title;
        do
        {
            _consoleService.WriteTitle(title);

            if (findPlayer.Any())
            {
                _consoleService.WriteTitle(title + $"\r\n{"ID",-5}{"First Name",-10}{"Last Name",-10}{"City",-10}{"Country",-10}" +
                    $"{"Street",-10}{"BuildingNumber",-5}{"City",-10}{"Country",-10}{"zip",-10}");

                foreach (var player in findPlayer)
                {
                    var result = findPlayer.IndexOf(player) == IdSelectedPlayer ? $"<= Select Press Enter" : string.Empty;
                    _consoleService.WriteLineMessage(_playerService.GetPlayerDetailView(player) + result);
                }
            }
            else
            {
                _consoleService.WriteLineErrorMessage("Not Find Player");
            }
            _consoleService.WriteLineMessage("---------------\r\n" + inputString.ToString());
            _consoleService.WriteLineMessage("Enter string");

            var keyFromUser = _consoleService.GetKeyFromUser();

            if (char.IsLetterOrDigit(keyFromUser.KeyChar))
            {
                if (findPlayer.Count == 1 && !string.IsNullOrEmpty(inputString.ToString()))
                {
                    inputString.Remove(inputString.Length - 1, 1);
                    _consoleService.WriteLineErrorMessage("No entry found !!!");
                }
                else
                {
                    if (string.IsNullOrEmpty(inputString.ToString()))
                    {
                        findPlayerTemp = _playerService.SearchPlayer(keyFromUser.KeyChar.ToString());
                    }
                    
                    inputString.Append(keyFromUser.KeyChar);
                    findPlayer = [.. findPlayer.Where(p => $"{p.Id} {p.FirstName} {p.LastName}".ToLower()
                    .Contains(inputString.ToString().ToLower())).OrderBy(i => i.FirstName)];
                    IdSelectedPlayer = 0;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Backspace && inputString.Length > 0)
            {
                inputString.Remove(inputString.Length - 1, 1);

                if (!string.IsNullOrEmpty(inputString.ToString()))
                {
                    findPlayer = [.. findPlayerTemp.Where(p => $"{p.Id} {p.FirstName} {p.LastName}".ToLower()
                    .Contains(inputString.ToString().ToLower())).OrderBy(i => i.FirstName)];
                }
                

            }
            else if (keyFromUser.Key == ConsoleKey.DownArrow && IdSelectedPlayer < findPlayer.Count - 1)
            {
                IdSelectedPlayer++;
            }
            else if (keyFromUser.Key == ConsoleKey.UpArrow && IdSelectedPlayer > 0)
            {
                IdSelectedPlayer--;
            }
            else if (keyFromUser.Key == ConsoleKey.Enter && findPlayer.Any())
            {
                var findPlayerToSelect = findPlayer.First(p => findPlayer.IndexOf(p) == IdSelectedPlayer);
                _consoleService.WriteLineMessage(_playerService.GetPlayerDetailView(findPlayerToSelect));

                if (_consoleService.AnswerYesOrNo("Selected Player"))
                {
                    return findPlayerToSelect.Id;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Escape)
            {
                break;
            }

            //findPlayerTemp = _playerService.SearchPlayer(inputString.ToString());
            //if (findPlayerTemp.Any())
            //{
            //    findPlayer.Clear();
            //    findPlayer.AddRange(findPlayerTemp);
            //    IdSelectedPlayer = 0;
            //}
            //else
            //{
            //    IdSelectedPlayer = 0;
            //    inputString.Remove(inputString.Length - 1, 1);
            //    _consoleService.WriteLineErrorMessage("No entry found !!");
            //}

        } while (true);

        return -1;
    }
}
