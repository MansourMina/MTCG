namespace MTCG.Models.Cards.MonsterCards
{
    public class Ork : MonsterCard
    {
        public static readonly ElementType ElementType = ElementType.Normal;
        public Ork(int damage, string id) : base(typeof(Ork).ToString(), damage, ElementType, id)
        {

        }
    }
}
