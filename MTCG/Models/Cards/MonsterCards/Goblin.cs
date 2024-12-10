namespace MTCG.Models.Cards.MonsterCards
{
    public class Goblin : MonsterCard
    {
        public Goblin(int damage, string id) : base(typeof(Goblin).ToString(), damage, ElementType.Normal, id)
        {

        }
    }
}
