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
        var playSystemMenuList = playSystem.ListMenuActions;
        var optionTournamentGamePlayMenuList = _actionService.GetMenuActionsByName("Go To Tournament");
        optionTournamentGamePlayMenuList.InsertRange(0, playSystemMenuList);
        while (true)
        {
            string message = string.Empty;
            ConsoleService.WriteTitle($"Tournaments {Tournament.Name} | Game System: {Tournament.GamePlaySystem} ");
            ConsoleService.WriteLineMessage($"Number of PLayers: {Tournament.NumberOfPlayer} | Number Of Groups: {Tournament.NumberOfGroups} | " +
                $"Type Name Of Game: {firstDuel.TypeNameOfGame} | " +
                $"Group Race To: {firstDuel.RaceTo}\n\r");

            if (Tournament.NumberOfPlayer < 8)
            {
                ConsoleService.WriteLineMessage("-> Minimum 8 players to start the tournament\n\r");
            }

            for (int i = 0; i < optionTournamentGamePlayMenuList.Count; i++)
            {
                if (optionTournamentGamePlayMenuList[i].Name == "Chenge The Game System" && Tournament.Start != DateTime.MinValue)
                {
                    optionTournamentGamePlayMenuList.Remove(optionTournamentGamePlayMenuList[i]);
                    i--;
                    continue;
                }

                ConsoleService.WriteLineMessage($"{$"{i + 1}.",-3} {optionTournamentGamePlayMenuList[i].Name}");
            }

            if (Tournament.Start == DateTime.MinValue)
            {
                message = playSystem.ViewTournamentBracket();
            }
            else
            {
                var allStartedDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
               .Where(d => d.StartGame != DateTime.MinValue && d.EndGame == DateTime.MinValue).Where(s => s.Interrupted.Equals(DateTime.MinValue)).ToList();
                if (allStartedDuels.Count > 0)
                {
                    message = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allStartedDuels);
                }
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option", message);
            var swichOption = string.Empty;

            if (operation >= 0 && operation <= optionTournamentGamePlayMenuList.Count)
            {
                swichOption = optionTournamentGamePlayMenuList[(int)operation - 1].Name;
            }

            switch (swichOption)
            {
                case "Chenge The Game System":
                    ChangeGamePlaySystem();
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
                    else if (optionTournamentGamePlayMenuList.Any(o => o.Name == swichOption))
                    {
                        playSystem.ExecuteAction(optionTournamentGamePlayMenuList.First(a => a.Name == swichOption));
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
            optionTournamentGamePlayMenuList.Clear();
            optionTournamentGamePlayMenuList = _actionService.GetMenuActionsByName("Go To Tournament");
            playSystemMenuList = playSystem.ListMenuActions;
            optionTournamentGamePlayMenuList.InsertRange(0, playSystemMenuList);
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
}