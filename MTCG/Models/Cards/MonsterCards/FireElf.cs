namespace MTCG.Models.Cards.MonsterCards
{
    public class FireElf : MonsterCard
    {
        public static readonly ElementType ElementType = ElementType.Fire;
        public FireElf(int damage, string id) : base(typeof(FireElf).ToString(), damage, ElementType, id)
        {

        }
    }
}
