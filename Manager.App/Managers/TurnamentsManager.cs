using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Manager.App.Managers;

public class TurnamentsManager
{
    private readonly MenuActionService _actionService;
    private readonly PlayerManager _playerManager;
    private readonly ITournamentsService _tournamentsService = new TournamentsService();
    private readonly IUserService _userService;
    private readonly IPlayerService _playerService;
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;

    public TurnamentsManager(MenuActionService actionService, PlayerManager playerManager, IUserService userService, IPlayerService playerService)
    {
        _actionService = actionService;
        _playerManager = playerManager;
        _userService = userService;
        _playerService = playerService;
        _singlePlayerDuelManager = new SinglePlayerDuelManager(_playerManager, _userService, _playerService);
    }
    public void SparringOptionView()
    {
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Tournaments");
        while (true)
        {
            ConsoleService.WriteTitle("Tournaments");
            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    //League
                    break;
                case 2:
                    NewOneDeyTournament();
                    ConsoleService.WriteLineMessageActionSuccess("Press Any Key..");
                    break;
                case 3:
                    _singlePlayerDuelManager.ListOfSinglePlayerDuelByTournamentOrSparring("ALL");
                    ConsoleService.GetKeyFromUser();
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
    public void NewOneDeyTournament()
    {
        Tournament tournament = CreateNewTournament();
        //List<Player> players = AddPlayerToTournament();

    }
    //private List<Player> AddPlayerToTournament()
    //{

    //}
    private Tournament? CreateNewTournament()
    {
        IClubManager clubManager = new ClubManager(_actionService, _userService);
        Tournament tournament = new Tournament();
        SinglePlayerDuel duel = new SinglePlayerDuel();

        tournament.CreatedById = _userService.GetIdActiveUser();

        do
        {
            ConsoleService.WriteTitle("Create New Tournament");
            tournament.Name = ConsoleService.GetRequiredStringFromUser("Enter Name ");

            if (string.IsNullOrEmpty(tournament.Name))
            {
                return null;
            }
            else if (_tournamentsService.GetAllItem().Any(t => t.Name.Equals(tournament.Name)))
            {
                ConsoleService.WriteLineErrorMessage("The name is already use. Please enter a different name.");
            }
            else
            {
                break;
            }
        } while (true);


        var club = clubManager.SearchClub("Create New Tournament Add Club");
        if (club == null)
        {
            club = clubManager.AddNewClub();
            if (club == null)
            {
                return null;
            }
            tournament.IdClub = club.Id;
        }
        AddGameplaySystem(tournament);
        if (string.IsNullOrEmpty(tournament.GameplaySystem))
        {
            return null;
        }
        _tournamentsService.AddNewTournament(tournament);
        duel = _singlePlayerDuelManager.NewTournamentSinglePlayerDue(duel, tournament.Id);
        if (duel == null)
        {
            return null;
        }
        ConsoleService.WriteLineMessage(duel.TypeNameOfGame);

        return tournament;
    }
    private bool AddGameplaySystem(Tournament tournament)
    {
        string[] settings = { "Gameplay System" };

        foreach (string setting in settings)
        {
            if (setting == "Gameplay System")
            {
                var gameplaySystem = new GameplaySystem();
                int idSelectTypeOfGame = 0;
                do
                {
                    ConsoleService.WriteTitle($"Add settings\r\n{setting}");
                    foreach (var game in gameplaySystem.GameplaySystemsList)
                    {
                        var formatText = gameplaySystem.GameplaySystemsList.IndexOf(game) == idSelectTypeOfGame ? $"> {game} <= Select Enter" :
                            $"  {game}";
                        ConsoleService.WriteLineMessage(formatText);
                    }
                    ConsoleKeyInfo inputKey = ConsoleService.GetKeyFromUser();
                    if (inputKey.Key == ConsoleKey.UpArrow && idSelectTypeOfGame > 0)
                    {
                        idSelectTypeOfGame--;
                    }
                    else if (inputKey.Key == ConsoleKey.DownArrow && idSelectTypeOfGame < gameplaySystem.GameplaySystemsList.Count - 1)
                    {
                        idSelectTypeOfGame++;
                    }
                    else if (inputKey.Key == ConsoleKey.Enter)
                    {
                        tournament.GameplaySystem = gameplaySystem.GameplaySystemsList[idSelectTypeOfGame];
                    }
                    else if (inputKey.Key == ConsoleKey.Escape)
                    {
                        return false;
                    }

                } while (tournament.GameplaySystem == null);
            }
        }
        return true;
    }
}
