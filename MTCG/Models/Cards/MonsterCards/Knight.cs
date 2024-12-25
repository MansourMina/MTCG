namespace MTCG.Models.Cards.MonsterCards
{
    public class Knight : MonsterCard
    {
        public static readonly ElementType ElementType = ElementType.Normal;
        public Knight(int damage, string id) : base(typeof(Knight).ToString(), damage, ElementType, id)
        {

        }
    }
}
