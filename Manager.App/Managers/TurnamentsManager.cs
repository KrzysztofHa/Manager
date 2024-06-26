using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.App.Managers.Helpers;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Manager.App.Managers;

public class TurnamentsManager
{
    private readonly MenuActionService _actionService;
    private readonly IPlayerManager _playerManager;
    private readonly ITournamentsService _tournamentsService = new TournamentsService();
    private readonly IUserService _userService;
    private readonly IPlayerService _playerService;
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;

    public TurnamentsManager(MenuActionService actionService, IPlayerManager playerManager, IUserService userService, IPlayerService playerService)
    {
        _actionService = actionService;
        _playerManager = playerManager;
        _userService = userService;
        _playerService = playerService;
        _singlePlayerDuelManager = new SinglePlayerDuelManager(_playerManager, _userService, _playerService);
    }
    public void TournamentOptionsView()
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
                    CreateNewTournament();
                    if (ConsoleService.AnswerYesOrNo("Start Now Tournament ?"))
                    {
                        // StartTournament(SearchTournament());
                    }
                    break;
                case 3:
                    _singlePlayerDuelManager.VievSinglePlayerDuelsByTournamentsOrSparrings(1);
                    ConsoleService.WriteLineMessage("Press Any Key...");
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
    public void StartTournament(Tournament tournament)
    {



    }

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
        }
        tournament.IdClub = club.Id;
        AddGameplaySystem(tournament);
        if (string.IsNullOrEmpty(tournament.GameplaySystem))
        {
            return null;
        }

        AddPlayerToTournaments(tournament, duel);
        _tournamentsService.AddNewTournament(tournament);

        duel = _singlePlayerDuelManager.NewTournamentSinglePlayerDue(duel, tournament.Id);
        if (duel == null)
        {
            return null;
        }
        return tournament;
    }
    private void AddPlayerToTournaments(Tournament tournament, SinglePlayerDuel duel)
    {
        PlayersToTournament playersToTournament = new(tournament);
        List<PlayerToTournament> listPlayersToTournament = playersToTournament.GetPlayerToTournament();

        while (true)
        {
            ViewListPlayersToTournament(tournament, listPlayersToTournament);
            ConsoleService.WriteLineMessage("Press Any Key To Add Next Player\r\n Or Escape (Esc) To Exit");
            var inputKey = ConsoleService.GetKeyFromUser();
            if (inputKey.Key == ConsoleKey.Escape)
            {
                break;
            }
            var player = _playerManager.SearchPlayer("Add Player");
            if (player == null)
            {
                break;
            }

            if (listPlayersToTournament.Any(p => p.IdPLayer == player.Id))
            {
                ConsoleService.WriteLineErrorMessage($"The Player {player.FirstName} {player.LastName} is on the list");
            }
            else
            {
                string playerCountry = _playerService.GetPlayerAddress(player).Country;
                listPlayersToTournament.Add(new PlayerToTournament(player, playerCountry));
            }
        }
        tournament.NumberOfPlayer = listPlayersToTournament.Count;

        playersToTournament.SavePlayersToTournament();
    }
    public void ViewListPlayersToTournament(Tournament tournament, List<PlayerToTournament> playerToTournaments)
    {
        if (playerToTournaments.Any())
        {
            ConsoleService.WriteTitle($"List Players Of {tournament.Name}");
            foreach (var player in playerToTournaments)
            {
                var formatText = playerToTournaments
                    .Where(s => playerToTournaments.IndexOf(s) == playerToTournaments.IndexOf(player))
                    .Select(p => new { number = $"{playerToTournaments.IndexOf(player) + 1}. " }).First().number
                     + $"{player.TinyFulName} {player.Country}";
                ConsoleService.WriteLineMessage(formatText);
            }
        }
        else
        {
            ConsoleService.WriteTitle($"List Players Of {tournament.Name}");
            ConsoleService.WriteLineErrorMessage("Empty List");
        }
    }
    private bool AddGameplaySystem(Tournament tournament)
    {
        string[] settings = ["Gameplay System"];

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
