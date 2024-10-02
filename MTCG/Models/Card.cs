using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public abstract class Card
    {
        public string Name { get; protected set; }
        public int Damage { get; }
        
        public ElementType ElementType { get; }

        public Card(string name, int damage, ElementType elementType)
        {
            Name = name;
            Damage = damage;
            ElementType = elementType;
        }
    }
}
