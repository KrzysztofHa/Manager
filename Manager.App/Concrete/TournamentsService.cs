using Manager.App.Abstract;
using Manager.App.Common;
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
        throw new NotImplementedException();
    }

    public List<Tournament> SearchTournament(string searchString)
    {
        List<Tournament> findTournaments = new List<Tournament>();
        if (!string.IsNullOrEmpty(searchString))
        {
            findTournaments = GetAllItem().Where(p => $"{p.Id} {p.Name} {p.GameplaySystem}".ToLower()
            .Contains(searchString.ToLower()) && p.IsActive == true).OrderBy(i => i.Name).ToList();
        }
        return findTournaments;
    }

    public void StartTournament(Tournament tournament)
    {
        throw new NotImplementedException();
    }

    public string GetTournamentDetailView(Tournament tournament)
    {
        if (tournament != null)
        {
            IClubService clubService = new ClubService();
            var clubToView = clubService.GetAllItem().FirstOrDefault(p => p.Id == tournament.IdClub);

            var startGame = tournament.Start.Equals(DateTime.MinValue) ? "Waiting" : tournament.Start.ToString();
            var endGame = tournament.End.Equals(DateTime.MinValue) ? "----------" : tournament.End.ToString();

            var formatTournamentDataToView = $"{tournament.Id,-5}".Remove(5) + $" {tournament.Name,-20}".Remove(21) +
                $" {tournament.GameplaySystem,-15}".Remove(16) + $" {clubToView.Name,-20}".Remove(21) +
                $" {startGame,-15}".Remove(15) + $" {endGame,-15}".Remove(15) + $" {tournament.NumberOfPlayer,-5}".Remove(5);
            return $"{formatTournamentDataToView,-97}";
        }
        return string.Empty;
    }
}