namespace Manager.Infrastructure.Entity
{
    public class BasePaths
    {
        public int Id { get; set; }
        public string PathName { get; set; }
        public string PathToFile { get; set; }
        public bool  IsActive { get; set; }
        public string UserName { get; set; }

    }
}
