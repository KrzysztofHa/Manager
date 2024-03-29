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
            foreach (var menuAction in SomeItem)
            {
                if (menuAction.MenuName == menuName)
                    result.Add(menuAction);
            }

            return result;
        }

        private void Initialize()
        {
            var menuCountry = new Country();
            AddSomeItem(new MenuAction(1, "Add Pleyer", "Main"));
            AddSomeItem(new MenuAction(2, "Pleyers", "Main"));
            AddSomeItem(new MenuAction(3, "Remove Pleyer", "Main"));
            AddSomeItem(new MenuAction(4, "Update Pleyer", "Main"));
            AddSomeItem(new MenuAction(5, "Tournaments Not implemented", "Main"));
            AddSomeItem(new MenuAction(6, "Ranking Not implemented ", "Main"));
            AddSomeItem(new MenuAction(7, "Sparing Not implemented", "Main"));

            for (int i = 0; i <= menuCountry.CountryList.Count - 1; i++)
            {
                AddSomeItem(new MenuAction(i + 1, menuCountry.CountryList[i], "Country"));
            }
        }
    }
}
