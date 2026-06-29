using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers;
using Manager.App.Managers.Helpers.GamePlaySystem;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.App.Managers.Helpers.TypeOfplayPlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;

namespace Manager.App.Managers;

public class TournamentGamePlayManager
{
    private readonly IPlayerManager _playerManager;
    private readonly ITournamentsManager _tournamentsManager;
    private readonly IPlayerService _playerService;
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    private readonly MenuActionService _actionService;
    public readonly TypeOfPlaySystem ListGamePlaySystem;
    private readonly PlaySystemManager _playSystemManager;

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
        _playSystemManager = new PlaySystemManager(_tournamentsManager, _singlePlayerDuelManager, _playersToTournament, _playerService, _playerManager);
        ListGamePlaySystem = _playSystemManager.ListGamePlaySystem;

        if (string.IsNullOrEmpty(tournament.GamePlaySystem))
        {
            ChangeGamePlaySystem();
            playSystem.AddPlayers();
        }
        else
        {
            playSystem = _playSystemManager.CreateNewPlaySystem(Tournament);
        }
    }

    public void GoToTournament()
    {
        if (Tournament.Start != DateTime.MinValue && Tournament.Interrupt != DateTime.MinValue)
        {
            _tournamentsManager.StartTournament(Tournament);
        }

        var firstDuel = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).FirstOrDefault();
        var playSystemMenuList = playSystem.ListMenuActions;
        var optionTournamentGamePlayMenuList = _actionService.GetMenuActionsByName("Go To Tournament");
        optionTournamentGamePlayMenuList.InsertRange(0, playSystemMenuList);
        while (true)
        {
            var round = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).LastOrDefault();
            string message = string.Empty;
            string messageBack = string.Empty;
            var numberOfGroups = string.Empty;
            if (Tournament.NumberOfGroups != 0)
            {
                numberOfGroups = $" | Number Of Groups: {Tournament.NumberOfGroups}";
            }
            string title = $"Tournaments {Tournament.Name} | Game System: {Tournament.GamePlaySystem} | Round: {round.Round}\n\r" +
                $"Number of PLayers: {Tournament.NumberOfPlayer}" +
                $" | Type Name Of Game: {firstDuel.TypeNameOfGame} | {numberOfGroups}" +
                $"Race To: {firstDuel.RaceTo} | Nr. of Tabes {Tournament.NumberOfTables}";

            for (int i = 0; i < optionTournamentGamePlayMenuList.Count; i++)
            {
                if (optionTournamentGamePlayMenuList[i].Name == "Chenge The Game System" && Tournament.Start != DateTime.MinValue)
                {
                    optionTournamentGamePlayMenuList.Remove(optionTournamentGamePlayMenuList[i]);
                    i--;
                    continue;
                }

                messageBack += ($"{$"{i + 1}.",-3} {optionTournamentGamePlayMenuList[i].Name}\n\r");
            }

            if (Tournament.Start == DateTime.MinValue)
            {
                message = playSystem.ViewTournamentBracket();
            }
            else
            {
                var allStartedDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                    .Where(d => d.StartGame != DateTime.MinValue && d.EndGame == DateTime.MinValue)
                    .Where(s => s.Interrupted.Equals(DateTime.MinValue)).ToList();

                if (allStartedDuels.Count > 0)
                {
                    title += $"\n\r\n\r{$"Currently Ongoing Duels",60}\n\r{$"-----------------------",60}";
                    message = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allStartedDuels);
                }
                else if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                    .All(d => d.EndGame != DateTime.MinValue && d.Round == "Eliminations"))
                {
                    message = playSystem.GetStatisticsOfText();
                    ConsoleService.GetKeyFromUser("Press any key to continue...");
                }
                else
                {
                    message = playSystem.ViewTournamentBracket();
                }
            }
            ConsoleService.WriteTitle(title);
            if (Tournament.NumberOfPlayer < 8)
            {
                ConsoleService.WriteLineMessage("-> Minimum 8 players to start the tournament\n\r");
            }
            int? operation = null;
            if (optionTournamentGamePlayMenuList.Count < 10)
            {
                operation = int.TryParse(ConsoleService.GetKeyFromUser(message, "Enter Option\n\r" + messageBack).KeyChar.ToString(), out int parsedOperation) ? parsedOperation : null;
            }
            else
            {
                operation = ConsoleService.GetIntNumberFromUser(message, "Enter Option\n\r" + messageBack);
            }

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
                gamePlaySystem = ListGamePlaySystem.ListTypeOfPlaySystems.First();
            }

            if (gamePlaySystem == ListGamePlaySystem.ListTypeOfPlaySystems[0] && Tournament.GamePlaySystem != gamePlaySystem)
            {
                Tournament.GamePlaySystem = gamePlaySystem;
                _tournamentsManager.UpdateTournament(Tournament);
                playSystem = new GroupPlaySystem(Tournament, _tournamentsManager, _singlePlayerDuelManager, _playersToTournament, _playerService, _playerManager);
            }
            else if (gamePlaySystem == ListGamePlaySystem.ListTypeOfPlaySystems[1] && Tournament.GamePlaySystem != gamePlaySystem)
            {
                Tournament.GamePlaySystem = gamePlaySystem;
                _tournamentsManager.UpdateTournament(Tournament);
                playSystem = new TwoKOPlaySystem(Tournament, _tournamentsManager, _singlePlayerDuelManager, _playersToTournament, _playerService, _playerManager);
            }
        }
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
                    foreach (var game in ListGamePlaySystem.ListTypeOfPlaySystems)
                    {
                        var formatText = ListGamePlaySystem.ListTypeOfPlaySystems.IndexOf(game) == idSelectTypeOfGame ? $"> {game} <= Select Enter" :
                            $"  {game}";
                        ConsoleService.WriteLineMessage(formatText);
                    }
                    ConsoleKeyInfo inputKey = ConsoleService.GetKeyFromUser();
                    if (inputKey.Key == ConsoleKey.UpArrow && idSelectTypeOfGame > 0)
                    {
                        idSelectTypeOfGame--;
                    }
                    else if (inputKey.Key == ConsoleKey.DownArrow && idSelectTypeOfGame < ListGamePlaySystem.ListTypeOfPlaySystems.Count - 1)
                    {
                        idSelectTypeOfGame++;
                    }
                    else if (inputKey.Key == ConsoleKey.Enter)
                    {
                        gamePlaySystem = ListGamePlaySystem.ListTypeOfPlaySystems[idSelectTypeOfGame];
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