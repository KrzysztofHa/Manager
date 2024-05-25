using Manager.App.Abstract;
using Manager.App.Concrete;
using Manager.Consol.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Helpers;

public class InitializeUser
{
    private readonly IUserService _userService;
    private readonly IConsoleService _consoleService;
    public string UserName { get; }

    public InitializeUser(IUserService userService, IConsoleService consoleService)
    {
        _userService = userService;
        _consoleService = consoleService;
        UserName = Environment.UserName;
        SetActiveUser();
    }
    public void SetActiveUser()
    {        
        string welcome = "When you run the application for the first time,\r\n" +
            "    You can set a name that will be displayed for you.\r\n" +
            "    You can change the Display Name at any time in the application settings.";
        _consoleService.WriteTitle($"Hello {UserName}");
        _consoleService.WriteLineMessage(welcome);
        if (_consoleService.AnswerYesOrNo("Want to change your display name now?"))
        {
            var displayName = _consoleService.GetStringFromUser("Enter Display Name");
            if (string.IsNullOrEmpty(displayName))
            {
                _consoleService.WriteLineErrorMessage("The display name has not been changed.");
                _userService.SetDisplayUserName(UserName);
            }
            else
            {
                _userService.SetDisplayUserName(displayName);
            }
        }
    }
}
