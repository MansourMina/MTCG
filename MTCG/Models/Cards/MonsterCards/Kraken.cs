namespace MTCG.Models.Cards.MonsterCards
{
    public class Kraken : MonsterCard
    {
        public Kraken(int damage, string id) : base(typeof(Kraken).ToString(), damage, ElementType.Water, id)
        {

        }
    }
}
