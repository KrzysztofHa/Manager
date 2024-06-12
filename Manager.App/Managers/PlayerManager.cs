using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.Text;

namespace Manager.App.Managers;

public class PlayerManager
{
    private readonly MenuActionService _actionService;
    private readonly IPlayerService _playerService;
    private readonly IUserService _userService;

    public PlayerManager(MenuActionService actionService, IPlayerService playerService, IUserService userService)
    {
        _actionService = actionService;
        _playerService = playerService;
        _userService = userService;
    }

    public void PlayerOptionView()
    {
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Players");
        while (true)
        {
            ConsoleService.WriteTitle("Manage Players");
            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    ListOfActivePlayersView();
                    ConsoleService.WriteLineMessageActionSuccess("Press Any Key");
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
                        ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
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
                _playerService.DeletePlayer(playerToRemove);
                ConsoleService.WriteLineMessageActionSuccess($"Remove PLayer {playerToRemove.Id} {playerToRemove.FirstName}" +
                    $" {playerToRemove.FirstName} Success !!");
                _playerService.SaveList();
                return playerToRemove.Id;
            }
        }
        return -1;
    }
    public int AddNewPlayer()
    {
        return AddOrUpdatePlayer();
    }
    public int UpdatePlayer()
    {
        return AddOrUpdatePlayer(true);
    }
    private int AddOrUpdatePlayer(bool isUpdatePlayer = false)
    {
        Player updatePlayer = new Player();
        Address playerAddress = new Address();
        string title = string.Empty;
        
        if (isUpdatePlayer)
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
        string[] property = ["First Name", "Last Name", "Street", "Building Number", "City", "Country", "Zip"];

        string updateString;
        foreach (var propertyItem in property)
        {
            var formatAddressToView = $" {playerAddress.Street,-10}".Remove(11) + $" {playerAddress.BuildingNumber,-10}".Remove(11) +
                 $" {playerAddress.City,-10}".Remove(11) + $" {playerAddress.Country,-10}".Remove(11) +
                    $" {playerAddress.Zip,-5}".Remove(6);
            var formatPlayerDataToView = $"{updatePlayer.Id,-5}".Remove(5) + $" {updatePlayer.FirstName,-20}".Remove(21) +
               $" {updatePlayer.LastName,-20}".Remove(21) + formatAddressToView;

            ConsoleService.WriteTitle($"{title}\r\n{"ID",-6}{"First Name",-21}{"Last Name",-21}" +
                   $"{"Street",-11}{"Number",-11}{"City",-11}{"Country",-11}{"zip",-6}");

            ConsoleService.WriteLineMessage($"{formatPlayerDataToView,-92}");

            if (propertyItem != "Country")
            {
                if (isUpdatePlayer)
                {
                    updateString = ConsoleService.GetStringFromUser(propertyItem);
                }
                else
                {
                    updateString = ConsoleService.GetRequiredStringFromUser(propertyItem);
                    if (string.IsNullOrEmpty(updateString))
                    {
                        return -1;
                    }
                }

                if (string.IsNullOrEmpty(updateString))
                {
                    if (updateString == null)
                    {
                        return -1;
                    }
                    else
                    {
                        continue;
                    }
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
                        playerAddress.City = updateString.ToLower();
                    }
                    else if (propertyItem == "Street")
                    {
                        playerAddress.Street = updateString.ToLower();
                    }
                    else if (propertyItem == "Zip")
                    {
                        playerAddress.Zip = updateString.ToLower();
                    }
                    else if (propertyItem == "Building Number")
                    {
                        playerAddress.BuildingNumber = updateString.ToLower();
                    }
                }
            }
            else
            {
                var countryListMenu = _actionService.GetMenuActionsByName("Country");
                do
                {
                    ConsoleService.WriteTitle(title);
                    for (int i = 0; i < countryListMenu.Count; i++)
                    {
                        ConsoleService.WriteLineMessage($"{i + 1}. {countryListMenu[i].Name}");
                    }

                    var inputInt = ConsoleService.GetIntNumberFromUser("Country");
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
                            if (ConsoleService.AnswerYesOrNo("You want to exit? \r\nThe entered data will not be saved"))
                            {
                                return -1;
                            }
                        }
                        ConsoleService.WriteLineErrorMessage($"No option nr: " + inputInt);
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
            _playerService.AddItem(_playerService.AddPlayerAddress(updatePlayer, playerAddress));
            _playerService.SaveList();
        }
        ConsoleService.WriteLineMessage(_playerService.GetPlayerDetailView(updatePlayer));
        ConsoleService.WriteLineMessageActionSuccess($"Data of player has been update");

        return updatePlayer.Id;
    }
    public bool ListOfActivePlayersView()
    {
        var activePlayer = _playerService.ListOfActivePlayers();
        if (activePlayer.Any())
        {
            ConsoleService.WriteTitle($"\r\n{"ID",-6}{"First Name",-21}{"Last Name",-21}" +
                    $"{"Street",-11}{"Number",-11}{"City",-11}{"Country",-11}{"zip",-6}");
            foreach (var player in activePlayer)
            {
                ConsoleService.WriteLineMessage(_playerService.GetPlayerDetailView(player));
            }
            return true;
        }
        else
        {
            ConsoleService.WriteLineErrorMessage("Empty List");
            return false;
        }
    }
    public int SearchPlayer(string title = "")
    {
        StringBuilder inputString = new StringBuilder();
        List<Player> findPlayer = _playerService.SearchPlayer(" ");
        List<Player> findPlayerTemp = new List<Player>();
        var address = new Address();
        int IdSelectedPlayer = 0;
        title = string.IsNullOrWhiteSpace(title) ? "Search Player" : title;
        do
        {
            ConsoleService.WriteTitle(title);

            if (findPlayer.Any())
            {
                ConsoleService.WriteTitle(title + $"\r\n{"ID",-6}{"First Name",-21}{"Last Name",-21}" +
                    $"{"Street",-11}{"Number",-11}{"City",-11}{"Country",-11}{"zip",-6}");

                foreach (var player in findPlayer)
                {
                    var formmatStringToView = findPlayer.IndexOf(player) == IdSelectedPlayer ?
                        ">" + _playerService.GetPlayerDetailView(player) + " < Select Press Enter" :
                        " " + _playerService.GetPlayerDetailView(player);

                    ConsoleService.WriteLineMessage(formmatStringToView);
                }
            }
            else
            {
                ConsoleService.WriteLineErrorMessage("Not Find Player");
            }
            ConsoleService.WriteLineMessage("---------------\r\n" + inputString.ToString());
            ConsoleService.WriteLineMessage(@"Enter string move /\ or \/  and enter Select");

            var keyFromUser = ConsoleService.GetKeyFromUser();

            if (char.IsLetterOrDigit(keyFromUser.KeyChar))
            {
                if (findPlayer.Count == 1 && !string.IsNullOrEmpty(inputString.ToString()))
                {
                    ConsoleService.WriteLineErrorMessage("No entry found !!!");
                }
                else
                {
                    inputString.Append(keyFromUser.KeyChar);

                    if (inputString.Length == 1)
                    {
                        findPlayerTemp = _playerService.SearchPlayer(inputString.ToString());
                        if (findPlayerTemp.Any())
                        {
                            findPlayer.Clear();
                            findPlayer.AddRange(findPlayerTemp);
                            IdSelectedPlayer = 0;
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                        }
                    }
                    else
                    {
                        findPlayer = [.. findPlayer.Where(p => $"{p.Id} {p.FirstName} {p.LastName}".ToLower().
                        Contains(inputString.ToString().ToLower())).OrderBy(i => i.FirstName)];
                        if (!findPlayer.Any())
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            findPlayer.AddRange([.. findPlayerTemp.Where(p => $"{p.Id} {p.FirstName} {p.LastName}".ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.FirstName)]);
                            ConsoleService.WriteLineErrorMessage("No entry found !!!");
                        }
                        IdSelectedPlayer = 0;
                    }
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
                ConsoleService.WriteLineMessage(_playerService.GetPlayerDetailView(findPlayerToSelect));

                if (ConsoleService.AnswerYesOrNo("Selected Player"))
                {
                    return findPlayerToSelect.Id;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Escape)
            {
                break;
            }

        } while (true);

        return -1;
    }
}
