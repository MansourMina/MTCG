using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public class SpellCard: Card
    {
        public SpellCard(string name, int damage, string elementType) : base(name, damage, elementType)
        {

        }
    }
}
