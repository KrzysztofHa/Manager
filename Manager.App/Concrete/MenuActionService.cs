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
        AddItem(new MenuAction(4300, "Start Or Resume Tournament", "Tournaments"));
        AddItem(new MenuAction(4400, "All Tournaments", "Tournaments"));
        AddItem(new MenuAction(4500, "Delete Tournaments Not Implemented", "Tournaments"));

        AddItem(new MenuAction(4110, "Players Not implemented", "League"));
        AddItem(new MenuAction(4120, "League Ranking Not implemented", "League"));

        AddItem(new MenuAction(4310, "Add Players", "Start Tournament"));
        AddItem(new MenuAction(4320, "Delete Player", "Start Tournament"));
        AddItem(new MenuAction(4330, "Set Number Of Groups", "Start Tournament"));
        AddItem(new MenuAction(4340, "Edit Groups Not Implemented", "Start Tournament"));
        AddItem(new MenuAction(4350, "Chenge The Game System", "Start Tournament"));
        AddItem(new MenuAction(4360, "Random Selection Of Players", "Start Tournament"));
        AddItem(new MenuAction(4370, "Players", "Start Tournament"));
        AddItem(new MenuAction(4380, "Reset Not Implemented", "Start Tournament"));

        AddItem(new MenuAction(5100, "Change Display Name", "Settings"));

        for (int i = 0; i <= menuCountry.CountryList.Count - 1; i++)
        {
            AddItem(new MenuAction(i + 1, menuCountry.CountryList[i], "Country"));
        }
    }
}