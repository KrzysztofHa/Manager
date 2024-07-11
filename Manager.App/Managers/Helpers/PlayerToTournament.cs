using Manager.App.Concrete.Helpers;
using Manager.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Manager.App.Managers.Helpers;

public class PlayerToTournament
{

    public string TinyFulName { get; set; }
    public string Country { get; set; }
    public string Group { get; set; }

    public int IdPLayer { get; set; }
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
