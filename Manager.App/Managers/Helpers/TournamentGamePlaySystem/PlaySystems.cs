using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers.TournamentGamePlaySystem;

public abstract class PlaySystems
{
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

    protected abstract void StartTournament();

    public abstract void AddPlayers();

    private void EditBracket()
    {
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

        while (true)
        {
            ConsoleService.WriteTitle(title);
            ConsoleService.WriteLineMessage(ViewTournamentBracket());

            if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count == 0)
            {
                ConsoleService.WriteLineErrorMessage("Empty List Of Player");
                return;
            }
            else if (Tournament.NumberOfGroups == 0)
            {
                ConsoleService.GetKeyFromUser("Press Any Key...");

                return;
            }
            ConsoleService.WriteLineMessage("\n\r=====================");

            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("\n\rEnter Option");
            switch (operation)
            {
                case 1:
                    MovePlayer();
                    break;

                case 2:
                    PlayersToTournamentInPlaySystem.LoadList(Tournament);
                    break;

                case 3:
                    ConsoleService.WriteTitle("");
                    ConsoleService.WriteLineErrorMessage("Attention!");
                    if (ConsoleService.AnswerYesOrNo("It will be impossible to reset the changes made.\r\nDo you want to perform this action?"))
                    {
                        RandomSelectionOfPlayers();
                    }
                    break;

                case 4:
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
        List<MenuAction> listAction =
        [
            new MenuAction(1, "Start Duel / Interrupt Duel","PlaySystem"),
            new MenuAction(2, "Update Duel Result", "PlaySystem"),
            new MenuAction(3, "Tournament View", "PlaySystem"),
            new MenuAction(4, "All Duels", "PlaySystem"),
            new MenuAction(5, "Chenge Race To", "PlaySystem"),
            new MenuAction(6, "Change Number Of Table", "PlaySystem"),
            new MenuAction(7, "Add Players", "PlaySystem"),
            new MenuAction(8, "Delete Player", "PlaySystem"),
            new MenuAction(9, "Edit Tournament Bracket", "PlaySystem"),
            new MenuAction(10, "Random Selection Of Players", "PlaySystem"),
            new MenuAction(11, "Players List", "PlaySystem"),
            .. GetExtendedMenuAction()
        ];

        if (Tournament.Start == DateTime.MinValue)
        {
            listAction.RemoveRange(0, 6);

            if (Tournament.NumberOfPlayer < 8)
            {
                listAction.First(a => a.Name == "Add Players").Name = " <----------  Add More Players";
            }
        }

        if (Tournament.Start != DateTime.MinValue)
        {
            listAction.Remove(listAction.First(a => a.Name == "Random Selection Of Players"));

            if (!_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).Any(d => d.StartGame != DateTime.MinValue))
            {
                listAction.First(a => a.Name == "Start Duel / Interrupt Duel").Name = "  <--------- Start Duel / Interrupt Duel";
            }
        }

        return listAction;
    }

    public void ExecuteAction(MenuAction menuAction)
    {
        var swichOption = menuAction.Id;
        if (menuAction.MenuName == "PlaySystem")
        {
            switch (swichOption)
            {
                case 1:
                    StartOrInterruptedTournamentDuel();
                    break;

                case 2:
                    UpdateDuelResult();
                    break;

                case 3:
                    ConsoleService.WriteTitle(ViewTournamentBracket());
                    ConsoleService.GetKeyFromUser("Press Any Key");
                    break;

                case 4:
                    var allDuelOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id);
                    var listAllDuelsInText = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allDuelOfTournament);
                    ConsoleService.WriteTitle($"All Duels Of Tournament {Tournament.Name}");
                    ConsoleService.WriteLineMessage(listAllDuelsInText);
                    ConsoleService.GetKeyFromUser("Press Any Key...");
                    break;

                case 5:
                    ChangeRaceTo();
                    break;

                case 6:
                    ChangeNumberOfTable();
                    break;

                case 7:
                    AddPlayers();
                    break;

                case 8:
                    RemovePlayerInTournament();
                    break;

                case 9:
                    EditBracket();
                    break;

                case 10:
                    RandomSelectionOfPlayers();
                    break;

                case 11:
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

            //StartAndInterruptedTournamentDuel(tournament, playersToTournament);
        }
    }

    protected virtual void RandomSelectionOfPlayers()
    {
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
            do
            {
                ConsoleService.WriteTitle("Update Duel");
                ConsoleService.WriteMessage(_singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(new List<SinglePlayerDuel>() { duelToUpdate }) + "\n\r");

                var resultFirstPlayer = ConsoleService.GetIntNumberFromUser($"\n\rEnter Result For {PlayersToTournamentInPlaySystem.ListPlayersToTournament
                    .First(p => p.IdPLayer == duelToUpdate.IdFirstPlayer).TinyFulName} ");

                if (resultFirstPlayer != null && resultFirstPlayer > 0 && resultFirstPlayer <= duelToUpdate.RaceTo)
                {
                    duelToUpdate.ScoreFirstPlayer = (int)resultFirstPlayer;
                }

                var resultSecondPlayer = ConsoleService.GetIntNumberFromUser($"\n\rEnter Result For {PlayersToTournamentInPlaySystem.ListPlayersToTournament
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

    public void StartOrInterruptedTournamentDuel()
    {
        if (Tournament.NumberOfTables == 0)
        {
            ChangeNumberOfTable();
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
                }
                else if (startedDuels.Count < Tournament.NumberOfTables)
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
}