using Manager.Domain.Entity;

namespace Manager.App.Abstract;

public interface ISinglePlayerDuelService
{
    void StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);

    void UpdateSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);

    void EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);

    void CreateTournamentSinglePlayerDue(SinglePlayerDuel singlePlayerDuel);

    List<SinglePlayerDuel> GetAllSinglePlayerDuel();

    List<SinglePlayerDuel> SearchSinglePlayerDuel(string searchString);

    string GetSinglePlayerDuelDetailView(SinglePlayerDuel duel);

    void InterruptDuel(SinglePlayerDuel duel);

    void removeTournamentDuelByIdPlayer(Tournament tournament, int idPlayer);
}