using Manager.App.Abstract;
using Manager.Consol.Concrete;

namespace Manager.Helpers;

public class InitializeUser
{
    private readonly IUserService _userService;
    public string UserName { get; }

    public InitializeUser(IUserService userService)
    {

        _userService = userService;
        UserName = Environment.UserName;
        SetActiveUser();
    }
    public void SetActiveUser()
    {
        string welcome = "When you run the application for the first time,\r\n" +
            "    You can set a name that will be displayed for you.\r\n" +
            "    You can change the Display Name at any time in the application settings.";
        ConsoleService.WriteTitle($"Hello {UserName}!");
        ConsoleService.WriteLineMessage(welcome);
        if (ConsoleService.AnswerYesOrNo("Want to change your display name now?"))
        {
            var displayName = ConsoleService.GetStringFromUser("Enter Display Name");
            if (string.IsNullOrEmpty(displayName))
            {
                ConsoleService.WriteLineErrorMessage("The display name has not been changed.");
                _userService.SetDisplayUserName(UserName);
            }
            else
            {
                _userService.SetDisplayUserName(displayName);
            }
        }
        else
        {
            _userService.SetDisplayUserName(UserName);
        }
    }
}
