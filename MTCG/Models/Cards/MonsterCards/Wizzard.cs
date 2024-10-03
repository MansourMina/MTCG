using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.Models.Cards.MonsterCards
{
    public class Wizzard : MonsterCard
    {
        public Wizzard(int damage) : base(typeof(Wizzard).ToString(), damage, ElementType.Normal)
        {

        }
    }
}
