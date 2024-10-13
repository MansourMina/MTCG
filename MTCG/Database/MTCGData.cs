using MTCG.Models;

namespace MTCG.Database
{
    public static class MTCGData
    {
        public static List<User> Users { get; private set; } = new List<User>();

        public static User? getUser(string name)
        {
            foreach (var user in Users)
            {
                if (user.Username == name) return user;
            }
            return null;
        }

        public static void checkTest(string username, string password)
        {
            var user = getUser(username);
            bool isPasswordValid = BCrypt.Net.BCrypt.EnhancedVerify(password, user.Password);
            Console.WriteLine(isPasswordValid);
        }
        public static void addUser(User user)
        {
            Users.Add(user);
        }
    }
}
