namespace MTCG.Models.Cards.MonsterCards
{
    public class Knights : MonsterCard
    {
        public Knights(int damage, string id) : base(typeof(Knights).ToString(), damage, ElementType.Normal, id)
        {

        }
    }
}
