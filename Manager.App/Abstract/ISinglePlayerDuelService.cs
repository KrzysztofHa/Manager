﻿using Manager.Domain.Entity;

namespace Manager.App.Abstract;

public interface ISinglePlayerDuelService
{
    void StartSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);

    void UpdateSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);

    void EndSinglePlayerDuel(SinglePlayerDuel singlePlayerDuel);

    void CreateTournamentSinglePlayerDue(SinglePlayerDuel singlePlayerDuel);

    List<SinglePlayerDuel> GetAllSinglePlayerDuel();

    List<SinglePlayerDuel> SearchSinglePlayerDuel(string searchString);

    string GetSinglePlayerDuelDetailsView(SinglePlayerDuel duel);

    void InterruptDuel(SinglePlayerDuel duel);

    void RemoveAllTournamentDuelsOrByIdPlayer(Tournament tournament, int idPlayer);
}