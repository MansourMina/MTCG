using System.Text.Json.Serialization;

namespace MTCG.Models
{
    public enum ElementType
    {
        Fire,
        Water,
        Normal
    }

    public enum CardType
    {
        SpellCard,
        MonsterCard
    }
    public class Card
    {
        [JsonInclude]
        public string Id { get; private set; }
        [JsonInclude]
        public string Name { get; protected set; }
        [JsonInclude]
        public int Damage { get; private set; }

        public ElementType ElementType { get; }

        public Card(string name, int damage, ElementType elementType, string id)
        {
            Name = name;
            Damage = damage;
            ElementType = elementType;
            Id = id;
        }

        [JsonConstructor]
        private Card() { }
    }
}
