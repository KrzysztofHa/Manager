using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Concrete;

namespace Manager.Helpers;

public class Settings
{
    private readonly IUserService _userService;
    private string UserName;
    public Settings(IUserService userService)
    {
        _userService = userService;
        UserName = Environment.UserName;
    }
    public void ChangeSettings()
    {
        var settingMenu = new MenuActionService().GetMenuActionsByName("Settings");

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
                    ChangeDisplayName();
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
    private void ChangeDisplayName()
    {
        ConsoleService.WriteTitle($"Display Name: {_userService.GetDisplayUserName()}");
        var displayName = ConsoleService.GetStringFromUser("Enter New Display Name:");
        if (string.IsNullOrEmpty(displayName))
        {
            ConsoleService.WriteLineErrorMessage("The display name has not been changed.");
            _userService.SetDisplayUserName(UserName);
        }
        else
        {
            ConsoleService.WriteLineMessage($"Changed Display Name: {_userService.SetDisplayUserName(displayName)}");
            ConsoleService.WriteLineErrorMessage("");
        }
    }
}
