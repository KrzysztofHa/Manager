using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.ComponentModel.Design;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Xunit.Sdk;

namespace Manager.App.Managers;

public class TournamentsManager : ITournamentsManager
{
    private readonly MenuActionService _actionService;
    private readonly IPlayerManager _playerManager;
    private readonly ITournamentsService _tournamentsService = new TournamentsService();
    private readonly IPlayerService _playerService;
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;

    public TournamentsManager(MenuActionService actionService, IPlayerManager playerManager, IPlayerService playerService)
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
                    var newTournament = CreateNewTournament();
                    if (newTournament != null)
                    {
                        ITournamentsManager tournamentsManager = this;
                        TournamentGamePlayManager tournamentGamePlayManager = new TournamentGamePlayManager(newTournament, tournamentsManager, _actionService, _playerManager, _playerService, _singlePlayerDuelManager);
                        tournamentGamePlayManager.GoToTournament();
                    }
                    break;

                case 2:
                    var tournament = SearchTournament();
                    if (tournament != null)
                    {
                        ITournamentsManager tournamentsManager = this;
                        TournamentGamePlayManager tournamentGamePlayManager = new TournamentGamePlayManager(tournament, tournamentsManager, _actionService, _playerManager, _playerService, _singlePlayerDuelManager);
                        tournamentGamePlayManager.GoToTournament();
                    }
                    break;

                case 3:
                    AllTournamentsView();
                    break;

                case 4:
                    DeleteTournament();
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

    //private void StartTournament(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    if (tournament == null || playersToTournament == null || tournament.End != DateTime.MinValue)
    //    {
    //        return;
    //    }

    //    _tournamentsService.StartTournament(tournament, _singlePlayerDuelManager);
    //    CreateDuelsToTournament(tournament, playersToTournament);
    //    var optionPlayerMenu = _actionService.GetMenuActionsByName("Start Tournament");
    //    string listStartedDuelsInText = String.Empty;

    //    while (true)
    //    {
    //        var allStartedDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //       .Where(d => d.StartGame != DateTime.MinValue && d.EndGame == DateTime.MinValue).Where(s => s.Interrupted.Equals(DateTime.MinValue)).ToList();

    //        listStartedDuelsInText = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allStartedDuels);

    //        if (string.IsNullOrEmpty(listStartedDuelsInText))
    //        {
    //            listStartedDuelsInText = ViewGroupsOr2KO(tournament, playersToTournament);
    //        }

    //        ConsoleService.WriteTitle($"Tournaments {tournament.Name} | Game System: {tournament.GamePlaySystem} | Start {tournament.Start} | Number Of Tables {tournament.NumberOfTables}");

    //        for (int i = 0; i < optionPlayerMenu.Count; i++)
    //        {
    //            ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
    //        }

    //        var operation = ConsoleService.GetIntNumberFromUser("Enter Option", listStartedDuelsInText);

    //        switch (operation)
    //        {
    //            case 1:
    //                StartAndInterruptedTournamentDuel(tournament, playersToTournament);
    //                break;

    //            case 2:
    //                UpdateDuelResult(tournament, playersToTournament);
    //                break;

    //            case 3:
    //                ConsoleService.WriteTitle($"View Groups Of Tournament {tournament.Name}");
    //                ConsoleService.WriteLineMessage(ViewGroupsOr2KO(tournament, playersToTournament));
    //                ConsoleService.GetKeyFromUser("Press Any Key...");
    //                break;

    //            case 4:
    //                var allDuelOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id);
    //                var listAllDuelsInText = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allDuelOfTournament);
    //                ConsoleService.WriteTitle($"All Duels Of Tournament {tournament.Name}");
    //                ConsoleService.WriteLineMessage(listAllDuelsInText);
    //                ConsoleService.GetKeyFromUser("Press Any Key...");
    //                break;

    //            case 5:
    //                AddPlayersToTournament(tournament, playersToTournament);
    //                break;

    //            case 6:
    //                RemovePlayerOfTournament(tournament, playersToTournament);
    //                break;

    //            case 7:
    //                MovePlayer(tournament, playersToTournament);
    //                break;

    //            case 8:
    //                ChangeRaceTo(tournament);
    //                break;

    //            case 9:
    //                ChangeNumberOfTable(tournament, playersToTournament);
    //                break;

    //            case 10:
    //                operation = null;
    //                break;

    //            default:
    //                if (operation == null)
    //                {
    //                    if (ConsoleService.AnswerYesOrNo("You want to Exit?"))
    //                    {
    //                        break;
    //                    }
    //                    operation = 0;
    //                }
    //                ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
    //                break;
    //        }

    //        if (operation == null)
    //        {
    //            _tournamentsService.InterruptTournament(tournament, _singlePlayerDuelManager);
    //            break;
    //        }
    //    }
    //}

    public void DeleteTournament()
    {
        var tournament = SearchTournament();
        if (tournament == null || tournament.End != DateTime.MinValue)
        {
            return;
        }

        _singlePlayerDuelManager.RemoveTournamentDuel(tournament);
        _tournamentsService.RemoveItem(tournament);
        _tournamentsService.SaveList();
    }

    private Tournament? CreateNewTournament()
    {
        IClubManager clubManager = new ClubManager(_actionService);
        Tournament tournament = new Tournament();

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

        _tournamentsService.AddNewTournament(tournament);
        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(tournament.Id);

        ITournamentsManager tournamentsManager = this;
        TournamentGamePlayManager tournamentGamePlaySystem = new(tournament, tournamentsManager, _actionService, _playerManager, _playerService, _singlePlayerDuelManager);

        _tournamentsService.UpdateItem(tournament);
        _tournamentsService.SaveList();

        return tournament;
    }

    public Tournament SearchTournament(string title = "")
    {
        StringBuilder inputString = new StringBuilder();
        List<Tournament> findTournamentTemp = _tournamentsService.SearchTournament(" ")
            .Where(t => t.End == DateTime.MinValue && t.IsActive == true).ToList();
        List<Tournament> findTournament = [];
        List<Tournament> findTournamentToView = [];
        findTournament.AddRange(findTournamentTemp);
        int maxEntriesToDisplay = 15;
        if (!findTournament.Any())
        {
            ConsoleService.WriteLineErrorMessage("Empty List");
            return null;
        }

        if (findTournament.Count >= maxEntriesToDisplay - 1)
        {
            findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
        }
        else
        {
            findTournamentToView = findTournament;
        }
        var address = new Address();
        int indexSelectedTournament = 0;
        title = string.IsNullOrWhiteSpace(title) ? "Search Tournament" : title;

        var headTableToview = title + $"\r\n    {" LP",-5}{"ID",-6}{"Name",-21}" +
                    $"{"Game System",-16}{"Club Name",-21}{"Start",-15}{"End",-15}{"Players",-11}";
        do
        {
            ConsoleService.WriteTitle(headTableToview);

            foreach (var tournament in findTournamentToView)
            {
                var formmatStringToView = findTournament.IndexOf(tournament) == indexSelectedTournament ?
                    "\r\n---> " + $"{findTournament.IndexOf(tournament) + 1,-5}".Remove(4) + _tournamentsService.GetTournamentDetailView(tournament) + $" <----\r\n" :
                    "     " + $"{findTournament.IndexOf(tournament) + 1,-5}".Remove(4) + _tournamentsService.GetTournamentDetailView(tournament);

                ConsoleService.WriteLineMessage(formmatStringToView);
            }

            ConsoleService.WriteLineMessage($"\r\n------(Found {findTournament.Count} Tournament)-------\r\n" + inputString.ToString());
            ConsoleService.WriteLineMessage(@"Enter string move UP or Down  and  press enter to Select");

            var keyFromUser = ConsoleService.GetKeyFromUser("Selected Tournament: "
                + findTournament[indexSelectedTournament].Name);

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
                        if (findTournamentTemp.Any(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())))
                        {
                            findTournament = [.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                            if (findTournament.Count >= maxEntriesToDisplay - 1)
                            {
                                findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
                            }
                            else
                            {
                                findTournamentToView = findTournament;
                            }
                            indexSelectedTournament = 0;
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            ConsoleService.WriteLineErrorMessage("No entries found !!!");
                        }
                    }
                    else
                    {
                        if (findTournamentTemp.Any(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())))
                        {
                            findTournament = [.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                            if (findTournament.Count >= maxEntriesToDisplay - 1)
                            {
                                findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
                            }
                            else
                            {
                                findTournamentToView = findTournament;
                            }
                            indexSelectedTournament = 0;
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            ConsoleService.WriteLineErrorMessage("No entry found !!!");
                        }
                    }
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Backspace && inputString.Length > 0)
            {
                inputString.Remove(inputString.Length - 1, 1);
                findTournament.Clear();
                findTournament.AddRange([.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower()
                    .Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)]);
                indexSelectedTournament = 0;
            }
            else if (keyFromUser.Key == ConsoleKey.DownArrow && indexSelectedTournament <= findTournament.Count - 2)
            {
                if (indexSelectedTournament >= maxEntriesToDisplay - 1)
                {
                    if (findTournament.IndexOf(findTournamentToView.First()) != findTournament.Count - maxEntriesToDisplay)
                    {
                        var nextPlayer = findTournamentToView.ElementAt(1);
                        var startIndex = findTournament.IndexOf(nextPlayer);
                        findTournamentToView.Clear();
                        findTournamentToView = findTournament.GetRange(startIndex, maxEntriesToDisplay);
                    }
                }
                indexSelectedTournament++;
            }
            else if (keyFromUser.Key == ConsoleKey.UpArrow && indexSelectedTournament > 0)
            {
                if (findTournament.IndexOf(findTournamentToView.First()) != findTournament.IndexOf(findTournament.First()))
                {
                    var nextPlayer = findTournamentToView.First();
                    findTournamentToView.Clear();
                    findTournamentToView = findTournament.GetRange(findTournament.IndexOf(nextPlayer) - 1, maxEntriesToDisplay);
                }
                indexSelectedTournament--;
            }
            else if (keyFromUser.Key == ConsoleKey.Enter && findTournament.Any())
            {
                var findTournamentToSelect = findTournament.First(p => findTournament.IndexOf(p) == indexSelectedTournament);
                ConsoleService.WriteTitle(headTableToview);
                ConsoleService.WriteLineMessage($"{_tournamentsService.GetTournamentDetailView(findTournamentToSelect),107}");

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
        var allTournaments = _tournamentsService.GetAllItem().Where(t => t.IsActive == true).OrderByDescending(t => t.CreatedDateTime).ToList();
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

    public void UpdateTournament(Tournament tournament)
    {
        _tournamentsService.UpdateItem(tournament);
        _tournamentsService.SaveList();
    }

    public void InterruptTournament(Tournament tournament)
    {
        _tournamentsService.InterruptTournament(tournament, _singlePlayerDuelManager);
    }

    public string GetTournamentDetailView(Tournament tournament)
    {
        return _tournamentsService.GetTournamentDetailView(tournament);
    }

    public void EndTournament(Tournament tournament)
    {
        _tournamentsService.EndTournament(tournament);
    }

    public void StartTournament(Tournament tournament)
    {
        _tournamentsService.StartTournament(tournament, _singlePlayerDuelManager);
    }
}