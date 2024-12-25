using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models.Cards.SpellCards
{
    internal class WaterSpell : SpellCard
    {
        public static readonly ElementType ElementType = ElementType.Water;
        public WaterSpell(int damage, string id) : base(typeof(WaterSpell).ToString(), damage, ElementType, id)
        {

        }
    }
}

