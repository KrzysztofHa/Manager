using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.Net.WebSockets;

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
                    Sparring();
                    break;
                case 2:

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
    public void Sparring()
    {
        //ISinglePlayerDuelService playerDuelService = new SinglePlayerDuelService();
        IService<Rack> rackService = new BaseService<Rack>();
        var players = new List<Player>();
        SinglePlayerDuel singlePlayerDuel = new SinglePlayerDuel() { ModifiedById = _userService.GetIdActiveUser() };
        Rack rack = new Rack() { ModifiedById = _userService.GetIdActiveUser() };
        if (!AddPlayersToSparring(singlePlayerDuel, players) || !AddSettingeRaceAndTypeGame(singlePlayerDuel))
        {
            return;
        }
        var tinyFulNameFirstPlayer = players.Where(p => players.IndexOf(p) == 0)
            .Select(p => ($"{p.FirstName.Remove(1)}.{p.LastName}")).First();
        var tinyFulNameSecondPlayer = players.Where(p => players.IndexOf(p) == 1)
            .Select(p => ($"{p.FirstName.Remove(1)}.{p.LastName}")).First();
        




        //playerDuelService.StartSinglePlayerDuel(singlePlayerDuel);
        for (int i = 1; i <= singlePlayerDuel.RaceTo; i++)
        {
            ConsoleService.WriteTitle($"{"Sparring", 24}");
            var formatNamesPlayersToView = $"{tinyFulNameFirstPlayer, 15} ({singlePlayerDuel.ScoreFirstPlayer}" +
                $" : {singlePlayerDuel.ScoreSecondPlayer}) {tinyFulNameSecondPlayer, -25}\r\n";

            ConsoleService.WriteLineMessage(formatNamesPlayersToView);
            ConsoleService.WriteLineMessage($"{$"{singlePlayerDuel.TypeNameOfGame} Race {singlePlayerDuel.RaceTo}", 27}");


            StartRack(rackService, rack, singlePlayerDuel);
        }

    }

    private void StartRack(IService<Rack> rackService, Rack rack, SinglePlayerDuel singlePlayerDuel)
    {
        





        throw new NotImplementedException();
    }

    private bool AddPlayersToSparring(SinglePlayerDuel singlePlayerDuel, List<Player> players)
    {
        ConsoleService.WriteTitle("Add Player");
        if (ConsoleService.AnswerYesOrNo("User is the first player?"))
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
                    var manyRack = ConsoleService.GetIntNumberFromUser("Enter To Many Rack");
                    if (manyRack == null)
                    {
                        singlePlayerDuel.RaceTo = 3;
                        return true;
                    }
                    if (manyRack >= 3 || manyRack <= 20)
                    {
                        singlePlayerDuel.RaceTo = (int)manyRack;
                    }

                } while (singlePlayerDuel.RaceTo < 3 || singlePlayerDuel.RaceTo > 20);
            }
        }
        return true;
    }
}
