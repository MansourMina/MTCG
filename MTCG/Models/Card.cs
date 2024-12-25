using MTCG.Models.Cards.MonsterCards;
using MTCG.Models.Cards.SpellCards;
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
        Spell,
        Monster
    }
    public class Card
    {
        [JsonInclude]
        public string Id { get; private set; }
        [JsonInclude]
        public string Name { get; protected set; }
        [JsonInclude]
        public int Damage { get; private set; }
        public ElementType ElementType { get; private set; }
        public CardType CardType { get; private set; }

        public Card(string name, int damage, ElementType elementType, CardType cardType, string id)
        {
            Name = name;
            Damage = damage;
            ElementType = elementType;
            Id = id;
            CardType = cardType;
        }

        public static ElementType GetElementTypeByName(string name) 
        {
            switch (name)
            {
                case nameof(Dragon):
                    return Dragon.ElementType;
                case nameof(FireElf):
                    return FireElf.ElementType;
                case nameof(Goblin):
                    return Goblin.ElementType;
                case nameof(Knight):
                    return Knight.ElementType;
                case nameof(Kraken):
                    return Kraken.ElementType;
                case nameof(Ork):
                    return Ork.ElementType;
                case nameof(WaterSpell):
                    return WaterSpell.ElementType;
                case nameof(Wizzard):
                    return Wizzard.ElementType;
                case nameof(WaterGoblin):
                    return WaterGoblin.ElementType;
                default:
                    return ElementType.Normal;

            }
        }

        public static CardType GetCardTypeByName(string name)
        {
            switch (name)
            {
                case nameof(Dragon):
                    return CardType.Monster;
                case nameof(FireElf):
                    return CardType.Monster;
                case nameof(Goblin):
                    return CardType.Monster;
                case nameof(Knight):
                    return CardType.Monster;
                case nameof(Kraken):
                    return CardType.Monster;
                case nameof(Ork):
                    return CardType.Monster;
                case nameof(WaterSpell):
                    return CardType.Spell;
                case nameof(Wizzard):
                    return CardType.Monster;
                case nameof(WaterGoblin):
                    return CardType.Monster;
                default:
                    return CardType.Spell;
            }
        }

        public void SetElementType(string type)
        {
            ElementType = GetElementTypeByName(type);
        }

        public void SetCardType(string type)
        {
            CardType = GetCardTypeByName(type);
        }

        [JsonConstructor]
        private Card() {
            
        }
    }
}
