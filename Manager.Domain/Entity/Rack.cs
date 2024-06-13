using Manager.Domain.Common;

namespace Manager.Domain.Entity;
public class Rack : BaseEntity
{
    public int IdSinglePlayerDuel { get; set; }
    public bool IsBreakAndRun { get; set; }
    public int IdPlayerWinner { get; set; }
    public DateTime StartGame { get { return CreatedDateTime; } }
    public DateTime EndGame { get; set; }
}
