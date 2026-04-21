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
        AddItem(new MenuAction(2000, "Sparring", "Main"));
        AddItem(new MenuAction(3000, "Tournaments", "Main"));
        AddItem(new MenuAction(4000, "Settings", "Main"));

        AddItem(new MenuAction(1100, "List Of Player", "Players"));
        AddItem(new MenuAction(1200, "Search Player", "Players"));
        AddItem(new MenuAction(1300, "Add New Player", "Players"));
        AddItem(new MenuAction(1400, "Edit Player", "Players"));
        AddItem(new MenuAction(1500, "Remove Player", "Players"));

        AddItem(new MenuAction(2100, "Start New Sparring", "Sparring"));
        AddItem(new MenuAction(2200, "Continue The Interrupted Sparring", "Sparring"));
        AddItem(new MenuAction(2200, "All Sparring", "Sparring"));

        AddItem(new MenuAction(3100, "Create New Tournament", "Tournaments"));
        AddItem(new MenuAction(3200, "Go To Tournament", "Tournaments"));
        AddItem(new MenuAction(3300, "All Tournaments", "Tournaments"));
        AddItem(new MenuAction(3400, "Delete Tournaments", "Tournaments"));

        AddItem(new MenuAction(4100, "Change Display Name", "Settings"));

        for (int i = 0; i <= menuCountry.CountryList.Count - 1; i++)
        {
            AddItem(new MenuAction(i + 1, menuCountry.CountryList[i], "Country"));
        }
    }
}