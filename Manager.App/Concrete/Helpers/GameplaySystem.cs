﻿namespace Manager.App.Concrete.Helpers;

public class GameplaySystem
{
    public List<string> GameplaySystemsList { get; set; }
    public GameplaySystem()
    {
        GameplaySystemsList = new List<string>() { "Group", "2Ko" };
    }
}
