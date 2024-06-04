namespace Manager.App.Concrete.Helpers;

public class Country
{
    public List<string> CountryList { get; set; }

    public Country()
    {
        CountryList = new List<string>() { "England", "Poland", "Deutchland" };
    }
}
