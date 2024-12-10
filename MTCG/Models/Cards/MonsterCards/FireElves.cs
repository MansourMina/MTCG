namespace MTCG.Models.Cards.MonsterCards
{
    public class FireElves : MonsterCard
    {
        public FireElves(int damage, string id) : base(typeof(FireElves).ToString(), damage, ElementType.Fire, id)
        {

        }
    }
}
