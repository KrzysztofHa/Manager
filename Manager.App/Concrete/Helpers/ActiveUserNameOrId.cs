using Manager.App.Abstract;
using Manager.App.Concrete;

namespace Manager.Helpers;

public static class ActiveUserNameOrId
{
    public static string NameActiveUser { get => GetName(); }
    public static int IdActiveUser { get => GetID(); }

    private static string GetName()
    {
        IUserService userService = new UserService();
        return userService.GetDisplayUserName();
    }

    private static int GetID()
    {
        IUserService userService = new UserService();
        return userService.GetIdActiveUser();
    }
}