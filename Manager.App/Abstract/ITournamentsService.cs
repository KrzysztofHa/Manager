using Manager.Domain.Entity;

namespace Manager.App.Abstract
{
    public interface ITournamentsService : IService<Tournament>
    {
        void AddNewTournament(Tournament tournament);

        void StartTournament(Tournament tournament);

        void EndTournament(Tournament tournament);

        List<Tournament> SearchTournament(string searchString);

        string GetTournamentDetailView(Tournament tournament);

        void InterruptTournament(Tournament tournament);
    }
}