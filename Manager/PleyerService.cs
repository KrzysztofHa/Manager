using Manager.Helpers;

namespace Manager
{
    public class PleyerService
    {
        public List<Pleyer> Pleyers { get; set; }
        public PleyerService()
        {
            Pleyers = new List<Pleyer>();
        }

        public ConsoleKeyInfo AddNewPleyerView(MenuActionService menuActionCountry)
        {

            var countryList = menuActionCountry.GetMenuActionsByName("Country");
            bool noCountry = false;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("  Add New Pleyer\n");

                if (noCountry)
                {
                    noCountry = false;
                    Console.WriteLine("Action You entered does not exist\n");
                }
                else
                {
                    Console.WriteLine($"Press number key in menu options\n");
                }

                for (int i = 0; i < countryList.Count; i++)
                {
                    Console.WriteLine($"{countryList[i].Id}. {countryList[i].Name}");
                }

                Console.WriteLine("\n\n\n\n       Press q to back in Main menu");
                var operation = Console.ReadKey();
                int.TryParse(operation.KeyChar.ToString(), out int numberCountry);

                if (numberCountry <= countryList.Count && numberCountry > 0)
                {
                    return operation;
                }

                if (operation.Key == ConsoleKey.Q)
                {
                    return operation;
                }
            }
        }

        public int AddNewPleyer(ConsoleKeyInfo country)
        {
            if (country.Key != ConsoleKey.Q)
            {
                var newPleyer = new Pleyer();
                var countryPleyer = new Country();
                int.TryParse(country.KeyChar.ToString(), out int numberCountry);
                Console.Clear();

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Add New Pleyer\n");
                    Console.WriteLine("Enter Pleyer name:");
                    var pleyerName = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(pleyerName))
                    {
                        pleyerName = pleyerName.ToString().Trim();
                        pleyerName = pleyerName.ToLower();
                        for (int i = 0; i <= pleyerName.Length - 1; i++)
                        {
                            if (char.IsWhiteSpace(pleyerName[i]))
                            {
                                pleyerName = pleyerName.Remove(i);
                                break;

                            }
                        }
                        newPleyer.Name = pleyerName[0].ToString().ToUpper();
                        newPleyer.Name += pleyerName.Substring(1);
                        newPleyer.Country = countryPleyer.CountryList[numberCountry - 1];
                        if (Pleyers.Count == 0)
                        {
                            newPleyer.Id = 1;
                            Pleyers.Add(newPleyer);
                        }
                        else
                        {
                            newPleyer.Id = Pleyers.Count + 1;
                            Pleyers.Add(newPleyer);
                        }
                        return newPleyer.Id;
                    }
                }
            }
            else
            {               
                return 0;
            }


        }

        public void ListOfPleyersView()
        {
            Console.Clear();
            Console.WriteLine("List Of Pleyers");
            foreach (var pleyer in Pleyers)
            {
                Console.WriteLine($"{pleyer.Id}. {pleyer.Name} Country: {pleyer.Country}");
            }
            Console.WriteLine("\nBack to Main Menu Press any key");
            Console.ReadKey();
        }



    }
}
