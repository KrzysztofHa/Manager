using Manager.App.Concrete;
using Manager.Consol.Concrete;

namespace Manager.Helpers;

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
        var userService = new UserService();

        for (int i = 0; i < settingMenu.Count; i++)
        {
            ConsoleService.WriteLineMessage($"{i + 1}. {settingMenu[i].Name}");
        }


        while (true)
        {
            ConsoleService.WriteTitle("Manage Players");
            for (int i = 0; i < settingMenu.Count; i++)
            {
                ConsoleService.WriteLineMessage($"{i + 1}. {settingMenu[i].Name}");
            }

            var operation = ConsoleService.GetIntNumberFromUser("Enter Option");
            switch (operation)
            {
                case 1:
                    ChangeDisplayName(userService);
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
                        ConsoleService.WriteLineErrorMessage("Enter a valid operation ID");
                    }
                    break;
            }

            if (operation == null)
            {
                break;
            }
        }
    }
    private void ChangeDisplayName(UserService userService)
    {
        ConsoleService.WriteTitle($"Display Name: {userService.GetDisplayUserName()}");
        var displayName = ConsoleService.GetStringFromUser("Enter New Display Name:");
        if (string.IsNullOrEmpty(displayName))
        {
            ConsoleService.WriteLineErrorMessage("The display name has not been changed.");
            userService.SetDisplayUserName(UserName);
        }
        else
        {
            ConsoleService.WriteLineMessage($"Changed Display Name: {userService.SetDisplayUserName(displayName)}");
            ConsoleService.WriteLineErrorMessage("");
        }
    }
}
