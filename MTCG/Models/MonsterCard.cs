using MTCG.Models.Cards.MonsterCards;

namespace MTCG.Models
{
    public class MonsterCard(string name, int damage, ElementType elementType, string id) : Card(name, damage, elementType, CardType.Monster, id)
    {
    }
}
