namespace MTCG.Models.Cards.MonsterCards
{
    public class Kraken : MonsterCard
    {
        public static readonly ElementType ElementType = ElementType.Water;
        public Kraken(int damage, string id) : base(typeof(Kraken).ToString(), damage, ElementType, id)
        {

        }
    }
}
