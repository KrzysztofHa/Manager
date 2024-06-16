namespace Manager.App.Abstract;

public interface IUserService
{
    int GetIdActiveUser();
    int GeIdPlayerOfActiveUser();
    string GetDisplayUserName();
    string GetUserName();
    string SetDisplayUserName(string displayName);
    int SetIdPlayerToActiveUser(int idPlayerOfActiveUser);
}
