using Manager.App.Abstract;
using Manager.App.Common;
using Manager.App.Managers;
using Manager.Domain.Entity;

namespace Manager.App.Concrete;

public class TournamentsService : BaseService<Tournament>, ITournamentsService
{
    public void AddNewTournament(Tournament tournament)
    {
        AddItem(tournament);
        SaveList();
    }

    public void EndTournament(Tournament tournament)
    {
        if (tournament != null)
        {
            tournament.End = DateTime.Now;
        }
    }

    public List<Tournament> SearchTournament(string searchString)
    {
        List<Tournament> findTournaments = new List<Tournament>();
        if (!string.IsNullOrEmpty(searchString))
        {
            findTournaments = GetAllItem().Where(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower()
            .Contains(searchString.ToLower()) && p.IsActive == true).OrderBy(i => i.Name).ToList();
        }
        return findTournaments;
    }

    public void StartTournament(Tournament tournament, ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        if (tournament.End == DateTime.MinValue)
        {
            if (tournament.Start == DateTime.MinValue)
            {
                tournament.Start = DateTime.Now;
            }
            else
            {
                tournament.Interrupt = DateTime.MinValue;
                tournament.Resume = DateTime.Now;
                var startedDuelsOfTournament = singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
               .Where(d => !d.StartGame.Equals(DateTime.MinValue)).ToArray();
                if (tournament.NumberOfTables > startedDuelsOfTournament.Count())
                {
                    for (var i = 0; i < tournament.NumberOfTables; i++)
                    {
                        singlePlayerDuelManager.StartSingleDuel(startedDuelsOfTournament[i]);
                    }
                }
            }
            SaveList();
        }
    }

    public string GetTournamentDetailView(Tournament tournament)
    {
        if (tournament != null)
        {
            IClubService clubService = new ClubService();
            var clubToView = clubService.GetAllItem().FirstOrDefault(p => p.Id == tournament.IdClub);

            var startGame = tournament.Start.Equals(DateTime.MinValue) ? "Waiting" : tournament.Start.ToShortDateString();
            var endGame = !tournament.Interrupt.Equals(DateTime.MinValue) && tournament.End.Equals(DateTime.MinValue) ?
            "Interrupted" : tournament.End.Equals(DateTime.MinValue) ? "In Progress" : tournament.End.ToShortTimeString();
            endGame = tournament.End.Equals(DateTime.MinValue) && startGame == "Waiting" ? "----------" : endGame;

            var formatTournamentDataToView = $"{tournament.Id,-5}".Remove(5) + $" {tournament.Name,-20}".Remove(21) +
                $" {tournament.GamePlaySystem,-15}".Remove(16) + $" {clubToView.Name,-20}".Remove(21) +
                $" {startGame,-15}".Remove(15) + $" {endGame,-15}".Remove(15) + $" {tournament.NumberOfPlayer,-5}".Remove(5);
            return $"{formatTournamentDataToView,-97}";
        }
        return string.Empty;
    }

    public void InterruptTournament(Tournament tournament, ISinglePlayerDuelManager singlePlayerDuelManager)
    {
        if (tournament == null && singlePlayerDuelManager == null)
        {
            return;
        }
        else
        {
            var startedDuelsOfTournament = singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
                .Where(d => !d.StartGame.Equals(DateTime.MinValue));
            foreach (var duel in startedDuelsOfTournament)
            {
                singlePlayerDuelManager.InterruptDuel(duel);
            }
            tournament.Interrupt = DateTime.Now;
            tournament.Resume = DateTime.MinValue;
            UpdateItem(tournament);
            SaveList();
        }
    }
}