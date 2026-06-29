using Manager.App.Abstract;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;

namespace Manager.App.Managers.Helpers.TournamentGamePlaySystem;

public abstract class PlaySystems
{
    // <summary>
    // The PlaySystems class is an abstract base class that provides a framework for managing tournament gameplay systems.
    // It defines common properties and methods that can be shared among different tournament play systems.
    // </summary>
    protected Tournament Tournament { get; }

    protected PlayersToTournament PlayersToTournamentInPlaySystem { get; }
    protected readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    protected readonly ITournamentsManager _tournamentsManager;
    protected readonly IPlayerService _playerService;
    protected readonly IPlayerManager _playerManager;

    public List<MenuAction> ListMenuActions
    { get { return GetMenuActions(); } }

    protected PlaySystems(Tournament tournament, ITournamentsManager tournamentsManager, ISinglePlayerDuelManager singlePlayerDuelManager, PlayersToTournament playersToTournament, IPlayerService playerService, IPlayerManager playerManager)
    {
        Tournament = tournament;
        _singlePlayerDuelManager = singlePlayerDuelManager;
        PlayersToTournamentInPlaySystem = playersToTournament;
        _singlePlayerDuelManager = singlePlayerDuelManager;
        _playerService = playerService;
        _playerManager = playerManager;
        _tournamentsManager = tournamentsManager;
        if (tournament.Start != DateTime.MinValue)
        {
            StartTournament();
        }
    }

    protected abstract void MovePlayer();

    protected abstract List<MenuAction> GetExtendedMenuAction();

    protected abstract void ExecuteExtendedAction(MenuAction menuAction);

    protected abstract void RemovePlayers(PlayerToTournament playerToRemove);

    public abstract string ViewTournamentBracket();

    public abstract string GetStatisticsOfText();

    protected abstract void StartTournament();

    protected abstract void StartNextRound();

    protected abstract void EndTournament();

    public abstract void AddPlayers();

    protected abstract void CreateDuelsToTournament(string round = "Eliminations");

    private void EditBracket()
    {
        // Display the menu for editing the tournament bracket
        // The menu options include moving players, resetting the bracket, random selection of players, and exiting the menu
        // If the tournament has already started, the random selection option is removed from the menu
        // The user can perform actions based on their selection, and changes can be saved or discarded before exiting the menu
        // The method also handles the display of the tournament bracket and checks for an empty list of players

        List<MenuAction> optionPlayerMenu =
        [
            new MenuAction(1, "Move Player", "EditBracket"),
            new MenuAction(2, "Reset", "EditBracket"),
            new MenuAction(3, "Random Selection Of Player", "EditBracket"),
            new MenuAction(4, "Exit", "EditBracket")
        ];

        if (Tournament.Start != DateTime.MinValue)
        {
            optionPlayerMenu.Remove(optionPlayerMenu[2]);
        }
        string title = GetType().Name;

        title = title.Remove(title.Length - "PlaySystems".Length + 1);
        var viewBracket = ViewTournamentBracket();
        if (string.IsNullOrEmpty(viewBracket))
        {
            ConsoleService.WriteLineErrorMessage("The tournament bracket is empty.");
            return;
        }

        while (true)
        {
            ConsoleService.WriteTitle(title);

            ConsoleService.WriteLineMessage(ViewTournamentBracket());

            if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count == 0)
            {
                ConsoleService.WriteLineErrorMessage("Empty List Of Player");
                return;
            }
            ConsoleService.WriteLineMessage("\n\r=====================");

            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            int? operation = null;
            if (optionPlayerMenu.Count < 10)
            {
                operation = int.TryParse(ConsoleService.GetKeyFromUser("Enter Option").KeyChar.ToString(), out int parsedOperation) ? parsedOperation : null;
            }
            else
            {
                operation = ConsoleService.GetIntNumberFromUser("Enter Option");
            }
            var action = string.Empty;
            if (operation >= 1 || operation <= optionPlayerMenu.Count)
            {
                action = optionPlayerMenu[(int)operation - 1].Name;
            }

            switch (action)
            {
                case "Move Player":
                    MovePlayer();
                    break;

                case "Reset":
                    PlayersToTournamentInPlaySystem.LoadList(Tournament);
                    break;

                case "Random Selection Of Player":
                    ConsoleService.WriteTitle("");
                    ConsoleService.WriteLineErrorMessage("Attention!");
                    if (ConsoleService.AnswerYesOrNo("It will be impossible to reset the changes made.\r\nDo you want to perform this action?"))
                    {
                        RandomSelectionOfPlayers();
                    }
                    break;

                case "Exit":
                    operation = null;
                    break;

                default:
                    if (operation == null)
                    {
                        if (!ConsoleService.AnswerYesOrNo("Exit To Tournament Menu?"))
                        {
                            ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                            operation = 0;
                        }
                    }
                    break;
            }

            if (operation == null)
            {
                if (ConsoleService.AnswerYesOrNo("Save Changes?"))
                {
                    PlayersToTournamentInPlaySystem.ListPlayersToTournament.OrderBy(p => p.Position);
                    PlayersToTournamentInPlaySystem.SavePlayersToTournament();
                    CreateDuelsToTournament();
                    return;
                }
                else
                {
                    if (!ConsoleService.AnswerYesOrNo("Back to Edit?"))
                    {
                        ConsoleService.WriteLineErrorMessage("Changes Not Save!");
                        return;
                    }
                }
            }
        }
    }

    private List<MenuAction> GetMenuActions()
    {
        // The GetMenuActions method generates a list of menu actions based on the current state of the tournament.
        // It creates a list of MenuAction objects representing different actions that can be performed in the tournament gameplay system.
        // The method checks the tournament's start date, number of players, and number of tables to determine which actions should be available.
        // It also allows for extended menu actions to be added by calling the GetExtendedMenuAction method.
        // The resulting list of menu actions is returned for use in the tournament gameplay system.

        List<MenuAction> listAction =
        [
            new MenuAction(0, "  <-----  Start Tournament", "PlaySystem"),
            new MenuAction(1, "Start Duel","PlaySystem"),
            new MenuAction(2, "Interrupt Duel","PlaySystem"),
            new MenuAction(3, "Update Duel Result", "PlaySystem"),
            new MenuAction(4, "Tournament View", "PlaySystem"),
            new MenuAction(5, "All Duels", "PlaySystem"),
            new MenuAction(6, "Chenge Race To", "PlaySystem"),
            new MenuAction(7, "Change Number Of Table", "PlaySystem"),
            new MenuAction(8, "Add Players", "PlaySystem"),
            new MenuAction(9, "Delete Player", "PlaySystem"),
            new MenuAction(10, "Edit Tournament Bracket", "PlaySystem"),
            new MenuAction(11, "Random Selection Of Players", "PlaySystem"),
            new MenuAction(12, "Players List", "PlaySystem")
        ];

        if (Tournament.Start == DateTime.MinValue)
        {
            listAction.RemoveRange(1, 4);

            if (Tournament.NumberOfPlayer < 8)
            {
                listAction.First(a => a.Name == "Add Players").Name = " <----------  Add More Players";
            }
        }
        if ((Tournament.NumberOfPlayer < 8 || Tournament.NumberOfTables < 1) && listAction.Exists(a => a.Name == "  <-----  Start Tournament"))
        {
            listAction.Remove(listAction.First(a => a.Name == "  <-----  Start Tournament"));
        }

        if (Tournament.NumberOfTables < 1)
        {
            listAction.First(a => a.Name == "Change Number Of Table").Name = "  <-----  Change Number Of Table";
        }

        if (Tournament.Start != DateTime.MinValue)
        {
            if (listAction.Exists(a => a.Name == "Change Number Of Table"))
            {
                listAction.Remove(listAction.First(a => a.Name == "Change Number Of Table"));
            }
            if (listAction.Exists(a => a.Name == "  <-----  Start Tournament"))
            {
                listAction.Remove(listAction.First(a => a.Name == "  <-----  Start Tournament"));
            }

            listAction.Remove(listAction.First(a => a.Name == "Random Selection Of Players"));
            listAction.Remove(listAction.First(a => a.Name == "Chenge Race To"));
            listAction.Remove(listAction.First(a => a.Name == "Add Players"));
            listAction.Remove(listAction.First(a => a.Name == "Delete Player"));
            listAction.Remove(listAction.First(a => a.Name == "Edit Tournament Bracket"));

            if (!_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).Any(d => d.StartGame != DateTime.MinValue))
            {
                listAction.First(a => a.Name == "Start Duel / Interrupt Duel").Name = "  <--------- Start Duel / Interrupt Duel";
            }
            if (Tournament.End != DateTime.MinValue)
            {
                listAction.Remove(listAction.First(a => a.Name == "Start Duel"));
                listAction.Remove(listAction.First(a => a.Name == "Interrupt Duel"));
                listAction.Remove(listAction.First(a => a.Name == "Update Duel Result"));
                listAction.Remove(listAction.First(a => a.Name == "Tournament View"));
            }
        }
        listAction.AddRange(GetExtendedMenuAction());

        return listAction;
    }

    public void ExecuteAction(MenuAction menuAction)
    {
        // The ExecuteAction method executes the selected menu action based on the provided MenuAction object.
        // It determines the action to be performed based on the menuAction's Id and MenuName properties.
        // If the MenuName is "PlaySystem", it performs specific actions related to tournament gameplay,
        // such as starting the tournament, starting duels, interrupting duels, updating duel results, viewing the tournament bracket,
        // displaying player lists, etc.

        var swichOption = menuAction.Id;
        if (menuAction.MenuName == "PlaySystem")
        {
            switch (swichOption)
            {
                case 0:
                    StartTournament();
                    break;

                case 1:
                    StartTournamentDuel();
                    break;

                case 2:
                    InterruptionTournamentDuel();
                    break;

                case 3:
                    UpdateDuelResult();
                    break;

                case 4:
                    ConsoleService.WriteTitle($"{$"Tournament Bracket {Tournament.Name}",72}");
                    ConsoleService.WriteLineMessage(ViewTournamentBracket());
                    ConsoleService.GetKeyFromUser("Press Any Key");
                    break;

                case 5:
                    var allDuelOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id);
                    var listAllDuelsInText = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allDuelOfTournament);
                    ConsoleService.WriteTitle($"All Duels Of Tournament {Tournament.Name}");
                    ConsoleService.WriteLineMessage(listAllDuelsInText);
                    ConsoleService.GetKeyFromUser("Press Any Key...");
                    break;

                case 6:
                    ChangeRaceTo();
                    break;

                case 7:
                    ChangeNumberOfTable();
                    break;

                case 8:
                    AddPlayers();
                    break;

                case 9:
                    RemovePlayerInTournament();
                    break;

                case 10:
                    EditBracket();
                    break;

                case 11:
                    RandomSelectionOfPlayers();
                    break;

                case 12:
                    PlayersToTournamentInPlaySystem.ViewListPlayersToTournament();
                    ConsoleService.GetKeyFromUser();
                    break;

                default:
                    ExecuteExtendedAction(menuAction);
                    break;
            }
        }
        else
        {
            ExecuteExtendedAction(menuAction);
        }
    }

    private void RemovePlayerInTournament()
    {
        // The RemovePlayerInTournament method handles the removal of a player from the tournament.
        // It checks if the tournament has started and displays a warning message to the user about potential disruptions to the group structure.
        // It also checks if the minimum number of players (8) is met before allowing the removal.
        List<Player> players = new List<Player>();

        if (Tournament.Start != DateTime.MinValue)
        {
            ConsoleService.WriteTitle("");
            ConsoleService.WriteLineErrorMessage("Attention!!!");
            if (!ConsoleService.AnswerYesOrNo("Remember that removing a player may disturb the group structure.\n\r" +
                "Players who are currently playing or have finished the match will not appear on the list."))
            {
                return;
            }
        }

        if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count < 8)
        {
            ConsoleService.WriteLineErrorMessage("You cannot remove a player. \n\rThe minimum number of players is 8.");
            return;
        }

        foreach (var playerToTournament in PlayersToTournamentInPlaySystem.ListPlayersToTournament)
        {
            var player = _playerService.GetItemById(playerToTournament.IdPLayer);
            bool isPlayerEndDuelOrPlay = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
           .Exists(p => (p.IdFirstPlayer == player.Id || p.IdSecondPlayer == player.Id) && (p.StartGame != DateTime.MinValue && p.Interrupted == DateTime.MinValue) || p.EndGame != DateTime.MinValue);

            if (player != null && !isPlayerEndDuelOrPlay)
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
            else
            {
                var playerToRemove = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);

                if (Tournament.Start != DateTime.MinValue)
                {
                    RemovePlayers(playerToRemove);
                    PlayersToTournamentInPlaySystem.RemovePlayerInTournament(playerToRemove);
                    Tournament.NumberOfPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count;
                    _tournamentsManager.UpdateTournament(Tournament);
                }
                else
                {
                    PlayersToTournamentInPlaySystem.RemovePlayerInTournament(playerToRemove);
                    Tournament.NumberOfPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count;
                    _tournamentsManager.UpdateTournament(Tournament);
                }
            }
        }
    }

    private void ChangeRaceTo()
    {
        // The ChangeRaceTo method allows the user to change the number of games required to win a duel in the tournament.
        // It retrieves the list of duels for the tournament and checks if any matches in the current round have already ended.
        // If so, it displays an error message and prevents the change. Otherwise, it prompts the user to enter a
        // new value for the number of games (between 3 and 20) and updates the RaceTo property for all duels in the current round.

        var duels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).ToList();
        var round = duels.First(d => d.EndGame == DateTime.MinValue).Round;

        if (duels.Any(p => p.Round == round && p.EndGame != DateTime.MinValue))
        {
            ConsoleService.WriteLineErrorMessage("Changing the number of games in this round is impossible because \n\r" +
                "the matches have already ended.");
            return;
        }
        else if (duels == null)
        {
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

    protected void ChangeNumberOfTable()
    {
        // The ChangeNumberOfTable method allows the user to change the number of tables used in the tournament.
        // It first checks if any duels have already started in the tournament.
        // If so, it displays the current number of tables and informs the user that changes to the tournament settings are limited based on ongoing matches.

        ConsoleService.WriteTitle("Chenge Number Of Table\n\r");

        string textToDisplayIfNoDuelsIsStarted = "After entering the number of Tables,\n\r" +
           " the system manages the start of the next match.\n\r" +
           "The user enters the result of the duel\n\r" +
           " and if one of the players reaches the required number of points,\n\r" +
           "the match will end on a given table and a new one will start.\n\r" +
           "After the first round of matches begins,\n\r" +
           "the user can still make changes to the tournament settings,\n\r" +
           "but they are limited depending on the matches being played.";

        if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).Any(d => d.StartGame != DateTime.MinValue))
        {
            textToDisplayIfNoDuelsIsStarted = $"Current number of tables {Tournament.NumberOfTables}";
        }

        var numberOfTable = ConsoleService.GetIntNumberFromUser("Enter Number Of Table", textToDisplayIfNoDuelsIsStarted);

        if (numberOfTable == null || numberOfTable <= 0 || Tournament.NumberOfTables == numberOfTable)
        {
            ConsoleService.WriteLineErrorMessage("Number Of Tables Not Change");
            return;
        }
        else
        {
            Tournament.NumberOfTables = (int)numberOfTable;
            _tournamentsManager.UpdateTournament(Tournament);
            AutomaticStartOrInterruptionTournamentDuel();
        }
    }

    protected virtual void RandomSelectionOfPlayers()
    {
        // The RandomSelectionOfPlayers method randomly assigns positions to players in the tournament bracket.
        // It first checks if there are at least 8 players and if the tournament has not started yet.
        // If these conditions are met, it creates a random list of players and assigns them positions in the bracket.
        // The method then saves the updated list of players to the tournament.

        if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count < 8 || Tournament.Start != DateTime.MinValue)
        {
            return;
        }
        var randomList = new List<PlayerToTournament>();
        randomList.AddRange(PlayersToTournamentInPlaySystem.ListPlayersToTournament);
        var random = new Random();

        while (randomList.Count > 0)
        {
            var randomNumber = random.Next(0, randomList.Count);
            var playerId = randomList[randomNumber].IdPLayer;
            randomList.RemoveAt(randomNumber);
            var player = PlayersToTournamentInPlaySystem.ListPlayersToTournament.First(p => p.IdPLayer == playerId);
            player.Position = randomList.Count + 1;
        }
        PlayersToTournamentInPlaySystem.ListPlayersToTournament = PlayersToTournamentInPlaySystem.ListPlayersToTournament.OrderBy(p => p.Position).ToList();
        PlayersToTournamentInPlaySystem.SavePlayersToTournament();
    }

    private void UpdateDuelResult()
    {
        var duelToUpdate = _singlePlayerDuelManager.SelectStartedDuelByTournament(Tournament.Id);

        if (duelToUpdate != null && PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count != 0)
        {
            var firstPlayer = duelToUpdate.ScoreFirstPlayer;
            var secondPlayer = duelToUpdate.ScoreSecondPlayer;
            do
            {
                ConsoleService.WriteTitle("Update Duel");
                ConsoleService.WriteMessage(_singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(new List<SinglePlayerDuel>() { duelToUpdate }) + "\n\r");

                var resultFirstPlayer = ConsoleService.GetIntNumberFromUser($"\n\rEnter Result For {PlayersToTournamentInPlaySystem.ListPlayersToTournament
                    .First(p => p.IdPLayer == duelToUpdate.IdFirstPlayer).TinyFulName} ");

                if (resultFirstPlayer != null && resultFirstPlayer > -1 && resultFirstPlayer <= duelToUpdate.RaceTo)
                {
                    duelToUpdate.ScoreFirstPlayer = (int)resultFirstPlayer;
                }
                else
                {
                    ConsoleService.WriteLineErrorMessage("Enter The Correct Result");
                    continue;
                }

                var resultSecondPlayer = ConsoleService.GetIntNumberFromUser($"\n\rEnter Result For {PlayersToTournamentInPlaySystem.ListPlayersToTournament
                    .First(p => p.IdPLayer == duelToUpdate.IdSecondPlayer).TinyFulName}");

                if (resultSecondPlayer != null && resultSecondPlayer > -1 && resultSecondPlayer <= duelToUpdate.RaceTo)
                {
                    duelToUpdate.ScoreSecondPlayer = (int)resultSecondPlayer;
                }
                else
                {
                    ConsoleService.WriteLineErrorMessage("Enter The Correct Result Score not changed");
                    continue;
                }

                if (duelToUpdate.ScoreFirstPlayer == duelToUpdate.ScoreSecondPlayer && duelToUpdate.ScoreFirstPlayer == duelToUpdate.RaceTo)
                {
                    duelToUpdate.ScoreFirstPlayer = firstPlayer;
                    duelToUpdate.ScoreSecondPlayer = secondPlayer;

                    ConsoleService.WriteLineErrorMessage("Enter The Correct Result Score not changed");
                    continue;
                }
            }
            while (ConsoleService.AnswerYesOrNo("You Want To Correct Entered Results?"));

            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToUpdate);
            if ((duelToUpdate.ScoreFirstPlayer == duelToUpdate.RaceTo || duelToUpdate.ScoreSecondPlayer == duelToUpdate.RaceTo) && duelToUpdate.ScoreSecondPlayer != duelToUpdate.ScoreFirstPlayer)
            {
                _singlePlayerDuelManager.EndSinglePlayerDuel(duelToUpdate);
                AutomaticStartOrInterruptionTournamentDuel();
            }
            else if (duelToUpdate.ScoreSecondPlayer == duelToUpdate.ScoreFirstPlayer && duelToUpdate.ScoreFirstPlayer == duelToUpdate.RaceTo)
            {
                duelToUpdate.ScoreFirstPlayer = firstPlayer;
                duelToUpdate.ScoreSecondPlayer = secondPlayer;
            }
        }
    }

    public void StartTournamentDuel()
    {
        // The StartTournamentDuel method is responsible for starting a duel in the tournament.
        // It retrieves the list of duels for the tournament and checks if there are any duels that have already started.
        // If there are duels that have started, it checks if the number of started duels is less than the number of tables available in the tournament.
        // If there are duels that can be started, it selects a duel to start and assigns it to a free table.
        // If there are no duels to start or if there are no free tables, it displays appropriate error messages to the user.

        var duelsOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .Where(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer != -1).ToList();
        var round = duelsOfTournament.Count > 0 ? duelsOfTournament.Last().Round : string.Empty;
        var startedDuels = duelsOfTournament
            .Where(d => d.StartGame != DateTime.MinValue && d.Interrupted == DateTime.MinValue && d.EndGame == DateTime.MinValue && d.Round == round).ToList();

        if (startedDuels.Count < Tournament.NumberOfTables)
        {
            var duelsToStart = duelsOfTournament
                .Where(m => m.StartGame == DateTime.MinValue || (m.StartGame != DateTime.MinValue && m.Interrupted != DateTime.MinValue))
                        .ToList();

            if (duelsToStart.Any())
            {
                foreach (var started in startedDuels)
                {
                    duelsToStart = duelsToStart
                        .Where(
                        d => d.IdFirstPlayer != started.IdFirstPlayer
                        && d.IdFirstPlayer != started.IdSecondPlayer
                        && d.IdSecondPlayer != started.IdFirstPlayer
                        && d.IdSecondPlayer != started.IdSecondPlayer).ToList();
                }
                if (duelsToStart.Any())
                {
                    var selectDuel = _singlePlayerDuelManager.SelectDuel(duelsToStart);

                    if (selectDuel != null)
                    {
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

                        if (freeTables.Count > 0)
                        {
                            selectDuel.TableNumber = freeTables.First();
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(selectDuel);
                            _singlePlayerDuelManager.StartSingleDuel(selectDuel);
                        }
                    }
                }
                else
                {
                    ConsoleService.WriteTitle("");
                    ConsoleService.WriteLineErrorMessage("No duels to start.");
                    ConsoleService.GetKeyFromUser("Press any key...");
                }
            }
            else
            {
                ConsoleService.WriteTitle("");
                ConsoleService.WriteLineErrorMessage("No duels to start.");
                ConsoleService.GetKeyFromUser("Press any key...");
            }
        }
        else
        {
            ConsoleService.WriteTitle("");
            ConsoleService.WriteLineErrorMessage("You cannot start a new duel.");
            ConsoleService.GetKeyFromUser("No free tables. Press any key...");
        }
    }

    public void InterruptionTournamentDuel()
    {
        // The InterruptionTournamentDuel method is responsible for interrupting a duel in the tournament.
        // It retrieves the list of duels for the tournament and checks if there are any duels that have already started and are not interrupted or ended.
        // If there are started duels, it prompts the user to select a duel to interrupt.

        var startedDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .Where(d => d.StartGame != DateTime.MinValue && d.Interrupted == DateTime.MinValue && d.EndGame == DateTime.MinValue).ToList();
        if (startedDuels.Count > 0)
        {
            var duelToIntrrypted = _singlePlayerDuelManager.SelectStartedDuelByTournament(Tournament.Id, "Select Duel To Stop");
            if (duelToIntrrypted != null)
            {
                duelToIntrrypted.TableNumber = 0;
                _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToIntrrypted);
                _singlePlayerDuelManager.InterruptDuel(duelToIntrrypted);
            }
        }
    }

    public void AutomaticStartOrInterruptionTournamentDuel()
    {
        // The AutomaticStartOrInterruptionTournamentDuel method automatically manages the starting and interruption of duels in the tournament.
        // It checks the number of tables available in the tournament and retrieves the list of duels for the tournament.
        // It determines the current round and identifies started duels, completed duels, and duels of the current round.

        if (Tournament.NumberOfTables == 0)
        {
            ChangeNumberOfTable();
            if (Tournament.NumberOfTables == 0)
            {
                return;
            }
        }

        var duelsOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .Where(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer != -1).ToList();
        var round = duelsOfTournament.Count > 0 ? duelsOfTournament.Last().Round : string.Empty;
        var startedDuels = duelsOfTournament
            .Where(d => d.StartGame != DateTime.MinValue && d.Interrupted == DateTime.MinValue && d.EndGame == DateTime.MinValue && d.Round == round).ToList();
        var completedDuelsOfRound = duelsOfTournament
            .Where(d => d.EndGame != DateTime.MinValue && d.Round == round).ToList();
        var duelsOfRound = duelsOfTournament.Where(d => d.Round == round).ToList();

        if (duelsOfTournament.Any(d => d.StartGame != DateTime.MinValue))
        {
            if (duelsOfRound.Count - completedDuelsOfRound.Count > 0)
            {
                if (startedDuels.Count < Tournament.NumberOfTables)
                {
                    var duelsToStart = duelsOfTournament.Where(m => m.StartGame == DateTime.MinValue || (m.StartGame != DateTime.MinValue && m.Interrupted != DateTime.MinValue))
                        .Except(startedDuels).Except(completedDuelsOfRound).OrderBy(d => d.StartNumberInTournament).ToList();

                    if (duelsToStart.Any())
                    {
                        foreach (var started in startedDuels)
                        {
                            duelsToStart = duelsToStart
                                .Where(
                                d => d.IdFirstPlayer != started.IdFirstPlayer
                                && d.IdFirstPlayer != started.IdSecondPlayer
                                && d.IdSecondPlayer != started.IdFirstPlayer
                                && d.IdSecondPlayer != started.IdSecondPlayer).ToList();
                        }

                        if (duelsToStart.Any())
                        {
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
                }
                else
                {
                    for (int i = startedDuels.Count - Tournament.NumberOfTables; i > 0; i--)
                    {
                        var duelToIntrrypted = _singlePlayerDuelManager
                            .SelectStartedDuelByTournament(Tournament.Id, "Select Duel To Stop", $" {i} games left to stop");

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
            }
        }
        if (duelsOfRound.Count > 0 && duelsOfRound.All(d => d.EndGame != DateTime.MinValue))
        {
            StartNextRound();
        }
    }
}