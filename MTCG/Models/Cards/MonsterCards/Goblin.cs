using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.Models.Cards.MonsterCards
{
    public class Goblin : MonsterCard
    {
        public Goblin(int damage) : base(typeof(Goblin).ToString(), damage, ElementType.Normal)
        {

        }
    }
}
