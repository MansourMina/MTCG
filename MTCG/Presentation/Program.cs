﻿using MTCG.Services;
using MTCG.Services.HTTP;
using System.Net;

namespace MTCG.Presentation
{
    struct user
    {
        public string Username;
        public string Password;
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            //       User leftPlayer = new User("Mina", "12344894");
            //       User rightPlayer = new User("Peter", "328190381");
            //;
            //       leftPlayer.Stack.Set(new List<Card>{
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new MonsterCard("Monsti1",40, ElementType.Fire),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire)


            //       });

            //       leftPlayer.Deck.Set(new List<Card>{
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new MonsterCard("Monsti1",40, ElementType.Fire),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire)
            //       });



            //       rightPlayer.Stack.Set(new List<Card>{
            //           new SpellCard("Speller1", 70, ElementType.Water),
            //           new MonsterCard("Monster1", 80, ElementType.Water),
            //           new SpellCard("Speller2", 100, ElementType.Water),
            //           new MonsterCard("Monser2", 98, ElementType.Water),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire),
            //           new SpellCard("Spelly1", 90, ElementType.Normal),
            //           new MonsterCard("Monsti1", 40, ElementType.Fire),
            //           new SpellCard("Spelly2", 200, ElementType.Water),
            //           new MonsterCard("Monsti2", 20, ElementType.Fire)

            //       });
            //       rightPlayer.Deck.Set(new List<Card>{
            //           new SpellCard("Speller1", 70, ElementType.Water),
            //           new MonsterCard("Monster1", 80, ElementType.Water),
            //           new SpellCard("Speller2", 100, ElementType.Normal),
            //           new MonsterCard("Monser2", 98, ElementType.Water)
            //       });

            //       Battle battle = new Battle(leftPlayer, rightPlayer); 
            //       battle.start();

            //Test Data
            List<Tuple<string, string>> Users = new List<Tuple<string, string>>
            {
                Tuple.Create("Mina", "Mina$2024"),
                Tuple.Create("JohnDoe", "JohnDoe!789"),
                Tuple.Create("AliceW", "Alice2023#"),
                Tuple.Create("BobSmith", "B0b$mith2024"),
                Tuple.Create("CathyBrown", "C@thy123"),
                Tuple.Create("DavidMiller", "David2024!"),
                Tuple.Create("EveJackson", "EveJ@ckson20"),
                Tuple.Create("FrankWhite", "Fr@nkW2023"),
                Tuple.Create("GraceHopper", "Grac3!Hopp"),
                Tuple.Create("HenryFord", "HenryF!123"),
                Tuple.Create("IsabellaJones", "Isa2024@Jones"),
                Tuple.Create("JamesBond", "J@mesB007"),
                Tuple.Create("KarenSmith", "K@ren!Smith"),
                Tuple.Create("LeoKing", "LeoK2023@"),
                Tuple.Create("MariaGarcia", "Maria!Gar123"),
                Tuple.Create("NathanDrake", "NathanD@rake"),
                Tuple.Create("OliviaRogers", "Olivia2024!"),
                Tuple.Create("PeterParker", "P@rk3r2023"),
                Tuple.Create("QuincyAdams", "Quincy@Adams"),
                Tuple.Create("RachelGreen", "RachelG!2024")
            };

            var registerService = new RegisterService();

            foreach (var user in Users)
                registerService.Register(user.Item1, user.Item2);

            // Start Server
            Server server = new Server(IPAddress.Any);
            server.Start();

        }
    }
}
