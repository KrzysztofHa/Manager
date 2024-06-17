using Manager.Domain.Common;

namespace Manager.Domain.Entity;

public class SinglePlayerDuel : BaseEntity
{
    public DateTime StartGame { get; set; }
    public DateTime EndGame { get; set; }
    public int RaceTo { get; set; }
    public string TypeNameOfGame { get; set; }
    public int? IdPlayerTournament { get; set; }
    public int IdFirstPlayer { get; set; }
    public int IdSecondPlayer { get; set; }
    public int ScoreFirstPlayer { get; set; }
    public int ScoreSecondPlayer { get; set; }
}
