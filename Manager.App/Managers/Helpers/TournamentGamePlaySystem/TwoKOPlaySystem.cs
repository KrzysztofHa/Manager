using Manager.App.Abstract;
using Manager.App.Managers.Helpers.TournamentGamePlaySystem;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;

namespace Manager.App.Managers.Helpers.GamePlaySystem;

public class TwoKOPlaySystem : PlaySystems
{
    // This class represents a tournament play system that follows a two-knockout (2KO) format.
    // It inherits from the PlaySystems base class and provides specific implementations for adding players, moving players, starting the tournament, creating duels, and viewing the tournament bracket.
    public TwoKOPlaySystem(Tournament tournament, ITournamentsManager tournamentsManager, ISinglePlayerDuelManager singlePlayerDuelManager, PlayersToTournament playersToTournament, IPlayerService playerService, IPlayerManager playerManager) : base(tournament, tournamentsManager, singlePlayerDuelManager, playersToTournament, playerService, playerManager)
    {
    }

    public override void AddPlayers()
    {
        var newPlayers = PlayersToTournamentInPlaySystem.AddPlayersToTournament();
        if (Tournament.NumberOfPlayer > 8)
        {
            AssignPlayersToGroupAndPosition();
        }

        if (newPlayers.Count > 0 && Tournament.Start != DateTime.MinValue)
        {
            CreateDuelsToTournament();
        }
    }

    private void AssignPlayersToGroupAndPosition()
    {
        // This method assigns players to groups and positions within the tournament.
        // It first retrieves a list of players who are currently in the tournament and orders them by their position.
        // It then iterates through the list of players and assigns them to groups (A, B, C, etc.) and positions within those groups.
        // The group is determined by the index of the player in the list, and the position within the group is incremented for each player.
        // Finally, it saves the updated player information to the tournament.

        var listPlayersToTournament = PlayersToTournamentInPlaySystem.ListPlayersToTournament.OrderBy(p => p.Position).ToList();
        char group = (char)65;
        var positionInGroup = 1;

        for (var i = 0; i < listPlayersToTournament.Count; i++)
        {
            if (i > 0 && i % 4 == 0)
            {
                group++;
                positionInGroup = 1;
            }

            listPlayersToTournament[i].Group = group.ToString();
            listPlayersToTournament[i].GroupPosition = positionInGroup++;
        }

        PlayersToTournamentInPlaySystem.SavePlayersToTournament();
    }

    protected override void MovePlayer()
    {
        // This method allows the user to move a player to a new position in the tournament bracket.
        // It first retrieves a list of players who are currently in the tournament and have not yet completed their duels or matches.
        // It then prompts the user to select a player to move and enter a new position for that player.
        // If the new position is valid, it swaps the positions of the selected player and the player currently in that position.
        // Finally, it updates the player information in the tournament to reflect the new positions.

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
                var newPosition = ConsoleService.GetIntNumberFromUser("Enter New Position", $"\n\r{PlayersToTournamentInPlaySystem.ViewPlayerToTournamentDetail(playerToMove)}");

                if (newPosition > 0 && newPosition != null && newPosition <= PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count)
                {
                    var playerToChange = PlayersToTournamentInPlaySystem.ListPlayersToTournament
                     .First(p => p.Position == (int)newPosition);
                    Tuple<string, int> groupAndPosition = new Tuple<string, int>(playerToChange.Group, playerToChange.GroupPosition);
                    playerToChange.Position = playerToMove.Position;
                    playerToChange.Group = playerToMove.Group;
                    playerToChange.GroupPosition = playerToMove.GroupPosition;
                    playerToMove.Group = groupAndPosition.Item1;
                    playerToMove.GroupPosition = groupAndPosition.Item2;
                    playerToMove.Position = (int)newPosition;
                }
            }
        }
    }

    protected override void StartTournament()
    {
        // This method starts the tournament if there are enough players and the tournament has not already ended.
        // It first checks if the number of players in the tournament is less than 8 or if the tournament has already ended. If either condition is true, it returns without starting the tournament.
        // If the tournament can be started, it calls the StartTournament method of the tournaments manager to start the tournament.
        if (Tournament.NumberOfPlayer < 8 || Tournament.End != DateTime.MinValue)
        {
            return;
        }

        _tournamentsManager.StartTournament(Tournament);
        CreateDuelsToTournament();
        StartDuelsInRoundOfTableNumber();
    }

    private void StartDuelsInRoundOfTableNumber(string round = "Eliminations")
    {
        // This method starts the duels in the current round of the tournament based on the number of tables available.
        // It retrieves all the duels in the tournament and checks if there are any duels that have not yet started.
        // If there are duels that have not started, it starts them based on the number of tables available in the tournament.
        // The method assigns table numbers to the duels and updates their start time.
        // It also handles the case where there are duels with a free win (i.e., one player has no opponent) and starts those duels as well.

        var allDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .Where(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer != -1).ToList();

        round = round != allDuels.Last().Round ? allDuels.Last().Round : round;

        var duelsOfFreeWin = allDuels.Where(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1).ToList();

        var duelsInRound = allDuels
            .Where(d => d.Round == round && d.StartGame != DateTime.MinValue).Except(duelsOfFreeWin);

        if (Tournament.Start != DateTime.MinValue && duelsInRound.All(d => d.StartGame == DateTime.MinValue))
        {
            var duelsToStart = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                .Where(d => d.EndGame == DateTime.MinValue && d.Round == round && d.IsActive == true && d.IdFirstPlayer != -1 && d.IdSecondPlayer != -1)
                .OrderBy(d => d.NumberDuelOfTournament).ToList();

            if (duelsToStart.Count > 0)
            {
                for (var i = 0; i < Tournament.NumberOfTables; i++)
                {
                    if (i >= duelsToStart.Count)
                    {
                        break;
                    }
                    duelsToStart[i].TableNumber = i + 1;
                    _singlePlayerDuelManager.StartSingleDuel(duelsToStart[i]);
                }
            }
        }
    }

    protected override void CreateDuelsToTournament(string round = "Eliminations")
    {
        // This method creates duels for the tournament based on the current round and the players who have not yet been assigned to a round.
        // It first checks if there are any new players who have not been assigned to a round. If there are, it assigns them to groups and positions.
        // It then retrieves all the duels in the current round and checks if there are any duels that have a second player with an ID of -1 (indicating a free win).
        // If there is only one new player and the current round is "Eliminations", it assigns that player to the duel with a free win.
        // If there are multiple new players and the current round is "Eliminations", it creates new duels for the new players in pairs.
        // If there is an odd number of new players, the last player will receive a free win and automatically advance to the next round.

        var listNewPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.Where(p => string.IsNullOrEmpty(p.Round)).ToList();

        if (listNewPlayer.Count > 0)
        {
            AssignPlayersToGroupAndPosition();
            var allDuelInRound = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                     .Where(d => d.Round == round && d.IdFirstPlayer != 0 && d.IdSecondPlayer != 0);

            if (listNewPlayer.Count == 1 && round == "Eliminations")
            {
                if (allDuelInRound.Any(d => d.IdSecondPlayer == -1))
                {
                    var duelToNewPlayer = allDuelInRound.First(d => d.IdSecondPlayer == -1);
                    duelToNewPlayer.IdSecondPlayer = listNewPlayer.First().IdPLayer;
                    duelToNewPlayer.ScoreFirstPlayer = 0;
                    duelToNewPlayer.ScoreSecondPlayer = 0;
                    listNewPlayer.First().Round = round;
                    duelToNewPlayer.Round = round;
                    _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToNewPlayer);
                }
                else
                {
                    var newDuel = _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                           Tournament.Id,
                           listNewPlayer.First().IdPLayer,
                           -1,
                           round);
                    newDuel.ScoreFirstPlayer = allDuelInRound.Max(d => d.RaceTo);
                    newDuel.ScoreSecondPlayer = 0;
                    newDuel.StartGame = DateTime.Now;
                    newDuel.EndGame = DateTime.Now;
                    _singlePlayerDuelManager.UpdateSinglePlayerDuel(newDuel);
                    listNewPlayer.First().Round = round;
                }
            }
            else if (listNewPlayer.Count >= 8 && round == "Eliminations")
            {
                for (int i = 0; i < listNewPlayer.Count; i += 2)
                {
                    if (((PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count - listNewPlayer.Count) % 2 != 0
                        || PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count - listNewPlayer.Count == 0)
                        && listNewPlayer[i].Equals(listNewPlayer.Last()))
                    {
                        var newDuel = _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                            Tournament.Id,
                            listNewPlayer.Last().IdPLayer,
                            -1,
                            round);
                        newDuel.ScoreFirstPlayer = allDuelInRound.Max(d => d.RaceTo);
                        newDuel.ScoreSecondPlayer = 0;
                        newDuel.StartGame = DateTime.Now;
                        newDuel.EndGame = DateTime.Now;
                        _singlePlayerDuelManager.UpdateSinglePlayerDuel(newDuel);
                        listNewPlayer.Last().Round = round;
                        break;
                    }
                    else if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                        .Any(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1))
                    {
                        var duelToupdate = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                            .First(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1);
                        duelToupdate.Round = round;
                        duelToupdate.ScoreFirstPlayer = listNewPlayer[i].IdPLayer;
                        duelToupdate.IdSecondPlayer = listNewPlayer[i + 1].IdPLayer;
                        listNewPlayer[i].Round = round;
                        listNewPlayer[i + 1].Round = round;
                        _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToupdate);

                        continue;
                    }
                    else
                    {
                        listNewPlayer[i].Round = round;
                        listNewPlayer[i + 1].Round = round;
                    }

                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                            Tournament.Id,
                            listNewPlayer[i].IdPLayer,
                            listNewPlayer[i + 1].IdPLayer,
                            round);
                }

                PlayersToTournamentInPlaySystem.SavePlayersToTournament();
            }
        }
        else if (round != "Eliminations")
        {
            if (round == "Cup Quarter Final" || round == "Cup Semi Final" || round == "Cup Final")
            {
                CreateDuelsKnockOutRound(round);
            }
            else
            {
                CreateDuelsEliminationsRound(round);
            }
        }
    }

    private void CreateDuelsKnockOutRound(string round)
    {
        // This method creates duels for the knockout rounds of the tournament based on the current round and the players who have qualified for that round.
        // It first retrieves the list of players who have qualified for the knockout round and orders them by their position.
        // It then creates new duels for the players in pairs, assigning them to the appropriate round and updating their positions and knockout status.
        // The method handles the "Cup Quarter Final", "Cup Semi Final", and "Cup Final" rounds, creating the necessary duels for each round based on the number of qualified players.
        // It also updates the player information in the tournament to reflect their progress in the knockout rounds.
        // Finally, it saves the updated player information to the tournament.

        if (round == "Cup Quarter Final")
        {
            var listPlayers = PlayersToTournamentInPlaySystem.GetPlayersToTournament();
            var listPlayersQualified = listPlayers.Where(p => p.Position <= 8).OrderBy(p => p.Position).ToList();
            if (listPlayersQualified.Count == 8)
            {
                _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                    Tournament.Id,
                    listPlayersQualified[0].IdPLayer,
                    listPlayersQualified[7].IdPLayer,
                    round);

                listPlayersQualified[0].Round = round;
                listPlayersQualified[0].TwoKO = "1";
                listPlayersQualified[0].CupPosition = 4;
                listPlayersQualified[7].Round = round;
                listPlayersQualified[7].TwoKO = "1";
                listPlayersQualified[7].CupPosition = 4;

                _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                    Tournament.Id,
                    listPlayersQualified[3].IdPLayer,
                    listPlayersQualified[4].IdPLayer,
                    round);

                listPlayersQualified[3].Round = round;
                listPlayersQualified[3].TwoKO = "2";
                listPlayersQualified[3].CupPosition = 4;
                listPlayersQualified[4].Round = round;
                listPlayersQualified[4].TwoKO = "2";
                listPlayersQualified[4].CupPosition = 4;

                _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                    Tournament.Id,
                    listPlayersQualified[1].IdPLayer,
                    listPlayersQualified[6].IdPLayer,
                    round);

                listPlayersQualified[1].Round = round;
                listPlayersQualified[1].TwoKO = "3";
                listPlayersQualified[1].CupPosition = 4;
                listPlayersQualified[6].Round = round;
                listPlayersQualified[6].TwoKO = "3";
                listPlayersQualified[6].CupPosition = 4;

                _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                    Tournament.Id,
                    listPlayersQualified[2].IdPLayer,
                    listPlayersQualified[5].IdPLayer,
                    round);

                listPlayersQualified[2].Round = round;
                listPlayersQualified[2].TwoKO = "4";
                listPlayersQualified[2].CupPosition = 4;
                listPlayersQualified[5].Round = round;
                listPlayersQualified[5].TwoKO = "4";
                listPlayersQualified[5].CupPosition = 4;
            }
        }
        else if (round == "Cup Semi Final")
        {
            var listPlayers = PlayersToTournamentInPlaySystem.GetPlayersToTournament();
            var allDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                .Where(d => d.Round == "Cup Quarter Final");

            foreach (var duel in allDuels)
            {
                if (duel.ScoreFirstPlayer == duel.RaceTo)
                {
                    var player = listPlayers.First(p => p.IdPLayer == duel.IdFirstPlayer);
                    player.Round = round;
                }
                else if (duel.ScoreSecondPlayer == duel.RaceTo)
                {
                    var player = listPlayers.First(p => p.IdPLayer == duel.IdSecondPlayer);

                    player.Round = round;
                }
            }

            var listPlayersQualified = listPlayers.Where(p => p.Round == round).OrderByDescending(p => p.TwoKO).ToList();

            if (listPlayersQualified.Count == 4)
            {
                _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                    Tournament.Id,
                    listPlayersQualified[0].IdPLayer,
                    listPlayersQualified[1].IdPLayer,
                    round);

                listPlayersQualified[0].Round = round;
                listPlayersQualified[0].TwoKO = "1";
                listPlayersQualified[0].CupPosition = 3;
                listPlayersQualified[1].Round = round;
                listPlayersQualified[1].TwoKO = "1";
                listPlayersQualified[1].CupPosition = 3;

                _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                    Tournament.Id,
                    listPlayersQualified[2].IdPLayer,
                    listPlayersQualified[3].IdPLayer,
                    round);
                listPlayersQualified[2].Round = round;
                listPlayersQualified[2].TwoKO = "2";
                listPlayersQualified[2].CupPosition = 3;
                listPlayersQualified[3].Round = round;
                listPlayersQualified[3].TwoKO = "2";
                listPlayersQualified[3].CupPosition = 3;
            }
        }
        else if (round == "Cup Final")
        {
            var listPlayers = PlayersToTournamentInPlaySystem.GetPlayersToTournament();
            var allDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                .Where(d => d.Round == "Cup Semi Final");
            foreach (var duel in allDuels)
            {
                if (duel.ScoreFirstPlayer == duel.RaceTo)
                {
                    var player = listPlayers.First(p => p.IdPLayer == duel.IdFirstPlayer);
                    player.Round = round;
                }
                else if (duel.ScoreSecondPlayer == duel.RaceTo)
                {
                    var player = listPlayers.First(p => p.IdPLayer == duel.IdSecondPlayer);

                    player.Round = round;
                }
            }

            var listPlayersQualified = listPlayers.Where(p => p.Round == round).OrderBy(p => p.Position).ToList();
            if (listPlayersQualified.Count == 2)
            {
                _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                    Tournament.Id,
                    listPlayersQualified[0].IdPLayer,
                    listPlayersQualified[1].IdPLayer,
                    round);
            }
        }
        PlayersToTournamentInPlaySystem.SavePlayersToTournament();
    }

    private void CreateDuelsEliminationsRound(string round)
    {
        // This method creates duels for the eliminations round of the tournament based on the current round and the players who have not yet lost.
        // It first retrieves all the duels in the tournament and determines the last round played.
        // It then retrieves the statistics for all players and creates lists of players who have not lost, players who have lost once, and players who have lost twice.
        // It then creates new duels for the players who have not lost, pairing them up in groups of two.
        // If there is an odd number of players who have not lost, the last player will receive a free win and automatically advance to the next round.
        // It then creates new duels for the players who have lost once, pairing them up in groups of two.
        // If there is an odd number of players who have lost once, the last player will receive a free win and automatically advance to the next round.
        // Finally, it saves the updated player information to the tournament.
        // This method assumes that the tournament is using a two-knockout system, where players are eliminated after losing twice.
        // It also assumes that the tournament is using a round-robin format, where players are grouped together and play against each other in a series of matches.

        var listDuelsAll = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).ToList();
        var lastRound = listDuelsAll.Last().Round;
        var statistics = GetStatistics();
        var listPlayers = PlayersToTournamentInPlaySystem.GetPlayersToTournament();
        var listPlayersNotLostByGroup = statistics.Where(s => s.Item2.First(st => st.Item1 == "Lost").Item2 == 0).Select(s => s.Item1).OrderBy(p => p.Group).ToList();
        var listPlayersOneLostByGroup = statistics.Where(s => s.Item2.First(st => st.Item1 == "Lost").Item2 == 1).Select(s => s.Item1).OrderBy(p => p.Group).ToList();
        var listPlayersLost = statistics.Where(s => s.Item2.First(st => st.Item1 == "Lost").Item2 == 2).Select(s => s.Item1).ToList();

        foreach (var duel in listDuelsAll.Where(d => d.Round == lastRound))
        {
            if (duel.ScoreFirstPlayer == duel.RaceTo)
            {
                var player = listPlayers.First(p => p.IdPLayer == duel.IdFirstPlayer);
                player.Round = round;
            }
            else if (duel.ScoreSecondPlayer == duel.RaceTo)
            {
                var player = listPlayers.First(p => p.IdPLayer == duel.IdSecondPlayer);

                player.Round = round;
            }
        }
        PlayersToTournamentInPlaySystem.SavePlayersToTournament();
        if (listPlayersNotLostByGroup.Count > 1)
        {
            if (listPlayersNotLostByGroup.Count % 2 == 0)
            {
                for (int i = 0; i < listPlayersNotLostByGroup.Count; i += 2)
                {
                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                            Tournament.Id,
                            listPlayersNotLostByGroup[i].IdPLayer,
                            listPlayersNotLostByGroup[i + 1].IdPLayer,
                            round);
                    if (listPlayersNotLostByGroup[i + 1].Equals(listPlayersNotLostByGroup.Last()))
                    {
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < listPlayersNotLostByGroup.Count; i += 2)
                {
                    if (listPlayersNotLostByGroup[i].Equals(listPlayersNotLostByGroup.Last()))
                    {
                        var newDuel = _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                          Tournament.Id,
                          listPlayersNotLostByGroup.Last().IdPLayer,
                          -1,
                          round);
                        newDuel.ScoreFirstPlayer = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                                                    .Where(d => d.Round == round).Max(d => d.RaceTo);
                        newDuel.ScoreSecondPlayer = 0;
                        newDuel.StartGame = DateTime.Now;
                        newDuel.EndGame = DateTime.Now;
                        _singlePlayerDuelManager.UpdateSinglePlayerDuel(newDuel);
                        break;
                    }
                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                            Tournament.Id,
                            listPlayersNotLostByGroup[i].IdPLayer,
                            listPlayersNotLostByGroup[i + 1].IdPLayer,
                            round);
                }
            }
        }

        if (listPlayersOneLostByGroup.Count > 1)
        {
            var listPLayersOneLostGroupedByRound = listPlayersOneLostByGroup.GroupBy(p => p.Round).OrderBy(g => g.Key).ToList();

            foreach (var keyRound in listPLayersOneLostGroupedByRound)
            {
                var players = keyRound.OrderBy(p => p.Group).ToList();
                if (players.Count % 2 == 0)
                {
                    for (int i = 0; i < players.Count; i += 2)
                    {
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                                Tournament.Id,
                                players[i].IdPLayer,
                                players[i + 1].IdPLayer,
                                round);
                        if (players[i + 1].Equals(players.Last()))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < players.Count; i += 2)
                    {
                        if (players[i].Equals(players.Last()))
                        {
                            var newDuel = _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                              Tournament.Id,
                              players.Last().IdPLayer,
                              -1,
                              round);
                            newDuel.ScoreFirstPlayer = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                                                        .Where(d => d.Round == round).Max(d => d.RaceTo);
                            newDuel.ScoreSecondPlayer = 0;
                            newDuel.StartGame = DateTime.Now;
                            newDuel.EndGame = DateTime.Now;
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(newDuel);
                            break;
                        }
                        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
                                Tournament.Id,
                                players[i].IdPLayer,
                                players[i + 1].IdPLayer,
                                round);
                    }
                }
            }
        }
    }

    public override string ViewTournamentBracket()
    {
        // This method generates a text representation of the tournament bracket for the 2KO system.
        // It retrieves all the duels in the tournament and groups them by round. It then formats the text to display the players and their scores in a bracket format.
        // If there are no duels yet, it displays a start list of players. If there are duels, it displays the tournament bracket with the players and their scores.
        // The method returns the formatted text representation of the tournament bracket.

        var formatText = string.Empty;
        string lineOne = string.Empty;
        string lineTwo = string.Empty;
        int numberItemOfLine = 6;
        int item = 0;
        var listAllDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id).Where(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer != -1 || d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1).ToList();
        var duelsGroupedByRound = listAllDuels.Where(m => m.Round.Contains("Eliminations")).GroupBy(d => d.Round).OrderByDescending(g => g.Key);

        if (PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count > 0 && !listAllDuels.Any())
        {
            formatText += $"\n\rStart List 2KO System\n\r\n\r";
            foreach (var player in PlayersToTournamentInPlaySystem.ListPlayersToTournament.OrderBy(p => p.Group))
            {
                if (player.Position % 2 != 0)
                {
                    lineOne += $" {player.Group}{player.GroupPosition}. {player.TinyFulName}".Remove(20);
                }
                else
                {
                    lineTwo += $" {player.Group}{player.GroupPosition}. {player.TinyFulName}".Remove(20);
                }

                item++;

                if (item == numberItemOfLine * 2 ||
                    player.Position == PlayersToTournamentInPlaySystem.ListPlayersToTournament.Count)
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
        else
        {
            formatText += $"\n\rTournament Bracket 2KO System\n\r\n\r";
            var listPlayerToTournamentSortedByPosition = PlayersToTournamentInPlaySystem.ListPlayersToTournament
           .OrderBy(p => p.Position)
           .ToList();
            var formatTextUp = string.Empty;
            var formatTextDown = string.Empty;

            if (listPlayerToTournamentSortedByPosition.Any(p => p.Round == "Cup Quarter Final"))
            {
                var groupingByRound = listPlayerToTournamentSortedByPosition
                    .GroupBy(p => p.Round)
                    .ToList();
                var allDuelsInCup = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                    .Where(d => d.Round == "Cup Quarter Final" || d.Round == "Cup Semi Final" || d.Round == "Cup Final").ToList();

                if (listPlayerToTournamentSortedByPosition.Any(d => d.Round == "Winner"))
                {
                    formatText += "\n\r";
                    formatText += $"{$"Winner: {listPlayerToTournamentSortedByPosition
                        .FirstOrDefault(p => p.Round == "Winner").FirstName} {listPlayerToTournamentSortedByPosition
                        .FirstOrDefault(p => p.Round == "Winner").LastName}\n\r",68}";
                    formatText += $"{"---------------",67}\n\r\n\r";
                }
                if (allDuelsInCup.Any(d => d.Round == "Cup Final"))
                {
                    formatText += $"{"Cup Final",63}\n\r";
                    foreach (var duel in allDuelsInCup.Where(d => d.Round == "Cup Final"))
                    {
                        var firstPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdFirstPlayer);
                        var secondPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdSecondPlayer);
                        formatText += $"{$""}" + "\n\r";
                        formatText += $"{$" {firstPlayer.FirstName} {firstPlayer.LastName}: {duel.ScoreFirstPlayer}",67}\n\r";
                        formatText += $"{$" {secondPlayer.FirstName} {secondPlayer.LastName}: {duel.ScoreSecondPlayer}",67}";
                        formatText += "\n\r\n\r";
                    }
                }
                if (allDuelsInCup.Any(d => d.Round == "Cup Semi Final"))
                {
                    formatText += $"{"Cup Semi Final",66}\n\r\n\r";
                    foreach (var duel in allDuelsInCup.Where(d => d.Round == "Cup Semi Final"))
                    {
                        var firstPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdFirstPlayer);
                        var secondPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdSecondPlayer);

                        formatTextUp += $"{$"{firstPlayer.FirstName} {firstPlayer.LastName}: {duel.ScoreFirstPlayer}",25}";

                        formatTextDown += $"{$"{secondPlayer.FirstName} {secondPlayer.LastName}: {duel.ScoreSecondPlayer}",25}";
                    }
                    formatText += $"{formatTextUp,81}\n\r" + $"{formatTextDown,81}";
                    formatText += "\n\r\n\r";
                }
                if (allDuelsInCup.Any(d => d.Round == "Cup Quarter Final"))
                {
                    formatTextDown = string.Empty;
                    formatTextUp = string.Empty;
                    formatText += $"{"Cup Quarter Final",68}\n\r\n\r";
                    foreach (var duel in allDuelsInCup.Where(d => d.Round == "Cup Quarter Final"))
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
            numberItemOfLine = 5;

            foreach (var key in duelsGroupedByRound)
            {
                formatText += $"Round: {key.First().Round}\n\r";
                foreach (var duel in key.OrderBy(d => d.Round).ThenBy(d => d.NumberDuelOfTournament))
                {
                    var firstPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdFirstPlayer);
                    var secondPlayer = PlayersToTournamentInPlaySystem.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == duel.IdSecondPlayer);
                    if (duel.IdSecondPlayer == -1)
                    {
                        secondPlayer = new PlayerToTournament() { FirstName = "Free", LastName = "Free Win", Group = " " };
                    }
                    lineOne += $"{$"{firstPlayer?.TinyFulName.Trim()}: {duel.ScoreFirstPlayer} |{firstPlayer?.Group}{firstPlayer?.GroupPosition}.",21}";
                    lineTwo += $"{$"{secondPlayer?.TinyFulName.Trim()}: {duel.ScoreSecondPlayer} |{secondPlayer?.Group}{secondPlayer?.GroupPosition}.",21}";
                    item++;
                    if (item == numberItemOfLine ||
                        duel.Equals(key.OrderBy(d => d.Round).ThenBy(d => d.NumberDuelOfTournament).Last()))
                    {
                        formatText += lineOne + "\n\r" + lineTwo + "\n\r\n\r";
                        lineOne = string.Empty;
                        lineTwo = string.Empty;
                        item = 0;
                    }
                }
            }
        }

        return formatText;
    }

    protected override void ExecuteExtendedAction(MenuAction menuAction)
    {
        // This method executes an extended action based on the selected menu option.
        // It retrieves the ID of the selected menu action and performs the corresponding action.
        // In this case, if the selected action is "View Statistics" (ID 0), it displays the tournament statistics.
        // If the selected action is not valid, it displays an error message.
        // The method uses the ConsoleService to display messages and get user input.

        var swichOption = menuAction.Id;
        switch (swichOption)
        {
            case 0:
                ConsoleService.WriteTitle("Statistics Of Eliminations");
                ConsoleService.WriteLineMessage(GetStatisticsOfText());
                ConsoleService.GetKeyFromUser("Press any key to continue...");
                break;

            default:
                ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                break;
        }
    }

    protected override List<MenuAction> GetExtendedMenuAction()
    {
        // This method returns a list of extended menu actions for the 2KO tournament system.
        // It creates a new list of MenuAction objects and adds a single action for viewing statistics.

        List<MenuAction> actions = new List<MenuAction>();

        actions.Add(new MenuAction(0, "View Statistics", "TwoKOPlaySystem"));

        return actions;
    }

    protected override void RemovePlayers(PlayerToTournament playerToRemove)
    {
        // This method removes a player from the tournament and updates the player list and positions accordingly.
        // It first checks if the player to remove is not null, and if so, it removes the player from the tournament duels using the _singlePlayerDuelManager.
        if (playerToRemove != null)
        {
            _singlePlayerDuelManager.RemoveTournamentDuel(Tournament, playerToRemove.IdPLayer);
        }
        var playerList = PlayersToTournamentInPlaySystem.ListPlayersToTournament;
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].Position = i + 1;
        }
        AssignPlayersToGroupAndPosition();
    }

    private List<Tuple<PlayerToTournament, List<Tuple<string, int>>>> GetStatistics()
    {
        // This method calculates and returns the statistics for all players in the tournament.
        // It retrieves the duels in the eliminations round and calculates the number of wins, losses, points won, points lost, and the difference between points won and lost for each player.
        // It then creates a list of tuples containing the player and their corresponding statistics.
        // The statistics are ordered by the number of wins and the difference between points won and lost.
        // The method returns the ordered list of statistics for all players in the tournament.

        var statisticsList = new List<Tuple<PlayerToTournament, List<Tuple<string, int>>>>();

        Tuple<PlayerToTournament, List<Tuple<string, int>>> duelsStatistic;
        var duelsOfEliminationsRound = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .Where(d => d.Round.Contains("Eliminations") == true);
        int winMatch, lostMatch, pointWin, pointLost, diffWinLost;
        var allMatchesStatistics = new List<Tuple<string, int>>();
        var players = PlayersToTournamentInPlaySystem.ListPlayersToTournament;

        foreach (var player in players)
        {
            winMatch = 0;
            pointWin = 0;
            pointLost = 0;
            lostMatch = 0;

            var duelsOfPlayer = duelsOfEliminationsRound
                .Where(d => (d.IdFirstPlayer == player.IdPLayer || d.IdSecondPlayer == player.IdPLayer) && d.EndGame != DateTime.MinValue);

            foreach (var duel in duelsOfPlayer)
            {
                if (duel.IdFirstPlayer == player.IdPLayer)
                {
                    pointWin += duel.ScoreFirstPlayer;
                    pointLost += duel.ScoreSecondPlayer;
                    if (duel.ScoreFirstPlayer == duel.RaceTo)
                    {
                        winMatch++;
                    }
                    else
                    {
                        lostMatch++;
                    }
                }
                else
                {
                    pointWin += duel.ScoreSecondPlayer;
                    pointLost += duel.ScoreFirstPlayer;
                    if (duel.ScoreSecondPlayer == duel.RaceTo)
                    {
                        winMatch++;
                    }
                    else
                    {
                        lostMatch++;
                    }
                }
            }
            allMatchesStatistics.Add(new Tuple<string, int>("Played", duelsOfPlayer.Count()));
            allMatchesStatistics.Add(new Tuple<string, int>("Win", winMatch));
            allMatchesStatistics.Add(new Tuple<string, int>("Lost", lostMatch));
            allMatchesStatistics.Add(new Tuple<string, int>("Pt.Win", pointWin));
            allMatchesStatistics.Add(new Tuple<string, int>("Pt.Lost", pointLost));
            allMatchesStatistics.Add(new Tuple<string, int>("Diff.Win-Lost", pointWin - pointLost));
            statisticsList.Add(new Tuple<PlayerToTournament, List<Tuple<string, int>>>(player, allMatchesStatistics));
            allMatchesStatistics = new List<Tuple<string, int>>();
            PlayersToTournamentInPlaySystem.SavePlayersToTournament();
        }

        var orderedStatistics = statisticsList.OrderByDescending(p => p.Item2.First(i => i.Item1 == "Win")).ThenByDescending(p => p.Item2.First(s => s.Item1 == "Diff.Win-Lost").Item2).ToList();
        foreach (var stat in orderedStatistics)
        {
            stat.Item1.Position = orderedStatistics.IndexOf(stat) + 1;
        }

        return orderedStatistics;
    }

    public override string GetStatisticsOfText()
    {
        // This method generates a text representation of the tournament statistics for the 2KO system.
        // It retrieves the statistics for all players and formats the text to display the players'
        // positions, names, and various statistics such as matches played, wins, losses, points won, points lost, and the difference between points won and lost.
        // The method returns the formatted text representation of the tournament statistics.
        // It uses the GetStatistics method to retrieve the statistics and then formats the text accordingly.

        var statistics = GetStatistics().OrderBy(p => p.Item1.Position);
        var statistickInText = string.Empty;
        var tableHead = statistics.First().Item2.Aggregate(string.Empty, (current, head) => current + $"{head.Item1} ");
        statistickInText = $"{$"{tableHead}",74}\n\r\n\r" + statistics.Aggregate(string.Empty, (current, stat) => current
        + $"{$"{stat.Item1.Position}.",-3} {stat.Item1.TinyFulName}{stat.Item2.First(s => s.Item1 == "Played").Item2,-5}" +
        $"{stat.Item2.First(s => s.Item1 == "Win").Item2,-5}{stat.Item2.First(s => s.Item1 == "Lost").Item2,-5}{stat.Item2.First(s => s.Item1 == "Pt.Win").Item2,-8}" +
        $"{stat.Item2.First(s => s.Item1 == "Pt.Lost").Item2,-5}" +
        $"{stat.Item2.First(s => s.Item1 == "Pt.Win").Item2 - stat.Item2.First(s => s.Item1 == "Pt.Lost").Item2,6}\n\r");
        return statistickInText;
    }

    protected override void EndTournament()
    {
        // This method ends the tournament by calling the EndTournament method of the tournaments manager.
        _tournamentsManager.EndTournament(Tournament);
    }

    protected override void StartNextRound()
    {
        // This method starts the next round of the tournament based on the last completed round.
        // It retrieves the last completed round from the list of duels in the tournament and determines the next round to be played.
        // It then creates the matchups for the next round and starts the duels for that round.
        // If the last completed round is "Cup Final", it ends the tournament and displays the winner.

        var lastRound = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
            .LastOrDefault(d => d.EndGame != DateTime.MinValue).Round;
        if (lastRound != null && lastRound == "Eliminations")
        {
            ConsoleService.WriteTitle("Creating matchups for the round (Eliminations Round II)");
            ConsoleService.GetKeyFromUser("Press Any Key..");
            CreateDuelsToTournament("Eliminations Round II");
            StartDuelsInRoundOfTableNumber("Eliminations Round II");
        }
        else if (lastRound != null && lastRound == "Eliminations Round II")
        {
            ConsoleService.WriteTitle("Creating matchups for the round (Eliminations Round III)");
            ConsoleService.GetKeyFromUser("Press Any Key..");
            CreateDuelsToTournament("Eliminations Round III");
            StartDuelsInRoundOfTableNumber("Eliminations Round III");
        }
        else if (lastRound == "Eliminations Round III")
        {
            ConsoleService.WriteTitle("Creating matchups for the round (Eliminations Round IV)");
            ConsoleService.GetKeyFromUser("Press Any Key..");
            CreateDuelsToTournament("Eliminations Round IV");
            StartDuelsInRoundOfTableNumber("Eliminations Round IV");
        }
        else if (lastRound == "Eliminations Round IV")
        {
            ConsoleService.WriteTitle($"End of Eliminations Tournament {Tournament.Name}");
            ConsoleService.WriteLineMessage("Creating matchups for the round (Cup Quarter Final)");
            ConsoleService.WriteLineMessage("The top 8 players qualify for the knockout stage.");
            ConsoleService.WriteLineMessage("Creating matchups for the round (Cup Quarter Final)");
            ConsoleService.GetKeyFromUser("Press Any Key..");
            var listPlayersNoQualified = PlayersToTournamentInPlaySystem.ListPlayersToTournament.Where(p => p.Position > 8).ToList();
            foreach (var player in listPlayersNoQualified)
            {
                player.CupPosition = 6; // or some other value to indicate they didn't qualify
            }

            CreateDuelsToTournament("Cup Quarter Final");
            StartDuelsInRoundOfTableNumber("Cup Quarter Final");
        }
        else if (lastRound == "Cup Quarter Final")
        {
            ConsoleService.WriteTitle($"End of Cup Quarter Final Tournament {Tournament.Name}");
            ConsoleService.WriteLineMessage("Creating matchups for the round (Cup Semi Final)");

            ConsoleService.GetKeyFromUser("Press Any Key..");
            CreateDuelsToTournament("Cup Semi Final");
            StartDuelsInRoundOfTableNumber("Cup Semi Final");
        }
        else if (lastRound == "Cup Semi Final")
        {
            ConsoleService.WriteTitle($"End of Cup Semi Final Tournament {Tournament.Name}");
            ConsoleService.WriteLineMessage("Creating matchups for the round (Cup Final)");
            ConsoleService.GetKeyFromUser("Press Any Key..");
            CreateDuelsToTournament("Cup Final");
            StartDuelsInRoundOfTableNumber("Cup Final");
        }
        else if (lastRound == "Cup Final")
        {
            var listPlayers = PlayersToTournamentInPlaySystem.ListPlayersToTournament;
            var duelsCupFinal = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(Tournament.Id)
                .FirstOrDefault(d => d.Round == "Cup Final");
            if (duelsCupFinal != null && duelsCupFinal.ScoreFirstPlayer == duelsCupFinal.RaceTo)
            {
                var player = listPlayers.First(p => p.IdPLayer == duelsCupFinal.IdFirstPlayer);
                player.Round = "Winner";
                player.CupPosition = 1;
                player = listPlayers.First(p => p.IdPLayer == duelsCupFinal.IdSecondPlayer);
                player.CupPosition = 2;
            }
            else if (duelsCupFinal != null && duelsCupFinal.ScoreSecondPlayer == duelsCupFinal.RaceTo)
            {
                var player = listPlayers.First(p => p.IdPLayer == duelsCupFinal.IdSecondPlayer);
                player.Round = "Winner";
                player.CupPosition = 1;
                player = listPlayers.First(p => p.IdPLayer == duelsCupFinal.IdFirstPlayer);
                player.CupPosition = 2;
            }
            PlayersToTournamentInPlaySystem.SavePlayersToTournament();
            var winners = listPlayers.First(p => p.Round == "Winner");
            ConsoleService.WriteTitle($"End of Cup Final Tournament {Tournament.Name}");
            ConsoleService.WriteLineMessageActionSuccess($"Winner: {winners.FirstName} {winners.LastName}");
            ConsoleService.WriteLineMessage("The tournament has ended.");
            ConsoleService.GetKeyFromUser("Press Any Key..");
            EndTournament();
        }
    }
}