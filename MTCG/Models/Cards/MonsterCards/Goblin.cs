namespace MTCG.Models.Cards.MonsterCards
{
    public class Goblin : MonsterCard
    {
        public Goblin(int damage) : base(typeof(Goblin).ToString(), damage, ElementType.Normal)
        {

        }
    }
}
