namespace MTCG.Models.Cards.MonsterCards
{
    public class Wizzard : MonsterCard
    {
        public static readonly ElementType ElementType = ElementType.Normal;
        public Wizzard(int damage, string id) : base(typeof(Wizzard).ToString(), damage, ElementType, id)
        {

        }
    }
}
