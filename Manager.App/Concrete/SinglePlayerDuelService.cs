﻿using Manager.App.Abstract;
using Manager.App.Common;
using Manager.Domain.Entity;

namespace Manager.App.Concrete;

public class SinglePlayerDuelService : BaseService<SinglePlayerDuel>, ISinglePlayerDuelService
{
    public void StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        AddItem(singlePlayerDuel);
        SaveList();
    }

    public void EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        singlePlayerDuel.EndGame = DateTime.Now;
        UpdateItem(singlePlayerDuel);
        SaveList();
    }

    public void UpdateSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel)
    {
        UpdateItem(singlePlayerDuel);
        SaveList();
    }

    public void InterruptDuel(SinglePlayerDuel singlePlayerDuel)
    {
        singlePlayerDuel.Interrupted = DateTime.Now;
        UpdateItem(singlePlayerDuel);
        SaveList();
    }

    public void CreateTournamentSinglePlayerDue(SinglePlayerDuel singlePlayerDuel)
    {
        var tournamentDuelLastNumberDuel = GetAllItem().Where(d => d.IdPlayerTournament == singlePlayerDuel.IdPlayerTournament).MaxBy(d => d.NumberDuelOfTournament);
        int lastNumberDuelsOfTournament = 0;
        if (tournamentDuelLastNumberDuel == null)
        {
            lastNumberDuelsOfTournament = 0;
        }
        else
        {
            lastNumberDuelsOfTournament += tournamentDuelLastNumberDuel.NumberDuelOfTournament + 1;
        }
        singlePlayerDuel.NumberDuelOfTournament = lastNumberDuelsOfTournament;
        AddItem(singlePlayerDuel);
        SaveList();
    }

    public List<SinglePlayerDuel> GetAllSinglePlayerDuel()
    {
        return GetAllItem();
    }

    public string GetSinglePlayerDuelDetailView(SinglePlayerDuel duel)
    {
        if (duel != null)
        {
            IPlayerService playerservice = new PlayerService();
            ITournamentsService tournamentServis = new TournamentsService();

            var firstPlayer = playerservice.GetItemById(duel.IdFirstPlayer) == null ? new Player { Id = duel.IdFirstPlayer, FirstName = "-", LastName = "Unknown" }
            : playerservice.GetItemById(duel.IdFirstPlayer);
            var secondPlayer = playerservice.GetItemById(duel.IdSecondPlayer) == null ? new Player { Id = duel.IdFirstPlayer, FirstName = "-", LastName = "Unknown" }
            : playerservice.GetItemById(duel.IdSecondPlayer);

            if (firstPlayer.LastName == "Unknown" && secondPlayer.LastName == "Unknown")
            {
                return string.Empty;
            }
            var idTournament = duel.IdPlayerTournament == null ? 0 : (int)duel.IdPlayerTournament;
            var tournament = tournamentServis.GetItemById(idTournament);

            var tournamentName = tournament != null ? tournament.Name : "Sparring";
            var endGameText = !duel.Interrupted.Equals(DateTime.MinValue) && duel.EndGame.Equals(DateTime.MinValue) ?
            "Interrupted" : duel.EndGame.Equals(DateTime.MinValue) ? "In Progress" : duel.EndGame.ToShortTimeString();

            var firstPlayerText = $"{firstPlayer.FirstName.Remove(1)}.{firstPlayer.LastName}";
            var secondPlayerText = $"{secondPlayer.FirstName.Remove(1)}.{secondPlayer.LastName}";

            var startGame = duel.StartGame.Equals(DateTime.MinValue) ? "Waiting" : duel.StartGame.ToShortDateString();
            var endGame = duel.EndGame.Equals(DateTime.MinValue) ? endGameText : duel.EndGame.ToShortDateString();

            var formatDuelDataToView = $"{duel.Id,-5}".Remove(5) + $" {tournamentName,-10}".Remove(11) +
                $" {duel.TypeNameOfGame,-10}".Remove(11) + $" {duel.RaceTo,-5}".Remove(6) +
                $" {firstPlayerText,-20}".Remove(21) + $" {secondPlayerText,-20}".Remove(21) +
                $" {startGame,-11}".Remove(12) + $" {endGame,-11}".Remove(12);

            return $"{formatDuelDataToView,-92}";
        }
        return string.Empty;
    }

    public List<SinglePlayerDuel> SearchSinglePlayerDuel(string searchString)
    {
        List<SinglePlayerDuel> findSinglePlayerDuels = new List<SinglePlayerDuel>();

        findSinglePlayerDuels = GetAllItem().Where(d => $"{d.Id} {d.RaceTo} {d.TypeNameOfGame}".ToLower()
               .Contains(searchString.ToLower()) && d.IsActive == true).ToList();

        if (!string.IsNullOrEmpty(searchString) && searchString != " ")
        {
            IPlayerService playerservice = new PlayerService();
            ITournamentsService tournamentservis = new TournamentsService();

            var findPlayers = playerservice.SearchPlayer(searchString);
            var findTournaments = tournamentservis.SearchTournament(searchString);
            var allduel = GetAllItem().Where(d => d.IsActive == true);

            if (findPlayers.Any())
            {
                var findDuelOfPlayer = findPlayers
                    .GroupJoin(allduel,
                    player => player.Id,
                    duel => duel.IdFirstPlayer,
                    (player, duels) => (player, duels)).ToList().SelectMany(p => p.duels).ToList();

                findDuelOfPlayer.AddRange(findPlayers
                    .GroupJoin(allduel,
                    player => player.Id,
                    duel => duel.IdSecondPlayer,
                    (player, duels) => (player, duels)).ToList().SelectMany(p => p.duels).ToList());

                findSinglePlayerDuels.AddRange(findDuelOfPlayer);
            }

            if (findTournaments.Any())
            {
                var findDuelOfTournament = findTournaments.GroupJoin(allduel,
                    tournament => tournament.Id,
                    duel => duel.IdPlayerTournament,
                    (tournament, duels) => (tournament, duels)).ToList().SelectMany(d => d.duels).ToList();

                findSinglePlayerDuels.AddRange(findDuelOfTournament);
            }
            var findDuel = findSinglePlayerDuels.Distinct().ToList();
            findSinglePlayerDuels.Clear();
            findSinglePlayerDuels = findDuel;
        }
        return findSinglePlayerDuels;
    }

    public void removeTournamentDuelByIdPlayer(Tournament tournament, int idPlayer)
    {
        var duelsToRemove = GetAllItem().Where(d => d.IdPlayerTournament == tournament.Id && (d.IdSecondPlayer == idPlayer || d.IdFirstPlayer == idPlayer)).ToList();
        if (duelsToRemove.Count > 0)
        {
            foreach (var duel in duelsToRemove)
            {
                GetAllItem().Remove(duel);
            }

            var allTournamentDuels = GetAllItem().Where(d => d.IdPlayerTournament == tournament.Id).ToList();
            if (allTournamentDuels.Count > 0)
            {
                for (int i = 0; i < allTournamentDuels.Count; ++i)
                {
                    allTournamentDuels[i].NumberDuelOfTournament = i;
                }
            }
            SaveList();
        }
    }
}