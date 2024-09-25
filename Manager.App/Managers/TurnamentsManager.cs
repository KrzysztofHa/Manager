using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers;
using Manager.App.Managers.Helpers.GamePlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.Numerics;
using System.Text;
using Xunit.Sdk;

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
                        GoToTournament(tournament);
                    }
                    break;

                case 3:
                    GoToTournament(SearchTournament());
                    break;

                case 4:
                    AllTournamentsView();
                    break;

                case 5:
                    DeleteTournament();
                    break;

                case 6:
                    operation = null;
                    break;

                default:
                    if (operation != null)
                    {
                        ConsoleService.WriteLineErrorMessage("Enter a valid operation ID\n\rPress Any Key...");
                    }
                    break;
            }

            if (operation == null)
            {
                break;
            }
        }
    }

    public void GoToTournament(Tournament tournament)
    {
        PlayersToTournament playersToTournament = new(tournament, _tournamentsService);

        if (!_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings().Any())
        {
            _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(new SinglePlayerDuel(), tournament.Id);
        }

        if (tournament == null)
        {
            return;
        }

        if (tournament.Start != DateTime.MinValue)
        {
            StartTournament(tournament, playersToTournament);
            return;
        }

        var optionPlayerMenu = _actionService.GetMenuActionsByName("Go To Tournament");
        while (true)
        {
            ConsoleService.WriteTitle($"Tournaments {tournament.Name} | Game System: {tournament.GamePlaySystem} ");
            ConsoleService.WriteLineMessage($"Number of PLayers: {tournament.NumberOfPlayer} | Number Of Groups: {tournament.NumberOfGroups} | " +
                $"Type Name Of Game: {_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id).First().TypeNameOfGame} | " +
                $"Group Race To: {_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings().First().RaceTo}\n\r");

            if (tournament.NumberOfPlayer < 8)
            {
                ConsoleService.WriteLineErrorMessage("Minimum 8 players to start the tournament\n\rPress Any Key...");
            }

            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");

            switch (operation)
            {
                case 1:
                    AddPlayersToTournament(tournament, playersToTournament);
                    break;

                case 2:
                    RemovePlayerOfTournament(tournament, playersToTournament);
                    break;

                case 3:
                    ChangeRaceTo(tournament);
                    break;

                case 4:
                    SetGroups(tournament, playersToTournament);
                    break;

                case 5:
                    EditGroupsOr2KOList(tournament, playersToTournament);
                    break;

                case 6:
                    UpdateGamePlaySystem(tournament);
                    break;

                case 7:
                    RandomSelectionOfPlayers(tournament, playersToTournament);
                    break;

                case 8:
                    ViewListPlayersToTournament(tournament, playersToTournament);
                    ConsoleService.GetKeyFromUser();
                    break;

                case 9:
                    if (tournament.Start == DateTime.MinValue)
                    {
                        ConsoleService.WriteTitle($"Run Tournament {tournament.Name}");
                        if (ConsoleService.AnswerYesOrNo("Before you proceed, make sure is correct everything."))
                        {
                            StartTournament(tournament, playersToTournament);
                        }
                    }
                    break;

                case 10:
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
                    ConsoleService.WriteLineErrorMessage("Enter a valid operation ID\n\rPress Any Key...");
                    break;
            }

            if (operation == null)
            {
                break;
            }
        }
    }

    private void ChangeRaceTo(Tournament tournament)
    {
        bool isPlayerEndDuel = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
                .Any(p => p.EndGame != DateTime.MinValue);
        if (isPlayerEndDuel)
        {
            return;
        }
        ConsoleService.WriteTitle("Change Race To");
        var raceTo = ConsoleService.GetIntNumberFromUser("Enter To Many frame Min 3 Max 20:");
        if (raceTo == null || raceTo < 3 || raceTo > 20)
        {
            return;
        }
        var duel = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings().First();
        duel.RaceTo = (int)raceTo;
        _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
    }

    private void StartTournament(Tournament tournament, PlayersToTournament playersToTournament)
    {
        if (tournament == null || playersToTournament == null)
        {
            return;
        }

        CreateDuelsToTournament(tournament, playersToTournament);

        var optionPlayerMenu = _actionService.GetMenuActionsByName("Start Tournament");
        while (true)
        {
            var tournamentDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id);
            ConsoleService.WriteTitle($"Tournaments {tournament.Name} | Game System: {tournament.GamePlaySystem} | Start {tournament.Start} ");

            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");

            switch (operation)
            {
                case 1:
                    //EndTournamentDuel();
                    break;

                case 2:
                    AddPlayersToTournament(tournament, playersToTournament);
                    break;

                case 3:
                    RemovePlayerOfTournament(tournament, playersToTournament);
                    break;

                case 4:
                    MovePlayer(tournament, playersToTournament);
                    break;

                case 5:
                    ChangeRaceTo(tournament);
                    break;

                case 6:
                    operation = null;
                    break;

                default:
                    if (operation == null)
                    {
                        if (ConsoleService.AnswerYesOrNo("You want to Exit?"))
                        {
                            _tournamentsService.InterruptedTournament(tournament);
                            break;
                        }
                        operation = 0;
                    }
                    ConsoleService.WriteLineErrorMessage("Enter a valid operation ID\n\rPress Any Key...");
                    break;
            }

            if (operation == null)
            {
                break;
            }
        }
    }

    private void CreateDuelsToTournament(Tournament tournament, PlayersToTournament playersToTournament)
    {
        if (tournament == null || playersToTournament == null)
        {
            return;
        }
        if (tournament.GamePlaySystem == "Group")
        {
            _tournamentsService.StartTournament(tournament);

            for (int i = 0; i < playersToTournament.ListPlayersToTournament.Count; i++)
            {
            }
        }
    }

    public void DeleteTournament()
    {
        _tournamentsService.RemoveItem(SearchTournament());
        _tournamentsService.SaveList();
    }

    private void SetGroups(Tournament tournament, PlayersToTournament playersToTournament)
    {
        int numberOfGroups = 0;
        int? enterNumber = 0;
        if (tournament.NumberOfPlayer < 8 || tournament.GamePlaySystem == "2KO")
        {
            ConsoleService.WriteLineErrorMessage("2KO Game System Set. Change To Group\n\rPress Any Key...");
            return;
        }
        ConsoleService.WriteTitle($"{tournament.NumberOfPlayer} Players allows the tournament to start:");
        if (string.IsNullOrEmpty(tournament.GamePlaySystem))
        {
            tournament.GamePlaySystem = "Group";
        }
        if (tournament.GamePlaySystem == "Group")
        {
            if (tournament.NumberOfPlayer >= 8 && tournament.NumberOfPlayer < 10)
            {
                ConsoleService.WriteLineMessage("Created 2 groups:\n\r" +
                    "4 players will advance from the group to the knockout round\n\rPress Any Key ...");
                ConsoleService.GetKeyFromUser();
                numberOfGroups = 2;
            }
            else if (tournament.NumberOfPlayer >= 10 && tournament.NumberOfPlayer < 12)
            {
                ConsoleService.WriteLineMessage("Created 2 groups:\n\r" +
                    "4 players will advance from the group to the knockout round\n\rPress Any Key ...");
                ConsoleService.GetKeyFromUser();
                numberOfGroups = 2;
            }
            else if (tournament.NumberOfPlayer >= 12 && tournament.NumberOfPlayer < 16)
            {
                ConsoleService.WriteLineMessage("2 groups, maximum 6 players per group:\n\r" +
                    "4 players will advance from the group to the knockout round\n\r----\n\r");
                ConsoleService.WriteLineMessage("4 groups:\n\r" +
                    "2 players will advance from the group to the knockout round\n\r----\n\r");
                enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 4 or 8");
            }
            else if (tournament.NumberOfPlayer >= 16 && tournament.NumberOfPlayer < 24)
            {
                if (tournament.NumberOfPlayer == 16)
                {
                    ConsoleService.WriteLineMessage("2 groups, maximum 8 players per group:" +
                                "\n\r4 players will advance from the group to the knockout round\n\r----\r\n");
                    ConsoleService.WriteLineMessage("4 groups:\n\r" +
                        "2 players will advance from the group to the knockout round\n\r----\n\r");
                    enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 2 or 4");
                }
                else
                {
                    numberOfGroups = 4;
                }
            }
            else if (tournament.NumberOfPlayer >= 24 && tournament.NumberOfPlayer < 32)
            {
                if (tournament.NumberOfPlayer == 24)
                {
                    ConsoleService.WriteLineMessage("4 groups,maximum 6 players per group:" +
                                "\n\r2 players will advance from the group to the knockout round\n\r----\n\r");
                    ConsoleService.WriteLineMessage("Eight groups\n\r" +
                        "2 players will advance from the group to the knockout round\n\r----\n\r");
                    enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 4 or 8");
                }
                else
                {
                    numberOfGroups = 8;
                }
            }
            else if (tournament.NumberOfPlayer >= 32 && tournament.NumberOfPlayer < 49)
            {
                ConsoleService.WriteLineMessage("Eight groups" +
                    "\n\r2 players will advance from the group to the knockout round\n\r----\n\rPress Any Key ...");
                numberOfGroups = 8;
            }
            else if (tournament.NumberOfPlayer >= 49)
            {
                ConsoleService.WriteLineMessage("Maximum 48 players");
            }
        }

        if (numberOfGroups == 0 && enterNumber == 2 || enterNumber == 4 || enterNumber == 8)
        {
            numberOfGroups = (int)enterNumber;
        }
        else if (numberOfGroups == 0)
        {
            ConsoleService.WriteLineErrorMessage("Something went wrong, please try again\n\rPress Any Key...");
            return;
        }

        tournament.NumberOfGroups = numberOfGroups;
        RandomSelectionOfPlayers(tournament, playersToTournament);
        _tournamentsService.SaveList();
    }

    private void EditGroupsOr2KOList(Tournament tournament, PlayersToTournament playersToTournament)
    {
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Edit Groups");
        var title = tournament.GamePlaySystem == "Group" ? "Edit Groups" : "Edit 2KO List";

        while (true)
        {
            ConsoleService.WriteTitle(title);
            ConsoleService.WriteLineMessage(ViewGroupsOr2KO(tournament, playersToTournament));
            if (playersToTournament.ListPlayersToTournament.Count == 0)
            {
                ConsoleService.WriteLineErrorMessage("Empty List Of Player\n\rPress Any Key...");
                return;
            }
            ConsoleService.WriteLineMessage("\n\r=====================");

            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("\n\rEnter Option");
            switch (operation)
            {
                case 1:
                    MovePlayer(tournament, playersToTournament);
                    break;

                case 2:
                    playersToTournament.LoadList(tournament);
                    break;

                case 3:
                    operation = null;
                    break;

                default:
                    if (operation == null)
                    {
                        if (!ConsoleService.AnswerYesOrNo("Exit To Tournament Menu?"))
                        {
                            ConsoleService.WriteLineErrorMessage("Enter a valid operation ID\n\rPress Any Key...");
                            operation = 0;
                        }
                    }
                    break;
            }

            if (operation == null)
            {
                if (ConsoleService.AnswerYesOrNo("Save Changes?"))
                {
                    playersToTournament.SavePlayersToTournament();
                }
                else
                {
                    if (!ConsoleService.AnswerYesOrNo("Back to Edit?"))
                    {
                        ConsoleService.WriteLineErrorMessage("Changes Not Save!\n\rPress Any Key...");
                        break;
                    }
                }
            }
        }
    }

    private void MovePlayer(Tournament tournament, PlayersToTournament playersToTournament)
    {
        List<Player> players = new List<Player>();
        foreach (var playerToTournament in playersToTournament.ListPlayersToTournament)
        {
            var player = _playerService.GetItemById(playerToTournament.IdPLayer);
            if (player != null)
            {
                players.Add(player);
            }
        }

        if (players.Count > 0)
        {
            var player = _playerManager.SearchPlayer("Select Player To Move", players);
            bool isPlayerEndDuel = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
                .Any(p => p.EndGame != DateTime.MinValue && p.Id == player.Id);
            if (player == null || isPlayerEndDuel)
            {
                return;
            }
            var playerToMove = playersToTournament.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);

            if (playerToMove != null)
            {
                ConsoleService.WriteTitle("Move Player");
                ConsoleService.WriteLineMessage(ViewGroupsOr2KO(tournament, playersToTournament));

                if (tournament.GamePlaySystem == "Group")
                {
                    string groups = string.Empty;

                    var groupingPlayer = playersToTournament.ListPlayersToTournament.GroupBy(p => p.Group);
                    if (tournament.NumberOfGroups == 2)
                    {
                        playerToMove.Group = groupingPlayer.First(g => g.Key != playerToMove.Group).Key;
                    }
                    else
                    {
                        var groupsList = groupingPlayer.Select(g => g.Key)
                                               .Where(group => group != playerToMove.Group)
                                               .ToList();
                        groups = string.Join(",", groupsList);

                        var key = ConsoleService.GetKeyFromUser($"{playerToMove.TinyFulName} Group: {playerToMove.Group}\n\r" +
                            $"Press Key {groups}");
                        var s = key.KeyChar.ToString().ToUpper();
                        if (groupingPlayer.Any(g => g.Key == key.KeyChar.ToString().ToUpper()) && key.KeyChar.ToString().ToUpper() != playerToMove.Group)
                        {
                            playerToMove.Group = key.KeyChar.ToString().ToUpper();
                        }
                    }
                }
                else
                {
                    ConsoleService.WriteTitle("Move Player");
                    ConsoleService.WriteLineMessage(ViewGroupsOr2KO(tournament, playersToTournament));
                    var newPosition = ConsoleService.GetIntNumberFromUser("Enter New Position", $"\n\r{ViewPlayerToTournamentDetail(playerToMove)}");

                    if (newPosition > 0 && newPosition != null && newPosition <= playersToTournament.ListPlayersToTournament.Count)
                    {
                        if (newPosition > playerToMove.Position)
                        {
                            for (int i = playerToMove.Position + 1; i <= (int)newPosition; i++)
                            {
                                var playerToChange = playersToTournament.ListPlayersToTournament
                                .First(p => p.Position == i);
                                playerToChange.Position = i - 1;
                                playerToChange.TwoKO = playerToChange.Position.ToString();

                                if (i == newPosition)
                                {
                                    playerToMove.Position = (int)newPosition;
                                    playerToMove.TwoKO = playerToMove.Position.ToString();
                                }
                            }
                        }
                        else if (newPosition < playerToMove.Position)
                        {
                            for (int i = playerToMove.Position - 1; i >= (int)newPosition; i--)
                            {
                                var playerToChange = playersToTournament.ListPlayersToTournament
                                .First(p => p.Position == i);

                                playerToChange.Position = i + 1;
                                playerToChange.TwoKO = playerToChange.Position.ToString();

                                if (i == newPosition)
                                {
                                    playerToMove.Position = (int)newPosition;
                                    playerToMove.TwoKO = playerToMove.Position.ToString();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void RandomSelectionOfPlayers(Tournament tournament, PlayersToTournament playersToTournament)
    {
        var randomList = new List<PlayerToTournament>();
        randomList.AddRange(playersToTournament.ListPlayersToTournament);
        var random = new Random();

        while (randomList.Count > 0)
        {
            var randomNumber = random.Next(0, randomList.Count);
            var playerId = randomList[randomNumber].IdPLayer;
            randomList.RemoveAt(randomNumber);
            var player = playersToTournament.ListPlayersToTournament.First(p => p.IdPLayer == playerId);
            player.TwoKO = (randomList.Count + 1).ToString();
            player.Position = randomList.Count + 1;
        }

        playersToTournament.SavePlayersToTournament();
        randomList.AddRange(playersToTournament.ListPlayersToTournament.OrderBy(p => p.TwoKO));

        char group = (char)65;
        if (tournament.GamePlaySystem == "Group" && tournament.NumberOfGroups != 0)
        {
            int numberPlayersOfGroup = randomList.Count / tournament.NumberOfGroups;
            for (int i = 0; i < tournament.NumberOfGroups; i++)
            {
                for (int j = 0; j < numberPlayersOfGroup; j++)
                {
                    randomList[i * numberPlayersOfGroup + j].Group = group.ToString().ToUpper();
                }
                group++;
            }
            var endPlayers = randomList.Count % tournament.NumberOfGroups;
            group = (char)65;

            for (int p = randomList.Count - endPlayers; p < randomList.Count; p++)
            {
                randomList[p].Group = group.ToString().ToUpper();

                if (group == (char)65 + tournament.NumberOfGroups)
                {
                    group = (char)65;
                }
                else
                {
                    group++;
                }
            }
        }

        playersToTournament.ListPlayersToTournament = randomList;
        playersToTournament.SavePlayersToTournament();
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
                ConsoleService.WriteLineErrorMessage("The entered name is already use. Please enter a different name.\n\rPress Any Key...");
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
        AddGamePlaySystem(tournament);
        if (string.IsNullOrEmpty(tournament.GamePlaySystem))
        {
            return null;
        }

        _tournamentsService.AddNewTournament(tournament);
        PlayersToTournament playersToTournament = new PlayersToTournament(tournament, _tournamentsService);
        AddPlayersToTournament(tournament, playersToTournament);
        _tournamentsService.UpdateItem(tournament);
        _tournamentsService.SaveList();
        duel = _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(duel, tournament.Id);
        if (duel == null)
        {
            return null;
        }

        return tournament;
    }

    private void AddPlayersToTournament(Tournament tournament, PlayersToTournament playersToTournament)
    {
        List<PlayerToTournament> listPlayersToTournament = playersToTournament.ListPlayersToTournament.OrderBy(p => p.Position).ToList();
        List<Player> players = new List<Player>();
        if (playersToTournament != null)
        {
            foreach (var playerToTournament in playersToTournament.ListPlayersToTournament)
            {
                var tournamentPlayer = _playerService.GetItemById(playerToTournament.IdPLayer);
                if (tournamentPlayer != null)
                {
                    players.Add(tournamentPlayer);
                }
            }
        }

        var player = _playerManager.SearchPlayer($"Add Player To tournament {tournament.Name}" +
            $"\n\rSelect Player On List Or Press Esc To Add New Player",
            null,
            players);
        if (player == null)
        {
            if (ConsoleService.AnswerYesOrNo("You want to add a new player"))
            {
                player = _playerManager.AddNewPlayer();
            }

            if (player == null)
            {
                return;
            }
        }
        var playeraddress = _playerService.GetPlayerAddress(player);

        if (listPlayersToTournament.Any(p => p.IdPLayer == player.Id))
        {
            ConsoleService.WriteLineErrorMessage($"The Player {player.FirstName} {player.LastName} is on the list\n\rPress Any Key...");
        }
        else
        {
            PlayerToTournament newPlayer = new(player, "------");

            if (listPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.Group)))
            {
                var groupingPlayers = listPlayersToTournament
               .GroupBy(group => group.Group, group => group).OrderBy(g => g.Count());
                newPlayer.Position = listPlayersToTournament.Max(p => p.Position) + 1;
                newPlayer.TwoKO = newPlayer.Position.ToString();
                newPlayer.Group = groupingPlayers.First().Key;
            }
            else
            {
                if (listPlayersToTournament.Any())
                {
                    newPlayer.Position = listPlayersToTournament.Max(p => p.Position) + 1;
                    newPlayer.TwoKO = newPlayer.Position.ToString();
                }
                else
                {
                    newPlayer.Position = 1;
                }
            }

            if (playeraddress != null)
            {
                newPlayer.Country = playeraddress.Country;
            }

            listPlayersToTournament.Add(newPlayer);
            tournament.NumberOfPlayer = listPlayersToTournament.Count;
            playersToTournament.ListPlayersToTournament = listPlayersToTournament;
        }
        _tournamentsService.SaveList();
        playersToTournament.SavePlayersToTournament();
        players.Add(player);
    }

    private void RemovePlayerOfTournament(Tournament tournament, PlayersToTournament playersToTournament)
    {
        List<Player> players = new List<Player>();
        if (playersToTournament.ListPlayersToTournament.Any())
        {
            foreach (var playerToTournament in playersToTournament.ListPlayersToTournament)
            {
                var player = _playerService.GetItemById(playerToTournament.IdPLayer);
                bool isPlayerEndDuel = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
                .Any(p => p.EndGame != DateTime.MinValue && p.Id == player.Id);

                if (player != null || isPlayerEndDuel)
                {
                    players.Add(player);
                }
            }
        }

        if (players.Count > 0)
        {
            var player = _playerManager.SearchPlayer("Remowe Player", players);
            if (player == null)
            {
                return;
            }
            var playerToRemowe = playersToTournament.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);
            if (playerToRemowe != null)
            {
                playersToTournament.ListPlayersToTournament.Remove(playerToRemowe);
                playersToTournament.SavePlayersToTournament();
                tournament.NumberOfPlayer = playersToTournament.ListPlayersToTournament.Count;
                _tournamentsService.SaveList();
            }
        }
    }

    public void ViewListPlayersToTournament(Tournament tournament, PlayersToTournament playersToTournament)
    {
        string formatText = string.Empty;
        if (playersToTournament.ListPlayersToTournament.Any())
        {
            ConsoleService.WriteTitle($"List Players Of {tournament.Name}");
            foreach (var player in playersToTournament.ListPlayersToTournament)
            {
                formatText = playersToTournament.ListPlayersToTournament
                    .Select(p => new { number = $"{playersToTournament.ListPlayersToTournament.IndexOf(player) + 1}. " }).First().number
                     + $"{player.TinyFulName} {player.Country}";
                ConsoleService.WriteLineMessage(formatText);
            }
        }
        else
        {
            ConsoleService.WriteTitle($"List Players Of {tournament.Name}");
            ConsoleService.WriteLineErrorMessage("Empty List\n\rPress Any Key...");
        }
    }

    public string ViewGroupsOr2KO(Tournament tournament, PlayersToTournament playersToTournament)
    {
        var formatText = string.Empty;
        if (tournament.GamePlaySystem == "2KO")
        {
            string lineOne = string.Empty;
            string lineTwo = string.Empty;
            int numberItemOfLine = 6;
            int item = 0;

            if (playersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.TwoKO)))
            {
                formatText += $"\n\rStart List 2KO System\n\r\n\r";
                foreach (var player in playersToTournament.ListPlayersToTournament.OrderBy(p => p.Position))
                {
                    if (player.Position % 2 != 0)
                    {
                        lineOne += $" {player.TwoKO}. {player.TinyFulName}".Remove(20);
                    }
                    else
                    {
                        lineTwo += $" {player.TwoKO}. {player.TinyFulName}".Remove(20);
                    }

                    item++;

                    if (item == numberItemOfLine * 2 ||
                        player.Position == playersToTournament.ListPlayersToTournament.Count)
                    {
                        if (player.Position % 2 != 0)
                        {
                            lineTwo += $"{" Free Win ",-30}";
                        }
                        formatText += lineOne + "\n\r" + lineTwo + "\n\r\n\r";
                        lineOne = string.Empty;
                        lineTwo = string.Empty;
                        item = 0;
                    }
                }
            }
        }
        else if (tournament.GamePlaySystem == "Group")
        {
            if (playersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.TwoKO)))
            {
                if (playersToTournament.ListPlayersToTournament.Any(p => string.IsNullOrEmpty(p.Group)))
                {
                    return formatText;
                }
                var groupingPlayer = playersToTournament.ListPlayersToTournament
                    .GroupBy(group => group.Group, group => group).OrderBy(g => g.Key).ToList();

                List<PlayerToTournament> formatList = new List<PlayerToTournament>();
                decimal numberLine = playersToTournament.ListPlayersToTournament.Count / tournament.NumberOfGroups;

                formatText += "\n\r";
                for (int i = 0; i < tournament.NumberOfGroups; i++)
                {
                    formatText += $"Group: {groupingPlayer[i].Key,-23}";
                }

                formatText += "\n\r";
                for (var j = 0; j <= Math.Floor(numberLine); j++)
                {
                    formatText += "\n\r";
                    for (var i = 0; i < tournament.NumberOfGroups; i++)
                    {
                        var player = groupingPlayer[i].Select(p => p).Except(formatList).FirstOrDefault();
                        if (player != null)
                        {
                            formatList.Add(player);
                            formatText += $"{player.TinyFulName}";
                        }
                        else
                        {
                            formatText += $"{" ",-30}";
                        }
                    }
                }
            }
        }

        return formatText;
    }

    public void UpdateGamePlaySystem(Tournament tournament)
    {
        AddGamePlaySystem(tournament);
        _tournamentsService.UpdateItem(tournament);
        _tournamentsService.SaveList();
    }

    private bool AddGamePlaySystem(Tournament tournament)
    {
        string[] settings = ["Gameplay System"];
        string esc = string.Empty;
        if (!string.IsNullOrEmpty(tournament.GamePlaySystem))
        {
            esc = tournament.GamePlaySystem.ToString();
        }

        tournament.GamePlaySystem = null;
        foreach (string setting in settings)
        {
            if (setting == "Gameplay System")
            {
                var gameplaySystem = new GamePlaySystem();
                int idSelectTypeOfGame = 0;
                do
                {
                    ConsoleService.WriteTitle($"Add settings\r\n{setting}");
                    foreach (var game in gameplaySystem.GamePlaySystemsList)
                    {
                        var formatText = gameplaySystem.GamePlaySystemsList.IndexOf(game) == idSelectTypeOfGame ? $"> {game} <= Select Enter" :
                            $"  {game}";
                        ConsoleService.WriteLineMessage(formatText);
                    }
                    ConsoleKeyInfo inputKey = ConsoleService.GetKeyFromUser();
                    if (inputKey.Key == ConsoleKey.UpArrow && idSelectTypeOfGame > 0)
                    {
                        idSelectTypeOfGame--;
                    }
                    else if (inputKey.Key == ConsoleKey.DownArrow && idSelectTypeOfGame < gameplaySystem.GamePlaySystemsList.Count - 1)
                    {
                        idSelectTypeOfGame++;
                    }
                    else if (inputKey.Key == ConsoleKey.Enter)
                    {
                        tournament.GamePlaySystem = gameplaySystem.GamePlaySystemsList[idSelectTypeOfGame];
                    }
                    else if (inputKey.Key == ConsoleKey.Escape)
                    {
                        tournament.GamePlaySystem = esc;
                        return false;
                    }
                } while (tournament.GamePlaySystem == null);
            }
        }
        return true;
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
            ConsoleService.WriteLineErrorMessage("Empty List\n\rPress Any Key...");
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
                    ConsoleService.WriteLineErrorMessage("No entries found !!!\n\rPress Any Key...");
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
                            ConsoleService.WriteLineErrorMessage("No entries found !!!\n\rPress Any Key...");
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
                            ConsoleService.WriteLineErrorMessage("No entry found !!!\n\rPress Any Key...");
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
            ConsoleService.WriteLineErrorMessage("Empty List\n\rPress Any Key...");
        }
        ConsoleService.WriteLineMessage("Press any key...");
        ConsoleService.GetKeyFromUser();
    }

    public string ViewPlayerToTournamentDetail(PlayerToTournament playerToTournament)
    {
        string formatText = string.Empty;
        if (playerToTournament != null)
        {
            formatText = $"Position: {playerToTournament.Position}.  {playerToTournament.TinyFulName} ";
        }
        return formatText;
    }
}