using Manager.Domain.Entity;

namespace Manager.App.Abstract
{
    public interface ISinglePlayerDuelManager
    {
        SinglePlayerDuel NewSingleDuel(int idFirstPlayer = 0, int idSecondPlayer = 0);

        SinglePlayerDuel NewTournamentSinglePlayerDuel(int idTournament, int idFirstPlayer = -1, int idSecondPlayer = -1, string round = "Eliminations");

        void VievSinglePlayerDuelsByTournamentsOrSparrings(int idTournament = 0);

        string GetListSinglePlayerDuelsInText(int idTournament = 0);

        List<SinglePlayerDuel>? GetSinglePlayerDuelsByTournamentsOrSparrings(int idTournament = 0);

        void StartSingleDuel(SinglePlayerDuel duel);

        void EndSinglePlayerDuel(SinglePlayerDuel duel);

        void UpdateSinglePlayerDuel(SinglePlayerDuel duel);

        void InterruptDuel(SinglePlayerDuel duel);

        SinglePlayerDuel? SearchInterruptedDuel(string title = "", int? idTournament = null);

        void RemoveTournamentDuel(Tournament tournament, int idDuel = 0);

        string GetViewInTextOfDetailsDuel(SinglePlayerDuel duel);
    }
}