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
    public readonly List<string> GamePlaySystemsList = new List<string>() { "Group", "2KO" };

    public Tournament Tournament { get; }

    public List<PlayerToTournament> PLayersList
    { get { return _playersToTournament.ListPlayersToTournament; } }

    private PlayersToTournament _playersToTournament;
    private PlaySystems playSystem;

    public TournamentGamePlayManager(Tournament tournament, ITournamentsManager tournamentsManager, MenuActionService actionService, IPlayerManager playerManager, IPlayerService playerService, ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        _tournamentsManager = tournamentsManager;
        _actionService = actionService;
        _playerManager = playerManager;
        _playerService = playerService;
        _singlePlayerDuelManager = singlePlayerDuelManager;
        Tournament = tournament;
        _playersToTournament = new PlayersToTournament(tournament, _tournamentsManager, playerManager, playerService);

        if (string.IsNullOrEmpty(tournament.GamePlaySystem))
        {
            ChangeGamePlaySystem();
            playSystem.AddPlayers();
        }
        else
        {
            if (tournament.GamePlaySystem == "Group")
            {
                playSystem = new GroupPlaySystem(Tournament, _tournamentsManager, _singlePlayerDuelManager, _playersToTournament, _playerService, _playerManager);
            }
            else
            {
                playSystem = new TwoKOPlaySystem(Tournament, _tournamentsManager, _singlePlayerDuelManager, _playersToTournament, _playerService, _playerManager);
            }
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

        var firstDuel = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).FirstOrDefault();
        var optionPlayerMenuList = playSystem.menuActions;
        optionPlayerMenuList.AddRange(_actionService.GetMenuActionsByName("Go To Tournament"));

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
                else if (optionPlayerMenuList[i].Name == "Start Tournament" && Tournament.Start == DateTime.MinValue)
                {
                    message = " <----- Start Tournament";
                    messageToDo += $"\n\r    Start Tournament";
                }
                else if (optionPlayerMenuList[i].Name == "Chenge The Game System" && Tournament.Start != DateTime.MinValue)
                {
                    optionPlayerMenuList.Remove(optionPlayerMenuList[i]);
                    i--;
                    continue;
                }
                else if (optionPlayerMenuList[i].Name == "Start Tournament" && Tournament.Start != DateTime.MinValue)
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

            if (Tournament.Start != DateTime.MinValue)
            {
                messageToDo = playSystem.ViewTournamentBracket();
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
                case "Chenge The Game System":
                    ChangeGamePlaySystem();
                    optionPlayerMenuList = playSystem.menuActions;
                    optionPlayerMenuList.AddRange(_actionService.GetMenuActionsByName("Go To Tournament"));
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
                    else if (optionPlayerMenuList.Any(o => o.Name == swichOption))
                    {
                        playSystem.ExecuteAction(optionPlayerMenuList.First(a => a.Name == swichOption));
                    }
                    else
                    {
                        ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                    }

                    break;
            }

            if (operation == null)
            {
                _tournamentsManager.InterruptTournament(Tournament);
                break;
            }
        }
    }

    private void ChangeGamePlaySystem()
    {
        if (Tournament.Start != DateTime.MinValue)
        {
            return;
        }
        else
        {
            var gamePlaySystem = GetGamePlaySystemFromUser();

            if (string.IsNullOrEmpty(gamePlaySystem) && string.IsNullOrEmpty(Tournament.GamePlaySystem))
            {
                gamePlaySystem = GamePlaySystemsList.First();
            }

            if (gamePlaySystem == GamePlaySystemsList[0] && Tournament.GamePlaySystem != gamePlaySystem)
            {
                Tournament.GamePlaySystem = gamePlaySystem;
                _tournamentsManager.UpdateTournament(Tournament);
                playSystem = new GroupPlaySystem(Tournament, _tournamentsManager, _singlePlayerDuelManager, _playersToTournament, _playerService, _playerManager);
            }
            else if (gamePlaySystem == GamePlaySystemsList[1] && Tournament.GamePlaySystem != gamePlaySystem)
            {
                Tournament.GamePlaySystem = gamePlaySystem;
                _tournamentsManager.UpdateTournament(Tournament);
                playSystem = new TwoKOPlaySystem(Tournament, _tournamentsManager, _singlePlayerDuelManager, _playersToTournament, _playerService, _playerManager);
            }
        }
    }

    public void StartOrInterruptedTournamentDuel()
    {
        if (Tournament.NumberOfTables == 0)
        {
            //ChangeNumberOfTable(Tournament, _playersToTournament);
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
        if (Tournament.Start != DateTime.MinValue)
        {
            return string.Empty;
        }
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
}