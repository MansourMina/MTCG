namespace MTCG.Models.Cards.MonsterCards
{
    public class Wizzard : MonsterCard
    {
        public Wizzard(int damage, string id) : base(typeof(Wizzard).ToString(), damage, ElementType.Normal, id)
        {

        }
    }
}
