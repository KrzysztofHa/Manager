﻿using Manager.Domain.Entity;

namespace Manager.App.Abstract
{
    public interface ISinglePlayerDuelManager
    {
        SinglePlayerDuel NewSingleDuel(int idFirstPlayer = 0, int idSecondPlayer = 0);

        SinglePlayerDuel NewTournamentSinglePlayerDuel(int idTournament, int idFirstPlayer = -1, int idSecondPlayer = -1, string round = "Eliminations");

        void VievAllSparring();

        string ConvertListSinglePlayerDuelsToText(List<SinglePlayerDuel> listSinglesPlayerDuels, string title = "Duels");

        List<SinglePlayerDuel>? GetSinglePlayerDuelsByTournamentsOrSparrings(int idTournament = 0);

        void StartSingleDuel(SinglePlayerDuel duel);

        void EndSinglePlayerDuel(SinglePlayerDuel duel);

        void UpdateSinglePlayerDuel(SinglePlayerDuel duel);

        void InterruptDuel(SinglePlayerDuel duel);

        SinglePlayerDuel? SelectStartedDuelByTournament(int idTournament, string title = "Select Started Duels", string backText = " ");

        SinglePlayerDuel? SelectDuelToStartByTournament(int idTournament, string title = "Select Interrupted Duels", string backText = " ");

        public SinglePlayerDuel? SelectInterruptedDuelBySparring(string title = "Sparring", string backText = " ");

        SinglePlayerDuel? SelectDuel(List<SinglePlayerDuel> singlePlayerDuels, string title = " ", string backText = " ");

        void RemoveTournamentDuel(Tournament tournament, int idPlayer = 0);

        void SearchDuel();

        string GetViewInTextOfDetailsDuel(SinglePlayerDuel duel);
    }
}