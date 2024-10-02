using MTCG.Models;
using MTCG.Services;

namespace MTCG.Presentation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            User user1 = new User("Mina", "dsadsadsa");
            User user2 = new User("Mansour", "dsadsadsa");
     ;
            user1.setDeck(new List<Card>{
                new SpellCard("1Card1", 90, ElementType.Normal),
                new MonsterCard("1Card2",40, ElementType.Fire),
                new SpellCard("1Card3", 200, ElementType.Normal),
                new MonsterCard("1Card4", 20, ElementType.Fire)
            });
            user2.setDeck(new List<Card>{
                new SpellCard("2Card1", 70, ElementType.Water),
                new MonsterCard("2Card2", 80, ElementType.Water),
                new SpellCard("2Card3", 100, ElementType.Normal),
                new MonsterCard("2Card4", 98, ElementType.Water)
            });
            Battle battle = new Battle(user1, user2); 
            Battle battle2 = new Battle(user1, user2);
            battle.start();

        }
    }
}
