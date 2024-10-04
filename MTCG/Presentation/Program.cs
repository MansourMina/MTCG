using MTCG.Models;
using MTCG.Services;

namespace MTCG.Presentation
{
    internal class Program
    {
        static void Main(string[] args)
        {
            User leftPlayer = new User("Mina", "12344894");
            User rightPlayer = new User("Peter", "328190381");
     ;
            leftPlayer.Stack.Set(new List<Card>{
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new MonsterCard("Monsti1",40, ElementType.Fire),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new MonsterCard("Monsti2", 20, ElementType.Fire),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new MonsterCard("Monsti2", 20, ElementType.Fire),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new MonsterCard("Monsti2", 20, ElementType.Fire),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new MonsterCard("Monsti2", 20, ElementType.Fire),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new MonsterCard("Monsti2", 20, ElementType.Fire)

               
            });

            leftPlayer.Deck.Set(new List<Card>{
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new MonsterCard("Monsti1",40, ElementType.Fire),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new MonsterCard("Monsti2", 20, ElementType.Fire)
            });



            rightPlayer.Stack.Set(new List<Card>{
                new SpellCard("Speller1", 70, ElementType.Water),
                new MonsterCard("Monster1", 80, ElementType.Water),
                new SpellCard("Speller2", 100, ElementType.Water),
                new MonsterCard("Monser2", 98, ElementType.Water),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new MonsterCard("Monsti2", 20, ElementType.Fire),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new MonsterCard("Monsti2", 20, ElementType.Fire),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new MonsterCard("Monsti2", 20, ElementType.Fire),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new MonsterCard("Monsti2", 20, ElementType.Fire),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new MonsterCard("Monsti2", 20, ElementType.Fire),
                new SpellCard("Spelly1", 90, ElementType.Normal),
                new MonsterCard("Monsti1", 40, ElementType.Fire),
                new SpellCard("Spelly2", 200, ElementType.Water),
                new MonsterCard("Monsti2", 20, ElementType.Fire)

            });
            rightPlayer.Deck.Set(new List<Card>{
                new SpellCard("Speller1", 70, ElementType.Water),
                new MonsterCard("Monster1", 80, ElementType.Water),
                new SpellCard("Speller2", 100, ElementType.Normal),
                new MonsterCard("Monser2", 98, ElementType.Water)
            });

            Battle battle = new Battle(leftPlayer, rightPlayer); 
            battle.start();

        }
    }
}
