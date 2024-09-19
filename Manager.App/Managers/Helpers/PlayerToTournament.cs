using Manager.Domain.Entity;
using System.Numerics;

namespace Manager.App.Managers.Helpers;

public class PlayerToTournament
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string TinyFulName { get { return $"{FirstName,-1}".Remove(1) + $".{LastName,-30}".Remove(29); } }
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
        FirstName = player.FirstName;
        LastName = player.LastName;        
        IdPLayer = player.Id;
        Country = country;
    }
}