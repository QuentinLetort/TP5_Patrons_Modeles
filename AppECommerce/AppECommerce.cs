using System;
using UserSDK;

namespace AppECommerce
{
    class AppECommerce
    {
        static void Main(string[] args)
        {
            User user = GetUser();
            Console.WriteLine($"Bienvenue {user.FirstName} {user.LastName}");
        }
        private static User GetUser()
        {
            User result = null;
            while (result == null)
            {
                Console.Write("Username: ");
                string input = Console.ReadLine();
                result = User.GetUser(input);
                if (result == null)
                {
                    Console.WriteLine("This user doesn't exist");
                }
            }
            return result;
        }
    }
}