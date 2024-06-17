namespace Manager.App.Concrete.Helpers;

public class GameplaySystem
{
    List<string> GameplaySystemsList { get; set; }
    public GameplaySystem()
    {
        GameplaySystemsList = new List<string>() { "group", "2ko" };
    }
}
