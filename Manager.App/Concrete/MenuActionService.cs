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

            AddItem(new MenuAction(1, "Players", "Main"));
            AddItem(new MenuAction(2, "Ranking Not implemented ", "Main"));
            AddItem(new MenuAction(3, "Sparing Not implemented", "Main"));
            AddItem(new MenuAction(4, "Tournaments Not implemented", "Main"));

            AddItem(new MenuAction(5, "List Of Player", "Players"));
            AddItem(new MenuAction(6, "Search Player NOt implemented", "Players"));
            AddItem(new MenuAction(7, "Add Player", "Players"));
            AddItem(new MenuAction(8, "Update Player", "Players"));
            AddItem(new MenuAction(9, "Remove Player", "Players"));

            AddItem(new MenuAction(10, "League Not implemented", "Tournaments"));
            AddItem(new MenuAction(11, "One day Not implemented", "Tournaments"));

            AddItem(new MenuAction(12, "Players Not implemented", "League"));
            AddItem(new MenuAction(13, "Ranking Not implemented", "League"));

            AddItem(new MenuAction(14, " Not implemented", "League"));
            AddItem(new MenuAction(15, " Not implemented", "OneDey"));

            for (int i = 0; i <= menuCountry.CountryList.Count - 1; i++)
            {
                AddItem(new MenuAction(i + 1, menuCountry.CountryList[i], "Country"));
            }
        }
    }
}
