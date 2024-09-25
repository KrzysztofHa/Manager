using Manager.Domain.Entity;

namespace Manager.App.Abstract
{
    public interface ISinglePlayerDuelManager
    {
        SinglePlayerDuel NewSingleDuel(int idFirstPlayer = 0, int idSecondPlayer = 0);

        SinglePlayerDuel NewTournamentSinglePlayerDuel(SinglePlayerDuel duel, int idTournament, int idFirstPlayer = -1, int idSecondPlayer = -1);

        void VievSinglePlayerDuelsByTournamentsOrSparrings(int idTournament = 0);

        List<SinglePlayerDuel>? GetSinglePlayerDuelsByTournamentsOrSparrings(int idTournament = 0);

        void StartSingleDuel(SinglePlayerDuel duel);

        void EndSinglePlayerDuel(SinglePlayerDuel duel);

        void UpdateSinglePlayerDuel(SinglePlayerDuel duel);

        void InterruptDuel(SinglePlayerDuel duel);

        SinglePlayerDuel? SearchInterruptedDuel(string title = "", int? idTournament = null);
    }
}