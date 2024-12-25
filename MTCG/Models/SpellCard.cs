namespace MTCG.Models
{
    public class SpellCard : Card
    {
        public SpellCard(string name, int damage, ElementType elementType, string id) : base(name, damage, elementType, CardType.Spell, id)
        {

        }
    }
}
