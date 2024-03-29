using Manager.App.Concrete;
using Manager.App.Concrete.Helpers;
using Manager.Domain.Entity;


namespace Manager.App.Managers
{
    public class PleyerManager
    {
        private readonly MenuActionService _actionService;
        private PleyerService _pleyerService;

        public PleyerManager(MenuActionService actionService)
        {
            _actionService = actionService;
            _pleyerService = new PleyerService();
        }

        public int AddNewPleyer()
        {
            var countryList = _actionService.GetMenuActionsByName("Country");
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
                    Console.WriteLine($"\n    Enter Country\n\n\nPress number key in menu options\n");
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
                    if (operation.Key != ConsoleKey.Q)
                    {
                        var newPleyer = new Pleyer();
                        var countryPleyer = new Country();
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
                                newPleyer.Id = _pleyerService.GetNextId();
                                _pleyerService.AddSomeItem(newPleyer);
                                return newPleyer.Id;
                            }
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }

                if (operation.Key == ConsoleKey.Q)
                {
                    return 0;
                }
            }
        }

        public int RemovePleyer()
        {            
            if (ListOfPleyers())
            {
                Console.WriteLine("\nPlease enter id for pleyer you want remove");
                var pleyerId = Console.ReadLine();
                int.TryParse(pleyerId, out int id);
                var pleyerToRemove = _pleyerService.SomeItem.FirstOrDefault(p => p.Id == id);
                if (pleyerToRemove != null)
                {
                    _pleyerService.RemoveSomeItem(pleyerToRemove);
                }
                else
                {
                    return 0;
                }
                return pleyerToRemove.Id;
            }
            else
            {
                return 0;
            }
        }
        public int UpdatePleyer()
        {

            if (ListOfPleyers())
            {
                Console.WriteLine("\nPlease enter id for pleyer you want update");
                var pleyerId = Console.ReadLine();
                if (int.TryParse(pleyerId, out int id))
                {
                    var pleyerToUpdate = _pleyerService.SomeItem.FirstOrDefault(p => p.Id == id);
                    if (pleyerToUpdate != null)
                    {
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine("Update Pleyer\n");
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
                                pleyerToUpdate.Name = pleyerName[0].ToString().ToUpper();
                                pleyerToUpdate.Name += pleyerName.Substring(1);
                                _pleyerService.UpdateSomeItem(pleyerToUpdate);
                                return pleyerToUpdate.Id;
                            }
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("Player no found");
                        Console.WriteLine("Press Any key");
                        Console.ReadKey();
                        return 0;
                    }
                }

                Console.Clear();
                Console.WriteLine("Player no found");
                Console.WriteLine("Press Any key");
                Console.ReadKey();
                return 0;
            }
            else
            {
                return 0;
            }
        }



        public bool ListOfPleyers()
        {
            if (_pleyerService.SomeItem.Any())
            {
                Console.Clear();
                Console.WriteLine("List Of Pleyers");
                foreach (var pleyer in _pleyerService.GetAllSomeItem())
                {
                    Console.WriteLine($"{pleyer.Id}. {pleyer.Name} Country: {pleyer.Country}");
                }
                return true;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Empty List");
                return false;
            }
        }
    }
}
