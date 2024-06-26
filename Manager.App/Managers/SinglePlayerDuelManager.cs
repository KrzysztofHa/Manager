﻿using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers;

public class SinglePlayerDuelManager
{
    private readonly ISinglePlayerDuelService _singlePlayerDuelService = new SinglePlayerDuelService();
    private readonly PlayerManager _playerManager;
    private readonly IUserService _userService;
    private readonly IPlayerService _playerService;

    public SinglePlayerDuelManager(PlayerManager playerManager, IUserService userService, IPlayerService playerService)
    {
        _playerManager = playerManager;
        _userService = userService;
        _playerService = playerService;
    }

    public SinglePlayerDuel NewSingleDuel(int idFirstPlayer = 0, int idSecondPlayer = 0)
    {
        var duel = new SinglePlayerDuel();
        if (idFirstPlayer == 0 || idSecondPlayer == 0)
        {
            AddPlayerToDuel(duel);            
            if (duel.IdSecondPlayer == 0 || duel.IdSecondPlayer == 0)
            {
                
                return null;
            }
        }
        else
        {
            duel.IdFirstPlayer = idFirstPlayer;
            if (duel.IdFirstPlayer == 0)
            {
                return null;
            }
            duel.IdSecondPlayer = idSecondPlayer;
        }

        if (string.IsNullOrEmpty(duel.TypeNameOfGame))
        {
            AddSettingsRaceAndTypeGame(duel);
            if (string.IsNullOrEmpty(duel.TypeNameOfGame))
            {                
                return null;
            }
        }
        duel.CreatedById = _userService.GetIdActiveUser();
        return duel;
    }
    private void AddPlayerToDuel(SinglePlayerDuel duel)
    {
        ConsoleService.WriteTitle("Add Player");
        if (duel.IdFirstPlayer == 0 && ConsoleService.AnswerYesOrNo("User is the player?"))
        {
            var idUserOfPlayer = _userService.GeIdPlayerOfActiveUser();
            if (idUserOfPlayer <= 0)
            {
                var searchPlayer = _playerManager.SearchPlayer();
                if (searchPlayer == null)
                {
                    searchPlayer = _playerManager.AddNewPlayer();
                    if (searchPlayer == null)
                    {
                        return;
                    }
                }
                idUserOfPlayer = searchPlayer.Id;
                _userService.SetIdPlayerToActiveUser(idUserOfPlayer);
                duel.IdFirstPlayer = idUserOfPlayer;
            }
        }
        else
        {
            var findPlayer = _playerManager.SearchPlayer();
            if (findPlayer == null)
            {
                findPlayer = _playerManager.AddNewPlayer();
                if (findPlayer == null)
                {
                    return;
                }
            }

            duel.IdFirstPlayer = findPlayer.Id;
        }

        ConsoleService.WriteTitle("Add First Player");
        ConsoleService.WriteLineMessageActionSuccess("Succes\r\nPress Any Key");

        var SecondPlayer = _playerManager.SearchPlayer();
        if (SecondPlayer != null && duel.IdFirstPlayer == SecondPlayer.Id)
        {
            while (duel.IdFirstPlayer == SecondPlayer.Id)
            {
                ConsoleService.WriteTitle("Add Second Player\r\n Press Any Key..");
                ConsoleService.WriteLineErrorMessage("The selected player is already added.");
                ConsoleService.GetKeyFromUser();

                SecondPlayer = _playerManager.SearchPlayer();
            }
        }

        if (SecondPlayer == null)
        {
            SecondPlayer = _playerManager.AddNewPlayer();
            if (SecondPlayer == null)
            {
                return;
            }
        }
        duel.IdSecondPlayer = SecondPlayer.Id;

        ConsoleService.WriteTitle("Add Second Player");
        ConsoleService.WriteLineMessageActionSuccess("Succes\r\nPress Any Key");
    }
    private bool AddSettingsRaceAndTypeGame(SinglePlayerDuel singlePlayerDuel)
    {
        string[] settings = { "Type Of Game", "Race To" };

        foreach (string setting in settings)
        {
            if (setting == "Type Of Game")
            {
                var typeOfGame = new TypeOfGame();
                int idSelectTypeOfGame = 0;
                do
                {
                    ConsoleService.WriteTitle($"Add settings\r\n{setting}");
                    foreach (var game in typeOfGame.ListTypeOfGames)
                    {
                        var formatText = typeOfGame.ListTypeOfGames.IndexOf(game) == idSelectTypeOfGame ? $"> {game} <= Select Enter" :
                            $"  {game}";
                        ConsoleService.WriteLineMessage(formatText);
                    }
                    ConsoleKeyInfo inputKey = ConsoleService.GetKeyFromUser();
                    if (inputKey.Key == ConsoleKey.UpArrow && idSelectTypeOfGame > 0)
                    {
                        idSelectTypeOfGame--;
                    }
                    else if (inputKey.Key == ConsoleKey.DownArrow && idSelectTypeOfGame < typeOfGame.ListTypeOfGames.Count - 1)
                    {
                        idSelectTypeOfGame++;
                    }
                    else if (inputKey.Key == ConsoleKey.Enter)
                    {
                        singlePlayerDuel.TypeNameOfGame = typeOfGame.ListTypeOfGames[idSelectTypeOfGame];
                    }
                    else if (inputKey.Key == ConsoleKey.Escape)
                    {
                        return false;
                    }

                } while (singlePlayerDuel.TypeNameOfGame == null);
            }

            if (setting == "Race To")
            {
                do
                {
                    ConsoleService.WriteTitle($"Add settings\r\n{setting}");
                    ConsoleService.WriteLineMessage("Min 3  Max 20");
                    var manyframe = ConsoleService.GetIntNumberFromUser("Enter To Many frame");
                    if (manyframe == null)
                    {
                        singlePlayerDuel.RaceTo = 3;
                        return true;
                    }
                    if (manyframe >= 3 || manyframe <= 20)
                    {
                        singlePlayerDuel.RaceTo = (int)manyframe;
                    }

                } while (singlePlayerDuel.RaceTo < 3 || singlePlayerDuel.RaceTo > 20);
            }
        }
        return true;
    }
    public void NewTournamentSinglePlayerDue(SinglePlayerDuel duel, int idTournament, int idFirstPlayer, int idSecondPlayer)
    {
        duel.IdPlayerTournament = idTournament;
        NewSingleDuel(idFirstPlayer, idSecondPlayer);
    }
    public void StartSingleDuel(SinglePlayerDuel duel)
    {
        _singlePlayerDuelService.StartSinglePlayerDuel(duel);
    }

    public void ListOfSinglePlayerDuelByTournamentOrSparring(string nameTournament = "Sparring", int idTournament = 0)
    {

        var listSinglesPlayerDuels = new List<SinglePlayerDuel>();
        if (nameTournament == "Sparring")
        {
            listSinglesPlayerDuels = _singlePlayerDuelService.GetAllSinglePlayerDuel()
                .Where(s => s.IdPlayerTournament == null).ToList();
        }
        else
        {
            listSinglesPlayerDuels = _singlePlayerDuelService.GetAllSinglePlayerDuel()
             .Where(s => s.IdPlayerTournament == idTournament).ToList();
        }


        if (!listSinglesPlayerDuels.Any())
        {
            ConsoleService.WriteLineErrorMessage("List Empty");
            return;
        }

        var listPlayers = _playerService.ListOfActivePlayers();
        var tally = listPlayers.Join(listSinglesPlayerDuels,
            player => player.Id,
            duel => duel.IdFirstPlayer,
        (player, duel) => new
        {
            FirstPlayer = $"{player.FirstName} {player.LastName}",
            SecondPleyer = listPlayers.Where(p => p.Id == duel.IdSecondPlayer)
            .Select(n => new { FulNamen = n.FirstName + " " + n.LastName }).First().FulNamen,
            duel.TypeNameOfGame,
            duel.RaceTo,
            duel.CreatedDateTime,
            duel.ScoreFirstPlayer,
            duel.ScoreSecondPlayer,
            duel.IdPlayerTournament,
        }).OrderBy(d => d.CreatedDateTime);

        ConsoleService.WriteTitle("ALL SPARRING");
        foreach (var duelView in tally)
        {
            var FormatToTextDuelsView = $"\r\nType Game: {duelView.TypeNameOfGame}" +
                $" Race To: {duelView.RaceTo} Date: {duelView.CreatedDateTime,0:d} " +
                $"Id Tournament: {duelView.IdPlayerTournament}" +
                $"\r\n    {duelView.FirstPlayer} {duelView.ScoreFirstPlayer} : " +
                $"{duelView.ScoreSecondPlayer} {duelView.SecondPleyer}";

            ConsoleService.WriteLineMessage(FormatToTextDuelsView);
        }
    }
    public void EndSinglePlayerDuel(SinglePlayerDuel duel)
    {
        _singlePlayerDuelService.EndSinglePlayerDuel(duel);
    }

    public void UpdateSinglePlayerDuel(SinglePlayerDuel duel)
    {
        _singlePlayerDuelService.UpdateSinglePlayerDuel(duel);
    }
}
