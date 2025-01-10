using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System.Text;

namespace Manager.App.Managers;

public class ClubManager : IClubManager
{
    private readonly IClubService _clubService = new ClubService();
    private readonly MenuActionService _actionService;

    public ClubManager(MenuActionService actionService)
    {
        _actionService = actionService;
    }

    public Club SearchClub(string title)
    {
        List<Club> findClubsList = _clubService.SearchClub(" ");

        if (!findClubsList.Any())
        {
            return null;
        }

        StringBuilder inputString = new StringBuilder();
        List<Club> findClubsTemp = new List<Club>();
        var address = new Address();
        int IdSelectedCub = 0;
        title = string.IsNullOrEmpty(title) ? "Search Club" : title;

        do
        {
            ConsoleService.WriteTitle(title);

            if (findClubsList.Any())
            {
                ConsoleService.WriteTitle(title + $"\r\n{"ID",-6}{"Club Name",-21}");

                foreach (var club in findClubsList)
                {
                    var formmatStringToView = findClubsList.IndexOf(club) == IdSelectedCub ?
                        ">" + club.Name + " < Select Press Enter" :
                        " " + club.Name;

                    ConsoleService.WriteLineMessage(formmatStringToView);
                }
            }
            else
            {
                ConsoleService.WriteLineErrorMessage("Not Found Club");
            }
            ConsoleService.WriteLineMessage("---------------\r\n" + inputString.ToString());
            ConsoleService.WriteLineMessage(@"Enter string, move /\ or \/  and ENTER Select or ESC add new entry ");

            var keyFromUser = ConsoleService.GetKeyFromUser();

            if (char.IsLetterOrDigit(keyFromUser.KeyChar))
            {
                inputString.Append(keyFromUser.KeyChar);

                if (inputString.Length == 1)
                {
                    findClubsTemp = _clubService.SearchClub(inputString.ToString());

                    if (findClubsTemp.Any())
                    {
                        findClubsList.Clear();
                        findClubsList.AddRange(findClubsTemp);
                        IdSelectedCub = 0;
                    }
                    else
                    {
                        inputString.Remove(inputString.Length - 1, 1);
                        ConsoleService.WriteLineErrorMessage("No entry found !!!");
                    }
                }
                else
                {
                    findClubsList = [.. findClubsList.Where(c => $"{c.Id} {c.Name}".ToLower().
                        Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                    if (!findClubsList.Any())
                    {
                        inputString.Remove(inputString.Length - 1, 1);
                        findClubsList.AddRange([.. findClubsTemp.Where(c => $"{c.Id} {c.Name}".ToLower().
                            Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)]);
                        ConsoleService.WriteLineErrorMessage("No entry found !!!");
                    }
                    IdSelectedCub = 0;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Backspace && inputString.Length > 0)
            {
                inputString.Remove(inputString.Length - 1, 1);

                if (!string.IsNullOrEmpty(inputString.ToString()))
                {
                    findClubsList = [.. findClubsTemp.Where(c => $"{c.Id} {c.Name}".ToLower()
                    .Contains(inputString.ToString().ToLower())).OrderBy(i => i.Name)];
                }
            }
            else if (keyFromUser.Key == ConsoleKey.DownArrow && IdSelectedCub < findClubsList.Count - 1)
            {
                IdSelectedCub++;
            }
            else if (keyFromUser.Key == ConsoleKey.UpArrow && IdSelectedCub > 0)
            {
                IdSelectedCub--;
            }
            else if (keyFromUser.Key == ConsoleKey.Enter && findClubsList.Any())
            {
                var findClubToSelect = findClubsList.First(p => findClubsList.IndexOf(p) == IdSelectedCub);
                ConsoleService.WriteLineMessage(findClubToSelect.Name);

                if (ConsoleService.AnswerYesOrNo("Selected Club"))
                {
                    return findClubToSelect;
                }
            }
            else if (keyFromUser.Key == ConsoleKey.Escape)
            {
                break;
            }
        } while (true);

        return null;
    }

    public Club AddNewClub()
    {
        return AddOrUpdateClub();
    }

    public void UpdateClub()
    {
        AddOrUpdateClub(true);
    }

    private Club AddOrUpdateClub(bool isUpdateClub = false)
    {
        Club updateClub = new Club();
        Address playerAddress = new Address();
        string title = string.Empty;

        if (isUpdateClub)
        {
            title = "Update Club";
            isUpdateClub = true;
            updateClub = _clubService.GetItemById(SearchClub("").Id);
            if (updateClub == null)
            {
                return updateClub;
            }
        }
        else
        {
            title = "Add New Club";
        }
        string[] property = ["Name", "Street", "Building Number", "City", "Country", "Zip"];

        string updateString;
        foreach (var propertyItem in property)
        {
            var formatAddressToView = $" {playerAddress.Street,-10}".Remove(11) + $" {playerAddress.BuildingNumber,-10}".Remove(11) +
                 $" {playerAddress.City,-10}".Remove(11) + $" {playerAddress.Country,-10}".Remove(11) +
                    $" {playerAddress.Zip,-5}".Remove(6);
            var formatClubDataToView = $"{updateClub.Id,-5}".Remove(5) + $" {updateClub.Name,-20}".Remove(21) + formatAddressToView;

            ConsoleService.WriteTitle($"{title}\r\n{"ID",-6}{"Name",-21}" +
                   $"{"Street",-11}{"Number",-11}{"City",-11}{"Country",-11}{"zip",-6}");

            ConsoleService.WriteLineMessage($"{formatClubDataToView,-92}");

            if (propertyItem != "Country")
            {
                if (isUpdateClub)
                {
                    updateString = ConsoleService.GetStringFromUser(propertyItem);
                }
                else
                {
                    updateString = ConsoleService.GetRequiredStringFromUser(propertyItem);
                    if (string.IsNullOrEmpty(updateString))
                    {
                        return null;
                    }
                }

                if (string.IsNullOrEmpty(updateString))
                {
                    if (updateString == null)
                    {
                        return null;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    if (propertyItem == "Name")
                    {
                        updateClub.Name = updateString;
                    }
                    else if (propertyItem == "City")
                    {
                        playerAddress.City = updateString.ToLower();
                    }
                    else if (propertyItem == "Street")
                    {
                        playerAddress.Street = updateString.ToLower();
                    }
                    else if (propertyItem == "Zip")
                    {
                        playerAddress.Zip = updateString.ToLower();
                    }
                    else if (propertyItem == "Building Number")
                    {
                        playerAddress.BuildingNumber = updateString.ToLower();
                    }
                }
            }
            else
            {
                var countryListMenu = _actionService.GetMenuActionsByName("Country");
                do
                {
                    ConsoleService.WriteTitle(title);
                    for (int i = 0; i < countryListMenu.Count; i++)
                    {
                        ConsoleService.WriteLineMessage($"{i + 1}. {countryListMenu[i].Name}");
                    }

                    var inputInt = ConsoleService.GetIntNumberFromUser("Country");
                    if (inputInt <= countryListMenu.Count)
                    {
                        var countryName = countryListMenu.FirstOrDefault(p => p.Name == countryListMenu[(int)inputInt - 1].Name);
                        if (countryName != null && !countryName.Name.Contains("Exit"))
                        {
                            playerAddress.Country = countryName.Name;
                        }
                        else if (countryName.Name.Contains("Exit"))
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (inputInt == null && isUpdateClub)
                        {
                            if (ConsoleService.AnswerYesOrNo("You want to exit? \r\nThe entered data will not be saved"))
                            {
                                return null;
                            }
                        }
                        ConsoleService.WriteLineErrorMessage($"No option nr: " + inputInt);
                    }
                } while (playerAddress.Country == null);
            }
        }
        if (isUpdateClub)
        {
            _clubService.UpdateItem(_clubService.AddClubAddress(updateClub, playerAddress));
            _clubService.SaveList();
        }
        else
        {
            _clubService.AddItem(_clubService.AddClubAddress(updateClub, playerAddress));
            _clubService.SaveList();
        }
        ConsoleService.WriteLineMessage(_clubService.GetClubDetailView(updateClub));
        ConsoleService.WriteLineMessageActionSuccess($"Data of Club has been update");

        return updateClub;
    }
}