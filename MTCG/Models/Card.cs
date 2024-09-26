using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public enum ELEMENTTYPE
    {
        Fire,
        Water,
        Normal
    }
    public abstract class Card
    {
        public string Name { get; set; }
        public int Damage
        {
            get;
        }
        
        public ELEMENTTYPE ElementType { get; }

        public Card(string name, int damage, ELEMENTTYPE elementType)
        {
            Name = name;
            Damage = damage;
            ElementType = elementType;
        }
    }
}
