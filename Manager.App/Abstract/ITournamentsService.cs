using Manager.App.Managers;
using Manager.Domain.Entity;

namespace Manager.App.Abstract
{
    public interface ITournamentsService : IService<Tournament>
    {
        void AddNewTournament(Tournament tournament);

        void StartTournament(Tournament tournament, ISinglePlayerDuelManager singlePlayerDuelManager);

        void EndTournament(Tournament tournament);

        List<Tournament> SearchTournament(string searchString);

        string GetTournamentDetailView(Tournament tournament);

        void InterruptTournament(Tournament tournament, ISinglePlayerDuelManager singlePlayerDuelManager);
    }
}