using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public class Card
    {
        public string Name { get; set; }
        public  int Damage;
        public string ElementType { get; }

        public Card(string name, int damage, string elementType) {
            Name = name;
            Damage = damage;
            ElementType = elementType;
        }
    }
}
