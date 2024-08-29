using Manager.Domain.Common;

namespace Manager.Domain.Entity;

public class Tournament : BaseEntity
{
    public string Name { get; set; }
    public int IdClub { get; set; }
    public string GameplaySystem { get; set; }
    public int? IdLeague { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int NumberOfPlayer { get; set; }
    public int NumberOfTables { get; set; }
    public int NumberOfGroups {  get; set; }
}
