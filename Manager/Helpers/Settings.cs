using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Abstract;
using Manager.Consol.Concrete;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Helpers
{
    public class Settings
    {
        private string UserName;
        public Settings()
        {
            UserName = Environment.UserName;
        }

        public void ChangeSettings()
        {
            var settingMenu = new MenuActionService().GetMenuActionsByName("Settings");
            var consoleService = new ConsoleService();
            var userService = new UserService();

            for (int i = 0; i < settingMenu.Count; i++)
            {
                consoleService.WriteLineMessage($"{i + 1}. {settingMenu[i].Name}");
            }


            while (true)
            {
                consoleService.WriteTitle("Manage Players");
                for (int i = 0; i < settingMenu.Count; i++)
                {
                    consoleService.WriteLineMessage($"{i + 1}. {settingMenu[i].Name}");
                }

                var operation = consoleService.GetIntNumberFromUser("Enter Option");
                switch (operation)
                {
                    case 1:
                        ChangeDisplayName(consoleService, userService);
                        break;
                    case 2:
                        operation = null;
                        break;
                    case 3:
                        break;

                    case 6:
                       
                        break;
                    default:
                        if (operation != null)
                        {
                            consoleService.WriteLineErrorMessage("Enter a valid operation ID");
                        }
                        break;
                }


                if (operation == null)
                {
                    Console.Clear();
                    break;
                }
            }
        }
        private void ChangeDisplayName(ConsoleService consoleService, UserService userService)
        {
            var displayName = consoleService.GetStringFromUser("Enter Display Name");
            if (string.IsNullOrEmpty(displayName))
            {
                consoleService.WriteLineErrorMessage("The display name has not been changed.");
                userService.SetDisplayUserName(UserName);
            }
            else
            {
                userService.SetDisplayUserName(displayName);
            }
        }
    }
}
