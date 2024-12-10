namespace MTCG.Models.Cards.MonsterCards
{
    public class Orks : MonsterCard
    {
        public Orks(int damage, string id) : base(typeof(Orks).ToString(), damage, ElementType.Normal, id)
        {

        }
    }
}
