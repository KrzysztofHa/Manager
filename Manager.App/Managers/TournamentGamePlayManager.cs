using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers;
using Manager.App.Managers.Helpers.GamePlaySystem;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.Runtime.Intrinsics.X86;

namespace Manager.App.Managers;

public class TournamentGamePlayManager
{
    private readonly IPlayerManager _playerManager;
    private readonly ITournamentsManager _tournamentsManager;
    private readonly IPlayerService _playerService;
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    private readonly MenuActionService _actionService;

    public Tournament Tournament { get; }
    public readonly List<string> GamePlaySystemsList = new List<string>() { "Group", "2KO" };
    public List<PlayerToTournament> PLayersList { get; }
    private PlayersToTournament playersToTournament;
    private PlaySystem playSystem;

    public TournamentGamePlayManager(Tournament tournament, ITournamentsManager tournamentsManager, MenuActionService actionService, IPlayerManager playerManager, IPlayerService playerService, ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        _tournamentsManager = tournamentsManager;
        _actionService = actionService;
        _playerManager = playerManager;
        _playerService = playerService;
        _singlePlayerDuelManager = singlePlayerDuelManager;
        Tournament = tournament;
        playersToTournament = new PlayersToTournament(tournament, _tournamentsManager);
        PLayersList = playersToTournament.ListPlayersToTournament;
        if (tournament.GamePlaySystem == "Group")
        {
            playSystem = new GroupPlaySystem(Tournament, _tournamentsManager, _singlePlayerDuelManager);
        }
        else
        {
            playSystem = new TwoKOPlaySystem(Tournament, _tournamentsManager, _singlePlayerDuelManager);
        }

        CheckTournament();
    }

    public void GoToTournament()
    {
        if (Tournament.End != DateTime.MinValue)
        {
            return;
        }
        else if (Tournament.Start != DateTime.MinValue && Tournament.Interrupt != DateTime.MinValue)
        {
            _tournamentsManager.StartTournament(Tournament);
        }
        PlayersToTournament playersToTournament = new(Tournament, _tournamentsManager);

        var firstDuel = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).FirstOrDefault();
        var optionPlayerMenuList = _actionService.GetMenuActionsByName("Go To Tournament");
        var skipOption = _actionService.GetMenuActionsByName("Start Tournament").Count - 1;
        if (firstDuel != null)
        {
            skipOption = 0;
            optionPlayerMenuList.InsertRange(0, _actionService.GetMenuActionsByName("Start Tournament").Where(o => o.Name != "Exit"));
        }

        while (true)
        {
            string messageToDo = "\n\rTo Do List:";
            ConsoleService.WriteTitle($"Tournaments {Tournament.Name} | Game System: {Tournament.GamePlaySystem} ");
            ConsoleService.WriteLineMessage($"Number of PLayers: {Tournament.NumberOfPlayer} | Number Of Groups: {Tournament.NumberOfGroups} | " +
                $"Type Name Of Game: {firstDuel.TypeNameOfGame} | " +
                $"Group Race To: {firstDuel.RaceTo}\n\r");

            var message = string.Empty;
            if (Tournament.NumberOfPlayer < 8)
            {
                ConsoleService.WriteLineMessage("-> Minimum 8 players to start the tournament\n\r");
            }

            for (int i = 0; i < optionPlayerMenuList.Count; i++)
            {
                if (optionPlayerMenuList[i].Name == "Add Players" && Tournament.NumberOfPlayer < 8)
                {
                    message = " <---------- Add More Players";
                    messageToDo += "\n\r    Add More Players";
                }
                else if (Tournament.NumberOfGroups == 0 && Tournament.GamePlaySystem == "Group" && optionPlayerMenuList[i].Name == "Set Number Of Groups")
                {
                    message = "  <--- Set";
                    messageToDo += $"\n\r    Set Number Of Groups";
                }
                else if (optionPlayerMenuList[i].Name == "Start Tournament" && firstDuel == null)
                {
                    message = " <----- Start Tournament";
                }
                else if (optionPlayerMenuList[i].Name == "Chenge The Game System" && firstDuel != null)
                {
                    optionPlayerMenuList.Remove(optionPlayerMenuList[i]);
                    i--;
                    continue;
                }
                else if (optionPlayerMenuList[i].Name == "Start Tournament" && firstDuel != null)
                {
                    optionPlayerMenuList.Remove(optionPlayerMenuList[i]);
                    i--;
                    continue;
                }
                else if (optionPlayerMenuList[i].Name == "Random Selection Of Players" && _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).Any(d => d.StartGame != DateTime.MinValue))
                {
                    optionPlayerMenuList.Remove(optionPlayerMenuList[i]);
                    i--;
                    continue;
                }

                ConsoleService.WriteLineMessage($"{$"{i + 1}.",-3} {optionPlayerMenuList[i].Name} {message}");
                message = string.Empty;
            }

            if (Tournament.Start == DateTime.MinValue)
            {
                messageToDo = playSystem.ViewTournamentBracket(playersToTournament);
            }
            else
            {
                var allStartedDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
               .Where(d => d.StartGame != DateTime.MinValue && d.EndGame == DateTime.MinValue).Where(s => s.Interrupted.Equals(DateTime.MinValue)).ToList();
                if (allStartedDuels.Count > 0)
                {
                    messageToDo = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allStartedDuels);
                }
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option", messageToDo);
            var swichOption = string.Empty;

            if (operation >= 0 && operation < optionPlayerMenuList.Count)
            {
                swichOption = optionPlayerMenuList[(int)operation - 1].Name;
            }

            switch (swichOption)
            {
                case "Start Duel / Interrupt Duel":
                    //"Start Duel / Interrupt Duel"
                    break;

                case "Update Duel Result":
                    //Update Duel Result
                    break;

                case "Tournament View":
                    ConsoleService.WriteTitle(playSystem.ViewTournamentBracket(playersToTournament));
                    ConsoleService.GetKeyFromUser("Press Any Key");
                    break;

                case "All Duels":
                    var allDuelOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id);
                    var listAllDuelsInText = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allDuelOfTournament);
                    ConsoleService.WriteTitle($"All Duels Of Tournament {Tournament.Name}");
                    ConsoleService.WriteLineMessage(listAllDuelsInText);
                    ConsoleService.GetKeyFromUser("Press Any Key...");
                    break;

                case "Chenge Race To":
                    ChangeRaceTo();
                    break;

                case "Add Players":
                    AddPlayersToTournament();
                    break;

                case "Delete Player":
                    RemovePlayerInTournament();
                    break;

                case "Move Player":
                    //EditGroupsOr2KOList(Tournament, playersToTournament);
                    MovePlayer();
                    break;

                case "Random Selection Of Players":
                    RandomSelectionOfPlayers();
                    break;

                case "Players List":
                    ViewListPlayersToTournament();
                    ConsoleService.GetKeyFromUser();
                    break;

                case "Set Number Of Groups":
                    if (Tournament.GamePlaySystem != "TwoKO")
                    {
                        //SetGroups(Tournament, playersToTournament);
                    }
                    break;

                case "Chenge The Game System":
                    var gamePlaySystem = GetGamePlaySystemFromUser();
                    Tournament.GamePlaySystem = string.IsNullOrEmpty(gamePlaySystem) ? Tournament.GamePlaySystem : gamePlaySystem;
                    break;

                case "Start Tournament":
                    if (Tournament.Start == DateTime.MinValue)
                    {
                        ConsoleService.WriteTitle($"Start Tournament {Tournament.Name}");
                        if (ConsoleService.AnswerYesOrNo("Before you proceed, make sure is correct everything."))
                        {
                            playSystem.StartTournament();
                        }
                    }
                    break;

                case "Exit":
                    operation = null;
                    break;

                default:
                    if (operation == null)
                    {
                        if (ConsoleService.AnswerYesOrNo("You want to Exit?"))
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
                _tournamentsManager.InterruptTournament(Tournament);
                break;
            }
        }
    }

    private void ChangeNumberOfTable(Tournament tournament, PlayersToTournament playersToTournament)
    {
        ConsoleService.WriteTitle("Chenge Number Of Table\n\r");

        string textToDisplayIfNoDuelsIsStarted = "After entering the number of Tables,\n\r" +
           " the system manages the start of the next match.\n\r" +
           "The user enters the result of the duel\n\r" +
           " and if one of the players reaches the required number of points,\n\r" +
           "the match will end on a given table and a new one will start.\n\r" +
           "After the first round of matches begins,\n\r" +
           "the user can still make changes to the tournament settings,\n\r" +
           "but they are limited depending on the matches being played.";

        if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id).Any(d => d.StartGame != DateTime.MinValue))
        {
            textToDisplayIfNoDuelsIsStarted = $"Current number of tables {tournament.NumberOfTables}";
        }

        var numberOfTable = ConsoleService.GetIntNumberFromUser("Enter Number Of Table", textToDisplayIfNoDuelsIsStarted);

        if (numberOfTable == null || numberOfTable <= 0 || tournament.NumberOfTables == numberOfTable)
        {
            ConsoleService.WriteLineErrorMessage("Number Of Tables Not Change");
            return;
        }
        else
        {
            tournament.NumberOfTables = (int)numberOfTable;
            _tournamentsManager.UpdateTournament(tournament);

            //StartAndInterruptedTournamentDuel(tournament, playersToTournament);
        }
    }

    private void ChangeRaceTo()
    {
        var duels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).ToList();
        var round = duels.First(d => d.EndGame == DateTime.MinValue).Round;

        if (duels.Any(p => p.Round == round && p.EndGame != DateTime.MinValue))
        {
            ConsoleService.WriteLineErrorMessage("Changing the number of games in this round is impossible because \n\r" +
                "the matches have already ended.");
            return;
        }
        ConsoleService.WriteTitle("Change Race To");
        var raceTo = ConsoleService.GetIntNumberFromUser("Enter To Many frame Min 3 Max 20:");
        if (raceTo == null || raceTo < 3 || raceTo > 20)
        {
            return;
        }

        if (!string.IsNullOrEmpty(round))
        {
            duels = duels.Where(d => d.Round == round).ToList();
        }

        foreach (var duel in duels)
        {
            duel.RaceTo = (int)raceTo;
            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
        }
    }

    public void StartTournament()
    {
    }

    public void StartOrInterruptedTournamentDuel()
    {
        if (Tournament.NumberOfTables == 0)
        {
            ChangeNumberOfTable(Tournament, playersToTournament);
            return;
        }

        var duelsOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .Where(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer != -1).ToList();
        var startedDuels = duelsOfTournament.Where(d => d.StartGame != DateTime.MinValue && d.Interrupted == DateTime.MinValue && d.EndGame == DateTime.MinValue).ToList();
        var completedDuels = duelsOfTournament.Where(d => d.EndGame != DateTime.MinValue).ToList();

        if (duelsOfTournament.Any(d => d.StartGame != DateTime.MinValue))
        {
            if (duelsOfTournament.Count - completedDuels.Count > startedDuels.Count)
            {
                if (startedDuels.Count == Tournament.NumberOfTables)
                {
                    var duelToStart = _singlePlayerDuelManager.SelectDuelToStartByTournament(Tournament.Id, "Select Duel To Start");

                    if (duelToStart != null)
                    {
                        var duelToIntrrypted = _singlePlayerDuelManager.SelectStartedDuelByTournament(Tournament.Id, "Select Duel To Stop");
                        if (duelToIntrrypted != null)
                        {
                            duelToStart.TableNumber = duelToIntrrypted.TableNumber;
                            duelToIntrrypted.TableNumber = 0;
                            _singlePlayerDuelManager.InterruptDuel(duelToIntrrypted);
                            _singlePlayerDuelManager.StartSingleDuel(duelToStart);
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToIntrrypted);
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToStart);
                        }
                    }
                    return;
                }

                if (startedDuels.Count < Tournament.NumberOfTables)
                {
                    var duelsToStart = duelsOfTournament.Except(startedDuels).Except(completedDuels).OrderBy(d => d.StartNumberInGroup).ToList();

                    if (duelsToStart.Any())
                    {
                        foreach (var started in startedDuels)
                        {
                            duelsToStart = duelsToStart
                                .Where(
                                d => d.IdFirstPlayer != started.IdFirstPlayer
                                && d.IdFirstPlayer != started.IdSecondPlayer
                                && d.IdSecondPlayer != started.IdFirstPlayer
                                && d.IdSecondPlayer != started.IdSecondPlayer).ToList(); ;
                        }

                        List<int> freeTables = new List<int>();

                        for (int i = 0; i < Tournament.NumberOfTables; i++)
                        {
                            if (startedDuels.Any(d => d.TableNumber == i + 1))
                            {
                                continue;
                            }
                            else
                            {
                                freeTables.Add(i + 1);
                            }
                        }

                        var numberDuelsTostart = freeTables.Count;

                        if (freeTables.Count > duelsToStart.Count)
                        {
                            numberDuelsTostart = duelsToStart.Count;
                        }

                        for (int i = 0; i < numberDuelsTostart; i++)
                        {
                            duelsToStart[i].TableNumber = freeTables[i];
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelsToStart[i]);
                            _singlePlayerDuelManager.StartSingleDuel(duelsToStart[i]);
                        }
                    }
                }
                else
                {
                    for (int i = startedDuels.Count - Tournament.NumberOfTables; i > 0; i--)
                    {
                        var duelToIntrrypted = _singlePlayerDuelManager.SelectStartedDuelByTournament(Tournament.Id, "Select Duel To Stop", $" {i} games left to stop");
                        if (duelToIntrrypted != null)
                        {
                            duelToIntrrypted.TableNumber = 0;
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToIntrrypted);
                            _singlePlayerDuelManager.InterruptDuel(duelToIntrrypted);
                        }
                        else
                        {
                            for (int j = i - 1; j >= 0; j--)
                            {
                                var startedDuel = startedDuels[i];
                                startedDuel.TableNumber = 0;
                                _singlePlayerDuelManager.UpdateSinglePlayerDuel(startedDuel);
                                _singlePlayerDuelManager.InterruptDuel(startedDuel);
                            }
                        }
                    }
                }

                return;
            }
        }
        else
        {
            if (Tournament.GamePlaySystem == "Group")
            {
                var tableNumber = 1;
                foreach (var duelToStart in duelsOfTournament.OrderBy(d => d.StartNumberInGroup))
                {
                    if (tableNumber <= Tournament.NumberOfTables)
                    {
                        tableNumber++;
                    }
                    else
                    {
                        break;
                    }

                    duelToStart.TableNumber = tableNumber;
                    _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToStart);
                    _singlePlayerDuelManager.StartSingleDuel(duelToStart);
                }
            }
            else
            {
                var tableNumber = 0;
                foreach (var duel in duelsOfTournament)
                {
                    if (tableNumber <= Tournament.NumberOfTables)
                    {
                        tableNumber++;
                    }
                    else
                    {
                        break;
                    }

                    duel.TableNumber = tableNumber;
                    _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
                    _singlePlayerDuelManager.StartSingleDuel(duel);
                }
            }
        }
    }

    public string CheckTournament()
    {
        return string.Empty;
    }

    public string GetGamePlaySystemFromUser()
    {
        string[] settings = ["Gameplay System"];
        string gamePlaySystem = string.Empty;

        foreach (string setting in settings)
        {
            if (setting == "Gameplay System")
            {
                int idSelectTypeOfGame = 0;
                do
                {
                    ConsoleService.WriteTitle($"Add settings\r\n{setting}");
                    foreach (var game in GamePlaySystemsList)
                    {
                        var formatText = GamePlaySystemsList.IndexOf(game) == idSelectTypeOfGame ? $"> {game} <= Select Enter" :
                            $"  {game}";
                        ConsoleService.WriteLineMessage(formatText);
                    }
                    ConsoleKeyInfo inputKey = ConsoleService.GetKeyFromUser();
                    if (inputKey.Key == ConsoleKey.UpArrow && idSelectTypeOfGame > 0)
                    {
                        idSelectTypeOfGame--;
                    }
                    else if (inputKey.Key == ConsoleKey.DownArrow && idSelectTypeOfGame < GamePlaySystemsList.Count - 1)
                    {
                        idSelectTypeOfGame++;
                    }
                    else if (inputKey.Key == ConsoleKey.Enter)
                    {
                        gamePlaySystem = GamePlaySystemsList[idSelectTypeOfGame];
                    }
                    else if (inputKey.Key == ConsoleKey.Escape)
                    {
                        return gamePlaySystem;
                    }
                } while (string.IsNullOrEmpty(gamePlaySystem));
            }
        }
        return gamePlaySystem;
    }

    protected virtual void RandomSelectionOfPlayers()
    {
        if (PLayersList.Count < 8)
        {
            return;
        }
        var randomList = new List<PlayerToTournament>();
        randomList.AddRange(PLayersList);
        var random = new Random();

        while (randomList.Count > 0)
        {
            var randomNumber = random.Next(0, randomList.Count);
            var playerId = randomList[randomNumber].IdPLayer;
            randomList.RemoveAt(randomNumber);
            var player = PLayersList.First(p => p.IdPLayer == playerId);
            player.TwoKO = (randomList.Count + 1).ToString();
            player.Position = randomList.Count + 1;
        }
        playersToTournament.ListPlayersToTournament = PLayersList.OrderBy(p => p.Position).ToList();
        playersToTournament.SavePlayersToTournament();
    }

    public void AddPlayersToTournament()
    {
        List<Player> players = new List<Player>();

        foreach (var playerToTournament in PLayersList)
        {
            var tournamentPlayer = _playerService.GetItemById(playerToTournament.IdPLayer);
            if (tournamentPlayer != null)
            {
                players.Add(tournamentPlayer);
            }
        }

        var player = _playerManager.SearchPlayer($"Add Player To tournament {Tournament.Name}" +
            $"\n\rSelect Player On List Or Press Esc To Add New Player",
            null,
            players);

        if (player == null)
        {
            if (ConsoleService.AnswerYesOrNo("You want to add a new player"))
            {
                player = _playerManager.AddNewPlayer();
            }

            if (player == null)
            {
                return;
            }
        }
        var playeraddress = _playerService.GetPlayerAddress(player);

        if (PLayersList.Any(p => p.IdPLayer == player.Id))
        {
            ConsoleService.WriteLineErrorMessage($"The Player {player.FirstName} {player.LastName} is on the list");
        }
        else
        {
            PlayerToTournament newPlayer = new(player, "------");

            if (PLayersList.Any(p => !string.IsNullOrEmpty(p.Group)))
            {
                var groupingPlayers = PLayersList
               .GroupBy(group => group.Group, group => group).OrderBy(g => g.Count());
                newPlayer.Position = PLayersList.Max(p => p.Position) + 1;
                newPlayer.TwoKO = newPlayer.Position.ToString();
                newPlayer.Group = groupingPlayers.First().Key;
            }
            else
            {
                if (PLayersList.Any())
                {
                    newPlayer.Position = PLayersList.Max(p => p.Position) + 1;
                    newPlayer.TwoKO = newPlayer.Position.ToString();
                }
                else
                {
                    newPlayer.Position = 1;
                    newPlayer.TwoKO = newPlayer.Position.ToString();
                }
            }

            if (playeraddress != null)
            {
                newPlayer.Country = playeraddress.Country;
            }

            PLayersList.Add(newPlayer);
            Tournament.NumberOfPlayer = PLayersList.Count;
        }
        _tournamentsManager.UpdateTournament(Tournament);
        playersToTournament.SavePlayersToTournament();

        ConsoleService.WriteTitle("");
        if (ConsoleService.AnswerYesOrNo("Add Next Player"))
        {
            AddPlayersToTournament();
        }
    }

    public void RemovePlayerInTournament()
    {
        List<Player> players = new List<Player>();
        if (PLayersList.Count > 8)
        {
            ConsoleService.WriteTitle("");
            ConsoleService.WriteLineErrorMessage("Attention!!!");
            if (!ConsoleService.AnswerYesOrNo("Remember that removing a player may disturb the group structure.\n\r" +
                "Players who are currently playing or have finished the match will not appear on the list."))
            {
                return;
            }

            foreach (var playerToTournament in PLayersList)
            {
                var player = _playerService.GetItemById(playerToTournament.IdPLayer);
                bool isPlayerEndDuelOrPlay = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
               .Exists(p => (p.IdFirstPlayer == player.Id || p.IdSecondPlayer == player.Id) && (p.StartGame != DateTime.MinValue && p.Interrupted == DateTime.MinValue) || p.EndGame != DateTime.MinValue);

                if (player != null && !isPlayerEndDuelOrPlay)
                {
                    players.Add(player);
                }
            }
        }
        else
        {
            ConsoleService.WriteLineErrorMessage("You cannot remove a player. \n\rThe minimum number of players is 8.");
        }

        if (players.Count > 0)
        {
            var player = _playerManager.SearchPlayer("Remowe Player", players);
            if (player == null)
            {
                return;
            }
            else
            {
                var playerToRemove = PLayersList.FirstOrDefault(p => p.IdPLayer == player.Id);

                if (playerToRemove != null)
                {
                    if (Tournament.GamePlaySystem == "Group")
                    {
                        var groupingPlayer = PLayersList.GroupBy(d => d.Group);
                        if (groupingPlayer.FirstOrDefault(g => g.Key == playerToRemove.Group).Count() <= 2)
                        {
                            ConsoleService.WriteLineErrorMessage("You cannot remove a player. Minimum number of players in group 2.");
                            return;
                        }
                        else
                        {
                            _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToRemove.IdPLayer);
                            PLayersList.Remove(playerToRemove);
                            playersToTournament.SavePlayersToTournament();
                            Tournament.NumberOfPlayer = PLayersList.Count;
                            _tournamentsManager.UpdateTournament(Tournament);
                        }
                    }
                    else
                    {
                        _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToRemove.IdPLayer);
                        PLayersList.Remove(playerToRemove);
                        playersToTournament.SavePlayersToTournament();
                        Tournament.NumberOfPlayer = PLayersList.Count;
                        _tournamentsManager.UpdateTournament(Tournament);
                    }
                }
            }
        }
    }

    public void ViewListPlayersToTournament()
    {
        string formatText = string.Empty;
        if (PLayersList.Any())
        {
            ConsoleService.WriteTitle($"List Players Of {Tournament.Name}");
            foreach (var player in PLayersList)
            {
                formatText = PLayersList
                    .Select(p => new { number = $"{PLayersList.IndexOf(player) + 1}. " }).First().number
                     + $"{player.TinyFulName} {player.Country}";
                ConsoleService.WriteLineMessage(formatText);
            }
        }
        else
        {
            ConsoleService.WriteTitle($"List Players Of {Tournament.Name}");
            ConsoleService.WriteLineErrorMessage("Empty List");
        }
    }

    public string ViewPlayerToTournamentDetail(PlayerToTournament playerToTournament)
    {
        string formatText = string.Empty;
        if (playerToTournament != null)
        {
            formatText = $"Position: {playerToTournament.Position}.  {playerToTournament.TinyFulName} ";
        }
        return formatText;
    }

    private void MovePlayer()
    {
        List<Player> players = new List<Player>();
        foreach (var playerToTournament in PLayersList)
        {
            var player = _playerService.GetItemById(playerToTournament.IdPLayer);
            bool isPlayerEndDuelOrPlay = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
               .Exists(p => (p.IdFirstPlayer == player.Id || p.IdSecondPlayer == player.Id) && (p.StartGame != DateTime.MinValue && p.Interrupted == DateTime.MinValue || p.EndGame != DateTime.MinValue));
            if (player != null && !isPlayerEndDuelOrPlay)
            {
                players.Add(player);
            }
        }

        if (players.Count > 0)
        {
            var player = _playerManager.SearchPlayer("Select Player To Move\n\r" +
                "List of players who can currently be transferred", players);

            if (player == null)
            {
                return;
            }

            var playerToMove = PLayersList.FirstOrDefault(p => p.IdPLayer == player.Id);

            if (playerToMove != null)
            {
                ConsoleService.WriteTitle("Move Player");
                ConsoleService.WriteLineMessage(playSystem.ViewTournamentBracket(playersToTournament));

                if (Tournament.GamePlaySystem == "Group")
                {
                    string groups = string.Empty;

                    var groupingPlayer = PLayersList.GroupBy(p => p.Group);

                    if (groupingPlayer.FirstOrDefault(g => g.Key == playerToMove.Group).Count() <= 2)
                    {
                        ConsoleService.WriteLineErrorMessage("You cannot remove a player. Minimum number of players in group 2.");
                        return;
                    }

                    var duelsPlayerToMove = _singlePlayerDuelManager
                        .GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                        .Where(p => p.IdFirstPlayer == playerToMove.IdPLayer || p.IdSecondPlayer == playerToMove.IdPLayer).ToList();

                    if (Tournament.NumberOfGroups == 2)
                    {
                        var newGroup = groupingPlayer.First(g => g.Key != playerToMove.Group).Key;
                        playerToMove.Group = newGroup;
                    }
                    else
                    {
                        var groupsList = groupingPlayer.Select(g => g.Key)
                                               .Where(group => group != playerToMove.Group)
                                               .ToList();
                        groups = string.Join(",", groupsList);

                        var key = ConsoleService.GetKeyFromUser($"{playerToMove.TinyFulName} Group: {playerToMove.Group}\n\r" +
                            $"Press Key {groups}");
                        var s = key.KeyChar.ToString().ToUpper();
                        if (groupingPlayer.Any(g => g.Key == key.KeyChar.ToString().ToUpper()) && key.KeyChar.ToString().ToUpper() != playerToMove.Group)
                        {
                            playerToMove.Group = key.KeyChar.ToString().ToUpper();
                        }
                    }

                    if (duelsPlayerToMove.Count != 0)
                    {
                        playerToMove.Round = string.Empty;
                        _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToMove.IdPLayer);
                        CreateDuelsToTournament(Tournament, playersToTournament);
                    }
                }
                else
                {
                    if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id) != null)
                    {
                        ConsoleService.WriteLineErrorMessage("Transferring a player is impossible");
                        return;
                    }
                    ConsoleService.WriteTitle("Move Player");
                    ConsoleService.WriteLineMessage(playSystem.ViewTournamentBracket(playersToTournament));
                    var newPosition = ConsoleService.GetIntNumberFromUser("Enter New Position", $"\n\r{ViewPlayerToTournamentDetail(playerToMove)}");

                    if (newPosition > 0 && newPosition != null && newPosition <= PLayersList.Count)
                    {
                        if (newPosition > playerToMove.Position)
                        {
                            for (int i = playerToMove.Position + 1; i <= (int)newPosition; i++)
                            {
                                var playerToChange = PLayersList
                                .First(p => p.Position == i);
                                playerToChange.Position = i - 1;
                                playerToChange.TwoKO = playerToChange.Position.ToString();

                                if (i == newPosition)
                                {
                                    playerToMove.Position = (int)newPosition;
                                    playerToMove.TwoKO = playerToMove.Position.ToString();
                                }
                            }
                        }
                        else if (newPosition < playerToMove.Position)
                        {
                            for (int i = playerToMove.Position - 1; i >= (int)newPosition; i--)
                            {
                                var playerToChange = PLayersList
                                .First(p => p.Position == i);

                                playerToChange.Position = i + 1;
                                playerToChange.TwoKO = playerToChange.Position.ToString();

                                if (i == newPosition)
                                {
                                    playerToMove.Position = (int)newPosition;
                                    playerToMove.TwoKO = playerToMove.Position.ToString();
                                }
                            }
                        }
                    }
                }
            }
            playersToTournament.SavePlayersToTournament();
        }
    }

    private void UpdateDuelResult()
    {
        var duelToUpdate = _singlePlayerDuelManager.SelectStartedDuelByTournament(Tournament.Id);

        if (duelToUpdate != null && PLayersList.Count != 0)
        {
            do
            {
                ConsoleService.WriteTitle("Update Duel");
                ConsoleService.WriteMessage(_singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(new List<SinglePlayerDuel>() { duelToUpdate }) + "\n\r");

                var resultFirstPlayer = ConsoleService.GetIntNumberFromUser($"\n\rEnter Result For {PLayersList
                    .First(p => p.IdPLayer == duelToUpdate.IdFirstPlayer).TinyFulName} ");

                if (resultFirstPlayer != null && resultFirstPlayer > 0 && resultFirstPlayer <= duelToUpdate.RaceTo)
                {
                    duelToUpdate.ScoreFirstPlayer = (int)resultFirstPlayer;
                }

                var resultSecondPlayer = ConsoleService.GetIntNumberFromUser($"\n\rEnter Result For {PLayersList
                    .First(p => p.IdPLayer == duelToUpdate.IdSecondPlayer).TinyFulName}");

                if (resultSecondPlayer != null && resultSecondPlayer > 0 && resultSecondPlayer <= duelToUpdate.RaceTo)
                {
                    duelToUpdate.ScoreSecondPlayer = (int)resultSecondPlayer;
                }
            }
            while (ConsoleService.AnswerYesOrNo("You Want To Correct Entered Results?"));

            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToUpdate);
            if (duelToUpdate.ScoreFirstPlayer == duelToUpdate.RaceTo || duelToUpdate.ScoreSecondPlayer == duelToUpdate.RaceTo)
            {
                _singlePlayerDuelManager.EndSinglePlayerDuel(duelToUpdate);
                StartOrInterruptedTournamentDuel();
            }
        }
    }

    private void CreateDuelsToTournament(Tournament tournament, PlayersToTournament playersToTournament, string round = "Eliminations")
    {
        if (tournament == null || playersToTournament == null)
        {
            return;
        }

        var listNewPlayer = playersToTournament.ListPlayersToTournament.Where(p => p.Round != round).ToList();

        if (listNewPlayer.Count == 0)
        {
            return;
        }

        if (tournament.GamePlaySystem == "Group")
        {
            var groupingPlayers = playersToTournament.ListPlayersToTournament.GroupBy(p => p.Group);

            foreach (var group in groupingPlayers)
            {
                var listPlayersOfGroup = group.ToList();
                if (group.Any(p => p.Round == round))
                {
                    listPlayersOfGroup = listPlayersOfGroup.OrderBy(p => p.Round).ToList();
                }

                for (int i = 0; i < listPlayersOfGroup.Count; i++)
                {
                    if (!listNewPlayer.Contains(listPlayersOfGroup[i]) && listPlayersOfGroup.Any(p => p.Round == round))
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(listPlayersOfGroup[i].Round) || !listPlayersOfGroup[i].Round.Equals(round) || listPlayersOfGroup.LastIndexOf(listPlayersOfGroup[i]) == 0)
                    {
                        listPlayersOfGroup[i].Round = round;
                    }

                    for (int j = i + 1; j < listPlayersOfGroup.Count; j++)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        tournament.Id,
                        listPlayersOfGroup[i].IdPLayer,
                        listPlayersOfGroup[j].IdPLayer,
                        round);
                    }
                }
            }

            playersToTournament.SavePlayersToTournament();
            //DetermineTheOrderOfDuelsToStartInGroup(tournament, playersToTournament);
        }
        else
        {
            if (listNewPlayer.Count == 1 && round == "Eliminations")
            {
                if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id).Any(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1))
                {
                    var duelToNewPlayer = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
                        .First(d => d.IdFirstPlayer > 1 && d.IdSecondPlayer == -1);
                    duelToNewPlayer.IdSecondPlayer = listNewPlayer.First().IdPLayer;
                    duelToNewPlayer.ScoreFirstPlayer = 0;
                    duelToNewPlayer.ScoreSecondPlayer = 0;
                    listNewPlayer.First().Round = round;
                    duelToNewPlayer.Round = round;
                    _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToNewPlayer);
                }
                else
                {
                    var newDuel = _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                           tournament.Id,
                           listNewPlayer.First().IdPLayer,
                           -1,
                           round);
                    newDuel.ScoreFirstPlayer = 3;
                    newDuel.ScoreSecondPlayer = 0;
                    listNewPlayer.First().Round = round;
                }
            }
            else
            {
                for (int i = 0; i < listNewPlayer.Count; i += 2)
                {
                    if (((playersToTournament.ListPlayersToTournament.Count - listNewPlayer.Count) % 2 != 0
                        || playersToTournament.ListPlayersToTournament.Count - listNewPlayer.Count == 0)
                        && listNewPlayer[i].Equals(listNewPlayer.Last()))
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                            tournament.Id,
                            listNewPlayer.Last().IdPLayer,
                            -1,
                            round);
                        listNewPlayer.Last().Round = round;
                        break;
                    }
                    else if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
                        .Any(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1))
                    {
                        var duelToupdate = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
                            .First(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1);
                        duelToupdate.Round = round;
                        duelToupdate.IdSecondPlayer = listNewPlayer[i].IdPLayer;
                        listNewPlayer[i].Round = round;
                        _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToupdate);
                        i--;
                        continue;
                    }
                    else
                    {
                        listNewPlayer[i].Round = round;
                        listNewPlayer[i + 1].Round = round;
                    }

                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                            tournament.Id,
                            listNewPlayer[i].IdPLayer,
                            listNewPlayer[i + 1].IdPLayer,
                            round);
                }
            }
            playersToTournament.SavePlayersToTournament();
        }
    }
}