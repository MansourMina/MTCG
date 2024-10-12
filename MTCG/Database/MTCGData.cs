using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Database
{
    public static class MTCGData
    {
        public static List<User> Users { get; private set; } = new List<User>
        {
            new User("Mina", "Mina$2024"),
            new User("JohnDoe", "JohnDoe!789"),
            new User("AliceW", "Alice2023#"),
            new User("BobSmith", "B0b$mith2024"),
            new User("CathyBrown", "C@thy123"),
            new User("DavidMiller", "David2024!"),
            new User("EveJackson", "EveJ@ckson20"),
            new User("FrankWhite", "Fr@nkW2023"),
            new User("GraceHopper", "Grac3!Hopp"),
            new User("HenryFord", "HenryF!123"),
            new User("IsabellaJones", "Isa2024@Jones"),
            new User("JamesBond", "J@mesB007"),
            new User("KarenSmith", "K@ren!Smith"),
            new User("LeoKing", "LeoK2023@"),
            new User("MariaGarcia", "Maria!Gar123"),
            new User("NathanDrake", "NathanD@rake"),
            new User("OliviaRogers", "Olivia2024!"),
            new User("PeterParker", "P@rk3r2023"),
            new User("QuincyAdams", "Quincy@Adams"),
            new User("RachelGreen", "RachelG!2024")
        };

        public static User? getUser(string name)
        {
            foreach(var user in Users)
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
