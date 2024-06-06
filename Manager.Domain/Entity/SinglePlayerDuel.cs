using Manager.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Manager.Domain.Entity;

public class SinglePlayerDuel : BaseEntity
{
    public DateTime StartGame { get; set; }
    public DateTime EndGame { get; set; }
    public int? IdPlayerMatch { get; set; }
    public int? IdPlayerTournament { get; set; }
    public int? ScoreFirstPlayer { get; set; }
    public int? ScoreSecondPlayer { get; set; }
}
