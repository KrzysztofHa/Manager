using Manager.Domain.Entity;

namespace Manager.App.Managers.Helpers;

public class PlayerToTournament
{
    public string TinyFulName { get; set; }
    public string Country { get; set; }
    public string Group { get; set; }
    public string TwoKO { get; set; }
    public string Round { get; set; }
    public int Position { get; set; }
    public int IdPLayer { get; set; }
    public string Cup { get; set; }

    public PlayerToTournament()
    {
    }

    public PlayerToTournament(Player player, string country)
    {
        TinyFulName = $"{player.FirstName}".Remove(1) + $".{player.LastName,-30}".Remove(29);
        IdPLayer = player.Id;
        Country = country;
    }
}