﻿using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.App.Managers.Helpers;
using Manager.App.Managers.Helpers.TypeOfGame;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;

using Manager.Helpers;
using System.Text;
using System.Text.RegularExpressions;


namespace Manager.App.Managers;

public class SinglePlayerDuelManager : ISinglePlayerDuelManager
{
    private readonly ISinglePlayerDuelService _singlePlayerDuelService = new SinglePlayerDuelService();

    private readonly IPlayerManager _playerManager;
    private readonly IPlayerService _playerService;

    public SinglePlayerDuelManager(IPlayerManager playerManager, IPlayerService playerService)
    {
        _playerManager = playerManager;

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

        return duel;
    }

    private void AddPlayerToDuel(SinglePlayerDuel duel)
    {
        IUserService userService = new UserService();
        ConsoleService.WriteTitle("Add Player");
        if (duel.IdFirstPlayer == 0 && ConsoleService.AnswerYesOrNo("User is the player?"))
        {
            var idUserOfPlayer = ActiveUserNameOrId.IdActiveUser;

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

            }
            userService.SetIdPlayerToActiveUser(idUserOfPlayer);
            duel.IdFirstPlayer = idUserOfPlayer;

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


        var secondPlayer = _playerManager.SearchPlayer();
        if (secondPlayer != null && duel.IdFirstPlayer == secondPlayer.Id)
        {
            while (duel.IdFirstPlayer == secondPlayer.Id)

            {
                ConsoleService.WriteLineErrorMessage("The selected player is already added.");
                ConsoleService.WriteTitle("Add Second Player\r\n Press Any Key..");
                ConsoleService.GetKeyFromUser();


                secondPlayer = _playerManager.SearchPlayer();
                if (secondPlayer == null)
                {
                    return;
                }
            }
        }

        if (secondPlayer == null)
        {
            secondPlayer = _playerManager.AddNewPlayer();
            if (secondPlayer == null)

            {
                return;
            }
        }

        duel.IdSecondPlayer = secondPlayer.Id;


        ConsoleService.WriteTitle("Add Second Player");
        ConsoleService.WriteLineMessageActionSuccess("Succes\r\nPress Any Key");
    }


    private bool AddSettingsRaceAndTypeGame(SinglePlayerDuel singlePlayerDuel)
    {
        string[] settings = ["Type Of Game", "Race To"];

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


    public SinglePlayerDuel NewTournamentSinglePlayerDuel(int idTournament, int idFirstPlayer, int idSecondPlayer, string round = "Eliminations")
    {
        SinglePlayerDuel templateSinglePlayerDuel = _singlePlayerDuelService.GetAllSinglePlayerDuel()
            .FirstOrDefault(d => d.Round == round && d.IdPlayerTournament == idTournament);
        var duel = new SinglePlayerDuel();

        if (templateSinglePlayerDuel == null)
        {
            duel = NewSingleDuel(idFirstPlayer, idSecondPlayer);
            duel.IdPlayerTournament = idTournament;
            duel.Round = round;
            _singlePlayerDuelService.CreateTournamentSinglePlayerDue(duel);
        }

        if (idFirstPlayer > 0 && (idSecondPlayer > 0 || idSecondPlayer == -1))
        {
            duel.TypeNameOfGame = templateSinglePlayerDuel.TypeNameOfGame;
            duel.RaceTo = templateSinglePlayerDuel.RaceTo;
            duel.Round = templateSinglePlayerDuel.Round;
            duel.IdPlayerTournament = idTournament;
            duel.IdFirstPlayer = idFirstPlayer;
            duel.IdSecondPlayer = idSecondPlayer;
            _singlePlayerDuelService.CreateTournamentSinglePlayerDue(duel);
        }

        return duel;
    }

    public void StartSingleDuel(SinglePlayerDuel duel)
    {
        if (duel.StartGame == DateTime.MinValue && duel.IdPlayerTournament == null)
        {
            duel.StartGame = DateTime.Now;
            duel.Interrupted = DateTime.MinValue;
            _singlePlayerDuelService.StartSinglePlayerDuel(duel);
        }
        else
        {
            if (duel.StartGame == DateTime.MinValue)
            {
                duel.StartGame = DateTime.Now;
            }
            duel.Interrupted = DateTime.MinValue;
            duel.Resume = DateTime.Now;
            _singlePlayerDuelService.UpdateSinglePlayerDuel(duel);
        }
    }

    public List<SinglePlayerDuel>? GetSinglePlayerDuelsByTournamentsOrSparrings(int idTournament = 0)
    {
        var listSinglesPlayerDuels = new List<SinglePlayerDuel>();
        if (idTournament == 0)
        {
            listSinglesPlayerDuels = _singlePlayerDuelService.GetAllSinglePlayerDuel()
                .Where(s => s.IdPlayerTournament == null && s.IsActive == true).ToList();

        }
        else
        {
            listSinglesPlayerDuels = _singlePlayerDuelService.GetAllSinglePlayerDuel()

             .Where(s => s.IdPlayerTournament == idTournament && s.IsActive == true).ToList();
        }
        return listSinglesPlayerDuels;
    }

    public string ConvertListSinglePlayerDuelsToText(List<SinglePlayerDuel> listSinglesPlayerDuels, string title = "Duels")
    {
        if (!listSinglesPlayerDuels.Any())
        {
            return string.Empty;
        }

        var listPlayers = _playerService.GetAllItem();
        var tally = listPlayers.Join(listSinglesPlayerDuels,
            player => player.Id,
            duel => duel.IdFirstPlayer,
        (player, duel) =>
        new
        {
            FirstPlayer = $"{player.FirstName} {player.LastName}",
            SecondPleyer = duel.IdFirstPlayer != -1 && duel.IdSecondPlayer == -1 ? "Free Win" : listPlayers.Where(p => p.Id == duel.IdSecondPlayer)
            .Select(n => new { FulNamen = n.FirstName + " " + n.LastName }).First().FulNamen,
            duel.NumberDuelOfTournament,

            duel.TypeNameOfGame,
            duel.RaceTo,
            duel.CreatedDateTime,
            duel.ScoreFirstPlayer,
            duel.ScoreSecondPlayer,
            duel.IdPlayerTournament,

            duel.Group,
            StartGame = duel.StartGame.Equals(DateTime.MinValue) ? "Waiting" : duel.StartGame.ToShortTimeString(),
            Inrerrupted = !duel.Interrupted.Equals(DateTime.MinValue) ? "Interrtupted" : "In Progress",
            EndGame = duel.EndGame.Equals(DateTime.MinValue) ? "----" : duel.EndGame.ToShortTimeString(),
        }).OrderBy(d => d.CreatedDateTime);

        string FormatToTextDuelsView = string.Empty;
        foreach (var duelView in tally)
        {
            var endGame = duelView.StartGame.Equals("Waiting") ? duelView.EndGame : duelView.Inrerrupted;
            endGame = !duelView.StartGame.Equals("Waiting") && !duelView.EndGame.Equals("----") ? duelView.EndGame : endGame;

            FormatToTextDuelsView += $"\r\nMatch: {duelView.NumberDuelOfTournament} Group: {duelView.Group} Type Game: {duelView.TypeNameOfGame}" +
            $" Race To: {duelView.RaceTo} " +
            $"Start Game: {duelView.StartGame} End Game: {endGame}" +
            $"\r\n{duelView.FirstPlayer,45} : {duelView.ScoreFirstPlayer}" +
            $"\r\n{duelView.SecondPleyer,45} : {duelView.ScoreSecondPlayer}";
        }

        return FormatToTextDuelsView;
    }

    public void VievAllSparring()
    {
        var singlePlayerDuels = GetSinglePlayerDuelsByTournamentsOrSparrings();
        var FormatToTextDuelsView = ConvertListSinglePlayerDuelsToText(singlePlayerDuels);
        if (string.IsNullOrEmpty(FormatToTextDuelsView) || singlePlayerDuels.Count == 0)
        {
            ConsoleService.WriteLineErrorMessage("List Empty");
            ConsoleService.GetKeyFromUser();
        }
        else
        {
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


    public void InterruptDuel(SinglePlayerDuel duel)
    {
        if (duel == null)
        {
            return;
        }
        else
        {
            _singlePlayerDuelService.InterruptDuel(duel);
        }
    }

    public SinglePlayerDuel? SelectInterruptedDuelBySparring(string title = "Sparring", string backText = " ")
    {
        List<SinglePlayerDuel> foundDuels = _singlePlayerDuelService.GetAllSinglePlayerDuel()
          .Where(d => d.Interrupted != DateTime.MinValue && d.IdPlayerTournament == null).ToList();
        return SelectDuel(foundDuels, title, backText);
    }

    public SinglePlayerDuel? SelectStartedDuelByTournament(int idTournament, string title = "Select Started Duels", string backText = " ")
    {
        List<SinglePlayerDuel> foundDuels = _singlePlayerDuelService.GetAllSinglePlayerDuel()
          .Where(d => d.Interrupted == DateTime.MinValue && !d.StartGame.Equals(DateTime.MinValue) && d.EndGame == DateTime.MinValue
          && d.IdPlayerTournament == idTournament).ToList();
        return SelectDuel(foundDuels, title, backText);
    }

    public SinglePlayerDuel? SelectDuelToStartByTournament(int idTournament, string title = "Select Interrupted Duels", string backText = " ")
    {
        List<SinglePlayerDuel> foundDuels = _singlePlayerDuelService.GetAllSinglePlayerDuel()
            .Where(d => d.IdPlayerTournament == idTournament
            && d.StartGame != DateTime.MinValue && d.Interrupted == DateTime.MinValue).ToList();

        return SelectDuel(_singlePlayerDuelService.GetAllSinglePlayerDuel().Where(d => d.IdPlayerTournament == idTournament).Except(foundDuels).ToList(), title, backText);
    }

    public void SearchDuel()
    {
        do
        {
            List<SinglePlayerDuel> foundDuels = _singlePlayerDuelService.SearchSinglePlayerDuel(" ");
            ConsoleService.WriteLineMessage(GetViewInTextOfDetailsDuel(SelectDuel(foundDuels)));
        }
        while (ConsoleService.GetKeyFromUser().Key == ConsoleKey.Escape);
    }

    public SinglePlayerDuel? SelectDuel(List<SinglePlayerDuel> singlePlayerDuels, string title = " ", string backText = " ")
    {
        StringBuilder inputString = new StringBuilder();
        List<string> findDuelsString = new List<string>();
        List<string> findDuelsStringTemp = new List<string>();
        int maxEntriesToDisplay = 15;
        List<string> findDuelsStringToView = new List<string>();
        List<SinglePlayerDuel> findDuelsTemp = new();
        findDuelsTemp.AddRange(singlePlayerDuels);

        if (!findDuelsTemp.Any())
        {
            ConsoleService.WriteLineErrorMessage("Empty List Of Duels");
            return null;
        }

        int indexSelectedDuel = 0;
        title = string.IsNullOrWhiteSpace(title) ? "Duels" : title;

        var headTableToview = title + $"\r\n    {" LP",-5}{"ID",-6}{"Tournament",-11}{"T.Game",-11}" +
                    $"{"Race",-6}{"First Player",-21}{"Second Player",-21}{"Start",-16}{"End",-16}";
        do
        {
            if (!findDuelsString.Any())
            {
                findDuelsStringToView.Clear();
                findDuelsStringTemp.Clear();
                foreach (var duel in findDuelsTemp)
                {
                    var duelString = _singlePlayerDuelService.GetSinglePlayerDuelDetailsView(duel);
                    if (duelString == string.Empty)
                    {
                        continue;
                    }
                    findDuelsStringTemp.Add(duelString);
                }
                findDuelsString.AddRange(findDuelsStringTemp);

                if (findDuelsString.Count >= maxEntriesToDisplay)
                {
                    findDuelsStringToView = findDuelsString.GetRange(0, maxEntriesToDisplay);
                }
                else
                {
                    findDuelsStringToView = findDuelsString;
                }
            }

            if (!findDuelsStringToView.Any())
            {
                ConsoleService.WriteLineErrorMessage("Not Found Duel");
            }
            else
            {
                ConsoleService.WriteTitle(headTableToview);

                foreach (var duelString in findDuelsStringToView)
                {
                    var formmatStringToView = findDuelsString.IndexOf(duelString) == indexSelectedDuel ?
                        "\r\n--> " + $"{findDuelsString.IndexOf(duelString) + 1,-5}".Remove(5) + duelString + $" <---\r\n" :
                        "    " + $"{findDuelsString.IndexOf(duelString) + 1,-5}".Remove(5) + duelString;

                    ConsoleService.WriteLineMessage(formmatStringToView);
                }
            }
            ConsoleService.WriteLineMessage($"\r\n------(Found {findDuelsString.Count} Duel)-------\r\n" + inputString.ToString());
            ConsoleService.WriteLineMessage(@"Enter string move UP or Down  and  press enter to Select");

            var keyFromUser = ConsoleService.GetKeyFromUser(backText);

            if (char.IsLetterOrDigit(keyFromUser.KeyChar))
            {
                if (findDuelsString.Count == 1 && !string.IsNullOrEmpty(inputString.ToString()))
                {
                    ConsoleService.WriteLineErrorMessage("No entries found !!!");
                }
                else
                {
                    inputString.Append(keyFromUser.KeyChar);

                    if (inputString.Length == 1)
                    {
                        List<SinglePlayerDuel> secondFindDuelsTemp = new();
                        secondFindDuelsTemp.AddRange(singlePlayerDuels);

                        if (secondFindDuelsTemp.Count == 1 &&
                            _singlePlayerDuelService.GetSinglePlayerDuelDetailsView(secondFindDuelsTemp.First()) != string.Empty)
                        {
                            findDuelsTemp.Clear();
                            findDuelsTemp = secondFindDuelsTemp;
                        }

                        if (secondFindDuelsTemp.Any())
                        {
                            findDuelsString.Clear();
                        }
                        else
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            ConsoleService.WriteLineErrorMessage("No entries found !!!");
                            findDuelsString.Clear();
                        }
                    }
                    else
                    {
                        findDuelsString = [.. findDuelsString.Where(p => p.ToLower().
                        Contains(inputString.ToString().ToLower())).OrderBy(i => i.Split(" ")[6])];
                        if (!findDuelsString.Any())
                        {
                            inputString.Remove(inputString.Length - 1, 1);
                            findDuelsString.AddRange([.. findDuelsStringTemp.Where(p => p.ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.Split(" ")[6])]);
                            ConsoleService.WriteLineErrorMessage("No entry found !!!");
                        }
                        findDuelsStringToView.Clear();
                        if (findDuelsString.Count >= maxEntriesToDisplay - 1)
                        {
                            findDuelsStringToView = findDuelsString.GetRange(0, maxEntriesToDisplay);
                        }
                        else
                        {
                            findDuelsStringToView = findDuelsString;
                        }
                        indexSelectedDuel = 0;
                    }
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Backspace && inputString.Length > 0)
            {
                inputString.Remove(inputString.Length - 1, 1);

                if (!string.IsNullOrEmpty(inputString.ToString()))
                {
                    findDuelsString = [.. findDuelsString.Where(p => p.ToLower().
                        Contains(inputString.ToString().ToLower())).OrderBy(i => i.Split(" ")[6])];
                    indexSelectedDuel = 0;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.DownArrow && indexSelectedDuel <= findDuelsString.Count - 2)
            {
                if (indexSelectedDuel >= maxEntriesToDisplay - 1)
                {
                    if (findDuelsString.IndexOf(findDuelsStringToView.First()) != findDuelsString.Count - maxEntriesToDisplay)
                    {
                        var nextDuel = findDuelsStringToView.ElementAt(1);
                        var startIndex = findDuelsString.IndexOf(nextDuel);
                        findDuelsStringToView.Clear();
                        findDuelsStringToView = findDuelsString.GetRange(startIndex, maxEntriesToDisplay);
                    }
                }
                indexSelectedDuel++;
            }
            else if (keyFromUser.Key == ConsoleKey.UpArrow && indexSelectedDuel > 0)
            {
                if (findDuelsString.IndexOf(findDuelsStringToView.First()) != findDuelsString.IndexOf(findDuelsString.First()))
                {
                    var nextDuel = findDuelsStringToView.First();
                    findDuelsStringToView.Clear();
                    findDuelsStringToView = findDuelsString.GetRange(findDuelsString.IndexOf(nextDuel) - 1, maxEntriesToDisplay);
                }
                indexSelectedDuel--;
            }
            else if (keyFromUser.Key == ConsoleKey.Enter && findDuelsString.Any())
            {
                var findDuelToSelect = findDuelsTemp.First(p => findDuelsTemp.IndexOf(p) == indexSelectedDuel);
                ConsoleService.WriteTitle(headTableToview);
                ConsoleService.WriteLineMessage($"{_singlePlayerDuelService.GetSinglePlayerDuelDetailsView(findDuelToSelect),108}");

                if (ConsoleService.AnswerYesOrNo("Selected Duel"))
                {
                    return findDuelToSelect;
                }
                indexSelectedDuel = 0;
            }
            else if (keyFromUser.Key == ConsoleKey.Escape)
            {
                break;
            }
        } while (true);

        return null;
    }

    public void RemoveTournamentDuel(Tournament tournament, int idPlayer)
    {
        _singlePlayerDuelService.RemoveAllTournamentDuelsOrByIdPlayer(tournament, idPlayer);
    }

    public string GetViewInTextOfDetailsDuel(SinglePlayerDuel duel)
    {
        return _singlePlayerDuelService.GetSinglePlayerDuelDetailsView(duel);
    }
}

