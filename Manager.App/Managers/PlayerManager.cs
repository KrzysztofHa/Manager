using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.Text;

namespace Manager.App.Managers;

public class PlayerManager : IPlayerManager
{
    private readonly MenuActionService _actionService;
    private readonly IPlayerService _playerService;

    public PlayerManager(MenuActionService actionService, IPlayerService playerService)
    {
        _actionService = actionService;
        _playerService = playerService;
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
        if (playerId != null)
        {
            var playerToRemove = _playerService.GetAllItem().FirstOrDefault(p => p.Id == playerId.Id);
            if (playerToRemove != null)
            {
                _playerService.DeletePlayer(playerToRemove);
                ConsoleService.WriteLineMessageActionSuccess($"Remove PLayer {playerToRemove.Id} {playerToRemove.FirstName}" +
                    $" {playerToRemove.FirstName} Success !!");
                _playerService.SaveList();
                return playerToRemove.Id;
            }
        }
        return -1;
    }
    public Player AddNewPlayer()
    {
        var player = new Player();
        if (GetDataFromUser(player) == null)
        {
            return null;
        }

        Func<Player, bool> isPlayerExist = (player) =>
        {
            var test = _playerService.GetAllItem().Any(p => p.FirstName.ToLower() == player.FirstName.ToLower() &&
             p.LastName.ToLower() == player.LastName.ToLower() && p.IdAddress == player.IdAddress);

            return test;
        };

        while (isPlayerExist(player))
        {
            ConsoleService.WriteLineErrorMessage("The player with the given Data is already exists.");
            if (ConsoleService.AnswerYesOrNo("Do you want to correct the entered Data ?"))
            {
                if (GetDataFromUser(player) == null)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        _playerService.AddItem(player);
        _playerService.SaveList();
        ConsoleService.WriteTitle(_playerService.GetPlayerDetailView(player));
        ConsoleService.WriteLineMessageActionSuccess($"Add Player Success !");

        return player;
    }
    public Player UpdatePlayer()
    {
        Func<Player, bool> isPlayerExist = (player) =>
        {
            var test = _playerService.GetAllItem().Any(p => p.FirstName.ToLower() == player.FirstName.ToLower() &&
             p.LastName.ToLower() == player.LastName.ToLower() && p.IdAddress == player.IdAddress &&
             p.Id != player.Id);
            return test;
        };

        var player = _playerService.GetItemById(SearchPlayer().Id);
        if (GetDataFromUser(player) == null)
        {
            return null;
        }

        while (isPlayerExist(player))
        {
            ConsoleService.WriteLineErrorMessage("The player with the given Data is already exists.");
            if (ConsoleService.AnswerYesOrNo("Do you want to correct the entered Data ?"))
            {
                if (GetDataFromUser(player) == null)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        if (player == null)
        {
            return null;
        }
        _playerService.UpdateItem(player);
        _playerService.SaveList();
        ConsoleService.WriteTitle(_playerService.GetPlayerDetailView(player));
        ConsoleService.WriteLineMessageActionSuccess($"Update Player Success !");

        return player;
    }
    private Player? GetDataFromUser(Player player)
    {
        Address playerAddress = new Address();
        string title = string.Empty;
        var isUpdatePlayer = player.Id != 0 ? true : false;
        if (isUpdatePlayer)
        {
            title = "Update Player";
            playerAddress = _playerService.GetPlayerAddress(player);
            if (player.Id != 0)
            {
                if (player == null)
                {
                    return null;
                }
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
            var formatPlayerDataToView = $"{player.Id,-5}".Remove(5) + $" {player.FirstName,-20}".Remove(21) +
               $" {player.LastName,-20}".Remove(21) + formatAddressToView;


            ConsoleService.WriteTitle($"{title}\r\n{"ID",-6}{"First Name",-21}{"Last Name",-21}" +
                   $"{"Street",-11}{"Number",-11}{"City",-11}{"Country",-11}{"zip",-6}");

            ConsoleService.WriteLineMessage(formatPlayerDataToView);

            if (propertyItem != "Country")
            {
                if (isUpdatePlayer)
                {
                    updateString = ConsoleService.GetStringFromUser($"----- \r\n" + propertyItem);
                }
                else
                {
                    updateString = ConsoleService.GetRequiredStringFromUser("\r\n" + propertyItem);
                    if (string.IsNullOrEmpty(updateString))
                    {
                        return null;
                    }
                }

                if (string.IsNullOrEmpty(updateString))
                {
                    if (updateString == null)
                    {
                        return null;
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
                        player.FirstName = updateString;
                    }
                    else if (propertyItem == "Last Name")
                    {
                        player.LastName = updateString;
                    }
                    else if (propertyItem == "City")
                    {
                        playerAddress.City = updateString;
                    }
                    else if (propertyItem == "Street")
                    {
                        playerAddress.Street = updateString;
                    }
                    else if (propertyItem == "Zip")
                    {
                        playerAddress.Zip = updateString;
                    }
                    else if (propertyItem == "Building Number")
                    {
                        playerAddress.BuildingNumber = updateString;
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
                            return null;
                        }
                    }
                    else
                    {
                        if (inputInt == null && isUpdatePlayer)
                        {
                            if (ConsoleService.AnswerYesOrNo("You want to exit? \r\nThe entered data will not be saved"))
                            {
                                return null;
                            }
                        }
                        ConsoleService.WriteLineErrorMessage($"No option nr: " + inputInt);
                    }
                } while (playerAddress.Country == null);
            }
        }

        _playerService.AddPlayerAddress(player, playerAddress);
        return player;
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
    public Player? SearchPlayer(string title = "")
    {
        StringBuilder inputString = new StringBuilder();
        List<Player> findPlayers = _playerService.SearchPlayer(" ");
        if (!findPlayers.Any())
        {
            ConsoleService.WriteLineErrorMessage("Empty List");
            return null;
        }
        List<Player> findPlayersTemp = new List<Player>();
        var address = new Address();
        int IdSelectedPlayer = 0;
        title = string.IsNullOrWhiteSpace(title) ? "Search Player" : title;
        do
        {
            if (findPlayers.Any())
            {
                ConsoleService.WriteTitle(title + $"\r\n{"ID",-6}{"First Name",-21}{"Last Name",-21}" +
                    $"{"Street",-11}{"Number",-11}{"City",-11}{"Country",-11}{"zip",-6}");

                foreach (var player in findPlayers)
                {
                    var formmatStringToView = findPlayers.IndexOf(player) == IdSelectedPlayer ?
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
                if (findPlayers.Count == 1 && !string.IsNullOrEmpty(inputString.ToString()))
                {
                    ConsoleService.WriteLineErrorMessage("No entry found !!!");
                }
                else
                {
                    inputString.Append(keyFromUser.KeyChar);

                    if (inputString.Length == 1)
                    {
                        findPlayersTemp = _playerService.SearchPlayer(inputString.ToString());
                        if (findPlayersTemp.Any())
                        {
                            findPlayers.Clear();
                            findPlayers.AddRange(findPlayersTemp);
                            IdSelectedPlayer = 0;
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                        }
                    }
                    else
                    {
                        findPlayers = [.. findPlayers.Where(p => $"{p.Id} {p.FirstName} {p.LastName}".ToLower().
                        Contains(inputString.ToString().ToLower())).OrderBy(i => i.FirstName)];
                        if (!findPlayers.Any())
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            findPlayers.AddRange([.. findPlayersTemp.Where(p => $"{p.Id} {p.FirstName} {p.LastName}".ToLower().
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
                    findPlayers = [.. findPlayersTemp.Where(p => $"{p.Id} {p.FirstName} {p.LastName}".ToLower()
                    .Contains(inputString.ToString().ToLower())).OrderBy(i => i.FirstName)];
                }
            }
            else if (keyFromUser.Key == ConsoleKey.DownArrow && IdSelectedPlayer < findPlayers.Count - 1)
            {
                IdSelectedPlayer++;
            }
            else if (keyFromUser.Key == ConsoleKey.UpArrow && IdSelectedPlayer > 0)
            {
                IdSelectedPlayer--;
            }
            else if (keyFromUser.Key == ConsoleKey.Enter && findPlayers.Any())
            {
                var findPlayersToSelect = findPlayers.First(p => findPlayers.IndexOf(p) == IdSelectedPlayer);
                ConsoleService.WriteLineMessage(_playerService.GetPlayerDetailView(findPlayersToSelect));

                if (ConsoleService.AnswerYesOrNo("Selected Player"))
                {
                    return findPlayersToSelect;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Escape)
            {
                break;
            }

        } while (true);

        return null;
    }
}
