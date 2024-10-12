using Manager.App.Common;
using Manager.App.Concrete.Helpers;
using Manager.Domain.Entity;

namespace Manager.App.Concrete;

public class MenuActionService : BaseService<MenuAction>
{
    public MenuActionService()
    {
        Initialize();
    }

    public List<MenuAction> GetMenuActionsByName(string menuName)
    {
        List<MenuAction> result = new List<MenuAction>();
        result = GetAllItem().FindAll(p => p.MenuName == menuName);
        result.Add(new MenuAction(0, "Exit", menuName));
        return result;
    }

    private void Initialize()
    {
        var menuCountry = new Country();

        AddItem(new MenuAction(1000, "Manage Players", "Main"));
        AddItem(new MenuAction(2000, "Global Ranking Not implemented ", "Main"));
        AddItem(new MenuAction(3000, "Sparring", "Main"));
        AddItem(new MenuAction(4000, "Tournaments", "Main"));
        AddItem(new MenuAction(5000, "Settings", "Main"));

        AddItem(new MenuAction(1100, "List Of Player", "Players"));
        AddItem(new MenuAction(1200, "Search Player", "Players"));
        AddItem(new MenuAction(1300, "Add New Player", "Players"));
        AddItem(new MenuAction(1400, "Edit Player", "Players"));
        AddItem(new MenuAction(1500, "Remove Player", "Players"));

        AddItem(new MenuAction(3100, "Start New Sparring", "Sparring"));
        AddItem(new MenuAction(3200, "Continue The Interrupted Sparring", "Sparring"));
        AddItem(new MenuAction(3200, "All Sparring", "Sparring"));

        AddItem(new MenuAction(4100, "League Not implemented", "Tournaments"));
        AddItem(new MenuAction(4200, "Create New Tournament", "Tournaments"));
        AddItem(new MenuAction(4300, "Go To Tournament", "Tournaments"));
        AddItem(new MenuAction(4400, "All Tournaments", "Tournaments"));
        AddItem(new MenuAction(4500, "Delete Tournaments", "Tournaments"));

        AddItem(new MenuAction(4110, "Players Not implemented", "League"));
        AddItem(new MenuAction(4120, "League Ranking Not implemented", "League"));

        AddItem(new MenuAction(4310, "Add Players", "Go To Tournament"));
        AddItem(new MenuAction(4320, "Delete Player", "Go To Tournament"));
        AddItem(new MenuAction(4330, "Chenge Race To", "Go To Tournament"));
        AddItem(new MenuAction(4340, "Set Number Of Groups", "Go To Tournament"));
        AddItem(new MenuAction(4350, "Edit Groups Or 2KO List", "Go To Tournament"));
        AddItem(new MenuAction(4360, "Chenge The Game System", "Go To Tournament"));
        AddItem(new MenuAction(4370, "Random Selection Of Players", "Go To Tournament"));
        AddItem(new MenuAction(4380, "Players", "Go To Tournament"));
        AddItem(new MenuAction(4390, "Start Tournament", "Go To Tournament"));

        AddItem(new MenuAction(4341, "Move Player", "Edit Groups"));
        AddItem(new MenuAction(4342, "Reset Changes", "Edit Groups"));

        AddItem(new MenuAction(4391, "Start Duel", "Start Tournament"));
        AddItem(new MenuAction(4392, "Update Duel Result", "Start Tournament"));
        AddItem(new MenuAction(4393, "Group View", "Start Tournament"));
        AddItem(new MenuAction(4394, "All Duels", "Start Tournament"));
        AddItem(new MenuAction(4395, "Add Player", "Start Tournament"));
        AddItem(new MenuAction(4396, "Delete Player", "Start Tournament"));
        AddItem(new MenuAction(4397, "Move Player", "Start Tournament"));
        AddItem(new MenuAction(4398, "Change Race To", "Start Tournament"));

        AddItem(new MenuAction(5100, "Change Display Name", "Settings"));

        for (int i = 0; i <= menuCountry.CountryList.Count - 1; i++)
        {
            AddItem(new MenuAction(i + 1, menuCountry.CountryList[i], "Country"));
        }
    }
}