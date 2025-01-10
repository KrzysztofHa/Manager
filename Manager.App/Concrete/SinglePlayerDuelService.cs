using Manager.App.Abstract;
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
        singlePlayerDuel.Resume = DateTime.MinValue;
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
        return GetAllItem().Where(s => s.IsActive == true).ToList();
    }

    public string GetSinglePlayerDuelDetailsView(SinglePlayerDuel duel)
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
            var endGame = duel.EndGame.ToShortTimeString();
            if (duel.EndGame.Equals(DateTime.MinValue) && duel.Interrupted.Equals(DateTime.MinValue) && duel.StartGame.Equals(DateTime.MinValue))
            {
                endGame = "----";
            }
            else if (!duel.Interrupted.Equals(DateTime.MinValue))
            {
                endGame = "Interrupted";
            }
            else if (!duel.StartGame.Equals(DateTime.MinValue) && duel.Interrupted.Equals(DateTime.MinValue))
            {
                endGame = "In Progress";
            }

            var firstPlayerText = $"{firstPlayer.FirstName.Remove(1)}.{firstPlayer.LastName}";
            var secondPlayerText = $"{secondPlayer.FirstName.Remove(1)}.{secondPlayer.LastName}";

            var startGame = duel.StartGame.Equals(DateTime.MinValue) ? "Waiting" : duel.StartGame.ToShortDateString();

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
        List<SinglePlayerDuel> foundSinglePlayerDuels = new List<SinglePlayerDuel>();

        foundSinglePlayerDuels = GetAllItem().Where(d => $"{d.Id} {d.RaceTo} {d.TypeNameOfGame}".ToLower()
               .Contains(searchString.ToLower()) && d.IsActive == true).ToList();

        if (!string.IsNullOrEmpty(searchString) && searchString != " ")
        {
            IPlayerService playerservice = new PlayerService();
            ITournamentsService tournamentservis = new TournamentsService();

            var foundPlayers = playerservice.SearchPlayer(searchString);
            var foundTournaments = tournamentservis.SearchTournament(searchString);
            var allduel = GetAllItem().Where(d => d.IsActive == true);

            if (foundPlayers.Any())
            {
                var foundDuelOfPlayer = foundPlayers
                    .GroupJoin(allduel,
                    player => player.Id,
                    duel => duel.IdFirstPlayer,
                    (player, duels) => (player, duels)).ToList().SelectMany(p => p.duels).ToList();

                foundDuelOfPlayer.AddRange(foundPlayers
                    .GroupJoin(allduel,
                    player => player.Id,
                    duel => duel.IdSecondPlayer,
                    (player, duels) => (player, duels)).ToList().SelectMany(p => p.duels).ToList());

                foundSinglePlayerDuels.AddRange(foundDuelOfPlayer);
            }

            if (foundTournaments.Any())
            {
                var foundDuelOfTournament = foundTournaments.GroupJoin(allduel,
                    tournament => tournament.Id,
                    duel => duel.IdPlayerTournament,
                    (tournament, duels) => (tournament, duels)).ToList().SelectMany(d => d.duels).ToList();

                foundSinglePlayerDuels.AddRange(foundDuelOfTournament);
            }
            var foundDuel = foundSinglePlayerDuels.Distinct().ToList();
            foundSinglePlayerDuels.Clear();
            foundSinglePlayerDuels = foundDuel;
        }
        return foundSinglePlayerDuels;
    }

    public void RemoveAllTournamentDuelsOrByIdPlayer(Tournament tournament, int idPlayer = 0)
    {
        var duelsToRemove = GetAllItem().Where(d => d.IdPlayerTournament == tournament.Id && d.IsActive == true).ToList();

        if (duelsToRemove.Any(d => d.IdFirstPlayer == idPlayer || d.IdSecondPlayer == idPlayer))
        {
            duelsToRemove = duelsToRemove.Where(d => d.IdFirstPlayer == idPlayer || d.IdSecondPlayer == idPlayer).ToList();
            if (duelsToRemove.Any(d => d.EndGame != DateTime.MinValue && (d.StartGame != DateTime.MinValue && d.Interrupted == DateTime.MinValue)))
            {
                return;
            }

            if (tournament.GamePlaySystem == "2KO")
            {
                if (duelsToRemove.First().IdFirstPlayer == idPlayer && duelsToRemove.First().IdSecondPlayer == -1)
                {
                    foreach (var duel in duelsToRemove)
                    {
                        RemoveItem(duel);
                    }
                }
                else
                {
                    if (duelsToRemove.First().IdFirstPlayer == idPlayer)
                    {
                        duelsToRemove.First().IdFirstPlayer = duelsToRemove.First().IdSecondPlayer;
                        duelsToRemove.First().IdSecondPlayer = -1;
                    }
                    else
                    {
                        duelsToRemove.First().IdSecondPlayer = -1;
                    }
                }
            }
            else
            {
                foreach (var duel in duelsToRemove)
                {
                    RemoveItem(duel);
                }
            }

            SaveList();
        }
        else if (idPlayer == 0)
        {
            var allTournamentDuels = GetAllItem().Where(d => d.IdPlayerTournament == tournament.Id && d.IsActive == true).OrderBy(d => d.NumberDuelOfTournament).ToList();
            if (allTournamentDuels.Count > 0)
            {
                for (int i = 0; i < allTournamentDuels.Count; ++i)
                {
                    allTournamentDuels[i].NumberDuelOfTournament = i;
                }
            }
        }
        SaveList();
    }
}