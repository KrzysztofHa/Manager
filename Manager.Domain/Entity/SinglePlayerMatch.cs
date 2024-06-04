using Manager.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Manager.Domain.Entity;

public class SinglePlayerMatch : BaseEntity
{
    public int IdFirstPlayer { get; set; }
    public int ScoreFirstPlayer { get; set; }
    public int IdSecondPlayer { get; set; }
    public int ScoreSecondPlayer { get; set; }
}
