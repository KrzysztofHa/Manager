using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers.TournamentGamePlaySystem;

public abstract class PlaySystems
{
    protected Tournament Tournament { get; }
    protected PlayersToTournament PlayersToTournament { get; }
    protected readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    protected readonly ITournamentsManager _tournamentsManager;
    protected readonly IPlayerService _playerService;
    protected readonly IPlayerManager _playerManager;
    public List<MenuAction> menuActions { get; set; }

    protected PlaySystems(Tournament tournament, ITournamentsManager tournamentsManager, ISinglePlayerDuelManager singlePlayerDuelManager, PlayersToTournament playersToTournament, IPlayerService playerService, IPlayerManager playerManager)
    {
        Tournament = tournament;
        _singlePlayerDuelManager = singlePlayerDuelManager;
        PlayersToTournament = playersToTournament;
        _singlePlayerDuelManager = singlePlayerDuelManager;
        _playerService = playerService;
        _playerManager = playerManager;
        _tournamentsManager = tournamentsManager;
        menuActions = GetMenuActions();
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
        ];

        string title = GetType().Name.Replace("PlaySystems", "");

        while (true)
        {
            ConsoleService.WriteTitle(title);
            ConsoleService.WriteLineMessage(ViewTournamentBracket());
            if (PlayersToTournament.ListPlayersToTournament.Count == 0)
            {
                ConsoleService.WriteLineErrorMessage("Empty List Of Player");
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
                    PlayersToTournament.LoadList(Tournament);
                    break;

                case 3:
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
                    PlayersToTournament.SavePlayersToTournament();
                }
                else
                {
                    if (!ConsoleService.AnswerYesOrNo("Back to Edit?"))
                    {
                        ConsoleService.WriteLineErrorMessage("Changes Not Save!");
                        break;
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
            new MenuAction(9, "Edit Bracket", "PlaySystem"),
            new MenuAction(10, "Random Selection Of Players", "PlaySystem"),
            new MenuAction(11, "Players List", "PlaySystem"),
            new MenuAction(12, "Start Tournament", "PlaySystem"),
            .. GetExtendedMenuAction(),
        ];

        if (Tournament.Start == DateTime.MinValue)
        {
            listAction.RemoveRange(0, 6);
        }
        return listAction;
    }

    public void ExecuteAction(MenuAction menuAction)
    {
        var swichOption = menuAction.Name;
        switch (swichOption)
        {
            case "Start Duel / Interrupt Duel":
                //"Start Duel / Interrupt Duel"
                break;

            case "Update Duel Result":
                //Update Duel Result
                break;

            case "Tournament View":
                ConsoleService.WriteTitle(ViewTournamentBracket());
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

            case "Change Number Of Table":
                ChangeNumberOfTable();
                break;

            case "Add Players":
                AddPlayers();
                break;

            case "Delete Player":
                RemovePlayerInTournament();
                break;

            case "Edit Bracket":
                EditBracket();
                break;

            case "Random Selection Of Players":
                RandomSelectionOfPlayers();
                break;

            case "Players List":
                PlayersToTournament.ViewListPlayersToTournament();
                ConsoleService.GetKeyFromUser();
                break;

            case "Set Number Of Groups":
                if (GetType().Name == "GroupPlaySystem")
                {
                }
                break;

            case "Start Tournament":
                if (Tournament.Start == DateTime.MinValue)
                {
                    ConsoleService.WriteTitle($"Start Tournament {Tournament.Name}");
                    if (ConsoleService.AnswerYesOrNo("Before you proceed, make sure is correct everything."))
                    {
                        StartTournament();
                    }
                }
                break;

            default:
                ExecuteExtendedAction(menuAction);
                break;
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

        if (PlayersToTournament.ListPlayersToTournament.Count < 8)
        {
            ConsoleService.WriteLineErrorMessage("You cannot remove a player. \n\rThe minimum number of players is 8.");
            return;
        }

        foreach (var playerToTournament in PlayersToTournament.ListPlayersToTournament)
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
                var playerToRemove = PlayersToTournament.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);

                if (Tournament.Start != DateTime.MinValue)
                {
                    RemovePlayers(playerToRemove);
                    PlayersToTournament.RemovePlayerInTournament(playerToRemove);
                    Tournament.NumberOfPlayer = PlayersToTournament.ListPlayersToTournament.Count;
                    _tournamentsManager.UpdateTournament(Tournament);
                }
                else
                {
                    PlayersToTournament.RemovePlayerInTournament(playerToRemove);
                    Tournament.NumberOfPlayer = PlayersToTournament.ListPlayersToTournament.Count;
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

    private void ChangeNumberOfTable()
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

    private void RandomSelectionOfPlayers()
    {
        if (PlayersToTournament.ListPlayersToTournament.Count < 8 || Tournament.Start != DateTime.MinValue)
        {
            return;
        }
        var randomList = new List<PlayerToTournament>();
        randomList.AddRange(PlayersToTournament.ListPlayersToTournament);
        var random = new Random();

        while (randomList.Count > 0)
        {
            var randomNumber = random.Next(0, randomList.Count);
            var playerId = randomList[randomNumber].IdPLayer;
            randomList.RemoveAt(randomNumber);
            var player = PlayersToTournament.ListPlayersToTournament.First(p => p.IdPLayer == playerId);
            player.TwoKO = (randomList.Count + 1).ToString();
            player.Position = randomList.Count + 1;
        }
        PlayersToTournament.ListPlayersToTournament = PlayersToTournament.ListPlayersToTournament.OrderBy(p => p.Position).ToList();
        PlayersToTournament.SavePlayersToTournament();
    }

    protected void CreateDuelsToTournament(string round = "Eliminations")
    {
        var listNewPlayer = PlayersToTournament.ListPlayersToTournament.Where(p => p.Round != round).ToList();

        if (listNewPlayer.Count == 0)
        {
            return;
        }

        if (Tournament.GamePlaySystem == "Group")
        {
            var groupingPlayers = PlayersToTournament.ListPlayersToTournament.GroupBy(p => p.Group);

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
                        Tournament.Id,
                        listPlayersOfGroup[i].IdPLayer,
                        listPlayersOfGroup[j].IdPLayer,
                        round);
                    }
                }
            }

            PlayersToTournament.SavePlayersToTournament();
            //DetermineTheOrderOfDuelsToStartInGroup(tournament, playersToTournament);
        }
        else
        {
            if (listNewPlayer.Count == 1 && round == "Eliminations")
            {
                if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).Any(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1))
                {
                    var duelToNewPlayer = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
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
                           Tournament.Id,
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
                    if (((PlayersToTournament.ListPlayersToTournament.Count - listNewPlayer.Count) % 2 != 0
                        || PlayersToTournament.ListPlayersToTournament.Count - listNewPlayer.Count == 0)
                        && listNewPlayer[i].Equals(listNewPlayer.Last()))
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                            Tournament.Id,
                            listNewPlayer.Last().IdPLayer,
                            -1,
                            round);
                        listNewPlayer.Last().Round = round;
                        break;
                    }
                    else if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                        .Any(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1))
                    {
                        var duelToupdate = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
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
                            Tournament.Id,
                            listNewPlayer[i].IdPLayer,
                            listNewPlayer[i + 1].IdPLayer,
                            round);
                }
            }
            PlayersToTournament.SavePlayersToTournament();
        }
    }
}