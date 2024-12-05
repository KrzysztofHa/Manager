﻿using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class GroupPlaySystem
{
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;
    private readonly ITournamentsManager _tournamentManager;

    public GroupPlaySystem(Tournament tournament, ITournamentsManager tournamentsService, ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        _singlePlayerDuelManager = singlePlayerDuelManager;
        _tournamentManager = tournamentsService;
    }

    private void SetGroups(Tournament tournament, PlayersToTournament playersToTournament)
    {
        int numberOfGroups = 0;
        int? enterNumber = 0;
        if (tournament.GamePlaySystem == "2KO")
        {
            ConsoleService.WriteLineErrorMessage("2KO Game System Set. Change To Group");
            return;
        }
        else if (tournament.NumberOfPlayer < 8)
        {
            ConsoleService.WriteLineErrorMessage("Add More Players");
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
            else if (tournament.NumberOfPlayer >= 16 && tournament.NumberOfPlayer < 24)
            {
                if (tournament.NumberOfPlayer == 16)
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
                ConsoleService.GetKeyFromUser();
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
            ConsoleService.WriteLineErrorMessage("Something went wrong, please try again");
            return;
        }

        tournament.NumberOfGroups = numberOfGroups;
    }

    private void DetermineTheOrderOfDuelsToStartInGroup(Tournament tournament, PlayersToTournament playersToTournament)
    {
        var allTournamentDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
            .Where(d => d.IsActive == true && d.IdFirstPlayer != -1);

        var groupingPlayers = playersToTournament.ListPlayersToTournament.GroupBy(p => p.Group);
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
}