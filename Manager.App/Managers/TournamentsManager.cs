﻿using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.ComponentModel.Design;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Xunit.Sdk;

namespace Manager.App.Managers;

public class TournamentsManager : ITournamentsManager
{
    private readonly MenuActionService _actionService;
    private readonly IPlayerManager _playerManager;
    private readonly ITournamentsService _tournamentsService = new TournamentsService();
    private readonly IPlayerService _playerService;
    private readonly ISinglePlayerDuelManager _singlePlayerDuelManager;

    public TournamentsManager(MenuActionService actionService, IPlayerManager playerManager, IPlayerService playerService)
    {
        _actionService = actionService;
        _playerManager = playerManager;
        _playerService = playerService;
        _singlePlayerDuelManager = new SinglePlayerDuelManager(_playerManager, _playerService);
    }

    public void TournamentOptionsView()
    {
        var optionPlayerMenu = _actionService.GetMenuActionsByName("Tournaments");
        while (true)
        {
            ConsoleService.WriteTitle("Tournaments");
            for (int i = 0; i < optionPlayerMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    CreateNewTournament();
                    break;

                case 2:
                    var tournament = SearchTournament();
                    if (tournament != null)
                    {
                        ITournamentsManager tournamentsManager = this;
                        TournamentGamePlayManager tournamentGamePlayManager = new TournamentGamePlayManager(tournament, tournamentsManager, _actionService, _playerManager, _playerService, _singlePlayerDuelManager);
                        tournamentGamePlayManager.GoToTournament();
                    }
                    break;

                case 3:
                    AllTournamentsView();
                    break;

                case 4:
                    DeleteTournament();
                    break;

                case 5:
                    operation = null;
                    break;

                default:
                    if (operation != null)
                    {
                        ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                    }
                    break;
            }

            if (operation == null)
            {
                break;
            }
        }
    }

    //public void GoToTournament(Tournament tournament)
    //{
    //    if (tournament == null)
    //    {
    //        return;
    //    }
    //    PlayersToTournament playersToTournament = new(tournament, _tournamentsService);
    //    TournamentGamePlayManager tournamentGamePlayManager = new TournamentGamePlayManager(tournament);

    //    var firstDuel = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id).FirstOrDefault();
    //    var optionPlayerMenuList = _actionService.GetMenuActionsByName("Go To Tournament");
    //    int skipOption = 0;
    //    if (firstDuel != null)
    //    {
    //        skipOption = _actionService.GetMenuActionsByName("Start Tournament").Count;
    //        optionPlayerMenuList.InsertRange(0, _actionService.GetMenuActionsByName("Start Tournament"));
    //    }

    //    while (true)
    //    {
    //        string mesageToDo = "\n\rTo Do List:";
    //        ConsoleService.WriteTitle($"Tournaments {tournament.Name} | Game System: {tournament.GamePlaySystem} ");
    //        ConsoleService.WriteLineMessage($"Number of PLayers: {tournament.NumberOfPlayer} | Number Of Groups: {tournament.NumberOfGroups} | " +
    //            $"Type Name Of Game: {firstDuel.TypeNameOfGame} | " +
    //            $"Group Race To: {firstDuel.RaceTo}\n\r");

    //        var mesage = string.Empty;
    //        if (tournament.NumberOfPlayer < 8)
    //        {
    //            ConsoleService.WriteLineMessage("-> Minimum 8 players to start the tournament\n\r");
    //        }

    //        for (int i = 0 + skipOption; i < optionPlayerMenuList.Count; i++)
    //        {
    //            if (optionPlayerMenuList[i].Name == "Add Players" && tournament.NumberOfPlayer < 8)
    //            {
    //                mesage = " <---------- Add More Players";
    //                mesageToDo += "\n\r    Add More Players";
    //            }
    //            else if (tournament.NumberOfGroups == 0 && tournament.GamePlaySystem == "Group" && optionPlayerMenuList[i].Name == "Set Number Of Groups")
    //            {
    //                mesage = "  <--- Set";
    //                mesageToDo += $"\n\r    Set Number Of Groups";
    //            }
    //            else if (optionPlayerMenuList[i].Name == "Start Tournament")
    //            {
    //                mesage = " <----- Start Tournament";
    //            }
    //            else if (optionPlayerMenuList[i].Name == "Chenge The Game System" && firstDuel != null)
    //            {
    //                continue;
    //            }

    //            ConsoleService.WriteLineMessage($"{i + 1 - skipOption}. {optionPlayerMenuList[i].Name} {mesage}");
    //            mesage = string.Empty;
    //        }

    //        if (tournament.NumberOfPlayer > 8 && tournament.NumberOfGroups != 0 || tournament.GamePlaySystem == "2KO")
    //        {
    //            mesageToDo = ViewGroupsOr2KO(tournament, playersToTournament);
    //        }

    //        var operation = ConsoleService.GetIntNumberFromUser("Enter Option", mesageToDo);
    //        if (operation <= 0)
    //        {
    //            operation = 500;
    //        }

    //        switch (operation)
    //        {
    //            case 1:
    //                //"Start Duel / Interrupt Duel"
    //                break;

    //            case 2:
    //                //Update Duel Result
    //                break;

    //            case 3:
    //                //Group/2KO View
    //                break;

    //            case 4:
    //                //All Duels
    //                break;

    //            case 5:
    //                ChangeRaceTo(tournament);
    //                break;

    //            case 6:
    //                AddPlayersToTournament(tournament, playersToTournament);
    //                break;

    //            case 7:
    //                RemovePlayerOfTournament(tournament, playersToTournament);
    //                break;

    //            case 8:
    //                EditGroupsOr2KOList(tournament, playersToTournament);
    //                MovePlayer(tournament, playersToTournament);
    //                break;

    //            case 9:
    //                RandomSelectionOfPlayers(tournament, playersToTournament);
    //                break;

    //            case 10:
    //                ViewListPlayersToTournament(tournament, playersToTournament);
    //                ConsoleService.GetKeyFromUser();
    //                break;

    //            case 11:
    //                if (tournament.GamePlaySystem != "TwoKO")
    //                {
    //                    SetGroups(tournament, playersToTournament);
    //                }
    //                break;

    //            case 12:
    //                var gamePlaySystem = tournamentGamePlayManager.GetGamePlaySystemFromUser();
    //                tournament.GamePlaySystem = string.IsNullOrEmpty(gamePlaySystem) ? tournament.GamePlaySystem : gamePlaySystem;
    //                break;

    //            case 13:
    //                if (tournament.Start == DateTime.MinValue)
    //                {
    //                    if (tournament.NumberOfGroups == 0 && tournament.GamePlaySystem == "Group")
    //                    {
    //                        break;
    //                    }

    //                    ConsoleService.WriteTitle($"Run Tournament {tournament.Name}");
    //                    if (ConsoleService.AnswerYesOrNo("Before you proceed, make sure is correct everything."))
    //                    {
    //                        StartTournament(tournament, playersToTournament);
    //                    }
    //                }
    //                break;

    //            case 14:
    //                operation = null;
    //                break;

    //            default:
    //                if (operation == null)
    //                {
    //                    if (ConsoleService.AnswerYesOrNo("You want to Exit?"))
    //                    {
    //                        break;
    //                    }
    //                    operation = 0;
    //                }
    //                ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
    //                break;
    //        }

    //        if (operation == null)
    //        {
    //            _tournamentsService.InterruptTournament(tournament, _singlePlayerDuelManager);
    //            break;
    //        }
    //    }
    //}

    //private void ChangeRaceTo(Tournament tournament)
    //{
    //    var duels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id).ToList();
    //    var round = duels.First(d => d.EndGame == DateTime.MinValue).Round;

    //    if (duels.Any(p => p.Round == round && p.EndGame != DateTime.MinValue))
    //    {
    //        ConsoleService.WriteLineErrorMessage("Changing the number of games in this round is impossible because \n\r" +
    //            "the matches have already ended.");
    //        return;
    //    }
    //    ConsoleService.WriteTitle("Change Race To");
    //    var raceTo = ConsoleService.GetIntNumberFromUser("Enter To Many frame Min 3 Max 20:");
    //    if (raceTo == null || raceTo < 3 || raceTo > 20)
    //    {
    //        return;
    //    }

    //    if (!string.IsNullOrEmpty(round))
    //    {
    //        duels = duels.Where(d => d.Round == round).ToList();
    //    }

    //    foreach (var duel in duels)
    //    {
    //        duel.RaceTo = (int)raceTo;
    //        _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
    //    }
    //}

    //private void StartTournament(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    if (tournament == null || playersToTournament == null || tournament.End != DateTime.MinValue)
    //    {
    //        return;
    //    }

    //    _tournamentsService.StartTournament(tournament, _singlePlayerDuelManager);
    //    CreateDuelsToTournament(tournament, playersToTournament);
    //    var optionPlayerMenu = _actionService.GetMenuActionsByName("Start Tournament");
    //    string listStartedDuelsInText = String.Empty;

    //    while (true)
    //    {
    //        var allStartedDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //       .Where(d => d.StartGame != DateTime.MinValue && d.EndGame == DateTime.MinValue).Where(s => s.Interrupted.Equals(DateTime.MinValue)).ToList();

    //        listStartedDuelsInText = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allStartedDuels);

    //        if (string.IsNullOrEmpty(listStartedDuelsInText))
    //        {
    //            listStartedDuelsInText = ViewGroupsOr2KO(tournament, playersToTournament);
    //        }

    //        ConsoleService.WriteTitle($"Tournaments {tournament.Name} | Game System: {tournament.GamePlaySystem} | Start {tournament.Start} | Number Of Tables {tournament.NumberOfTables}");

    //        for (int i = 0; i < optionPlayerMenu.Count; i++)
    //        {
    //            ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
    //        }

    //        var operation = ConsoleService.GetIntNumberFromUser("Enter Option", listStartedDuelsInText);

    //        switch (operation)
    //        {
    //            case 1:
    //                StartAndInterruptedTournamentDuel(tournament, playersToTournament);
    //                break;

    //            case 2:
    //                UpdateDuelResult(tournament, playersToTournament);
    //                break;

    //            case 3:
    //                ConsoleService.WriteTitle($"View Groups Of Tournament {tournament.Name}");
    //                ConsoleService.WriteLineMessage(ViewGroupsOr2KO(tournament, playersToTournament));
    //                ConsoleService.GetKeyFromUser("Press Any Key...");
    //                break;

    //            case 4:
    //                var allDuelOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id);
    //                var listAllDuelsInText = _singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(allDuelOfTournament);
    //                ConsoleService.WriteTitle($"All Duels Of Tournament {tournament.Name}");
    //                ConsoleService.WriteLineMessage(listAllDuelsInText);
    //                ConsoleService.GetKeyFromUser("Press Any Key...");
    //                break;

    //            case 5:
    //                AddPlayersToTournament(tournament, playersToTournament);
    //                break;

    //            case 6:
    //                RemovePlayerOfTournament(tournament, playersToTournament);
    //                break;

    //            case 7:
    //                MovePlayer(tournament, playersToTournament);
    //                break;

    //            case 8:
    //                ChangeRaceTo(tournament);
    //                break;

    //            case 9:
    //                ChangeNumberOfTable(tournament, playersToTournament);
    //                break;

    //            case 10:
    //                operation = null;
    //                break;

    //            default:
    //                if (operation == null)
    //                {
    //                    if (ConsoleService.AnswerYesOrNo("You want to Exit?"))
    //                    {
    //                        break;
    //                    }
    //                    operation = 0;
    //                }
    //                ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
    //                break;
    //        }

    //        if (operation == null)
    //        {
    //            _tournamentsService.InterruptTournament(tournament, _singlePlayerDuelManager);
    //            break;
    //        }
    //    }
    //}

    //private void UpdateDuelResult(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    var duelToUpdate = _singlePlayerDuelManager.SelectStartedDuelByTournament(tournament.Id);

    //    if (duelToUpdate != null && playersToTournament.ListPlayersToTournament.Count != 0)
    //    {
    //        do
    //        {
    //            ConsoleService.WriteTitle("Update Duel");
    //            ConsoleService.WriteMessage(_singlePlayerDuelManager.ConvertListSinglePlayerDuelsToText(new List<SinglePlayerDuel>() { duelToUpdate }) + "\n\r");

    //            var resultFirstPlayer = ConsoleService.GetIntNumberFromUser($"\n\rEnter Result For {playersToTournament.ListPlayersToTournament
    //                .First(p => p.IdPLayer == duelToUpdate.IdFirstPlayer).TinyFulName} ");

    //            if (resultFirstPlayer != null && resultFirstPlayer > 0 && resultFirstPlayer <= duelToUpdate.RaceTo)
    //            {
    //                duelToUpdate.ScoreFirstPlayer = (int)resultFirstPlayer;
    //            }

    //            var resultSecondPlayer = ConsoleService.GetIntNumberFromUser($"\n\rEnter Result For {playersToTournament.ListPlayersToTournament
    //                .First(p => p.IdPLayer == duelToUpdate.IdSecondPlayer).TinyFulName}");

    //            if (resultSecondPlayer != null && resultSecondPlayer > 0 && resultSecondPlayer <= duelToUpdate.RaceTo)
    //            {
    //                duelToUpdate.ScoreSecondPlayer = (int)resultSecondPlayer;
    //            }
    //        }
    //        while (ConsoleService.AnswerYesOrNo("You Want To Correct Entered Results?"));

    //        _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToUpdate);
    //        if (duelToUpdate.ScoreFirstPlayer == duelToUpdate.RaceTo || duelToUpdate.ScoreSecondPlayer == duelToUpdate.RaceTo)
    //        {
    //            _singlePlayerDuelManager.EndSinglePlayerDuel(duelToUpdate);
    //            StartAndInterruptedTournamentDuel(tournament, playersToTournament);
    //        }
    //    }
    //}

    //private void ChangeNumberOfTable(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    ConsoleService.WriteTitle("Chenge Number Of Table\n\r");

    //    string textToDisplayIfNoDuelsIsStarted = "After entering the number of Tables,\n\r" +
    //       " the system manages the start of the next match.\n\r" +
    //       "The user enters the result of the duel\n\r" +
    //       " and if one of the players reaches the required number of points,\n\r" +
    //       "the match will end on a given table and a new one will start.\n\r" +
    //       "After the first round of matches begins,\n\r" +
    //       "the user can still make changes to the tournament settings,\n\r" +
    //       "but they are limited depending on the matches being played.";

    //    if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id).Any(d => d.StartGame != DateTime.MinValue))
    //    {
    //        textToDisplayIfNoDuelsIsStarted = $"Current number of tables {tournament.NumberOfTables}";
    //    }

    //    var numberOfTable = ConsoleService.GetIntNumberFromUser("Enter Number Of Table", textToDisplayIfNoDuelsIsStarted);

    //    if (numberOfTable == null || numberOfTable <= 0 || tournament.NumberOfTables == numberOfTable)
    //    {
    //        ConsoleService.WriteLineErrorMessage("Number Of Tables Not Change");
    //        return;
    //    }
    //    else
    //    {
    //        tournament.NumberOfTables = (int)numberOfTable;
    //        _tournamentsService.UpdateItem(tournament);
    //        _tournamentsService.SaveList();
    //        StartAndInterruptedTournamentDuel(tournament, playersToTournament);
    //    }
    //}

    //private void StartAndInterruptedTournamentDuel(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    if (tournament.NumberOfTables == 0)
    //    {
    //        ChangeNumberOfTable(tournament, playersToTournament);

    //        return;
    //    }

    //    var duelsOfTournament = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //        .Where(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer != -1).ToList();
    //    var startedDuels = duelsOfTournament.Where(d => d.StartGame != DateTime.MinValue && d.Interrupted == DateTime.MinValue && d.EndGame == DateTime.MinValue).ToList();
    //    var completedDuels = duelsOfTournament.Where(d => d.EndGame != DateTime.MinValue).ToList();

    //    if (duelsOfTournament.Any(d => d.StartGame != DateTime.MinValue))
    //    {
    //        if (duelsOfTournament.Count - completedDuels.Count > startedDuels.Count)
    //        {
    //            if (startedDuels.Count == tournament.NumberOfTables)
    //            {
    //                var duelToStart = _singlePlayerDuelManager.SelectDuelToStartByTournament(tournament.Id, "Select Duel To Start");

    //                if (duelToStart != null)
    //                {
    //                    var duelToIntrrypted = _singlePlayerDuelManager.SelectStartedDuelByTournament(tournament.Id, "Select Duel To Stop");
    //                    if (duelToIntrrypted != null)
    //                    {
    //                        duelToStart.TableNumber = duelToIntrrypted.TableNumber;
    //                        duelToIntrrypted.TableNumber = 0;
    //                        _singlePlayerDuelManager.InterruptDuel(duelToIntrrypted);
    //                        _singlePlayerDuelManager.StartSingleDuel(duelToStart);
    //                        _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToIntrrypted);
    //                        _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToStart);
    //                    }
    //                }
    //                return;
    //            }

    //            if (startedDuels.Count < tournament.NumberOfTables)
    //            {
    //                var duelsToStart = duelsOfTournament.Except(startedDuels).Except(completedDuels).OrderBy(d => d.StartNumberInGroup).ToList();

    //                if (duelsToStart.Any())
    //                {
    //                    foreach (var started in startedDuels)
    //                    {
    //                        duelsToStart = duelsToStart
    //                            .Where(
    //                            d => d.IdFirstPlayer != started.IdFirstPlayer
    //                            && d.IdFirstPlayer != started.IdSecondPlayer
    //                            && d.IdSecondPlayer != started.IdFirstPlayer
    //                            && d.IdSecondPlayer != started.IdSecondPlayer).ToList(); ;
    //                    }

    //                    List<int> freeTables = new List<int>();

    //                    for (int i = 0; i < tournament.NumberOfTables; i++)
    //                    {
    //                        if (startedDuels.Any(d => d.TableNumber == i + 1))
    //                        {
    //                            continue;
    //                        }
    //                        else
    //                        {
    //                            freeTables.Add(i + 1);
    //                        }
    //                    }

    //                    var numberDuelsTostart = freeTables.Count;

    //                    if (freeTables.Count > duelsToStart.Count)
    //                    {
    //                        numberDuelsTostart = duelsToStart.Count;
    //                    }

    //                    for (int i = 0; i < numberDuelsTostart; i++)
    //                    {
    //                        duelsToStart[i].TableNumber = freeTables[i];
    //                        _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelsToStart[i]);
    //                        _singlePlayerDuelManager.StartSingleDuel(duelsToStart[i]);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                for (int i = startedDuels.Count - tournament.NumberOfTables; i > 0; i--)
    //                {
    //                    var duelToIntrrypted = _singlePlayerDuelManager.SelectStartedDuelByTournament(tournament.Id, "Select Duel To Stop", $" {i} games left to stop");
    //                    if (duelToIntrrypted != null)
    //                    {
    //                        duelToIntrrypted.TableNumber = 0;
    //                        _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToIntrrypted);
    //                        _singlePlayerDuelManager.InterruptDuel(duelToIntrrypted);
    //                    }
    //                    else
    //                    {
    //                        for (int j = i - 1; j >= 0; j--)
    //                        {
    //                            var startedDuel = startedDuels[i];
    //                            startedDuel.TableNumber = 0;
    //                            _singlePlayerDuelManager.UpdateSinglePlayerDuel(startedDuel);
    //                            _singlePlayerDuelManager.InterruptDuel(startedDuel);
    //                        }
    //                    }
    //                }
    //            }

    //            return;
    //        }
    //    }
    //    else
    //    {
    //        if (tournament.GamePlaySystem == "Group")
    //        {
    //            var tableNumber = 1;
    //            foreach (var duelToStart in duelsOfTournament.OrderBy(d => d.StartNumberInGroup))
    //            {
    //                if (tableNumber <= tournament.NumberOfTables)
    //                {
    //                    tableNumber++;
    //                }
    //                else
    //                {
    //                    break;
    //                }

    //                duelToStart.TableNumber = tableNumber;
    //                _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToStart);
    //                _singlePlayerDuelManager.StartSingleDuel(duelToStart);
    //            }
    //        }
    //        else
    //        {
    //            var tableNumber = 0;
    //            foreach (var duel in duelsOfTournament)
    //            {
    //                if (tableNumber <= tournament.NumberOfTables)
    //                {
    //                    tableNumber++;
    //                }
    //                else
    //                {
    //                    break;
    //                }

    //                duel.TableNumber = tableNumber;
    //                _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
    //                _singlePlayerDuelManager.StartSingleDuel(duel);
    //            }
    //        }
    //    }
    //}

    //private void CreateDuelsToTournament(Tournament tournament, PlayersToTournament playersToTournament, string round = "Eliminations")
    //{
    //    if (tournament == null || playersToTournament == null)
    //    {
    //        return;
    //    }

    //    var listNewPlayer = playersToTournament.ListPlayersToTournament.Where(p => p.Round != round).ToList();

    //    if (listNewPlayer.Count == 0)
    //    {
    //        return;
    //    }

    //    if (tournament.GamePlaySystem == "Group")
    //    {
    //        var groupingPlayers = playersToTournament.ListPlayersToTournament.GroupBy(p => p.Group);

    //        foreach (var group in groupingPlayers)
    //        {
    //            var listPlayersOfGroup = group.ToList();
    //            if (group.Any(p => p.Round == round))
    //            {
    //                listPlayersOfGroup = listPlayersOfGroup.OrderBy(p => p.Round).ToList();
    //            }

    //            for (int i = 0; i < listPlayersOfGroup.Count; i++)
    //            {
    //                if (!listNewPlayer.Contains(listPlayersOfGroup[i]) && listPlayersOfGroup.Any(p => p.Round == round))
    //                {
    //                    continue;
    //                }

    //                if (string.IsNullOrEmpty(listPlayersOfGroup[i].Round) || !listPlayersOfGroup[i].Round.Equals(round) || listPlayersOfGroup.LastIndexOf(listPlayersOfGroup[i]) == 0)
    //                {
    //                    listPlayersOfGroup[i].Round = round;
    //                }

    //                for (int j = i + 1; j < listPlayersOfGroup.Count; j++)
    //                {
    //                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
    //                    tournament.Id,
    //                    listPlayersOfGroup[i].IdPLayer,
    //                    listPlayersOfGroup[j].IdPLayer,
    //                    round);
    //                }
    //            }
    //        }

    //        playersToTournament.SavePlayersToTournament();
    //        DetermineTheOrderOfDuelsToStartInGroup(tournament, playersToTournament);
    //    }
    //    else
    //    {
    //        if (listNewPlayer.Count == 1 && round == "Eliminations")
    //        {
    //            if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id).Any(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1))
    //            {
    //                var duelToNewPlayer = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //                    .First(d => d.IdFirstPlayer > 1 && d.IdSecondPlayer == -1);
    //                duelToNewPlayer.IdSecondPlayer = listNewPlayer.First().IdPLayer;
    //                duelToNewPlayer.ScoreFirstPlayer = 0;
    //                duelToNewPlayer.ScoreSecondPlayer = 0;
    //                listNewPlayer.First().Round = round;
    //                duelToNewPlayer.Round = round;
    //                _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToNewPlayer);
    //            }
    //            else
    //            {
    //                var newDuel = _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
    //                       tournament.Id,
    //                       listNewPlayer.First().IdPLayer,
    //                       -1,
    //                       round);
    //                newDuel.ScoreFirstPlayer = 3;
    //                newDuel.ScoreSecondPlayer = 0;
    //                listNewPlayer.First().Round = round;
    //            }
    //        }
    //        else
    //        {
    //            for (int i = 0; i < listNewPlayer.Count; i += 2)
    //            {
    //                if (((playersToTournament.ListPlayersToTournament.Count - listNewPlayer.Count) % 2 != 0
    //                    || playersToTournament.ListPlayersToTournament.Count - listNewPlayer.Count == 0)
    //                    && listNewPlayer[i].Equals(listNewPlayer.Last()))
    //                {
    //                    _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
    //                        tournament.Id,
    //                        listNewPlayer.Last().IdPLayer,
    //                        -1,
    //                        round);
    //                    listNewPlayer.Last().Round = round;
    //                    break;
    //                }
    //                else if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //                    .Any(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1))
    //                {
    //                    var duelToupdate = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //                        .First(d => d.IdFirstPlayer != -1 && d.IdSecondPlayer == -1);
    //                    duelToupdate.Round = round;
    //                    duelToupdate.IdSecondPlayer = listNewPlayer[i].IdPLayer;
    //                    listNewPlayer[i].Round = round;
    //                    _singlePlayerDuelManager.UpdateSinglePlayerDuel(duelToupdate);
    //                    i--;
    //                    continue;
    //                }
    //                else
    //                {
    //                    listNewPlayer[i].Round = round;
    //                    listNewPlayer[i + 1].Round = round;
    //                }

    //                _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(
    //                        tournament.Id,
    //                        listNewPlayer[i].IdPLayer,
    //                        listNewPlayer[i + 1].IdPLayer,
    //                        round);
    //            }
    //        }
    //        playersToTournament.SavePlayersToTournament();
    //    }
    //}

    //private void DetermineTheOrderOfDuelsToStartInGroup(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    var allTournamentDuels = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //        .Where(d => d.IsActive == true && d.IdFirstPlayer != -1);

    //    var groupingPlayers = playersToTournament.ListPlayersToTournament.GroupBy(p => p.Group);
    //    List<SinglePlayerDuel> duelsOfGroup = new();
    //    foreach (var groupPlayers in groupingPlayers)
    //    {
    //        List<int[]> queueDuels = new List<int[]>();
    //        var numberOfPlayerInGroup = groupPlayers.Count();

    //        List<int> firstSequenceNumberOfDuel = new List<int>();
    //        List<int> secondSequenceNumberOfDuel = new List<int>();
    //        int numberDuelsInRound = numberOfPlayerInGroup / 2;

    //        var addDuelsToQueue = (List<int> firstSequence, List<int> secondSequence) =>
    //        {
    //            queueDuels.AddRange(firstSequence
    //            .Zip(secondSequence, (first, second) => new int[2] { first, second }));
    //        };

    //        if (numberOfPlayerInGroup % 2 != 0)
    //        {
    //            numberDuelsInRound++;
    //        }

    //        for (int i = 1; i <= numberDuelsInRound; i++)
    //        {
    //            firstSequenceNumberOfDuel.Add(i);
    //            secondSequenceNumberOfDuel.Add(numberDuelsInRound + i);
    //        }

    //        for (int i = 1; i <= numberDuelsInRound; i++)
    //        {
    //            addDuelsToQueue(firstSequenceNumberOfDuel, secondSequenceNumberOfDuel);
    //            secondSequenceNumberOfDuel.Add(secondSequenceNumberOfDuel.First());
    //            secondSequenceNumberOfDuel.Remove(secondSequenceNumberOfDuel.First());
    //        }

    //        for (int i = 1; i < numberDuelsInRound; i++)
    //        {
    //            for (int j = i + 1; j <= numberDuelsInRound; j++)
    //            {
    //                queueDuels.Add([i, j]);
    //                queueDuels.Add([numberDuelsInRound + i, numberDuelsInRound + j]);
    //            }
    //        }

    //        if (numberOfPlayerInGroup % 2 != 0)
    //        {
    //            var intToRemove = queueDuels.Where(d => d[1] == numberOfPlayerInGroup + 1).ToList();
    //            foreach (var intRe in intToRemove)
    //            {
    //                queueDuels.Remove(intRe);
    //            }
    //        }

    //        foreach (var player in groupPlayers)
    //        {
    //            duelsOfGroup.AddRange(allTournamentDuels
    //                .Where(d => d.IdFirstPlayer == player.IdPLayer || d.IdSecondPlayer == player.IdPLayer)
    //                .Except(duelsOfGroup));
    //        }
    //        var listGroupingPlayers = groupPlayers.ToList();

    //        foreach (var duel in duelsOfGroup)
    //        {
    //            duel.Group = groupPlayers.Key;
    //            duel.StartNumberInGroup = queueDuels
    //                .FindIndex(d => d[0] == listGroupingPlayers.FindIndex(p => p.IdPLayer == duel.IdFirstPlayer) + 1 && d[1] == listGroupingPlayers.FindIndex(p => p.IdPLayer == duel.IdSecondPlayer) + 1
    //                 || d[0] == listGroupingPlayers.FindIndex(p => p.IdPLayer == duel.IdSecondPlayer) + 1 && d[1] == listGroupingPlayers.FindIndex(p => p.IdPLayer == duel.IdFirstPlayer) + 1) + 1;
    //            _singlePlayerDuelManager.UpdateSinglePlayerDuel(duel);
    //        }

    //        duelsOfGroup.Clear();
    //    }
    //}

    public void DeleteTournament()
    {
        var tournament = SearchTournament();
        if (tournament == null || tournament.End != DateTime.MinValue)
        {
            return;
        }

        _singlePlayerDuelManager.RemoveTournamentDuel(tournament);
        _tournamentsService.RemoveItem(tournament);
        _tournamentsService.SaveList();
    }

    //private void SetGroups(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    int numberOfGroups = 0;
    //    int? enterNumber = 0;
    //    if (tournament.GamePlaySystem == "2KO")
    //    {
    //        ConsoleService.WriteLineErrorMessage("2KO Game System Set. Change To Group");
    //        return;
    //    }
    //    else if (tournament.NumberOfPlayer < 8)
    //    {
    //        ConsoleService.WriteLineErrorMessage("Add More Players");
    //        return;
    //    }
    //    ConsoleService.WriteTitle($"{tournament.NumberOfPlayer} Players allows the tournament to start:");
    //    if (string.IsNullOrEmpty(tournament.GamePlaySystem))
    //    {
    //        tournament.GamePlaySystem = "Group";
    //    }
    //    if (tournament.GamePlaySystem == "Group")
    //    {
    //        if (tournament.NumberOfPlayer >= 8 && tournament.NumberOfPlayer < 10)
    //        {
    //            ConsoleService.WriteLineMessage("Created 2 groups:\n\r" +
    //                "4 players will advance from the group to the knockout round\n\rPress Any Key ...");
    //            ConsoleService.GetKeyFromUser();
    //            numberOfGroups = 2;
    //        }
    //        else if (tournament.NumberOfPlayer >= 10 && tournament.NumberOfPlayer < 12)
    //        {
    //            ConsoleService.WriteLineMessage("Created 2 groups:\n\r" +
    //                "4 players will advance from the group to the knockout round\n\rPress Any Key ...");
    //            ConsoleService.GetKeyFromUser();
    //            numberOfGroups = 2;
    //        }
    //        else if (tournament.NumberOfPlayer >= 12 && tournament.NumberOfPlayer < 16)
    //        {
    //            ConsoleService.WriteLineMessage("2 groups:\n\r" +
    //                "4 players will advance from the group to the knockout round\n\r----\n\r");
    //            ConsoleService.WriteLineMessage("4 groups:\n\r" +
    //                "2 players will advance from the group to the knockout round\n\r----\n\r");
    //            enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 2 or 4");
    //            if (enterNumber != 4 || enterNumber != 2)
    //            {
    //                return;
    //            }
    //        }
    //        else if (tournament.NumberOfPlayer >= 16 && tournament.NumberOfPlayer < 24)
    //        {
    //            if (tournament.NumberOfPlayer == 16)
    //            {
    //                ConsoleService.WriteLineMessage("2 groups, maximum 8 players per group:" +
    //                            "\n\r4 players will advance from the group to the knockout round\n\r----\r\n");
    //                ConsoleService.WriteLineMessage("4 groups:\n\r" +
    //                    "2 players will advance from the group to the knockout round\n\r----\n\r");
    //                enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 2 or 4");
    //                if (enterNumber != 4 || enterNumber != 2)
    //                {
    //                    return;
    //                }
    //            }
    //            else
    //            {
    //                numberOfGroups = 4;
    //            }
    //        }
    //        else if (tournament.NumberOfPlayer >= 24 && tournament.NumberOfPlayer < 32)
    //        {
    //            if (tournament.NumberOfPlayer == 24)
    //            {
    //                ConsoleService.WriteLineMessage("4 groups,maximum 6 players per group:" +
    //                            "\n\r2 players will advance from the group to the knockout round\n\r----\n\r");
    //                ConsoleService.WriteLineMessage("Eight groups\n\r" +
    //                    "2 players will advance from the group to the knockout round\n\r----\n\r");
    //                enterNumber = ConsoleService.GetIntNumberFromUser("Enter number of groups 4 or 8");
    //            }
    //            else
    //            {
    //                numberOfGroups = 8;
    //            }
    //        }
    //        else if (tournament.NumberOfPlayer >= 32 && tournament.NumberOfPlayer < 49)
    //        {
    //            ConsoleService.WriteLineMessage("Eight groups" +
    //                "\n\r2 players will advance from the group to the knockout round\n\r----\n\rPress Any Key ...");
    //            ConsoleService.GetKeyFromUser();
    //            numberOfGroups = 8;
    //        }
    //        else if (tournament.NumberOfPlayer >= 49)
    //        {
    //            ConsoleService.WriteLineMessage("Maximum 48 players");
    //        }
    //    }

    //    if (numberOfGroups == 0 && enterNumber == 2 || enterNumber == 4 || enterNumber == 8)
    //    {
    //        numberOfGroups = (int)enterNumber;
    //    }
    //    else if (numberOfGroups == 0)
    //    {
    //        ConsoleService.WriteLineErrorMessage("Something went wrong, please try again");
    //        return;
    //    }

    //    tournament.NumberOfGroups = numberOfGroups;
    //    RandomSelectionOfPlayers(tournament, playersToTournament);
    //    _tournamentsService.SaveList();
    //}

    //private void EditGroupsOr2KOList(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    var optionPlayerMenu = _actionService.GetMenuActionsByName("Edit Groups");
    //    var title = tournament.GamePlaySystem == "Group" ? "Edit Groups" : "Edit 2KO List";

    //    while (true)
    //    {
    //        ConsoleService.WriteTitle(title);
    //        ConsoleService.WriteLineMessage(ViewGroupsOr2KO(tournament, playersToTournament));
    //        if (playersToTournament.ListPlayersToTournament.Count == 0)
    //        {
    //            ConsoleService.WriteLineErrorMessage("Empty List Of Player");
    //            return;
    //        }
    //        ConsoleService.WriteLineMessage("\n\r=====================");

    //        for (int i = 0; i < optionPlayerMenu.Count; i++)
    //        {
    //            ConsoleService.WriteLineMessage($"{i + 1}. {optionPlayerMenu[i].Name}");
    //        }

    //        var operation = ConsoleService.GetIntNumberFromUser("\n\rEnter Option");
    //        switch (operation)
    //        {
    //            case 1:
    //                MovePlayer(tournament, playersToTournament);
    //                break;

    //            case 2:
    //                playersToTournament.LoadList(tournament);
    //                break;

    //            case 3:
    //                operation = null;
    //                break;

    //            default:
    //                if (operation == null)
    //                {
    //                    if (!ConsoleService.AnswerYesOrNo("Exit To Tournament Menu?"))
    //                    {
    //                        ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
    //                        operation = 0;
    //                    }
    //                }
    //                break;
    //        }

    //        if (operation == null)
    //        {
    //            if (ConsoleService.AnswerYesOrNo("Save Changes?"))
    //            {
    //                playersToTournament.SavePlayersToTournament();
    //            }
    //            else
    //            {
    //                if (!ConsoleService.AnswerYesOrNo("Back to Edit?"))
    //                {
    //                    ConsoleService.WriteLineErrorMessage("Changes Not Save!");
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}

    //private void MovePlayer(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    List<Player> players = new List<Player>();
    //    foreach (var playerToTournament in playersToTournament.ListPlayersToTournament)
    //    {
    //        var player = _playerService.GetItemById(playerToTournament.IdPLayer);
    //        bool isPlayerEndDuelOrPlay = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //           .Exists(p => (p.IdFirstPlayer == player.Id || p.IdSecondPlayer == player.Id) && (p.StartGame != DateTime.MinValue && p.Interrupted == DateTime.MinValue || p.EndGame != DateTime.MinValue));
    //        if (player != null && !isPlayerEndDuelOrPlay)
    //        {
    //            players.Add(player);
    //        }
    //    }

    //    if (players.Count > 0)
    //    {
    //        var player = _playerManager.SearchPlayer("Select Player To Move\n\r" +
    //            "List of players who can currently be transferred", players);

    //        if (player == null)
    //        {
    //            return;
    //        }

    //        var playerToMove = playersToTournament.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);

    //        if (playerToMove != null)
    //        {
    //            ConsoleService.WriteTitle("Move Player");
    //            ConsoleService.WriteLineMessage(ViewGroupsOr2KO(tournament, playersToTournament));

    //            if (tournament.GamePlaySystem == "Group")
    //            {
    //                string groups = string.Empty;

    //                var groupingPlayer = playersToTournament.ListPlayersToTournament.GroupBy(p => p.Group);

    //                if (groupingPlayer.FirstOrDefault(g => g.Key == playerToMove.Group).Count() <= 2)
    //                {
    //                    ConsoleService.WriteLineErrorMessage("You cannot remove a player. Minimum number of players in group 2.");
    //                    return;
    //                }

    //                var duelsPlayerToMove = _singlePlayerDuelManager
    //                    .GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //                    .Where(p => p.IdFirstPlayer == playerToMove.IdPLayer || p.IdSecondPlayer == playerToMove.IdPLayer).ToList();

    //                if (tournament.NumberOfGroups == 2)
    //                {
    //                    var newGroup = groupingPlayer.First(g => g.Key != playerToMove.Group).Key;
    //                    playerToMove.Group = newGroup;
    //                }
    //                else
    //                {
    //                    var groupsList = groupingPlayer.Select(g => g.Key)
    //                                           .Where(group => group != playerToMove.Group)
    //                                           .ToList();
    //                    groups = string.Join(",", groupsList);

    //                    var key = ConsoleService.GetKeyFromUser($"{playerToMove.TinyFulName} Group: {playerToMove.Group}\n\r" +
    //                        $"Press Key {groups}");
    //                    var s = key.KeyChar.ToString().ToUpper();
    //                    if (groupingPlayer.Any(g => g.Key == key.KeyChar.ToString().ToUpper()) && key.KeyChar.ToString().ToUpper() != playerToMove.Group)
    //                    {
    //                        playerToMove.Group = key.KeyChar.ToString().ToUpper();
    //                    }
    //                }

    //                if (duelsPlayerToMove.Count != 0)
    //                {
    //                    playerToMove.Round = string.Empty;
    //                    _singlePlayerDuelManager.RemoveTournamentDuel(tournament, playerToMove.IdPLayer);
    //                    CreateDuelsToTournament(tournament, playersToTournament);
    //                }
    //            }
    //            else
    //            {
    //                if (_singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id) != null)
    //                {
    //                    ConsoleService.WriteLineErrorMessage("Transferring a player is impossible");
    //                    return;
    //                }
    //                ConsoleService.WriteTitle("Move Player");
    //                ConsoleService.WriteLineMessage(ViewGroupsOr2KO(tournament, playersToTournament));
    //                var newPosition = ConsoleService.GetIntNumberFromUser("Enter New Position", $"\n\r{ViewPlayerToTournamentDetail(playerToMove)}");

    //                if (newPosition > 0 && newPosition != null && newPosition <= playersToTournament.ListPlayersToTournament.Count)
    //                {
    //                    if (newPosition > playerToMove.Position)
    //                    {
    //                        for (int i = playerToMove.Position + 1; i <= (int)newPosition; i++)
    //                        {
    //                            var playerToChange = playersToTournament.ListPlayersToTournament
    //                            .First(p => p.Position == i);
    //                            playerToChange.Position = i - 1;
    //                            playerToChange.TwoKO = playerToChange.Position.ToString();

    //                            if (i == newPosition)
    //                            {
    //                                playerToMove.Position = (int)newPosition;
    //                                playerToMove.TwoKO = playerToMove.Position.ToString();
    //                            }
    //                        }
    //                    }
    //                    else if (newPosition < playerToMove.Position)
    //                    {
    //                        for (int i = playerToMove.Position - 1; i >= (int)newPosition; i--)
    //                        {
    //                            var playerToChange = playersToTournament.ListPlayersToTournament
    //                            .First(p => p.Position == i);

    //                            playerToChange.Position = i + 1;
    //                            playerToChange.TwoKO = playerToChange.Position.ToString();

    //                            if (i == newPosition)
    //                            {
    //                                playerToMove.Position = (int)newPosition;
    //                                playerToMove.TwoKO = playerToMove.Position.ToString();
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        playersToTournament.SavePlayersToTournament();
    //    }
    //}

    //private void RandomSelectionOfPlayers(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    var randomList = new List<PlayerToTournament>();
    //    randomList.AddRange(playersToTournament.ListPlayersToTournament);
    //    var random = new Random();

    //    while (randomList.Count > 0)
    //    {
    //        var randomNumber = random.Next(0, randomList.Count);
    //        var playerId = randomList[randomNumber].IdPLayer;
    //        randomList.RemoveAt(randomNumber);
    //        var player = playersToTournament.ListPlayersToTournament.First(p => p.IdPLayer == playerId);
    //        player.TwoKO = (randomList.Count + 1).ToString();
    //        player.Position = randomList.Count + 1;
    //    }

    //    playersToTournament.SavePlayersToTournament();
    //    randomList.AddRange(playersToTournament.ListPlayersToTournament.OrderBy(p => p.TwoKO));

    //    char group = (char)65;
    //    if (tournament.GamePlaySystem == "Group" && tournament.NumberOfGroups != 0)
    //    {
    //        int numberPlayersOfGroup = randomList.Count / tournament.NumberOfGroups;
    //        for (int i = 0; i < tournament.NumberOfGroups; i++)
    //        {
    //            for (int j = 0; j < numberPlayersOfGroup; j++)
    //            {
    //                randomList[i * numberPlayersOfGroup + j].Group = group.ToString().ToUpper();
    //            }
    //            group++;
    //        }
    //        var endPlayers = randomList.Count % tournament.NumberOfGroups;
    //        group = (char)65;

    //        for (int p = randomList.Count - endPlayers; p < randomList.Count; p++)
    //        {
    //            randomList[p].Group = group.ToString().ToUpper();

    //            if (group == (char)65 + tournament.NumberOfGroups)
    //            {
    //                group = (char)65;
    //            }
    //            else
    //            {
    //                group++;
    //            }
    //        }
    //    }

    //    playersToTournament.ListPlayersToTournament = randomList;
    //    playersToTournament.SavePlayersToTournament();
    //}

    private Tournament? CreateNewTournament()
    {
        IClubManager clubManager = new ClubManager(_actionService);
        Tournament tournament = new Tournament();

        do
        {
            ConsoleService.WriteTitle("Create New Tournament");
            tournament.Name = ConsoleService.GetRequiredStringFromUser("Enter Name ");

            if (string.IsNullOrEmpty(tournament.Name))
            {
                return null;
            }
            else if (_tournamentsService.GetAllItem().Any(t => t.Name.Equals(tournament.Name)))
            {
                ConsoleService.WriteLineErrorMessage("The entered name is already use. Please enter a different name.");
            }
            else
            {
                break;
            }
        } while (true);

        var club = clubManager.SearchClub("Create New Tournament Add Club");
        if (club == null)
        {
            club = clubManager.AddNewClub();
            if (club == null)
            {
                return null;
            }
        }
        tournament.IdClub = club.Id;

        _tournamentsService.AddNewTournament(tournament);
        _singlePlayerDuelManager.NewTournamentSinglePlayerDuel(tournament.Id);

        ITournamentsManager tournamentsManager = this;
        TournamentGamePlayManager tournamentGamePlaySystem = new(tournament, tournamentsManager, _actionService, _playerManager, _playerService, _singlePlayerDuelManager);
        tournament.GamePlaySystem = tournamentGamePlaySystem.GetGamePlaySystemFromUser();

        if (string.IsNullOrEmpty(tournament.GamePlaySystem))
        {
            return null;
        }
        PlayersToTournament playersToTournament = new PlayersToTournament(tournament, tournamentsManager, _playerManager, _playerService);
        playersToTournament.AddPlayersToTournament();
        _tournamentsService.UpdateItem(tournament);
        _tournamentsService.SaveList();

        return tournament;
    }

    //public void AddPlayersToTournament(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    List<PlayerToTournament> listPlayersToTournament = playersToTournament.ListPlayersToTournament.OrderBy(p => p.Position).ToList();
    //    List<Player> players = new List<Player>();

    //    if (playersToTournament != null)
    //    {
    //        foreach (var playerToTournament in playersToTournament.ListPlayersToTournament)
    //        {
    //            var tournamentPlayer = _playerService.GetItemById(playerToTournament.IdPLayer);
    //            if (tournamentPlayer != null)
    //            {
    //                players.Add(tournamentPlayer);
    //            }
    //        }
    //    }

    //    var player = _playerManager.SearchPlayer($"Add Player To tournament {tournament.Name}" +
    //        $"\n\rSelect Player On List Or Press Esc To Add New Player",
    //        null,
    //        players);

    //    if (player == null)
    //    {
    //        if (ConsoleService.AnswerYesOrNo("You want to add a new player"))
    //        {
    //            player = _playerManager.AddNewPlayer();
    //        }

    //        if (player == null)
    //        {
    //            return;
    //        }
    //    }
    //    var playeraddress = _playerService.GetPlayerAddress(player);

    //    if (listPlayersToTournament.Any(p => p.IdPLayer == player.Id))
    //    {
    //        ConsoleService.WriteLineErrorMessage($"The Player {player.FirstName} {player.LastName} is on the list");
    //    }
    //    else
    //    {
    //        PlayerToTournament newPlayer = new(player, "------");

    //        if (listPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.Group)))
    //        {
    //            var groupingPlayers = listPlayersToTournament
    //           .GroupBy(group => group.Group, group => group).OrderBy(g => g.Count());
    //            newPlayer.Position = listPlayersToTournament.Max(p => p.Position) + 1;
    //            newPlayer.TwoKO = newPlayer.Position.ToString();
    //            newPlayer.Group = groupingPlayers.First().Key;
    //        }
    //        else
    //        {
    //            if (listPlayersToTournament.Any())
    //            {
    //                newPlayer.Position = listPlayersToTournament.Max(p => p.Position) + 1;
    //                newPlayer.TwoKO = newPlayer.Position.ToString();
    //            }
    //            else
    //            {
    //                newPlayer.Position = 1;
    //                newPlayer.TwoKO = newPlayer.Position.ToString();
    //            }
    //        }

    //        if (playeraddress != null)
    //        {
    //            newPlayer.Country = playeraddress.Country;
    //        }

    //        listPlayersToTournament.Add(newPlayer);
    //        tournament.NumberOfPlayer = listPlayersToTournament.Count;
    //        playersToTournament.ListPlayersToTournament = listPlayersToTournament;
    //    }
    //    _tournamentsService.SaveList();
    //    playersToTournament.SavePlayersToTournament();

    //    players.Add(player);

    //    ConsoleService.WriteTitle("");
    //    if (ConsoleService.AnswerYesOrNo("Add Next Player"))
    //    {
    //        AddPlayersToTournament(tournament, playersToTournament);
    //    }
    //}

    //public void RemovePlayerOfTournament(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    List<Player> players = new List<Player>();
    //    if (playersToTournament.ListPlayersToTournament.Count > 8)
    //    {
    //        ConsoleService.WriteTitle("");
    //        ConsoleService.WriteLineErrorMessage("Attention!!!");
    //        if (!ConsoleService.AnswerYesOrNo("Remember that removing a player may disturb the group structure.\n\r" +
    //            "Players who are currently playing or have finished the match will not appear on the list."))
    //        {
    //            return;
    //        }

    //        foreach (var playerToTournament in playersToTournament.ListPlayersToTournament)
    //        {
    //            var player = _playerService.GetItemById(playerToTournament.IdPLayer);
    //            bool isPlayerEndDuelOrPlay = _singlePlayerDuelManager.GetSinglePlayerDuelsByTournamentsOrSparrings(tournament.Id)
    //           .Exists(p => (p.IdFirstPlayer == player.Id || p.IdSecondPlayer == player.Id) && (p.StartGame != DateTime.MinValue && p.Interrupted == DateTime.MinValue) || p.EndGame != DateTime.MinValue);

    //            if (player != null && !isPlayerEndDuelOrPlay)
    //            {
    //                players.Add(player);
    //            }
    //        }
    //    }
    //    else
    //    {
    //        ConsoleService.WriteLineErrorMessage("You cannot remove a player. \n\rThe minimum number of players is 8.");
    //    }

    //    if (players.Count > 0)
    //    {
    //        var player = _playerManager.SearchPlayer("Remowe Player", players);
    //        if (player == null)
    //        {
    //            return;
    //        }
    //        else
    //        {
    //            var playerToRemove = playersToTournament.ListPlayersToTournament.FirstOrDefault(p => p.IdPLayer == player.Id);

    //            if (playerToRemove != null)
    //            {
    //                if (tournament.GamePlaySystem == "Group")
    //                {
    //                    var groupingPlayer = playersToTournament.ListPlayersToTournament.GroupBy(d => d.Group);
    //                    if (groupingPlayer.FirstOrDefault(g => g.Key == playerToRemove.Group).Count() <= 2)
    //                    {
    //                        ConsoleService.WriteLineErrorMessage("You cannot remove a player. Minimum number of players in group 2.");
    //                        return;
    //                    }
    //                    else
    //                    {
    //                        _singlePlayerDuelManager.RemoveTournamentDuel(tournament, playerToRemove.IdPLayer);
    //                        playersToTournament.ListPlayersToTournament.Remove(playerToRemove);
    //                        playersToTournament.SavePlayersToTournament();
    //                        tournament.NumberOfPlayer = playersToTournament.ListPlayersToTournament.Count;
    //                        _tournamentsService.SaveList();
    //                    }
    //                }
    //                else
    //                {
    //                    _singlePlayerDuelManager.RemoveTournamentDuel(tournament, playerToRemove.IdPLayer);
    //                    playersToTournament.ListPlayersToTournament.Remove(playerToRemove);
    //                    playersToTournament.SavePlayersToTournament();
    //                    tournament.NumberOfPlayer = playersToTournament.ListPlayersToTournament.Count;
    //                    _tournamentsService.SaveList();
    //                }
    //            }
    //        }
    //    }
    //}

    //public void ViewListPlayersToTournament(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    string formatText = string.Empty;
    //    if (playersToTournament.ListPlayersToTournament.Any())
    //    {
    //        ConsoleService.WriteTitle($"List Players Of {tournament.Name}");
    //        foreach (var player in playersToTournament.ListPlayersToTournament)
    //        {
    //            formatText = playersToTournament.ListPlayersToTournament
    //                .Select(p => new { number = $"{playersToTournament.ListPlayersToTournament.IndexOf(player) + 1}. " }).First().number
    //                 + $"{player.TinyFulName} {player.Country}";
    //            ConsoleService.WriteLineMessage(formatText);
    //        }
    //    }
    //    else
    //    {
    //        ConsoleService.WriteTitle($"List Players Of {tournament.Name}");
    //        ConsoleService.WriteLineErrorMessage("Empty List");
    //    }
    //}

    //public string ViewGroupsOr2KO(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    var formatText = string.Empty;
    //    if (tournament.GamePlaySystem == "2KO")
    //    {
    //        if (playersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.TwoKO)))
    //        {
    //            formatText += $"\n\rStart List 2KO System\n\r\n\r";
    //            foreach (var player in playersToTournament.ListPlayersToTournament.OrderBy(p => p.Position))
    //            {
    //                formatText += $" {playersToTournament.ListPlayersToTournament.IndexOf(player) + 1}. {player.FirstName} {player.LastName} {player.Country}\n\r";
    //            }
    //        }
    //    }
    //    else if (tournament.GamePlaySystem == "Group")
    //    {
    //        if (playersToTournament.ListPlayersToTournament.Any(p => !string.IsNullOrEmpty(p.TwoKO)))
    //        {
    //            if (playersToTournament.ListPlayersToTournament.Any(p => string.IsNullOrEmpty(p.Group)))
    //            {
    //                return formatText = "Set Groups";
    //            }
    //            var groupingPlayer = playersToTournament.ListPlayersToTournament
    //                .GroupBy(group => group.Group, group => group).OrderBy(g => g.Key).ToList();

    //            List<PlayerToTournament> formatList = new List<PlayerToTournament>();
    //            decimal numberLine = playersToTournament.ListPlayersToTournament.Count / tournament.NumberOfGroups;

    //            formatText += "\n\r";
    //            for (int i = 0; i < tournament.NumberOfGroups; i++)
    //            {
    //                formatText += $"Group: {groupingPlayer[i].Key,-23}";
    //            }

    //            formatText += "\n\r";
    //            for (var j = 0; j <= Math.Floor(numberLine); j++)
    //            {
    //                formatText += "\n\r";
    //                for (var i = 0; i < tournament.NumberOfGroups; i++)
    //                {
    //                    var player = groupingPlayer[i].Select(p => p).Except(formatList).FirstOrDefault();
    //                    if (player != null)
    //                    {
    //                        formatList.Add(player);
    //                        formatText += $"{player.TinyFulName}";
    //                    }
    //                    else
    //                    {
    //                        formatText += $"{" ",-30}";
    //                    }
    //                }
    //            }
    //        }
    //    }

    //    return formatText;
    //}

    //private void SetGamePlaySystem(Tournament tournament, PlayersToTournament playersToTournament)
    //{
    //    TournamentGamePlayManager tournamentGamePlaySystem = new TournamentGamePlayManager(tournament);
    //    tournament.GamePlaySystem = tournamentGamePlaySystem.GetGamePlaySystemFromUser();
    //}

    public Tournament SearchTournament(string title = "")
    {
        StringBuilder inputString = new StringBuilder();
        List<Tournament> findTournamentTemp = _tournamentsService.SearchTournament(" ")
            .Where(t => t.End == DateTime.MinValue && t.IsActive == true).ToList();
        List<Tournament> findTournament = [];
        List<Tournament> findTournamentToView = [];
        findTournament.AddRange(findTournamentTemp);
        int maxEntriesToDisplay = 15;
        if (!findTournament.Any())
        {
            ConsoleService.WriteLineErrorMessage("Empty List");
            return null;
        }

        if (findTournament.Count >= maxEntriesToDisplay - 1)
        {
            findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
        }
        else
        {
            findTournamentToView = findTournament;
        }
        var address = new Address();
        int indexSelectedTournament = 0;
        title = string.IsNullOrWhiteSpace(title) ? "Search Tournament" : title;

        var headTableToview = title + $"\r\n    {" LP",-5}{"ID",-6}{"Name",-21}" +
                    $"{"Game System",-16}{"Club Name",-21}{"Start",-15}{"End",-15}{"Players",-11}";
        do
        {
            ConsoleService.WriteTitle(headTableToview);

            foreach (var tournament in findTournamentToView)
            {
                var formmatStringToView = findTournament.IndexOf(tournament) == indexSelectedTournament ?
                    "\r\n---> " + $"{findTournament.IndexOf(tournament) + 1,-5}".Remove(4) + _tournamentsService.GetTournamentDetailView(tournament) + $" <----\r\n" :
                    "     " + $"{findTournament.IndexOf(tournament) + 1,-5}".Remove(4) + _tournamentsService.GetTournamentDetailView(tournament);

                ConsoleService.WriteLineMessage(formmatStringToView);
            }

            ConsoleService.WriteLineMessage($"\r\n------(Found {findTournament.Count} Tournament)-------\r\n" + inputString.ToString());
            ConsoleService.WriteLineMessage(@"Enter string move UP or Down  and  press enter to Select");

            var keyFromUser = ConsoleService.GetKeyFromUser("Selected Tournament: "
                + findTournament[indexSelectedTournament].Name);

            if (char.IsLetterOrDigit(keyFromUser.KeyChar))
            {
                if (findTournament.Count == 1 && !string.IsNullOrEmpty(inputString.ToString()))
                {
                    ConsoleService.WriteLineErrorMessage("No entries found !!!");
                }
                else
                {
                    inputString.Append(keyFromUser.KeyChar);

                    if (inputString.Length == 1)
                    {
                        if (findTournamentTemp.Any(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())))
                        {
                            findTournament = [.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                            if (findTournament.Count >= maxEntriesToDisplay - 1)
                            {
                                findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
                            }
                            else
                            {
                                findTournamentToView = findTournament;
                            }
                            indexSelectedTournament = 0;
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            ConsoleService.WriteLineErrorMessage("No entries found !!!");
                        }
                    }
                    else
                    {
                        if (findTournamentTemp.Any(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())))
                        {
                            findTournament = [.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                            if (findTournament.Count >= maxEntriesToDisplay - 1)
                            {
                                findTournamentToView = findTournament.GetRange(0, maxEntriesToDisplay);
                            }
                            else
                            {
                                findTournamentToView = findTournament;
                            }
                            indexSelectedTournament = 0;
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            ConsoleService.WriteLineErrorMessage("No entry found !!!");
                        }
                    }
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Backspace && inputString.Length > 0)
            {
                inputString.Remove(inputString.Length - 1, 1);
                findTournament.Clear();
                findTournament.AddRange([.. findTournamentTemp.Where(p => $"{p.Id} {p.Name} {p.GamePlaySystem}".ToLower()
                    .Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)]);
                indexSelectedTournament = 0;
            }
            else if (keyFromUser.Key == ConsoleKey.DownArrow && indexSelectedTournament <= findTournament.Count - 2)
            {
                if (indexSelectedTournament >= maxEntriesToDisplay - 1)
                {
                    if (findTournament.IndexOf(findTournamentToView.First()) != findTournament.Count - maxEntriesToDisplay)
                    {
                        var nextPlayer = findTournamentToView.ElementAt(1);
                        var startIndex = findTournament.IndexOf(nextPlayer);
                        findTournamentToView.Clear();
                        findTournamentToView = findTournament.GetRange(startIndex, maxEntriesToDisplay);
                    }
                }
                indexSelectedTournament++;
            }
            else if (keyFromUser.Key == ConsoleKey.UpArrow && indexSelectedTournament > 0)
            {
                if (findTournament.IndexOf(findTournamentToView.First()) != findTournament.IndexOf(findTournament.First()))
                {
                    var nextPlayer = findTournamentToView.First();
                    findTournamentToView.Clear();
                    findTournamentToView = findTournament.GetRange(findTournament.IndexOf(nextPlayer) - 1, maxEntriesToDisplay);
                }
                indexSelectedTournament--;
            }
            else if (keyFromUser.Key == ConsoleKey.Enter && findTournament.Any())
            {
                var findTournamentToSelect = findTournament.First(p => findTournament.IndexOf(p) == indexSelectedTournament);
                ConsoleService.WriteTitle(headTableToview);
                ConsoleService.WriteLineMessage($"{_tournamentsService.GetTournamentDetailView(findTournamentToSelect),107}");

                if (ConsoleService.AnswerYesOrNo("Selected Player"))
                {
                    return findTournamentToSelect;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Escape)
            {
                break;
            }
        } while (true);

        return null;
    }

    public void AllTournamentsView()
    {
        var allTournaments = _tournamentsService.GetAllItem().Where(t => t.IsActive == true).OrderByDescending(t => t.CreatedDateTime).ToList();
        var headTableToview = "All Tournaments" + $"\r\n{" LP",-5}{"ID",-6}{"Name",-21}" +
                   $"{"Game System",-16}{"Club Name",-21}{"Start",-15}{"End",-15}{"Players",-11}";

        if (allTournaments.Any())
        {
            ConsoleService.WriteTitle(headTableToview);

            foreach (var tournament in allTournaments)
            {
                var formmatStringToView = $" {allTournaments.IndexOf(tournament) + 1,-5}".Remove(5) + _tournamentsService.GetTournamentDetailView(tournament);

                ConsoleService.WriteLineMessage(formmatStringToView);
            }
        }
        else
        {
            ConsoleService.WriteLineErrorMessage("Empty List");
        }
        ConsoleService.WriteLineMessage("Press any key...");
        ConsoleService.GetKeyFromUser();
    }

    public void UpdateTournament(Tournament tournament)
    {
        _tournamentsService.UpdateItem(tournament);
        _tournamentsService.SaveList();
    }

    public void InterruptTournament(Tournament tournament)
    {
        _tournamentsService.InterruptTournament(tournament, _singlePlayerDuelManager);
    }

    public string GetTournamentDetailView(Tournament tournament)
    {
        return _tournamentsService.GetTournamentDetailView(tournament);
    }

    public void EndTournament(Tournament tournament)
    {
        _tournamentsService.EndTournament(tournament);
    }

    public void StartTournament(Tournament tournament)
    {
        _tournamentsService.StartTournament(tournament, _singlePlayerDuelManager);
    }

    //public string ViewPlayerToTournamentDetail(PlayerToTournament playerToTournament)
    //{
    //    string formatText = string.Empty;
    //    if (playerToTournament != null)
    //    {
    //        formatText = $"Position: {playerToTournament.Position}.  {playerToTournament.TinyFulName} ";
    //    }
    //    return formatText;
    //}
}