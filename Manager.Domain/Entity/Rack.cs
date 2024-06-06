using Manager.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Domain.Entity;

public class Rack : BaseEntity
{
    public int IdFirstPlayer {  get; set; }
    public int IdSecondPlayer {  get; set; }
    public int IdDuel { get; set; }
    public bool IsBreakAndRun { get; set; }
    public int IdPlayerWinner { get; set; }
    public DateTime StartGame { get; set; }
    public DateTime EndGame { get; set; }
}
