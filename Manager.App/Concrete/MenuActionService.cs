﻿using Manager.App.Common;
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
        AddItem(new MenuAction(4000, "Tournaments Not implemented", "Main"));
        AddItem(new MenuAction(5000, "Settings", "Main"));

        AddItem(new MenuAction(1100, "List Of Player", "Players"));
        AddItem(new MenuAction(1200, "Search Player", "Players"));
        AddItem(new MenuAction(1300, "Add Or Update Player", "Players"));
        AddItem(new MenuAction(1400, "Remove Player", "Players"));

        AddItem(new MenuAction(3100, "Start New Sparring", "Sparring"));
        AddItem(new MenuAction(3200, "All Sparring", "Sparring"));

        AddItem(new MenuAction(4100, "League Not implemented", "Tournaments"));
        AddItem(new MenuAction(4200, "One Day Not implemented", "Tournaments"));

        AddItem(new MenuAction(4110, "Players Not implemented", "League"));
        AddItem(new MenuAction(4120, "Tournament Ranking Not implemented", "League"));

        AddItem(new MenuAction(5100, "Change Display Name", "Settings"));

        for (int i = 0; i <= menuCountry.CountryList.Count - 1; i++)
        {
            AddItem(new MenuAction(i + 1, menuCountry.CountryList[i], "Country"));
        }
    }
}
