using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers.TournamentGamePlaySystem;

public abstract class PlaySystem
{
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;

    protected PlaySystem(ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        _singlePlayerDuelManager = singlePlayerDuelManager;
    }

    protected virtual void RandomSelectionOfPlayers(PlayersToTournament playersToTournament)
    {
        if (playersToTournament.ListPlayersToTournament.Count < 8 || playersToTournament == null)
        {
            return;
        }
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
    }

    public void StartAndInterruptedTournamentDuel(Tournament tournament, PlayersToTournament playersToTournament)
    {
        if (tournament.NumberOfTables == 0)
        {
            ChangeNumberOfTable(tournament, playersToTournament);
            return;
        }

        var duelsOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
            .Where(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer != -1).ToList();
        var startedDuels = duelsOfTournament.Where(d => d.StartGame != DateTime.MinValue && d.Interrupted == DateTime.MinValue && d.EndGame == DateTime.MinValue).ToList();
        var completedDuels = duelsOfTournament.Where(d => d.EndGame != DateTime.MinValue).ToList();

        if (duelsOfTournament.Any(d => d.StartGame != DateTime.MinValue))
        {
            if (duelsOfTournament.Count - completedDuels.Count > startedDuels.Count)
            {
                if (startedDuels.Count == tournament.NumberOfTables)
                {
                    var duelToStart = _singlePlayerDuelManager.SelectDuelToStartByTournament(tournament.Id, "Select Duel To Start");

                    if (duelToStart != null)
                    {
                        var duelToIntrrypted = _singlePlayerDuelManager.SelectStartedDuelByTournament(tournament.Id, "Select Duel To Stop");
                        if (duelToIntrrypted != null)
                        {
                            duelToStart.TableNumber = duelToIntrrypted.TableNumber;
                            duelToIntrrypted.TableNumber = 0;
                            _singlePlayerDuelManager.InterruptDuel(duelToIntrrypted);
                            _singlePlayerDuelManager.StartSingleDuel(duelToStart);
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToIntrrypted);
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToStart);
                        }
                    }
                    return;
                }

                if (startedDuels.Count < tournament.NumberOfTables)
                {
                    var duelsToStart = duelsOfTournament.Except(startedDuels).Except(completedDuels).OrderBy(d => d.StartNumberInGroup).ToList();

                    if (duelsToStart.Any())
                    {
                        foreach (var started in startedDuels)
                        {
                            duelsToStart = duelsToStart
                                .Where(
                                d => d.IdFirstPlayer != started.IdFirstPlayer
                                && d.IdFirstPlayer != started.IdSecondPlayer
                                && d.IdSecondPlayer != started.IdFirstPlayer
                                && d.IdSecondPlayer != started.IdSecondPlayer).ToList(); ;
                        }

                        List<int> freeTables = new List<int>();

                        for (int i = 0; i < tournament.NumberOfTables; i++)
                        {
                            if (startedDuels.Any(d => d.TableNumber == i + 1))
                            {
                                continue;
                            }
                            else
                            {
                                freeTables.Add(i + 1);
                            }
                        }

                        var numberDuelsTostart = freeTables.Count;

                        if (freeTables.Count > duelsToStart.Count)
                        {
                            numberDuelsTostart = duelsToStart.Count;
                        }

                        for (int i = 0; i < numberDuelsTostart; i++)
                        {
                            duelsToStart[i].TableNumber = freeTables[i];
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelsToStart[i]);
                            _singlePlayerDuelManager.StartSingleDuel(duelsToStart[i]);
                        }
                    }
                }
                else
                {
                    for (int i = startedDuels.Count - tournament.NumberOfTables; i > 0; i--)
                    {
                        var duelToIntrrypted = _singlePlayerDuelManager.SelectStartedDuelByTournament(tournament.Id, "Select Duel To Stop", $" {i} games left to stop");
                        if (duelToIntrrypted != null)
                        {
                            duelToIntrrypted.TableNumber = 0;
                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToIntrrypted);
                            _singlePlayerDuelManager.InterruptDuel(duelToIntrrypted);
                        }
                        else
                        {
                            for (int j = i - 1; j >= 0; j--)
                            {
                                var startedDuel = startedDuels[i];
                                startedDuel.TableNumber = 0;
                                _singlePlayerDuelManager.UpdateSinglePlayerDuel(startedDuel);
                                _singlePlayerDuelManager.InterruptDuel(startedDuel);
                            }
                        }
                    }
                }

                return;
            }
        }
        else
        {
            if (tournament.GamePlaySystem == "Group")
            {
                var tableNumber = 1;
                foreach (var duelToStart in duelsOfTournament.OrderBy(d => d.StartNumberInGroup))
                {
                    if (tableNumber <= tournament.NumberOfTables)
                    {
                        tableNumber++;
                    }
                    else
                    {
                        break;
                    }

                    duelToStart.TableNumber = tableNumber;
                    _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToStart);
                    _singlePlayerDuelManager.StartSingleDuel(duelToStart);
                }
            }
            else
            {
                var tableNumber = 0;
                foreach (var duel in duelsOfTournament)
                {
                    if (tableNumber <= tournament.NumberOfTables)
                    {
                        tableNumber++;
                    }
                    else
                    {
                        break;
                    }

                    duel.TableNumber = tableNumber;
                    _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
                    _singlePlayerDuelManager.StartSingleDuel(duel);
                }
            }
        }
    }

    protected void ChangeNumberOfTable(Tournament tournament, PlayersToTournament playersToTournament)
    {
        ConsoleService.WriteTitle("Chenge Number Of Table\n\r");

        string textToDisplayIfNoDuelsIsStarted = "After entering the number of Tables,\n\r" +
           " the system manages the start of the next match.\n\r" +
           "The user enters the result of the duel\n\r" +
           " and if one of the players reaches the required number of points,\n\r" +
           "the match will end on a given table and a new one will start.\n\r" +
           "After the first round of matches begins,\n\r" +
           "the user can still make changes to the tournament settings,\n\r" +
           "but they are limited depending on the matches being played.";

        if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id).Any(d => d.StartGame != DateTime.MinValue))
        {
            textToDisplayIfNoDuelsIsStarted = $"Current number of tables {tournament.NumberOfTables}";
        }

        var numberOfTable = ConsoleService.GetIntNumberFromUser("Enter Number Of Table", textToDisplayIfNoDuelsIsStarted);

        if (numberOfTable == null || numberOfTable <= 0 || tournament.NumberOfTables == numberOfTable)
        {
            ConsoleService.WriteLineErrorMessage("Number Of Tables Not Change");
            return;
        }
        else
        {
            tournament.NumberOfTables = (int)numberOfTable;
            _tournamentsService.UpdateItem(tournament);
            _tournamentsService.SaveList();
            StartAndInterruptedTournamentDuel(tournament, playersToTournament);
        }
    }

    protected abstract void CreateDuelsToTournament(Tournament tournament, PlayersToTournament playersToTournament, string round = "Eliminations");

    protected abstract string ViewGroupsOr2KO(Tournament tournament, PlayersToTournament playersToTournament);
}