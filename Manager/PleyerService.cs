namespace Manager
{
    public class PleyerService
    {
        public List<Pleyer> Pleyers { get; set; }
        public PleyerService()
        {
            Pleyers = new List<Pleyer>();
        }

        public void AddNewPleyerView()
        {
            var newPleyer = new Pleyer();
            Console.Clear();
            Console.WriteLine("Add New Pleyer\n");
            while (true)
            {
                Console.WriteLine("Enter name:");
                var userName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(userName))
                {
                    userName = userName.ToString().Trim();
                    userName = userName.ToLower();
                    for (int i = 0; i <= userName.Length - 1; i++)
                    {
                        if (char.IsWhiteSpace(userName[i]))
                        {
                            userName = userName.Remove(i);
                            break;

                        }
                    }
                    newPleyer.Name = userName[0].ToString().ToUpper();
                    newPleyer.Name += userName.Substring(1);

                }

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
                break;
            }
        }

        public void ListOfPleyersView()
        {
            Console.Clear();
            Console.WriteLine("List Of Pleyers");
            foreach (var pleyer in Pleyers)
            {
                Console.WriteLine($"{pleyer.Id}. {pleyer.Name}");
            }
            Console.ReadKey();
        }



    }
}
