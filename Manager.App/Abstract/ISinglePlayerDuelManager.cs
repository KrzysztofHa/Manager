using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Abstract
{
    public interface ISinglePlayerDuelManager
    {
        SinglePlayerDuel NewSingleDuel(int idFirstPlayer = 0, int idSecondPlayer = 0);
        SinglePlayerDuel NewTournamentSinglePlayerDue(SinglePlayerDuel duel, int idTournament, int idFirstPlayer = -1, int idSecondPlayer = -1);
        List<SinglePlayerDuel> ListOfSinglePlayerDuelByTournamentOrSparring(string nameTournament = "Sparring", int idTournament = 0);
        void StartSingleDuel(SinglePlayerDuel duel);
        void EndSinglePlayerDuel(SinglePlayerDuel duel);
        void UpdateSinglePlayerDuel(SinglePlayerDuel duel);
    }
}
