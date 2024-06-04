using Manager.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Domain.Entity;

public class TeamPlayerMatch : PlayerMatch
{
    public List<Player> ListOfPlayerFirstTeam { get; set; }
    public List<Player> ListOfPlayerSecondTeam { get; set; }
    public int IdFirstTeam;
    public int IdSecondTeam;
    public int ScoreFirstTeam;
    public int ScoreSecondTeam;
}
