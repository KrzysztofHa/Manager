using Manager.App.Common;
using Manager.App.Concrete.Helpers;
using Manager.Domain.Entity;

namespace Manager.App.Concrete
{
    public class MenuActionService : BaseService<MenuAction>
    {
        public MenuActionService()
        {
            Initialize();
        }

        public List<MenuAction> GetMenuActionsByName(string menuName)
        {
            List<MenuAction> result = new List<MenuAction>();
            foreach (var menuAction in Items)
            {
                if (menuAction.MenuName == menuName)
                    result.Add(menuAction);
            }

            return result;
        }

        private void Initialize()
        {
            var menuCountry = new Country();

            AddItem(new MenuAction(1000, "Players", "Main"));
            AddItem(new MenuAction(2000, "Global Ranking Not implemented ", "Main"));
            AddItem(new MenuAction(3000, "Sparing Not implemented", "Main"));
            AddItem(new MenuAction(4000, "Tournaments Not implemented", "Main"));
            AddItem(new MenuAction(5000, "Settings Not implemented", "Main"));

            AddItem(new MenuAction(1100, "List Of Player", "Players"));
            AddItem(new MenuAction(1200, "Search Player Not implemented", "Players"));
            AddItem(new MenuAction(1300, "Add Player", "Players"));
            AddItem(new MenuAction(1400, "Update Player", "Players"));
            AddItem(new MenuAction(1500, "Remove Player", "Players"));
            

            AddItem(new MenuAction(4100, "League Not implemented", "Tournaments"));
            AddItem(new MenuAction(4200, "One day Not implemented", "Tournaments"));

            AddItem(new MenuAction(4110, "Players Not implemented", "League"));
            AddItem(new MenuAction(4120, "Tournament Ranking Not implemented", "League"));

           

            for (int i = 0; i <= menuCountry.CountryList.Count - 1; i++)
            {
                AddItem(new MenuAction(i + 1, menuCountry.CountryList[i], "Country"));
            }
        }
    }
}
