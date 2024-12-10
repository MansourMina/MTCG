namespace MTCG.Models.Cards.MonsterCards
{
    public class Dragon : MonsterCard
    {
        public Dragon(int damage, string id) : base(typeof(Dragon).ToString(), damage, ElementType.Fire, id)
        {

        }
    }
}
