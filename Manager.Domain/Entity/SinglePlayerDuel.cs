using Manager.Domain.Common;

namespace Manager.Domain.Entity;

public class SinglePlayerDuel : BaseEntity
{
    public int NumberDuelOfTournament { get; set; }
    public int StartNumberInGroup { get; set; }
    public DateTime StartGame { get; set; }
    public DateTime EndGame { get; set; }
    public int RaceTo { get; set; }
    public string TypeNameOfGame { get; set; }
    public int? IdPlayerTournament { get; set; }
    public int IdFirstPlayer { get; set; }
    public int IdSecondPlayer { get; set; }
    public int ScoreFirstPlayer { get; set; }
    public int ScoreSecondPlayer { get; set; }
    public bool IsWalkOver { get; set; }
    public string Round { get; set; }
    public string Group { get; set; }
    public int NumberOfTable { get; set; }
    public DateTime Interrupted { get; set; }
    public DateTime Resume { get; set; }
}