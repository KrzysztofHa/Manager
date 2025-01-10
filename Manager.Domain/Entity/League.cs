namespace Manager.Domain.Entity
{
    public class League
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StrartLeague { get; set; }
        public DateTime EndLeague { get; set; }
        public int NumberOfManyTournament { get; set; }
    }
}