using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using Manager.Helpers;

namespace Manager.App.Managers;

public class SparringManager
{
    private readonly MenuActionService _actionService;
    private readonly IPlayerManager _playerManager;
    private readonly IPlayerService _playerService;
    private readonly SinglePlayerDuelManager _singlePlayerDuelManager;
    public SparringManager(MenuActionService actionService, IPlayerManager playerManager, IPlayerService playerService)
    {
        _playerService = playerService;
        _actionService = actionService;
        _playerManager = playerManager;
        _singlePlayerDuelManager = new SinglePlayerDuelManager(_playerManager, _playerService);
    }
    public void SparringOptionsView()
    {
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Sparring");
        while (true)
        {
            ConsoleService.WriteTitle("Sparring");
            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    StartSparring();
                    break;
                case 2:
                    StartSparring(_singlePlayerDuelManager.SearchInterruptedDuel());
                    break;
                case 3:
                    AllSparring();
                    break;
                case 4:
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
        _singlePlayerDuelManager.VievSinglePlayerDuelsByTournamentsOrSparrings();
        ConsoleService.WriteLineMessageActionSuccess("Press Any Key..");
    }

    public void StartSparring(SinglePlayerDuel singlePlayerDuel = null)
    {
        IService<Frame> frameService = new BaseService<Frame>();
        Frame frame = new Frame();
        frameService.AddItem(frame);
        frameService.SaveList();
        if (singlePlayerDuel == null)
        {
            if (ConsoleService.AnswerYesOrNo("Start New Sparring ?"))
            {
                singlePlayerDuel = _singlePlayerDuelManager.NewSingleDuel();
                if (singlePlayerDuel == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        var players = _playerService.ListOfActivePlayers().Where(p => p.Id == singlePlayerDuel.IdFirstPlayer || p.Id == singlePlayerDuel.IdSecondPlayer).ToList();
        string[] tinyFulNamePlayers = {players.Where(p => players.IndexOf(p) == 0)
            .Select(p => ($"{p.FirstName.Remove(1)}.{p.LastName}")).First(),
        players.Where(p => players.IndexOf(p) == 1)
            .Select(p => ($"{p.FirstName.Remove(1)}.{p.LastName}")).First()};

        _singlePlayerDuelManager.StartSingleDuel(singlePlayerDuel);

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
                    _singlePlayerDuelManager.EndSinglePlayerDuel(singlePlayerDuel);
                    ConsoleService.WriteLineMessageActionSuccess($"{$"Winner {players[0].FirstName + " " + players[0].LastName}",30}");
                    return;
                }
                else if (singlePlayerDuel.ScoreSecondPlayer == singlePlayerDuel.RaceTo)
                {
                    _singlePlayerDuelManager.EndSinglePlayerDuel(singlePlayerDuel);
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
                    _singlePlayerDuelManager.UpdateSinglePlayerDuel(singlePlayerDuel);
                }
                else if (inputKey.Key == ConsoleKey.Escape)
                {
                    if (ConsoleService.AnswerYesOrNo("You want to leave the game?"))
                    {
                        return;
                    }
                    break;
                }

            } while (frame.IdPlayerWinner == 0);
            frame = new Frame();
            frameService.AddItem(frame);
        }
    }
}
