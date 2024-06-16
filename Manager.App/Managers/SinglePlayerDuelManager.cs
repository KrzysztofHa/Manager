using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;

namespace Manager.App.Managers;

public class SinglePlayerDuelManager
{
    private readonly MenuActionService _actionService;
    private readonly PlayerManager _playerManager;
    private readonly IUserService _userService;
    private readonly IPlayerService _playerService;
    public SinglePlayerDuelManager(MenuActionService actionService, PlayerManager playerManager, IUserService userService, IPlayerService playerService)
    {
        _playerService = playerService;
        _actionService = actionService;
        _playerManager = playerManager;
        _userService = userService;
    }
    public void SparringOptionView()
    {
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Sparring");
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
                    StartNewSparring();
                    break;
                case 2:
                    AllSparring();
                    ConsoleService.WriteLineMessageActionSuccess("Press Any Key..");
                    break;
                case 3:
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

    public void AllSparring()
    {
        ISinglePlayerDuelService singlePlayerDuelService = new SinglePlayerDuelService();
        IService<Frame> frameService = new BaseService<Frame>();
        var listAllSinglePlayerDuels = singlePlayerDuelService.GetAllSinglePlayerDuel();
        if (!listAllSinglePlayerDuels.Any())
        {
            ConsoleService.WriteLineErrorMessage("List Empty");
            return;
        }

        var listframes = frameService.GetAllItem();
        var listPlayers = _playerService.ListOfActivePlayers();
        var tally = listPlayers.Join(listAllSinglePlayerDuels,
            player => player.Id,
            duel => duel.IdFirstPlayer,
        (player, duel) => new
        {
            FirstPlayer = $"{player.FirstName} {player.LastName}",
            SecondPleyer = listPlayers.Where(p => p.Id == duel.IdSecondPlayer)
            .Select(n => new { FulNamen = n.FirstName + " " + n.LastName }).First().FulNamen,
            duel.TypeNameOfGame,
            duel.RaceTo,
            duel.CreatedDateTime,
            duel.ScoreFirstPlayer,
            duel.ScoreSecondPlayer,
        });

        ConsoleService.WriteTitle("ALL SPARRING");
        foreach (var duelView in tally)
        {
            var FormatToTextDuelsView = $"\r\nType Game: {duelView.TypeNameOfGame} Race To: {duelView.RaceTo} Date: {duelView.CreatedDateTime,0:D}" +
                $"\r\n    {duelView.FirstPlayer} {duelView.ScoreFirstPlayer} : {duelView.ScoreSecondPlayer} {duelView.SecondPleyer}";

            ConsoleService.WriteLineMessage(FormatToTextDuelsView);
        }
    }

    public void StartNewSparring()
    {
        ISinglePlayerDuelService singlePlayerDuelService = new SinglePlayerDuelService();
        IService<Frame> frameService = new BaseService<Frame>();
        var players = new List<Player>();
        SinglePlayerDuel singlePlayerDuel = new SinglePlayerDuel() { CreatedById = _userService.GetIdActiveUser() };
        Frame frame = new Frame() { CreatedById = _userService.GetIdActiveUser() };
        frameService.AddItem(frame);
        frameService.SaveList();
        if (!AddPlayersToSparring(singlePlayerDuel, players) || !AddSettingeRaceAndTypeGame(singlePlayerDuel))
        {
            return;
        }

        string[] tinyFulNamePlayers = {players.Where(p => players.IndexOf(p) == 0)
            .Select(p => ($"{p.FirstName.Remove(1)}.{p.LastName}")).First(),
        players.Where(p => players.IndexOf(p) == 1)
            .Select(p => ($"{p.FirstName.Remove(1)}.{p.LastName}")).First()};

        singlePlayerDuelService.StartSinglePlayerDuel(singlePlayerDuel);

        while (true)
        {
            frame.IdSinglePlayerDuel = singlePlayerDuel.Id;
            do
            {
                ConsoleService.WriteTitle($"{"Sparring",24}");
                var formatNamesPlayersToView = $"{tinyFulNamePlayers[0],15} ({singlePlayerDuel.ScoreFirstPlayer}" +
                    $" : {singlePlayerDuel.ScoreSecondPlayer}) {tinyFulNamePlayers[1],-25}\r\n";

                ConsoleService.WriteLineMessage(formatNamesPlayersToView);
                ConsoleService.WriteLineMessage($"{$"{singlePlayerDuel.TypeNameOfGame} Race {singlePlayerDuel.RaceTo}",27}");
                if (singlePlayerDuel.ScoreFirstPlayer == singlePlayerDuel.RaceTo)
                {
                    singlePlayerDuelService.EndSinglePlayerDuel(singlePlayerDuel);
                    ConsoleService.WriteLineMessageActionSuccess($"{$"Winner {players[0].FirstName + " " + players[0].LastName}",30}");
                    return;
                }
                else if (singlePlayerDuel.ScoreSecondPlayer == singlePlayerDuel.RaceTo)
                {
                    singlePlayerDuelService.EndSinglePlayerDuel(singlePlayerDuel);
                    ConsoleService.WriteLineMessageActionSuccess($"{$"Winner {players[1].FirstName + " " + players[1].LastName}",30}");
                    return;
                }
                ConsoleService.WriteLineMessage("Press Enter To Update Score");

                var inputKey = ConsoleService.GetKeyFromUser();
                if (inputKey.Key == ConsoleKey.Enter)
                {
                    var idSelsctedPlayer = 0;
                    do
                    {
                        ConsoleService.WriteTitle($"{"Sparring",24}");
                        ConsoleService.WriteLineMessage(formatNamesPlayersToView + "\r\nSelect Winner");
                        foreach (var namePlayer in tinyFulNamePlayers)
                        {
                            var formatText = tinyFulNamePlayers[idSelsctedPlayer] == (namePlayer) ?
                                $">" + $"{tinyFulNamePlayers[idSelsctedPlayer],-20}".Remove(20) + " <= Select Enter" :
                                $" {namePlayer,-20}".Remove(20);
                            ConsoleService.WriteLineMessage(formatText);
                        }

                        ConsoleKeyInfo input = ConsoleService.GetKeyFromUser();
                        if (input.Key == ConsoleKey.UpArrow && idSelsctedPlayer > 0)
                        {
                            idSelsctedPlayer--;
                        }
                        else if (input.Key == ConsoleKey.DownArrow && idSelsctedPlayer < tinyFulNamePlayers.Length - 1)
                        {
                            idSelsctedPlayer++;
                        }
                        else if (input.Key == ConsoleKey.Enter)
                        {
                            if (idSelsctedPlayer == 0)
                            {
                                singlePlayerDuel.ScoreFirstPlayer++;
                                frame.IdPlayerWinner = singlePlayerDuel.IdFirstPlayer;
                            }
                            else
                            {
                                singlePlayerDuel.ScoreSecondPlayer++;
                                frame.IdPlayerWinner = singlePlayerDuel.IdSecondPlayer;
                            }
                            break;
                        }
                        else if (input.Key == ConsoleKey.Escape)
                        {
                            break;
                        }
                    } while (true);

                    if (ConsoleService.AnswerYesOrNo("Break And Run Yes or No"))
                    {
                        frame.IsBreakAndRun = true;
                    }
                    frame.EndGame = DateTime.Now;
                    frameService.UpdateItem(frame);
                    frameService.SaveList();
                    singlePlayerDuelService.UpdateSinglePlayerDuel(singlePlayerDuel);
                }
                else if (inputKey.Key == ConsoleKey.Escape)
                {
                    if (ConsoleService.AnswerYesOrNo("you want to leave the game?"))
                    {
                        return;
                    }
                    break;
                }

            } while (frame.IdPlayerWinner == 0);
            frame = new Frame() { CreatedById = _userService.GetIdActiveUser() };
            frameService.AddItem(frame);
        }
    }

    private bool AddPlayersToSparring(SinglePlayerDuel singlePlayerDuel, List<Player> players)
    {
        ConsoleService.WriteTitle("Add Player");
        if (ConsoleService.AnswerYesOrNo("User is the player?"))
        {
            var idUserOfPlayer = _userService.GeIdPlayerOfActiveUser();
            if (idUserOfPlayer <= 0)
            {
                idUserOfPlayer = _playerManager.SearchPlayer();
                if (idUserOfPlayer == -1)
                {
                    idUserOfPlayer = _playerManager.AddNewPlayer();
                }
                _userService.SetIdPlayerToActiveUser(idUserOfPlayer);
            }
            singlePlayerDuel.IdFirstPlayer = idUserOfPlayer;
            players.Add(_playerService.GetItemById(idUserOfPlayer));
        }
        else
        {
            var findPlayer = _playerManager.SearchPlayer();
            if (_playerService.ListOfActivePlayers().Any(p => p.Id == findPlayer))
            {
                var idUserOfPlayer = _playerManager.AddNewPlayer();
                if (idUserOfPlayer == -1)
                {
                    return false;
                }
                singlePlayerDuel.IdFirstPlayer = idUserOfPlayer;
                players.Append(_playerService.GetItemById(idUserOfPlayer));
            }
        }

        ConsoleService.WriteTitle("Add First Player");
        ConsoleService.WriteLineMessageActionSuccess("Succes\r\nPress Any Key");

        var idSecondPlayer = _playerManager.SearchPlayer();
        if (players[0].Id == idSecondPlayer)
        {
            while (players[0].Id == idSecondPlayer)
            {
                ConsoleService.WriteTitle("Add Second Player\r\n Press Any Key..");
                ConsoleService.WriteLineErrorMessage("The selected player is already added.");
                ConsoleService.GetKeyFromUser();

                idSecondPlayer = _playerManager.SearchPlayer();
            }
        }



        if (idSecondPlayer == -1)
        {
            idSecondPlayer = _playerManager.AddNewPlayer();
            if (idSecondPlayer == -1)
            {
                return false;
            }
        }
        singlePlayerDuel.IdSecondPlayer = idSecondPlayer;
        players.Add(_playerService.GetItemById(idSecondPlayer));

        ConsoleService.WriteTitle("Add Second Player");
        ConsoleService.WriteLineMessageActionSuccess("Succes\r\nPress Any Key");
        return true;
    }

    private bool AddSettingeRaceAndTypeGame(SinglePlayerDuel singlePlayerDuel)
    {
        string[] settings = { "Type Of Game", "Race To" };

        foreach (string setting in settings)
        {
            if (setting == "Type Of Game")
            {
                var typeOfGame = new TypeOfGame();
                int idSelectTypeOfGame = 0;
                do
                {
                    ConsoleService.WriteTitle($"Add settings\r\n{setting}");
                    foreach (var game in typeOfGame.ListTypeOfGames)
                    {
                        var formatText = typeOfGame.ListTypeOfGames.IndexOf(game) == idSelectTypeOfGame ? $"> {game} <= Select Enter" :
                            $"  {game}";
                        ConsoleService.WriteLineMessage(formatText);
                    }
                    ConsoleKeyInfo inputKey = ConsoleService.GetKeyFromUser();
                    if (inputKey.Key == ConsoleKey.UpArrow && idSelectTypeOfGame > 0)
                    {
                        idSelectTypeOfGame--;
                    }
                    else if (inputKey.Key == ConsoleKey.DownArrow && idSelectTypeOfGame < typeOfGame.ListTypeOfGames.Count - 1)
                    {
                        idSelectTypeOfGame++;
                    }
                    else if (inputKey.Key == ConsoleKey.Enter)
                    {
                        singlePlayerDuel.TypeNameOfGame = typeOfGame.ListTypeOfGames[idSelectTypeOfGame];
                    }
                    else if (inputKey.Key == ConsoleKey.Escape)
                    {
                        return false;
                    }

                } while (singlePlayerDuel.TypeNameOfGame == null);
            }

            if (setting == "Race To")
            {
                do
                {
                    ConsoleService.WriteTitle($"Add settings\r\n{setting}");
                    ConsoleService.WriteLineMessage("Min 3  Max 20");
                    var manyframe = ConsoleService.GetIntNumberFromUser("Enter To Many frame");
                    if (manyframe == null)
                    {
                        singlePlayerDuel.RaceTo = 3;
                        return true;
                    }
                    if (manyframe >= 3 || manyframe <= 20)
                    {
                        singlePlayerDuel.RaceTo = (int)manyframe;
                    }

                } while (singlePlayerDuel.RaceTo < 3 || singlePlayerDuel.RaceTo > 20);
            }
        }
        return true;
    }
}
