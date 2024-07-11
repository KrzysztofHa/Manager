using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.App.Concrete.Helpers.GamePlaySystem;
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
    private readonly IPlayerService _playerService;
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;

    public TurnamentsManager(MenuActionService actionService, IPlayerManager playerManager, IPlayerService playerService)
    {
        _actionService = actionService;
        _playerManager = playerManager;
        _playerService = playerService;
        _singlePlayerDuelManager = new SinglePlayerDuelManager(_playerManager, _playerService);
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
                    var tournament = CreateNewTournament();
                    if (tournament != null)
                    {
                        if (ConsoleService.AnswerYesOrNo("Start tournament now ?"))
                        {
                            StartTournament(tournament);
                        }
                    }
                    break;
                case 3:
                    StartTournament(SearchTournament());
                    break;
                case 4:
                    AllTournamentsView();
                    break;
                case 5:
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
        IClubManager clubManager = new ClubManager(_actionService);
        Tournament tournament = new Tournament();
        SinglePlayerDuel duel = new SinglePlayerDuel();

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
                ConsoleService.WriteLineErrorMessage("The entered name is already use. Please enter a different name.");
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
    public Tournament SearchTournament(string title = "")
    {
        StringBuilder inputString = new StringBuilder();
        List<Tournament> findTournament = _tournamentsService.SearchTournament(" ")
            .Where(t => t.End == DateTime.MinValue).ToList();
        int maxEntriesToDisplay = 15;
        if (!findTournament.Any())
        {
            ConsoleService.WriteLineErrorMessage("Empty List");
            return null;
        }

        List<Tournament> findTournamentToView = new List<Tournament>();

        if (findTournament.Count >= maxEntriesToDisplay - 1)
        {
            findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
        }
        else
        {
            findTournamentToView = findTournament;
        }

        List<Tournament> findTournamentTemp = new List<Tournament>();
        var address = new Address();
        int indexSelectedPlayer = 0;
        title = string.IsNullOrWhiteSpace(title) ? "Search Tournament" : title;

        var headTableToview = title + $"\r\n    {" LP",-5}{"ID",-6}{"Name",-21}" +
                    $"{"Game System",-16}{"Club Name",-21}{"Start",-15}{"End",-15}{"Players",-11}";
        do
        {
            if (findTournamentToView.Any())
            {
                ConsoleService.WriteTitle(headTableToview);

                foreach (var tournament in findTournamentToView)
                {
                    var formmatStringToView = findTournament.IndexOf(tournament) == indexSelectedPlayer ?
                        "---> " + $"{findTournament.IndexOf(tournament) + 1,-5}".Remove(4) + _tournamentsService.GetTournamentDetailView(tournament) + $" <----\r\n" :
                        "     " + $"{findTournament.IndexOf(tournament) + 1,-5}".Remove(4) + _tournamentsService.GetTournamentDetailView(tournament);

                    ConsoleService.WriteLineMessage(formmatStringToView);
                }
            }
            else
            {
                ConsoleService.WriteLineErrorMessage("Not Found Tournament");
            }
            ConsoleService.WriteLineMessage($"\r\n------(Found {findTournament.Count} Tournament)-------\r\n" + inputString.ToString());
            ConsoleService.WriteLineMessage(@"Enter string move UP or Down  and  press enter to Select");

            var keyFromUser = ConsoleService.GetKeyFromUser();

            if (char.IsLetterOrDigit(keyFromUser.KeyChar))
            {
                if (findTournament.Count == 1 && !string.IsNullOrEmpty(inputString.ToString()))
                {
                    ConsoleService.WriteLineErrorMessage("No entries found !!!");
                }
                else
                {
                    inputString.Append(keyFromUser.KeyChar);

                    if (inputString.Length == 1)
                    {
                        findTournamentTemp = _tournamentsService.SearchTournament(inputString.ToString())
                            .Where(t => t.End == DateTime.MinValue).ToList();
                        if (findTournamentTemp.Any())
                        {
                            findTournament.Clear();
                            findTournament.AddRange(findTournamentTemp);
                            findTournamentToView.Clear();
                            if (findTournament.Count >= maxEntriesToDisplay - 1)
                            {
                                findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
                            }
                            else
                            {
                                findTournamentToView = findTournament;
                            }
                            indexSelectedPlayer = 0;
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                        }
                    }
                    else
                    {
                        findTournament = [.. findTournament.Where(p => $"{p.Id} {p.Name}".ToLower().
                        Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                        if (!findTournament.Any())
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            findTournament.AddRange([.. findTournamentTemp.Where(p => $"{p.Id} {p.Name}".ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)]);
                            ConsoleService.WriteLineErrorMessage("No entry found !!!");
                        }
                        findTournamentToView.Clear();
                        if (findTournament.Count >= maxEntriesToDisplay - 1)
                        {
                            findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
                        }
                        else
                        {
                            findTournamentToView = findTournament;
                        }
                        indexSelectedPlayer = 0;
                    }
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Backspace && inputString.Length > 0)
            {
                inputString.Remove(inputString.Length - 1, 1);

                if (!string.IsNullOrEmpty(inputString.ToString()))
                {
                    findTournament = [.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} ".ToLower()
                    .Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                    indexSelectedPlayer = 0;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.DownArrow && indexSelectedPlayer <= findTournament.Count - 2)
            {
                if (indexSelectedPlayer >= maxEntriesToDisplay - 1)
                {
                    if (findTournament.IndexOf(findTournamentToView.First()) != findTournament.Count - maxEntriesToDisplay)
                    {
                        var nextPlayer = findTournamentToView.ElementAt(1);
                        var startIndex = findTournament.IndexOf(nextPlayer);
                        findTournamentToView.Clear();
                        findTournamentToView = findTournament.GetRange(startIndex, maxEntriesToDisplay);
                    }
                }
                indexSelectedPlayer++;
            }
            else if (keyFromUser.Key == ConsoleKey.UpArrow && indexSelectedPlayer > 0)
            {
                if (findTournament.IndexOf(findTournamentToView.First()) != findTournament.IndexOf(findTournament.First()))
                {
                    var nextPlayer = findTournamentToView.First();
                    findTournamentToView.Clear();
                    findTournamentToView = findTournament.GetRange(findTournament.IndexOf(nextPlayer) - 1, maxEntriesToDisplay);
                }
                indexSelectedPlayer--;
            }
            else if (keyFromUser.Key == ConsoleKey.Enter && findTournament.Any())
            {
                var findTournamentToSelect = findTournament.First(p => findTournament.IndexOf(p) == indexSelectedPlayer);
                ConsoleService.WriteTitle(headTableToview);
                ConsoleService.WriteLineMessage($"{_tournamentsService.GetTournamentDetailView(findTournamentToSelect),106}");

                if (ConsoleService.AnswerYesOrNo("Selected Player"))
                {
                    return findTournamentToSelect;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Escape)
            {
                break;
            }

        } while (true);

        return null;
    }
    public void AllTournamentsView()
    {
        var allTournaments = _tournamentsService.GetAllItem().OrderByDescending(t => t.CreatedDateTime).ToList();
        var headTableToview = "All Tournaments" + $"\r\n{" LP",-5}{"ID",-6}{"Name",-21}" +
                   $"{"Game System",-16}{"Club Name",-21}{"Start",-15}{"End",-15}{"Players",-11}";

        if (allTournaments.Any())
        {
            ConsoleService.WriteTitle(headTableToview);

            foreach (var tournament in allTournaments)
            {
                var formmatStringToView = $" {allTournaments.IndexOf(tournament) + 1,-5}".Remove(5) + _tournamentsService.GetTournamentDetailView(tournament);

                ConsoleService.WriteLineMessage(formmatStringToView);
            }
        }
        else
        {
            ConsoleService.WriteLineErrorMessage("Empty List");
        }
        ConsoleService.WriteLineMessage("Press any key...");
        ConsoleService.GetKeyFromUser();
    }
}
