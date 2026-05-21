using Manager.App.Abstract;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class GroupPlaySystem : PlaySystems
{
    public GroupPlaySystem(Tournament tournament, ITournamentsManager tournamentsManager, ISinglePlayerDuelManager singlePlayerDuelManager, PlayersToTournament playersToTournament, IPlayerService playerService, IPlayerManager playerManager) : base(tournament, tournamentsManager, singlePlayerDuelManager, playersToTournament, playerService, playerManager)
    {
    }

    private void CreateQuarterFinalMatches()
    {
        // Collect advancing players per group ordered by GroupPosition
        var advancingPlayers = PlayersToTournamentInPlaySystem.ListPlayersToTournament
            .Where(p => p.Round == "CupQuarterFinal")
            .GroupBy(p => p.Group)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.OrderBy(p => p.GroupPosition).ToList());

        if (!advancingPlayers.Any())
        {
            return;
        }

        var groups = advancingPlayers.Keys.OrderBy(k => k).ToList();
        int groupCount = groups.Count;

        // helper to check existing duels
        bool DuelExists(int firstId, int secondId, string round)
        {
            return _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                .Any(d => d.Round == round && ((d.IdFirstPlayer == firstId && d.IdSecondPlayer == secondId) || (d.IdFirstPlayer == secondId && d.IdSecondPlayer == firstId)));
        }

        // 2 groups: top 4 from each group -> pair A1-B4, A2-B3, A3-B2, A4-B1,
        if (groupCount == 2)
        {
            var a = advancingPlayers[groups[0]];
            var b = advancingPlayers[groups[1]];
            var pairs = new List<(int, int)>
            {
                (a[0].IdPLayer, b[3].IdPLayer),
                (a[1].IdPLayer, b[2].IdPLayer),
                (a[2].IdPLayer, b[1].IdPLayer),
                (a[3].IdPLayer, b[0].IdPLayer)
            };

            foreach (var (f, s) in pairs)
            {
                if (!DuelExists(f, s, "CupQuarterFinal"))
                {
                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(Tournament.Id, f, s, "CupQuarterFinal");
                }
            }
            return;
        }

        // 4 groups: top 2 from each group -> seeding as: A1-D2, B2-C1, A2-B1, C2-D1
        if (groupCount == 4)
        {
            // ensure groups A,B,C,D exist
            var a = advancingPlayers.ContainsKey("A") ? advancingPlayers["A"] : null;
            var b = advancingPlayers.ContainsKey("B") ? advancingPlayers["B"] : null;
            var c = advancingPlayers.ContainsKey("C") ? advancingPlayers["C"] : null;
            var d = advancingPlayers.ContainsKey("D") ? advancingPlayers["D"] : null;
            if (a == null || b == null || c == null || d == null)
            {
                return;
            }

            var quads = new List<(int, int)>
            {
                (a[0].IdPLayer, d[1].IdPLayer),
                (b[1].IdPLayer, c[0].IdPLayer),
                (a[1].IdPLayer, b[0].IdPLayer),
                (c[1].IdPLayer, d[0].IdPLayer)
            };

            foreach (var (f, s) in quads)
            {
                if (!DuelExists(f, s, "CupQuarterFinal"))
                {
                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(Tournament.Id, f, s, "CupQuarterFinal");
                }
            }
            return;
        }

        // 8 groups: top 1 from each group -> build bracket so same-group meet only in final
        if (groupCount == 8)
        {
            // typical seeding for 8 groups: A vs H, B vs G, C vs F, D vs E
            var a = advancingPlayers.ContainsKey("A") ? advancingPlayers["A"][0] : null;
            var b = advancingPlayers.ContainsKey("B") ? advancingPlayers["B"][0] : null;
            var c = advancingPlayers.ContainsKey("C") ? advancingPlayers["C"][0] : null;
            var d = advancingPlayers.ContainsKey("D") ? advancingPlayers["D"][0] : null;
            var e = advancingPlayers.ContainsKey("E") ? advancingPlayers["E"][0] : null;
            var f = advancingPlayers.ContainsKey("F") ? advancingPlayers["F"][0] : null;
            var g = advancingPlayers.ContainsKey("G") ? advancingPlayers["G"][0] : null;
            var h = advancingPlayers.ContainsKey("H") ? advancingPlayers["H"][0] : null;

            var pairs = new List<(int, int)>();
            if (a != null && h != null) pairs.Add((a.IdPLayer, h.IdPLayer));
            if (b != null && g != null) pairs.Add((b.IdPLayer, g.IdPLayer));
            if (c != null && f != null) pairs.Add((c.IdPLayer, f.IdPLayer));
            if (d != null && e != null) pairs.Add((d.IdPLayer, e.IdPLayer));

            foreach (var (fId, sId) in pairs)
            {
                if (!DuelExists(fId, sId, "CupQuarterFinal"))
                {
                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(Tournament.Id, fId, sId, "CupQuarterFinal");
                }
            }
            return;
        }
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
            PlayersToTournamentInPlaySystem.SavePlayersToTournament();
        }
        else { ConsoleService.WriteLineErrorMessage("Set Groups"); }
    }

    private void DetermineTheOrderOfDuelsToStartInGroup()
    {
        var allTournamentDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .Where(d => d.IsActive == true && d.IdFirstPlayer != -1);
        if (allTournamentDuels.Count() > 0)
        {
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

            var sortedDuels = allTournamentDuels.OrderBy(d => d.StartNumberInGroup).ToList();
            foreach (var duel in sortedDuels)
            {
                duel.StartNumberInTournament = sortedDuels.IndexOf(duel) + 1;
                _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
            }
        }
    }

    public List<Tuple<string, List<Tuple<PlayerToTournament, List<Tuple<string, int>>, List<Tuple<int, bool>>>>>> GetStatistic()
    {
        var statisticsList = new List<Tuple<string, List<Tuple<PlayerToTournament, List<Tuple<string, int>>, List<Tuple<int, bool>>>>>>();
        var statisticsListOfGroup = new List<Tuple<PlayerToTournament, List<Tuple<string, int>>, List<Tuple<int, bool>>>>();
        Tuple<PlayerToTournament, List<Tuple<string, int>>, List<Tuple<int, bool>>> duelStatistic;
        var allDuel = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id);
        int winMatch, pointWin, pointLost;
        var directDuels = new List<Tuple<int, bool>>();
        Tuple<int, bool> singleDirectDuelstatistic;
        var allMatchesStatistics = new List<Tuple<string, int>>();
        var groupingPlayerByGroup = PlayersToTournamentInPlaySystem.ListPlayersToTournament.GroupBy(p => p.Group).ToList();
        foreach (var key in groupingPlayerByGroup)
        {
            foreach (var player in key)
            {
                winMatch = 0;
                pointWin = 0;
                pointLost = 0;
                bool dirctDuel = false;
                var duelOfPlayer = allDuel.Where(d => (d.IdFirstPlayer == player.IdPLayer || d.IdSecondPlayer == player.IdPLayer) && d.Round == "Eliminations" && d.EndGame != DateTime.MinValue);
                allMatchesStatistics.Add(new Tuple<string, int>("Played", duelOfPlayer.Count()));
                foreach (var duel in duelOfPlayer)
                {
                    if (duel.IdFirstPlayer == player.IdPLayer)
                    {
                        pointWin += duel.ScoreFirstPlayer;
                        pointLost += duel.ScoreSecondPlayer;
                        if (duel.ScoreFirstPlayer == duel.RaceTo)
                        {
                            winMatch++;
                            dirctDuel = true;
                        }
                        singleDirectDuelstatistic = Tuple.Create(duel.IdSecondPlayer, dirctDuel);
                        directDuels.Add(singleDirectDuelstatistic);
                    }
                    else
                    {
                        pointWin += duel.ScoreSecondPlayer;
                        pointLost += duel.ScoreFirstPlayer;
                        if (duel.ScoreSecondPlayer == duel.RaceTo)
                        {
                            winMatch++;
                            dirctDuel = true;
                        }
                        singleDirectDuelstatistic = Tuple.Create(duel.IdFirstPlayer, dirctDuel);
                        directDuels.Add(singleDirectDuelstatistic);
                    }
                    dirctDuel = false;
                }
                allMatchesStatistics.Add(new Tuple<string, int>("Win", winMatch));
                allMatchesStatistics.Add(new Tuple<string, int>("Pt.Win", pointWin));
                allMatchesStatistics.Add(new Tuple<string, int>("Pt.Lost", pointLost));
                duelStatistic = new Tuple<PlayerToTournament, List<Tuple<string, int>>, List<Tuple<int, bool>>>(player, allMatchesStatistics, directDuels);
                var listOfDirectDuelsOfPlayer = statisticsListOfGroup
                    .Where(e => e.Item2.Any(t => t.Item1 == "Win" && t.Item2 == winMatch) && e.Item2.Any(t => t.Item1 == "Pt.Win" && t.Item2 == pointWin) && e.Item2.Any(t => t.Item1 == "Pt.Lost" && t.Item2 == pointLost)).ToList();
                if (allDuel.All(d => d.Round == "Eliminations" && d.EndGame != DateTime.MinValue))
                {
                    if (listOfDirectDuelsOfPlayer.Count() > 0)
                    {
                        foreach (var duel in listOfDirectDuelsOfPlayer)
                        {
                            var directDuelResult = duel.Item3.FirstOrDefault(d => d.Item1 == player.IdPLayer).Item2;
                            if (!directDuelResult)
                            {
                                var index = statisticsListOfGroup.IndexOf(duel);
                                statisticsListOfGroup.Insert(index, duelStatistic);
                                SetPositionInGroup(statisticsListOfGroup);
                            }
                        }
                    }
                    else
                    {
                        statisticsListOfGroup.Add(duelStatistic);
                        SetPositionInGroup(statisticsListOfGroup);
                    }
                }
                else
                {
                    statisticsListOfGroup.Add(duelStatistic);
                    SetPositionInGroup(statisticsListOfGroup);
                }

                statisticsListOfGroup = statisticsListOfGroup.OrderBy(s => s.Item1.GroupPosition).ToList();
                directDuels = new List<Tuple<int, bool>>();
                allMatchesStatistics = new List<Tuple<string, int>>();
            }
            statisticsList.Add(new Tuple<string, List<Tuple<PlayerToTournament, List<Tuple<string, int>>, List<Tuple<int, bool>>>>>(key.Key, statisticsListOfGroup));
            statisticsListOfGroup = new List<Tuple<PlayerToTournament, List<Tuple<string, int>>, List<Tuple<int, bool>>>>();
        }

        SetCupPosition(statisticsList, allDuel);

        return statisticsList;
    }

    private void SetPositionInGroup(List<Tuple<PlayerToTournament, List<Tuple<string, int>>, List<Tuple<int, bool>>>> statistics)
    {
        if (!_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).Any(d => d.Round == "CupQuarterFinal"))
        {
            statistics = statistics.OrderByDescending(s => s.Item2.FirstOrDefault(t => t.Item1 == "Win").Item2)
            .ThenByDescending(s => s.Item2.FirstOrDefault(t => t.Item1 == "Pt.Win").Item2)
            .ThenBy(s => s.Item2.FirstOrDefault(t => t.Item1 == "Pt.Lost").Item2).ToList();
            foreach (var player in statistics)
            {
                player.Item1.GroupPosition = statistics.IndexOf(player) + 1;
            }
            PlayersToTournamentInPlaySystem.SavePlayersToTournament();
        }
    }

    private void SetCupPosition(List<Tuple<string, List<Tuple<PlayerToTournament, List<Tuple<string, int>>, List<Tuple<int, bool>>>>>> statisticsList, List<SinglePlayerDuel>? allDuel)
    {
        if (!allDuel.Any(m => m.Round == "CupQuarterFinal"))
        {
            var playersToKnockout = 8;
            var playersToKnockoutOfGroup = 8 / Tournament.NumberOfGroups;

            var playerToKnockout = statisticsList.SelectMany(g => g.Item2.Where(p => p.Item1.GroupPosition <= 2)).ToList();

            foreach (var group in statisticsList)
            {
                var positionOfCupAfterGroup = 5;
                for (int i = 1; i <= group.Item2.Count; i++)
                {
                    if (i <= 2)
                    {
                        group.Item2.ElementAt(i - 1).Item1.Round = "CupQuarterFinal";
                        group.Item2.ElementAt(i - 1).Item1.CupPosition = positionOfCupAfterGroup;
                    }
                    else
                    {
                        positionOfCupAfterGroup++;
                        group.Item2.ElementAt(i - 1).Item1.CupPosition = positionOfCupAfterGroup;
                    }
                }
            }
            PlayersToTournamentInPlaySystem.SavePlayersToTournament();
            return;
        }

        var duelsCupQuarterFinal = allDuel
           .Where(d => d.Round == "CupQuarterFinal" && d.EndGame != DateTime.MinValue).ToList();
        if (!allDuel.Any(d => d.Round == "CupSemiFinal") && duelsCupQuarterFinal.Count > 0 && duelsCupQuarterFinal.All(d => d.EndGame != DateTime.MinValue))
        {
            foreach (var duel in duelsCupQuarterFinal)
            {
                if (duel.ScoreFirstPlayer == duel.RaceTo)
                {
                    var player = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdFirstPlayer);
                    player.Round = "CupSemiFinal";
                    if (!duelsCupQuarterFinal.Any(d => d.Round == "CupSemiFinal"))
                    {
                        player.CupPosition = player.CupPosition - 1;
                    }
                }
                else
                {
                    var player = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdSecondPlayer);
                    player.Round = "CupSemiFinal";
                    if (!duelsCupQuarterFinal.Any(d => d.Round == "CupSemiFinal"))
                    {
                        player.CupPosition = player.CupPosition - 1;
                    }
                }
            }
            PlayersToTournamentInPlaySystem.SavePlayersToTournament();
            return;
        }

        var duelsCupSemiFinal = allDuel
            .Where(d => d.Round == "CupSemiFinal" && d.EndGame != DateTime.MinValue).ToList();
        if (!allDuel.Any(d => d.Round == "CupFinal") && duelsCupSemiFinal.Count > 0 && duelsCupSemiFinal.All(d => d.EndGame != DateTime.MinValue))
        {
            foreach (var duel in duelsCupSemiFinal)
            {
                if (duel.ScoreFirstPlayer == duel.RaceTo)
                {
                    var player = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdFirstPlayer);
                    player.Round = "CupFinal";
                    if (!duelsCupSemiFinal.Any(d => d.Round == "CupFinal"))
                    {
                        player.CupPosition = player.CupPosition - 1;
                    }
                }
                else
                {
                    var player = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdSecondPlayer);
                    player.Round = "CupFinal";
                    if (!duelsCupSemiFinal.Any(d => d.Round == "CupFinal"))
                    {
                        player.CupPosition = player.CupPosition - 1;
                    }
                }
            }
            PlayersToTournamentInPlaySystem.SavePlayersToTournament();
            return;
        }

        var duelCupFinal = allDuel
            .Where(d => d.Round == "CupFinal" && d.EndGame != DateTime.MinValue).ToList();
        if (duelCupFinal.Count > 0 && duelCupFinal.All(d => d.EndGame != DateTime.MinValue))
        {
            foreach (var duel in duelCupFinal)
            {
                if (duel.ScoreFirstPlayer == duel.RaceTo)
                {
                    var player = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdFirstPlayer);
                    player.CupPosition = 1;

                    player.Round = "Winner";
                }
                else
                {
                    var player = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdSecondPlayer);
                    player.CupPosition = 1;

                    player.Round = "Winner";
                }
            }
            PlayersToTournamentInPlaySystem.SavePlayersToTournament();
            return;
        }
    }

    public override string ViewStatisticOfText()
    {
        var formatText = string.Empty;
        var statisticList = GetStatistic();

        if (Tournament.NumberOfGroups == 0)
        {
            return "Set Groups\r\n";
        }

        if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Any(p => string.IsNullOrEmpty(p.Group)) && Tournament.Start == DateTime.MinValue)
        {
            AssignPlayersToGroups();
        }

        foreach (var group in statisticList)
        {
            formatText += "\n\r";

            formatText += $"Group: {group.Item1,-25} {group.Item2[0].Item2[0].Item1} {group.Item2[0].Item2[1].Item1} {group.Item2[0].Item2[2].Item1} {group.Item2[0].Item2[3].Item1}";

            formatText += "\n\r";
            foreach (var playerStatistic in group.Item2)
            {
                if (playerStatistic != null)
                {
                    formatText += $" {playerStatistic.Item1.GroupPosition}. {playerStatistic.Item1.TinyFulName}  {playerStatistic.Item2[0].Item2,-4} {playerStatistic.Item2[1].Item2,-5} {playerStatistic.Item2[2].Item2,-5} {playerStatistic.Item2[3].Item2}\n\r";
                }
            }
        }
        return formatText;
    }

    public override string ViewTournamentBracket()
    {
        var formatText = string.Empty;
        var formatTextUp = string.Empty;
        var formatTextDown = string.Empty;

        if (Tournament.NumberOfGroups == 0)
        {
            return formatText;
        }

        if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Any(p => string.IsNullOrEmpty(p.Group)) && Tournament.Start == DateTime.MinValue)
        {
            AssignPlayersToGroups();
        }

        var listPlayerToTournamentSortedByCupPosition = PlayersToTournamentInPlaySystem.ListPlayersToTournament
            .OrderBy(p => p.CupPosition)
            .ToList();
        if (listPlayerToTournamentSortedByCupPosition.Any(p => p.Round == "CupQuarterFinal"))
        {
            var groupingByRound = listPlayerToTournamentSortedByCupPosition
                .GroupBy(p => p.Round)
                .ToList();
            var allDuelsInCup = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                .Where(d => d.Round == "CupQuarterFinal" || d.Round == "CupSemiFinal" || d.Round == "CupFinal").ToList();

            if (listPlayerToTournamentSortedByCupPosition.Any(d => d.Round == "Winner"))
            {
                formatText += "\n\r";
                formatText += $"{$"Winner: {listPlayerToTournamentSortedByCupPosition.FirstOrDefault(p => p.Round == "Winner").FirstName} {listPlayerToTournamentSortedByCupPosition.FirstOrDefault(p => p.Round == "Winner").LastName}\n\r",68}";
                formatText += $"{"---------------",67}\n\r\n\r";
            }
            if (allDuelsInCup.Any(d => d.Round == "CupFinal"))
            {
                formatText += $"{"CupFinal",63}\n\r";
                foreach (var duel in allDuelsInCup.Where(d => d.Round == "CupFinal"))
                {
                    var firstPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdFirstPlayer);
                    var secondPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdSecondPlayer);
                    formatText += $"{$""}" + "\n\r";
                    formatText += $"{$" {firstPlayer.FirstName} {firstPlayer.LastName}: {duel.ScoreFirstPlayer}",67}\n\r";
                    formatText += $"{$" {secondPlayer.FirstName} {secondPlayer.LastName}: {duel.ScoreSecondPlayer}",67}";
                    formatText += "\n\r\n\r";
                }
            }
            if (allDuelsInCup.Any(d => d.Round == "CupSemiFinal"))
            {
                formatText += $"{"CupSemiFinal",66}\n\r\n\r";
                foreach (var duel in allDuelsInCup.Where(d => d.Round == "CupSemiFinal"))
                {
                    var firstPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdFirstPlayer);
                    var secondPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdSecondPlayer);

                    formatTextUp += $"{$"{firstPlayer.FirstName} {firstPlayer.LastName}: {duel.ScoreFirstPlayer}",25}";

                    formatTextDown += $"{$"{secondPlayer.FirstName} {secondPlayer.LastName}: {duel.ScoreSecondPlayer}",25}";
                }
                formatText += $"{formatTextUp,81}\n\r" + $"{formatTextDown,81}";
                formatText += "\n\r\n\r";
            }
            if (allDuelsInCup.Any(d => d.Round == "CupQuarterFinal"))
            {
                formatTextDown = string.Empty;
                formatTextUp = string.Empty;
                formatText += $"{"CupQuarterFinal",68}\n\r\n\r";
                foreach (var duel in allDuelsInCup.Where(d => d.Round == "CupQuarterFinal"))
                {
                    var firstPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdFirstPlayer);
                    var secondPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdSecondPlayer);

                    formatTextUp += $"{$"{firstPlayer.FirstName} {firstPlayer.LastName}: {duel.ScoreFirstPlayer}",25} ";

                    formatTextDown += $"{$"{secondPlayer.FirstName} {secondPlayer.LastName}: {duel.ScoreSecondPlayer}",25} ";
                }
                formatText += $"{formatTextUp}\n\r" + $"{formatTextDown}";
                formatText += "\n\r\n\r";
            }
        }

        var groupingPlayerAndSortingByGroupPosition = PlayersToTournamentInPlaySystem.ListPlayersToTournament
        .GroupBy(p => p.Group)
        .OrderBy(g => g.Key)
        .Select(g => g.OrderBy(p => p.GroupPosition).ToList())
        .ToList();

        List<PlayerToTournament> formatList = new List<PlayerToTournament>();
        decimal numberLine = PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count / Tournament.NumberOfGroups;

        formatText += "\n\r";
        for (int i = 0; i < Tournament.NumberOfGroups; i++)
        {
            formatText += $"Group: {groupingPlayerAndSortingByGroupPosition[i][0].Group,-27}";
        }

        formatText += "\n\r";
        for (var j = 0; j <= Math.Floor(numberLine); j++)
        {
            formatText += "\n\r";
            for (var i = 0; i < Tournament.NumberOfGroups; i++)
            {
                var player = groupingPlayerAndSortingByGroupPosition[i].Select(p => p).Except(formatList).FirstOrDefault();
                if (player != null)
                {
                    if (player.GroupPosition == 0)
                    {
                        player.GroupPosition = groupingPlayerAndSortingByGroupPosition[i].IndexOf(player) + 1;
                    }
                    formatList.Add(player);
                    formatText += $" {player.GroupPosition}. {player.TinyFulName}";
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

    protected override void CreateDuelsToTournament(string round = "Eliminations")
    {
        if (round == "Eliminations")
        {
            var listPlayerToBeAssignedToTheDuel = PlayersToTournamentInPlaySystem.ListPlayersToTournament
                .Where(p => string.IsNullOrEmpty(p.Round)).ToList();
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
        else if (round == "CupSemiFinal")
        {
            CreateCupSemiFinalMaches();
        }
        else if (round == "CupQuarterFinal")
        {
            CreateQuarterFinalMatches();
        }
        else if (round == "CupFinal")
        {
            CreateCupFinalMatch();
        }
    }

    private void CreateCupFinalMatch()
    {
        var listPlayerToBeAssignedToTheDuel = PlayersToTournamentInPlaySystem.ListPlayersToTournament
            .Where(p => p.Round == "CupFinal").ToList();
        if (!_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).Any(d => d.Round == "CupFinal"))
        {
            if (listPlayerToBeAssignedToTheDuel.Count == 2)
            {
                _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                Tournament.Id,
                listPlayerToBeAssignedToTheDuel[0].IdPLayer,
                listPlayerToBeAssignedToTheDuel[1].IdPLayer,
                "CupFinal");
            }
        }
    }

    private void CreateCupSemiFinalMaches()
    {
        var listPlayerToBeAssignedToTheDuel = PlayersToTournamentInPlaySystem.ListPlayersToTournament
            .Where(p => p.Round == "CupSemiFinal").ToList();

        var groupcount = Tournament.NumberOfGroups;
        if (groupcount == 2)
        {
            foreach (var player in listPlayerToBeAssignedToTheDuel)
            {
                if (player.Group == "A" && player.GroupPosition == 1)
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "B" && p.GroupPosition == 2 || p.Group == "A" && p.GroupPosition == 3);
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                }
                else if (player.Group == "B" && player.GroupPosition == 4)
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "B" && p.GroupPosition == 2 || p.Group == "A" && p.GroupPosition == 3);
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                }
                else if (player.Group == "A" && player.GroupPosition == 2)
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "B" && p.GroupPosition == 1 || p.Group == "A" && p.GroupPosition == 4);
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                }
                else if (player.Group == "B" && player.GroupPosition == 3)
                {
                    var secondPlayer2 = listPlayerToBeAssignedToTheDuel
                    .FirstOrDefault(p => p.Group == "B" && p.GroupPosition == 1 || p.Group == "A" && p.GroupPosition == 4);
                    if (secondPlayer2 != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer2.IdPLayer,
                        "CupSemiFinal");
                    }
                }
            }
        }

        if (groupcount == 4)
        {
            foreach (var player in listPlayerToBeAssignedToTheDuel)
            {
                if (player.Group == "A" && player.GroupPosition == 1)
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "B" && p.GroupPosition == 2 || p.Group == "C" && p.GroupPosition == 1);
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                }
                else if (player.Group == "D" && player.GroupPosition == 2)
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "B" && p.GroupPosition == 2 || p.Group == "C" && p.GroupPosition == 1);
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                }
                else if (player.Group == "A" && player.GroupPosition == 2)
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "D" && p.GroupPosition == 1 || p.Group == "C" && p.GroupPosition == 2);
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                }
                else if (player.Group == "B" && player.GroupPosition == 1)
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "C" && p.GroupPosition == 2 || p.Group == "D" && p.GroupPosition == 1);
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                }
            }
        }
        if (groupcount == 8)
        {
            foreach (var player in listPlayerToBeAssignedToTheDuel)
            {
                if (player.Group == "A")
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "H");
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                }
                else if (player.Group == "B")
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "G");
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                }
                else if (player.Group == "C")
                {
                    var secondPlayer = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "F");
                    if (secondPlayer != null)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                        Tournament.Id,
                        player.IdPLayer,
                        secondPlayer.IdPLayer,
                        "CupSemiFinal");
                    }
                    else if (player.Group == "D")
                    {
                        var secondPlayer2 = listPlayerToBeAssignedToTheDuel
                        .FirstOrDefault(p => p.Group == "E");
                        if (secondPlayer2 != null)
                        {
                            _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                            Tournament.Id,
                            player.IdPLayer,
                            secondPlayer2.IdPLayer,
                            "CupSemiFinal");
                        }
                    }
                }
            }
        }
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
        if (Tournament.NumberOfGroups != 0)
        {
            base.RandomSelectionOfPlayers();
            AssignPlayersToGroups();
        }
        else
        {
            ConsoleService.WriteLineErrorMessage("Set Groups");
        }
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
                        DetermineTheOrderOfDuelsToStartInGroup();
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
            actions.Add(new MenuAction(3, "View Group Statistics", "GroupPlaySystem"));
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

            case 3:
                ConsoleService.WriteTitle($"Tournament {Tournament.Name}");
                ConsoleService.WriteMessage(ViewStatisticOfText());
                ConsoleService.GetKeyFromUser("Press any key ...");
                break;

            default:
                ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                break;
        }
    }

    protected override void EndTournament()
    {
        _tournamentsManager.EndTournament(Tournament);
    }

    protected override void StartNextRound()
    {
        var statistiklist = GetStatistic().ToList();
        if (statistiklist.Any(p => p.Item2.Any(s => s.Item1.Round == "Winner")))
        {
            ConsoleService.WriteTitle($"End Tournament {Tournament.Name}");
            ConsoleService.GetKeyFromUser("Press Any Key...");
            EndTournament();
        }
        else if (statistiklist.Any(p => p.Item2.Any(s => s.Item1.Round == "CupFinal")))
        {
            ConsoleService.WriteTitle("The player who won the final match is the winner of the tournament.");
            ConsoleService.GetKeyFromUser("Press Any Key...");
            CreateDuelsToTournament("CupFinal");
            StartOrInterruptedTournamentDuel();
        }
        else if (statistiklist.Any(p => p.Item2.Any(s => s.Item1.Round == "CupSemiFinal")))
        {
            ConsoleService.WriteTitle("Start New Round");
            ConsoleService.GetKeyFromUser("Press Any Key...");
            CreateDuelsToTournament("CupSemiFinal");
            StartOrInterruptedTournamentDuel();
        }
        else if (statistiklist.Any(p => p.Item2.Any(s => s.Item1.Round == "CupQuarterFinal")))
        {
            ConsoleService.WriteTitle($"End Eliminations {Tournament.Name}");
            ConsoleService.WriteLineMessage("The players who will advance to the knockout round are those who took 1st and 2nd place in the group.\n\r" +
            "If there are more than 2 groups, the players will be paired according to the following scheme:\n\r" +
            "1st place of group A vs 2nd place of group D\n\r" +
            "1st place of group B vs 2nd place of group C\n\r" +
            "1st place of group C vs 2nd place of group B\n\r" +
            "1st place of group D vs 2nd place of group A\n\r");
            ConsoleService.GetKeyFromUser("Press Any Key...");
            CreateDuelsToTournament("CupQuarterFinal");
            StartOrInterruptedTournamentDuel();
        }
    }
}