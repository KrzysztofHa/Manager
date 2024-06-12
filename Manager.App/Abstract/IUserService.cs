namespace Manager.App.Abstract;

public interface IUserService
{
    public int GetIdActiveUser();
    public int GeIdPlayerOfActiveUser();
    public string GetDisplayUserName();
    public string GetUserName();
    public string SetDisplayUserName(string displayName);
    public int SetIdPlayerToActiveUser(int idPlayerOfActiveUser);
}
