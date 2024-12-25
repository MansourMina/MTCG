using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models.Cards.MonsterCards
{
    public class WaterGoblin: MonsterCard
    {
        public static readonly ElementType ElementType = ElementType.Water;

        public WaterGoblin(int damage, string id) : base(typeof(WaterGoblin).ToString(), damage, ElementType, id)
        {

        }
    }
}
