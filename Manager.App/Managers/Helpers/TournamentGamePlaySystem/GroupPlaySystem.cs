using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class GroupPlaySystem : PlaySystems
{
    public GroupPlaySystem(Tournament tournament, ITournamentsManager tournamentsManager, ISinglePlayerDuelManager singlePlayerDuelManager, PlayersToTournament playersToTournament, IPlayerService playerService, IPlayerManager playerManager) : base(tournament, tournamentsManager, singlePlayerDuelManager, playersToTournament, playerService, playerManager)
    {
    }

    public void SetGroups()
    {
        int numberOfGroups = 0;
        int? enterNumber = 0;

        if (Tournament.NumberOfPlayer < 8)
        {
            ConsoleService.WriteLineErrorMessage("Add More Players");
            return;
        }
        ConsoleService.WriteTitle($"{Tournament.NumberOfPlayer} Players allows the tournament to start:");

        if (Tournament.NumberOfPlayer >= 8 && Tournament.NumberOfPlayer < 10)
        {
            ConsoleService.WriteLineMessage("Created 2 groups:\n\r" +
                "4 players will advance from the group to the knockout round\n\rPress Any Key ...");
            ConsoleService.GetKeyFromUser();
            numberOfGroups = 2;
        }
        else if (Tournament.NumberOfPlayer >= 10 && Tournament.NumberOfPlayer < 12)
        {
            ConsoleService.WriteLineMessage("Created 2 groups:\n\r" +
                "4 players will advance from the group to the knockout round\n\rPress Any Key ...");
            ConsoleService.GetKeyFromUser();
            numberOfGroups = 2;
        }
        else if (Tournament.NumberOfPlayer >= 12 && Tournament.NumberOfPlayer < 16)
        {
            ConsoleService.WriteLineMessage("2 groups:\n\r" +
                "4 players will advance from the group to the knockout round\n\r----\n\r");
            ConsoleService.WriteLineMessage("4 groups:\n\r" +
                "2 players will advance from the group to the knockout round\n\r----\n\r");
            enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 2 or 4");
            if (enterNumber != 4 && enterNumber != 2)
            {
                return;
            }
        }
        else if (Tournament.NumberOfPlayer >= 16 && Tournament.NumberOfPlayer < 24)
        {
            if (Tournament.NumberOfPlayer == 16)
            {
                ConsoleService.WriteLineMessage("2 groups, maximum 8 players per group:" +
                            "\n\r4 players will advance from the group to the knockout round\n\r----\r\n");
                ConsoleService.WriteLineMessage("4 groups:\n\r" +
                    "2 players will advance from the group to the knockout round\n\r----\n\r");
                enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 2 or 4");
                if (enterNumber != 4 && enterNumber != 2)
                {
                    return;
                }
            }
            else
            {
                numberOfGroups = 4;
            }
        }
        else if (Tournament.NumberOfPlayer >= 24 && Tournament.NumberOfPlayer < 32)
        {
            if (Tournament.NumberOfPlayer == 24)
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
        else if (Tournament.NumberOfPlayer >= 32 && Tournament.NumberOfPlayer < 49)
        {
            ConsoleService.WriteLineMessage("Eight groups" +
                "\n\r2 players will advance from the group to the knockout round\n\r----\n\rPress Any Key ...");
            ConsoleService.GetKeyFromUser();
            numberOfGroups = 8;
        }
        else if (Tournament.NumberOfPlayer >= 49)
        {
            ConsoleService.WriteLineMessage("Maximum 48 players");
        }

        if (numberOfGroups == 0 && (enterNumber == 2 || enterNumber == 4 || enterNumber == 8))
        {
            numberOfGroups = (int)enterNumber;
        }
        else if (numberOfGroups == 0)
        {
            ConsoleService.WriteLineErrorMessage("Something went wrong, please try again");
            return;
        }

        Tournament.NumberOfGroups = numberOfGroups;
        _tournamentsManager.UpdateTournament(Tournament);
        base.RandomSelectionOfPlayers();
        if (Tournament.NumberOfGroups != 0 && PlayersToTournamentInPlaySystem.ListPlayersToTournament.Any(p => p.Group != string.Empty))
        {
            AssignPlayersToGroups();
        }
    }

    private void AssignPlayersToGroups()
    {
        List<PlayerToTournament> randomList = [.. PlayersToTournamentInPlaySystem.ListPlayersToTournament];

        char group = (char)65;
        if (Tournament.GamePlaySystem == "Group" && Tournament.NumberOfGroups != 0)
            if (Tournament.GamePlaySystem == "Group" && Tournament.NumberOfGroups != 0)
            {
                int numberPlayersOfGroup = randomList.Count / Tournament.NumberOfGroups;
                for (int i = 0; i < Tournament.NumberOfGroups; i++)
                {
                    for (int j = 0; j < numberPlayersOfGroup; j++)
                    {
                        randomList[i * numberPlayersOfGroup + j].Group = group.ToString().ToUpper();
                    }
                    group++;
                }
                var endPlayers = randomList.Count % Tournament.NumberOfGroups;
                group = (char)65;

                for (int p = randomList.Count - endPlayers; p < randomList.Count; p++)
                {
                    randomList[p].Group = group.ToString().ToUpper();

                    if (group == (char)65 + Tournament.NumberOfGroups)
                    {
                        group = (char)65;
                    }
                    else
                    {
                        group++;
                    }
                }
            }
        PlayersToTournamentInPlaySystem.SavePlayersToTournament();
    }

    private void DetermineTheOrderOfDuelsToStartInGroup()
    {
        var allTournamentDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .Where(d => d.IsActive == true && d.IdFirstPlayer != -1);

        var groupingPlayers = PlayersToTournamentInPlaySystem.ListPlayersToTournament.GroupBy(p => p.Group);
        List<SinglePlayerDuel> duelsOfGroup = new();
        foreach (var groupPlayers in groupingPlayers)
        {
            List<int[]> queueDuels = new List<int[]>();
            var numberOfPlayerInGroup = groupPlayers.Count();

            List<int> firstSequenceNumberOfDuel = new List<int>();
            List<int> secondSequenceNumberOfDuel = new List<int>();
            int numberDuelsInRound = numberOfPlayerInGroup / 2;

            var addDuelsToQueue = (List<int> firstSequence, List<int> secondSequence) =>
            {
                queueDuels.AddRange(firstSequence
                .Zip(secondSequence, (first, second) => new int[2] { first, second }));
            };

            if (numberOfPlayerInGroup % 2 != 0)
            {
                numberDuelsInRound++;
            }

            for (int i = 1; i <= numberDuelsInRound; i++)
            {
                firstSequenceNumberOfDuel.Add(i);
                secondSequenceNumberOfDuel.Add(numberDuelsInRound + i);
            }

            for (int i = 1; i <= numberDuelsInRound; i++)
            {
                addDuelsToQueue(firstSequenceNumberOfDuel, secondSequenceNumberOfDuel);
                secondSequenceNumberOfDuel.Add(secondSequenceNumberOfDuel.First());
                secondSequenceNumberOfDuel.Remove(secondSequenceNumberOfDuel.First());
            }

            for (int i = 1; i < numberDuelsInRound; i++)
            {
                for (int j = i + 1; j <= numberDuelsInRound; j++)
                {
                    queueDuels.Add([i, j]);
                    queueDuels.Add([numberDuelsInRound + i, numberDuelsInRound + j]);
                }
            }

            if (numberOfPlayerInGroup % 2 != 0)
            {
                var intToRemove = queueDuels.Where(d => d[1] == numberOfPlayerInGroup + 1).ToList();
                foreach (var intRe in intToRemove)
                {
                    queueDuels.Remove(intRe);
                }
            }

            foreach (var player in groupPlayers)
            {
                duelsOfGroup.AddRange(allTournamentDuels
                    .Where(d => d.IdFirstPlayer == player.IdPLayer || d.IdSecondPlayer == player.IdPLayer)
                    .Except(duelsOfGroup));
            }
            var listGroupingPlayers = groupPlayers.ToList();

            foreach (var duel in duelsOfGroup)
            {
                duel.Group = groupPlayers.Key;
                duel.StartNumberInGroup = queueDuels
                    .FindIndex(d => d[0] == listGroupingPlayers.FindIndex(p => p.IdPLayer == duel.IdFirstPlayer) + 1 && d[1] == listGroupingPlayers.FindIndex(p => p.IdPLayer == duel.IdSecondPlayer) + 1
                     || d[0] == listGroupingPlayers.FindIndex(p => p.IdPLayer == duel.IdSecondPlayer) + 1 && d[1] == listGroupingPlayers.FindIndex(p => p.IdPLayer == duel.IdFirstPlayer) + 1) + 1;
                _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
            }

            duelsOfGroup.Clear();
        }
    }

    public override string ViewTournamentBracket()
    {
        var formatText = string.Empty;

        if (Tournament.NumberOfGroups == 0)
        {
            return formatText;
        }

        if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Any(p => string.IsNullOrEmpty(p.Group)) && Tournament.Start == DateTime.MinValue)
        {
            AssignPlayersToGroups();
        }
        var groupingPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament
            .GroupBy(group => group.Group, group => group).OrderBy(g => g.Key).ToList();

        List<PlayerToTournament> formatList = new List<PlayerToTournament>();
        decimal numberLine = PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count / Tournament.NumberOfGroups;

        formatText += "\n\r";
        for (int i = 0; i < Tournament.NumberOfGroups; i++)
        {
            formatText += $"Group: {groupingPlayer[i].Key,-23}";
        }

        formatText += "\n\r";
        for (var j = 0; j <= Math.Floor(numberLine); j++)
        {
            formatText += "\n\r";
            for (var i = 0; i < Tournament.NumberOfGroups; i++)
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

        return formatText;
    }

    protected override void StartTournament()
    {
        if (Tournament.NumberOfPlayer < 8 || Tournament.End != DateTime.MinValue)
        {
            return;
        }

        _tournamentsManager.StartTournament(Tournament);
        CreateDuelsToTournament();
    }

    private void CreateDuelsToTournament(string round = "Eliminations")
    {
        var listPlayerToBeAssignedToTheDuel = PlayersToTournamentInPlaySystem.ListPlayersToTournament.Where(p => p.Round != round).ToList();

        if (listPlayerToBeAssignedToTheDuel.Count == 0)
        {
            return;
        }

        if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Any(p => string.IsNullOrEmpty(p.Group)))
        {
            return;
        }

        var groupingPlayers = PlayersToTournamentInPlaySystem.ListPlayersToTournament.GroupBy(p => p.Group);

        foreach (var group in groupingPlayers)
        {
            var listPlayersOfGroup = group.ToList();
            if (group.Any(p => p.Round == round))
            {
                listPlayersOfGroup = listPlayersOfGroup.OrderBy(p => p.Round).ToList();
            }

            for (int i = 0; i < listPlayersOfGroup.Count; i++)
            {
                if (!listPlayerToBeAssignedToTheDuel.Contains(listPlayersOfGroup[i]) && listPlayersOfGroup.Any(p => p.Round == round))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(listPlayersOfGroup[i].Round) || !listPlayersOfGroup[i].Round.Equals(round) || listPlayersOfGroup.LastIndexOf(listPlayersOfGroup[i]) == 0)
                {
                    listPlayersOfGroup[i].Round = round;
                }

                for (int j = i + 1; j < listPlayersOfGroup.Count; j++)
                {
                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                    Tournament.Id,
                    listPlayersOfGroup[i].IdPLayer,
                    listPlayersOfGroup[j].IdPLayer,
                    round);
                }
            }
        }

        PlayersToTournamentInPlaySystem.SavePlayersToTournament();
        DetermineTheOrderOfDuelsToStartInGroup();
    }

    public override void AddPlayers()
    {
        var newplayers = PlayersToTournamentInPlaySystem.AddPlayersToTournament();
        if (newplayers.Count > 0 && PlayersToTournamentInPlaySystem.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.Group)))
        {
            var groupingPlayers = PlayersToTournamentInPlaySystem.ListPlayersToTournament.Where(p => !string.IsNullOrEmpty(p.Group))
           .GroupBy(group => group.Group, group => group).OrderBy(g => g.Count()).Select(g => new { g.Key }).ToList();

            for (var i = 0; i < newplayers.Count; i++)
            {
                if (i == groupingPlayers.Count)
                {
                    i = -1;
                    continue;
                }
                newplayers.First().Group = groupingPlayers[i].Key;
                newplayers.Remove(newplayers.First());
            }
            PlayersToTournamentInPlaySystem.SavePlayersToTournament();
        }

        if (Tournament.Start != DateTime.MinValue)
        {
            CreateDuelsToTournament();
        }
    }

    protected override void RemovePlayers(PlayerToTournament playerToRemove)
    {
        var groupingPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.GroupBy(d => d.Group);
        if (groupingPlayer.FirstOrDefault(g => g.Key == playerToRemove.Group).Count() <= 2)
        {
            ConsoleService.WriteLineErrorMessage("You cannot remove a player. Minimum number of players in group 2.");
            return;
        }
        else
        {
            _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToRemove.IdPLayer);
        }
    }

    protected override void RandomSelectionOfPlayers()
    {
        base.RandomSelectionOfPlayers();
        AssignPlayersToGroups();
    }

    protected override void MovePlayer()
    {
        List<Player> players = new List<Player>();
        foreach (var playerToTournament in PlayersToTournamentInPlaySystem.ListPlayersToTournament)
        {
            var player = _playerService.GetItemById(playerToTournament.IdPLayer);
            bool isPlayerEndDuelOrPlay = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
               .Exists(p => (p.IdFirstPlayer == player.Id || p.IdSecondPlayer == player.Id) && (p.StartGame != DateTime.MinValue && p.Interrupted == DateTime.MinValue || p.EndGame != DateTime.MinValue));
            if (player != null && !isPlayerEndDuelOrPlay)
            {
                players.Add(player);
            }
        }

        if (players.Count > 0)
        {
            var player = _playerManager.SearchPlayer("Select Player To Move\n\r" +
                "List of players who can currently be transferred", players);

            if (player == null)
            {
                return;
            }

            var playerToMove = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);

            if (playerToMove != null)
            {
                ConsoleService.WriteTitle("Move Player");
                ConsoleService.WriteLineMessage(ViewTournamentBracket());

                string groups = string.Empty;

                var groupingPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.GroupBy(p => p.Group);

                if (groupingPlayer.FirstOrDefault(g => g.Key == playerToMove.Group).Count() <= 2)
                {
                    ConsoleService.WriteLineErrorMessage("You cannot remove a player. Minimum number of players in group 2.");
                    return;
                }

                if (Tournament.NumberOfGroups == 2)
                {
                    var newGroup = groupingPlayer.First(g => g.Key != playerToMove.Group).Key;
                    playerToMove.Group = newGroup;
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

                if (Tournament.Start != DateTime.MinValue)
                {
                    var duelsPlayerToMove = _singlePlayerDuelManager
                        .GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                        .Where(p => p.IdFirstPlayer == playerToMove.IdPLayer || p.IdSecondPlayer == playerToMove.IdPLayer).ToList();

                    if (duelsPlayerToMove.Count > 0)
                    {
                        playerToMove.Round = string.Empty;
                        _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToMove.IdPLayer);
                        CreateDuelsToTournament();
                    }
                }
            }
            else
            {
                ConsoleService.WriteTitle("No Player To Move");
                ConsoleService.GetKeyFromUser("Press Any Key...");
            }
        }
    }

    protected override List<MenuAction> GetExtendedMenuAction()
    {
        List<MenuAction> actions =
        [
               new MenuAction(1, "Set Number Of Groups", "GroupPlaySystem"),
               new MenuAction(2, "  <-----  Start Tournament", "GroupPlaySystem")
        ];

        if (Tournament.NumberOfGroups == 0)
        {
            actions.First(a => a.Name == "Set Number Of Groups").Name = "  <-----  Set Number Of Groups";
            actions.Remove(actions.First(a => a.Name == "  <-----  Start Tournament"));
        }

        if (Tournament.NumberOfPlayer < 8 && actions.Exists(a => a.Name == "  <-----  Start Tournament"))
        {
            actions.Remove(actions.First(a => a.Name == "  <-----  Start Tournament"));
        }

        if (Tournament.Start != DateTime.MinValue)
        {
            actions.Remove(actions.First(a => a.Name == "Set Number Of Groups"));
            actions.Remove(actions.First(a => a.Name == "  <-----  Start Tournament"));
        }
        return actions;
    }

    protected override void ExecuteExtendedAction(MenuAction menuAction)
    {
        var swichOption = menuAction.Id;
        switch (swichOption)
        {
            case 1:
                SetGroups();
                break;

            case 2:
                if (Tournament.Start == DateTime.MinValue)
                {
                    ConsoleService.WriteTitle($"Start Tournament {Tournament.Name}");
                    if (ConsoleService.AnswerYesOrNo("Before you proceed, make sure is correct everything."))
                    {
                        StartTournament();
                    }
                }
                break;

            default:
                ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                break;
        }
    }
}