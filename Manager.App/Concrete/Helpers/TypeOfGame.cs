namespace Manager.App.Concrete.Helpers;

public class TypeOfGame
{
    public List<string> ListTypeOfGames { get; set; }
    public TypeOfGame()
    {
        ListTypeOfGames = new List<string>() { "8 balls", "9 balls", "10 balls" };
    }
}
