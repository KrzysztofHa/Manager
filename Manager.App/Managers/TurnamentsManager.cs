using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers;
using Manager.App.Managers.Helpers.GamePlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.Numerics;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Manager.App.Managers;

public class TurnamentsManager
{
    private readonly MenuActionService _actionService;
    private readonly IPlayerManager _playerManager;
    private readonly ITournamentsService _tournamentsService = new TournamentsService();
    private readonly IPlayerService _playerService;
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;

    public TurnamentsManager(MenuActionService actionService, IPlayerManager playerManager, IPlayerService playerService)
    {
        _actionService = actionService;
        _playerManager = playerManager;
        _playerService = playerService;
        _singlePlayerDuelManager = new SinglePlayerDuelManager(_playerManager, _playerService);
    }

    public void TournamentOptionsView()
    {
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Tournaments");
        while (true)
        {
            ConsoleService.WriteTitle("Tournaments");
            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    //League
                    break;

                case 2:
                    var tournament = CreateNewTournament();
                    if (tournament != null)
                    {
                        if (ConsoleService.AnswerYesOrNo("Start tournament now ?"))
                        {
                            StartTournament(tournament);
                        }
                    }
                    break;

                case 3:
                    StartTournament(SearchTournament());
                    break;

                case 4:
                    AllTournamentsView();
                    break;

                case 5:
                    //delete tournament
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

    public void StartTournament(Tournament tournament)
    {
        if (tournament == null)
        {
            return;
        }
        PlayersToTournament playersToTournament = new(tournament, _tournamentsService);
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Start Tournament");
        while (true)
        {
            ConsoleService.WriteTitle($"Tournaments {tournament.Name} | Game System: {tournament.GameplaySystem}");
            ConsoleService.WriteLineMessage($"Number of PLayers: {tournament.NumberOfPlayer} Number Of Groups: {tournament.NumberOfGroups}\n\r");

            if (tournament.NumberOfPlayer < 8)
            {
                ConsoleService.WriteLineErrorMessage("Minimum 8 players to start the tournament");
            }

            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option", ViewGroupsOr2KO(tournament, playersToTournament));

            switch (operation)
            {
                case 1:
                    AddPlayersToTournament(tournament, playersToTournament);
                    break;

                case 2:
                    RemovePlayerOfTournament(tournament, playersToTournament);
                    break;

                case 3:
                    SetGroups(tournament, playersToTournament);
                    break;

                case 4:
                    //edit groups
                    break;

                case 5:
                    UpdateGamePlaySystem(tournament);
                    break;

                case 6:
                    RandomSelectionOfPlayers(tournament, playersToTournament);
                    break;

                case 7:
                    ViewListPlayersToTournament(tournament, playersToTournament);
                    ConsoleService.GetKeyFromUser();
                    break;

                case 8:
                    //reset
                    break;

                case 9:
                    operation = null;
                    break;

                default:
                    if (operation == null)
                    {
                        if (ConsoleService.AnswerYesOrNo("You want to Exit ?"))
                        {
                            break;
                        }
                        operation = 0;
                    }
                    ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                    break;
            }

            if (operation == null)
            {
                break;
            }
        }
    }

    private void SetGroups(Tournament tournament, PlayersToTournament playersToTournament)
    {
        int numberOfGroups = 0;
        int? enterNumber = 0;
        if (tournament.NumberOfPlayer < 8 || tournament.GameplaySystem == "2KO")
        {
            ConsoleService.WriteLineErrorMessage("2KO Game System Set Change To Group");
            return;
        }
        ConsoleService.WriteTitle($"{tournament.NumberOfPlayer} Players allows the tournament to start:");
        if (string.IsNullOrEmpty(tournament.GameplaySystem))
        {
            tournament.GameplaySystem = "Group";
        }
        if (tournament.GameplaySystem == "Group")
        {
            if (tournament.NumberOfPlayer >= 8 && tournament.NumberOfPlayer < 10)
            {
                ConsoleService.WriteLineMessage("Created 2 groups:\n\r" +
                    "4 players will advance from the group to the knockout round\n\rPress Any Key ...");
                ConsoleService.GetKeyFromUser();
                numberOfGroups = 2;
            }
            else if (tournament.NumberOfPlayer >= 10 && tournament.NumberOfPlayer < 12)
            {
                ConsoleService.WriteLineMessage("Created 2 groups:\n\r" +
                    "4 players will advance from the group to the knockout round\n\rPress Any Key ...");
                ConsoleService.GetKeyFromUser();
                numberOfGroups = 2;
            }
            else if (tournament.NumberOfPlayer >= 12 && tournament.NumberOfPlayer < 16)
            {
                ConsoleService.WriteLineMessage("2 groups, maximum 6 players per group:\n\r" +
                    "4 players will advance from the group to the knockout round\n\r----\n\r");
                ConsoleService.WriteLineMessage("4 groups:\n\r" +
                    "2 players will advance from the group to the knockout round\n\r----\n\r");
                enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 4 or 8");
            }
            else if (tournament.NumberOfPlayer >= 16 && tournament.NumberOfPlayer < 24)
            {
                if (tournament.NumberOfPlayer == 16)
                {
                    ConsoleService.WriteLineMessage("2 groups, maximum 8 players per group:" +
                                "\n\r4 players will advance from the group to the knockout round\n\r----\r\n");
                    ConsoleService.WriteLineMessage("4 groups:\n\r" +
                        "2 players will advance from the group to the knockout round\n\r----\n\r");
                    enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 2 or 4");
                }
                else
                {
                    numberOfGroups = 4;
                }
            }
            else if (tournament.NumberOfPlayer >= 24 && tournament.NumberOfPlayer < 32)
            {
                if (tournament.NumberOfPlayer == 24)
                {
                    ConsoleService.WriteLineMessage("4 groups,maximum 6 players per group:" +
                                "\n\r2 players will advance from the group to the knockout round\n\r----\n\r");
                    ConsoleService.WriteLineMessage("Eight groups\n\r" +
                        "2 players will advance from the group to the knockout round\n\r----\n\r");
                    enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 4 or 8");
                }
                else
                {
                    numberOfGroups = 8;
                }
            }
            else if (tournament.NumberOfPlayer >= 32 && tournament.NumberOfPlayer < 49)
            {
                ConsoleService.WriteLineMessage("Eight groups" +
                    "\n\r2 players will advance from the group to the knockout round\n\r----\n\rPress Any Key ...");
                numberOfGroups = 8;
            }
            else if (tournament.NumberOfPlayer >= 49)
            {
                ConsoleService.WriteLineMessage("Maximum 48 players");
            }
        }

        if (numberOfGroups == 0 && enterNumber == 2 || enterNumber == 4 || enterNumber == 8)
        {
            numberOfGroups = (int)enterNumber;
        }
        else if (numberOfGroups == 0)
        {
            ConsoleService.WriteLineErrorMessage("Something went wrong, please try again");
            return;
        }

        tournament.NumberOfGroups = numberOfGroups;
        RandomSelectionOfPlayers(tournament, playersToTournament);
        _tournamentsService.SaveList();
    }

    private void EditGroups()
    {
    }

    private void RandomSelectionOfPlayers(Tournament tournament, PlayersToTournament playersToTournament)
    {
        var randomList = new List<PlayerToTournament>();
        randomList.AddRange(playersToTournament.ListPlayersToTournament);
        var random = new Random();

        while (randomList.Count > 0)
        {
            var randomNumber = random.Next(0, randomList.Count);
            var playerId = randomList[randomNumber].IdPLayer;
            randomList.RemoveAt(randomNumber);
            var player = playersToTournament.ListPlayersToTournament.First(p => p.IdPLayer == playerId);
            player.TwoKO = (randomList.Count + 1).ToString();
            player.Position = randomList.Count + 1;
        }

        playersToTournament.SavePlayersToTournament();
        randomList.AddRange(playersToTournament.ListPlayersToTournament.OrderBy(p => p.TwoKO));

        char group = (char)65;
        if (tournament.GameplaySystem == "Group" && tournament.NumberOfGroups != 0)
        {
            int numberPlayersOfGroup = randomList.Count / tournament.NumberOfGroups;
            for (int i = 0; i < tournament.NumberOfGroups; i++)
            {
                for (int j = 0; j < numberPlayersOfGroup; j++)
                {
                    randomList[i * numberPlayersOfGroup + j].Group = group.ToString().ToUpper();
                }
                group++;
            }
            var endPlayers = randomList.Count % tournament.NumberOfGroups;
            group = (char)65;

            for (int p = randomList.Count - endPlayers; p < randomList.Count; p++)
            {
                randomList[p].Group = group.ToString().ToUpper();

                if (group == (char)65 + tournament.NumberOfGroups)
                {
                    group = (char)65;
                }
                else
                {
                    group++;
                }
            }
        }

        playersToTournament.ListPlayersToTournament = randomList;
        playersToTournament.SavePlayersToTournament();
    }

    private Tournament? CreateNewTournament()
    {
        IClubManager clubManager = new ClubManager(_actionService);
        Tournament tournament = new Tournament();
        SinglePlayerDuel duel = new SinglePlayerDuel();

        do
        {
            ConsoleService.WriteTitle("Create New Tournament");
            tournament.Name = ConsoleService.GetRequiredStringFromUser("Enter Name ");

            if (string.IsNullOrEmpty(tournament.Name))
            {
                return null;
            }
            else if (_tournamentsService.GetAllItem().Any(t => t.Name.Equals(tournament.Name)))
            {
                ConsoleService.WriteLineErrorMessage("The entered name is already use. Please enter a different name.");
            }
            else
            {
                break;
            }
        } while (true);

        var club = clubManager.SearchClub("Create New Tournament Add Club");
        if (club == null)
        {
            club = clubManager.AddNewClub();
            if (club == null)
            {
                return null;
            }
        }
        tournament.IdClub = club.Id;
        AddGamePlaySystem(tournament);
        if (string.IsNullOrEmpty(tournament.GameplaySystem))
        {
            return null;
        }

        _tournamentsService.AddNewTournament(tournament);
        PlayersToTournament playersToTournament = new PlayersToTournament(tournament, _tournamentsService);
        AddPlayersToTournament(tournament, playersToTournament);
        _tournamentsService.UpdateItem(tournament);
        _tournamentsService.SaveList();
        duel = _singlePlayerDuelManager.NewTournamentSinglePlayerDue(duel, tournament.Id);
        if (duel == null)
        {
            return null;
        }

        return tournament;
    }

    private void AddPlayersToTournament(Tournament tournament, PlayersToTournament playersToTournament)
    {
        List<PlayerToTournament> listPlayersToTournament = playersToTournament.ListPlayersToTournament;

        while (true)
        {
            ViewListPlayersToTournament(tournament, playersToTournament);
            ConsoleService.WriteLineMessage("Press Any Key To Add Next Player \n\r Or Escape (Esc) To Exit");
            var inputKey = ConsoleService.GetKeyFromUser();
            if (inputKey.Key == ConsoleKey.Escape)
            {
                break;
            }
            var player = _playerManager.SearchPlayer("Add Player");
            if (player == null)
            {
                if (ConsoleService.AnswerYesOrNo("You want to add a new player"))
                {
                    player = _playerManager.AddNewPlayer();
                }
                else
                {
                    break;
                }
                if (player == null)
                {
                    return;
                }
            }

            if (listPlayersToTournament.Any(p => p.IdPLayer == player.Id))
            {
                ConsoleService.WriteLineErrorMessage($"The Player {player.FirstName} {player.LastName} is on the list");
            }
            else
            {
                PlayerToTournament newPlayer = new(player, "------");
                var playeraddress = _playerService.GetPlayerAddress(player);

                if (listPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.Group)))
                {
                    var groupingPlayers = listPlayersToTournament
                   .GroupBy(group => group.Group, group => group).OrderBy(p => p.Count());
                    newPlayer.Position = groupingPlayers.First().Count();
                    newPlayer.TwoKO = newPlayer.Position.ToString();
                    newPlayer.Group = groupingPlayers.First().Key;
                }

                if (playeraddress != null)
                {
                    newPlayer.Country = playeraddress.Country;
                }

                listPlayersToTournament.Add(newPlayer);
            }
        }
        tournament.NumberOfPlayer = listPlayersToTournament.Count;
        _tournamentsService.SaveList();

        playersToTournament.SavePlayersToTournament();
    }

    private void RemovePlayerOfTournament(Tournament tournament, PlayersToTournament playersToTournament)
    {
        List<Player> players = new List<Player>();
        foreach (var playerToTournament in playersToTournament.ListPlayersToTournament)
        {
            var player = _playerService.GetItemById(playerToTournament.IdPLayer);
            if (player != null)
            {
                players.Add(player);
            }
        }
        if (players.Count > 0)
        {
            var player = _playerManager.SearchPlayer("Remowe Player", players);
            if (player == null)
            {
                return;
            }
            var playerToRemowe = playersToTournament.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);
            if (playerToRemowe != null)
            {
                playersToTournament.ListPlayersToTournament.Remove(playerToRemowe);
                playersToTournament.SavePlayersToTournament();
                tournament.NumberOfPlayer = playersToTournament.ListPlayersToTournament.Count;
                _tournamentsService.SaveList();
            }
        }
    }

    public void ViewListPlayersToTournament(Tournament tournament, PlayersToTournament playersToTournament)
    {
        if (playersToTournament.ListPlayersToTournament.Any())
        {
            ConsoleService.WriteTitle($"List Players Of {tournament.Name}");
            foreach (var player in playersToTournament.ListPlayersToTournament)
            {
                var formatText = playersToTournament.ListPlayersToTournament
                    .Select(p => new { number = $"{playersToTournament.ListPlayersToTournament.IndexOf(player) + 1}. " }).First().number
                     + $"{player.TinyFulName} {player.Country}";
                ConsoleService.WriteLineMessage(formatText);
            }
        }
        else
        {
            ConsoleService.WriteTitle($"List Players Of {tournament.Name}");
            ConsoleService.WriteLineErrorMessage("Empty List");
        }
    }

    public string ViewGroupsOr2KO(Tournament tournament, PlayersToTournament playersToTournament)
    {
        var formatText = string.Empty;
        if (tournament.GameplaySystem == "2KO")
        {
            int numberOfWriteLine = 0;
            int numberOfItem = 5;
            if (playersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.TwoKO)))
            {
                formatText += $"\n\rStart List 2KO System\n\r\n\r";
                foreach (var player in playersToTournament.ListPlayersToTournament.OrderBy(p => p.Position))
                {
                    if (numberOfWriteLine == numberOfItem + numberOfWriteLine)
                    {
                        formatText += "\n\r";
                    }
                    formatText += $"{player.TwoKO}. {player.TinyFulName}".Remove(20);
                    numberOfItem++;
                }
            }
        }
        else if (tournament.GameplaySystem == "Group")
        {
            if (playersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.TwoKO)))
            {
                if (playersToTournament.ListPlayersToTournament.Any(p => string.IsNullOrEmpty(p.Group)))
                {
                    return formatText;
                }
                var groupingPlayer = playersToTournament.ListPlayersToTournament
                    .GroupBy(group => group.Group, group => group).ToList();

                List<PlayerToTournament> formatList = new List<PlayerToTournament>();
                decimal numberLine = playersToTournament.ListPlayersToTournament.Count / tournament.NumberOfGroups;

                formatText += "\n\r";
                for (int i = 0; i < tournament.NumberOfGroups; i++)
                {
                    formatText += $"Group: {groupingPlayer[i].Key,-23}";
                }

                formatText += "\n\r";
                for (var j = 0; j <= Math.Floor(numberLine); j++)
                {
                    formatText += "\n\r";
                    for (var i = 0; i < tournament.NumberOfGroups; i++)
                    {
                        var player = groupingPlayer[i].Select(p => p).Except(formatList).FirstOrDefault();
                        if (player != null)
                        {
                            formatList.Add(player);
                            formatText += $"{player.TinyFulName}";
                        }
                        else
                        {
                            formatText += $"{" ",-30}";
                        }
                    }
                }
            }
        }

        return formatText;
    }

    public void UpdateGamePlaySystem(Tournament tournament)
    {
        AddGamePlaySystem(tournament);
        _tournamentsService.UpdateItem(tournament);
        _tournamentsService.SaveList();
    }

    private bool AddGamePlaySystem(Tournament tournament)
    {
        string[] settings = ["Gameplay System"];
        var esc = tournament.GameplaySystem.ToString();
        tournament.GameplaySystem = null;
        foreach (string setting in settings)
        {
            if (setting == "Gameplay System")
            {
                var gameplaySystem = new GameplaySystem();
                int idSelectTypeOfGame = 0;
                do
                {
                    ConsoleService.WriteTitle($"Add settings\r\n{setting}");
                    foreach (var game in gameplaySystem.GameplaySystemsList)
                    {
                        var formatText = gameplaySystem.GameplaySystemsList.IndexOf(game) == idSelectTypeOfGame ? $"> {game} <= Select Enter" :
                            $"  {game}";
                        ConsoleService.WriteLineMessage(formatText);
                    }
                    ConsoleKeyInfo inputKey = ConsoleService.GetKeyFromUser();
                    if (inputKey.Key == ConsoleKey.UpArrow && idSelectTypeOfGame > 0)
                    {
                        idSelectTypeOfGame--;
                    }
                    else if (inputKey.Key == ConsoleKey.DownArrow && idSelectTypeOfGame < gameplaySystem.GameplaySystemsList.Count - 1)
                    {
                        idSelectTypeOfGame++;
                    }
                    else if (inputKey.Key == ConsoleKey.Enter)
                    {
                        tournament.GameplaySystem = gameplaySystem.GameplaySystemsList[idSelectTypeOfGame];
                    }
                    else if (inputKey.Key == ConsoleKey.Escape)
                    {
                        tournament.GameplaySystem = esc;
                        return false;
                    }
                } while (tournament.GameplaySystem == null);
            }
        }
        return true;
    }

    public Tournament SearchTournament(string title = "")
    {
        StringBuilder inputString = new StringBuilder();
        List<Tournament> findTournamentTemp = _tournamentsService.SearchTournament(" ")
            .Where(t => t.End == DateTime.MinValue).ToList();
        List<Tournament> findTournament = [];
        List<Tournament> findTournamentToView = [];
        findTournament.AddRange(findTournamentTemp);
        int maxEntriesToDisplay = 15;
        if (!findTournament.Any())
        {
            ConsoleService.WriteLineErrorMessage("Empty List");
            return null;
        }

        if (findTournament.Count >= maxEntriesToDisplay - 1)
        {
            findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
        }
        else
        {
            findTournamentToView = findTournament;
        }
        var address = new Address();
        int indexSelectedTournament = 0;
        title = string.IsNullOrWhiteSpace(title) ? "Search Tournament" : title;

        var headTableToview = title + $"\r\n    {" LP",-5}{"ID",-6}{"Name",-21}" +
                    $"{"Game System",-16}{"Club Name",-21}{"Start",-15}{"End",-15}{"Players",-11}";
        do
        {
            ConsoleService.WriteTitle(headTableToview);

            foreach (var tournament in findTournamentToView)
            {
                var formmatStringToView = findTournament.IndexOf(tournament) == indexSelectedTournament ?
                    "\r\n---> " + $"{findTournament.IndexOf(tournament) + 1,-5}".Remove(4) + _tournamentsService.GetTournamentDetailView(tournament) + $" <----\r\n" :
                    "     " + $"{findTournament.IndexOf(tournament) + 1,-5}".Remove(4) + _tournamentsService.GetTournamentDetailView(tournament);

                ConsoleService.WriteLineMessage(formmatStringToView);
            }

            ConsoleService.WriteLineMessage($"\r\n------(Found {findTournament.Count} Tournament)-------\r\n" + inputString.ToString());
            ConsoleService.WriteLineMessage(@"Enter string move UP or Down  and  press enter to Select");

            var keyFromUser = ConsoleService.GetKeyFromUser("Id Selected Tournament:" + indexSelectedTournament);

            if (char.IsLetterOrDigit(keyFromUser.KeyChar))
            {
                if (findTournament.Count == 1 && !string.IsNullOrEmpty(inputString.ToString()))
                {
                    ConsoleService.WriteLineErrorMessage("No entries found !!!");
                }
                else
                {
                    inputString.Append(keyFromUser.KeyChar);

                    if (inputString.Length == 1)
                    {
                        if (findTournamentTemp.Any(p => $"{p.Id} {p.Name} {p.GameplaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())))
                        {
                            findTournament = [.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} {p.GameplaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                            if (findTournament.Count >= maxEntriesToDisplay - 1)
                            {
                                findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
                            }
                            else
                            {
                                findTournamentToView = findTournament;
                            }
                            indexSelectedTournament = 0;
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            ConsoleService.WriteLineErrorMessage("No entries found !!!");
                        }
                    }
                    else
                    {
                        if (findTournamentTemp.Any(p => $"{p.Id} {p.Name} {p.GameplaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())))
                        {
                            findTournament = [.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} {p.GameplaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                            if (findTournament.Count >= maxEntriesToDisplay - 1)
                            {
                                findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
                            }
                            else
                            {
                                findTournamentToView = findTournament;
                            }
                            indexSelectedTournament = 0;
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            ConsoleService.WriteLineErrorMessage("No entry found !!!");
                        }
                    }
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Backspace && inputString.Length > 0)
            {
                inputString.Remove(inputString.Length - 1, 1);
                findTournament.Clear();
                findTournament.AddRange([.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} {p.GameplaySystem}".ToLower()
                    .Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)]);
                indexSelectedTournament = 0;
            }
            else if (keyFromUser.Key == ConsoleKey.DownArrow && indexSelectedTournament <= findTournament.Count - 2)
            {
                if (indexSelectedTournament >= maxEntriesToDisplay - 1)
                {
                    if (findTournament.IndexOf(findTournamentToView.First()) != findTournament.Count - maxEntriesToDisplay)
                    {
                        var nextPlayer = findTournamentToView.ElementAt(1);
                        var startIndex = findTournament.IndexOf(nextPlayer);
                        findTournamentToView.Clear();
                        findTournamentToView = findTournament.GetRange(startIndex, maxEntriesToDisplay);
                    }
                }
                indexSelectedTournament++;
            }
            else if (keyFromUser.Key == ConsoleKey.UpArrow && indexSelectedTournament > 0)
            {
                if (findTournament.IndexOf(findTournamentToView.First()) != findTournament.IndexOf(findTournament.First()))
                {
                    var nextPlayer = findTournamentToView.First();
                    findTournamentToView.Clear();
                    findTournamentToView = findTournament.GetRange(findTournament.IndexOf(nextPlayer) - 1, maxEntriesToDisplay);
                }
                indexSelectedTournament--;
            }
            else if (keyFromUser.Key == ConsoleKey.Enter && findTournament.Any())
            {
                var findTournamentToSelect = findTournament.First(p => findTournament.IndexOf(p) == indexSelectedTournament);
                ConsoleService.WriteTitle(headTableToview);
                ConsoleService.WriteLineMessage($"{_tournamentsService.GetTournamentDetailView(findTournamentToSelect),106}");

                if (ConsoleService.AnswerYesOrNo("Selected Player"))
                {
                    return findTournamentToSelect;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Escape)
            {
                break;
            }
        } while (true);

        return null;
    }

    public void AllTournamentsView()
    {
        var allTournaments = _tournamentsService.GetAllItem().OrderByDescending(t => t.CreatedDateTime).ToList();
        var headTableToview = "All Tournaments" + $"\r\n{" LP",-5}{"ID",-6}{"Name",-21}" +
                   $"{"Game System",-16}{"Club Name",-21}{"Start",-15}{"End",-15}{"Players",-11}";

        if (allTournaments.Any())
        {
            ConsoleService.WriteTitle(headTableToview);

            foreach (var tournament in allTournaments)
            {
                var formmatStringToView = $" {allTournaments.IndexOf(tournament) + 1,-5}".Remove(5) + _tournamentsService.GetTournamentDetailView(tournament);

                ConsoleService.WriteLineMessage(formmatStringToView);
            }
        }
        else
        {
            ConsoleService.WriteLineErrorMessage("Empty List");
        }
        ConsoleService.WriteLineMessage("Press any key...");
        ConsoleService.GetKeyFromUser();
    }
}