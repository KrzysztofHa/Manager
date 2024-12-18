using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.Numerics;

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
        if (Tournament.GamePlaySystem == "2KO")
        {
            ConsoleService.WriteLineErrorMessage("2KO Game System Set. Change To Group");
            return;
        }
        else if (Tournament.NumberOfPlayer < 8)
        {
            ConsoleService.WriteLineErrorMessage("Add More Players");
            return;
        }
        ConsoleService.WriteTitle($"{Tournament.NumberOfPlayer} Players allows the tournament to start:");
        if (string.IsNullOrEmpty(Tournament.GamePlaySystem))
        {
            Tournament.GamePlaySystem = "Group";
        }
        if (Tournament.GamePlaySystem == "Group")
        {
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
                if (enterNumber != 4 || enterNumber != 2)
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
                    if (enterNumber != 4 || enterNumber != 2)
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
        }

        if (numberOfGroups == 0 && enterNumber == 2 || enterNumber == 4 || enterNumber == 8)
        {
            numberOfGroups = (int)enterNumber;
        }
        else if (numberOfGroups == 0)
        {
            ConsoleService.WriteLineErrorMessage("Something went wrong, please try again");
            return;
        }

        Tournament.NumberOfGroups = numberOfGroups;
    }

    private void DetermineTheOrderOfDuelsToStartInGroup()
    {
        var allTournamentDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .Where(d => d.IsActive == true && d.IdFirstPlayer != -1);

        var groupingPlayers = PlayersToTournament.ListPlayersToTournament.GroupBy(p => p.Group);
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
        if (PlayersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.TwoKO)))
        {
            if (PlayersToTournament.ListPlayersToTournament.Any(p => string.IsNullOrEmpty(p.Group)))
            {
                return formatText = "Set Groups";
            }
            var groupingPlayer = PlayersToTournament.ListPlayersToTournament
                .GroupBy(group => group.Group, group => group).OrderBy(g => g.Key).ToList();

            List<PlayerToTournament> formatList = new List<PlayerToTournament>();
            decimal numberLine = PlayersToTournament.ListPlayersToTournament.Count / Tournament.NumberOfGroups;

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
        }
        return formatText;
    }

    public override void StartTournament()
    {
        throw new NotImplementedException();
    }

    public override void AddPlayers()
    {
        var newPlayers = PlayersToTournament.AddPlayersToTournament();
        if (newPlayers.Count > 0 && PlayersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.Group)))
        {
            var groupingPlayers = PlayersToTournament.ListPlayersToTournament
           .GroupBy(group => group.Group, group => group).OrderBy(g => g.Count()).Select(g => new { g.Key }).ToList();

            for (var i = 0; i <= groupingPlayers.Count; i++)
            {
                if (i == groupingPlayers.Count)
                {
                    i = -1;
                    continue;
                }
                newPlayers.First().Group = groupingPlayers[i].Key;
                newPlayers.Remove(newPlayers.First());
            }
        }

        if (newPlayers.Count > 0 && Tournament.Start != DateTime.MinValue)
        {
        }
    }

    protected override void RemovePlayers(PlayerToTournament playerToRemove)
    {
        var groupingPlayer = PlayersToTournament.ListPlayersToTournament.GroupBy(d => d.Group);
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

    public override void MovePlayer()
    {
        throw new NotImplementedException();
    }
}