namespace Manager.Helpers
{
    public class LogIn
    {
        public static string UserName;
        public LogIn()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter You name:");
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
                    UserName = userName[0].ToString().ToUpper();
                    UserName += userName.Substring(1);
                    break;
                }
            }
        }
    }
}
