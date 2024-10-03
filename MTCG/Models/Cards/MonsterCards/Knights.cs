using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.Models.Cards.MonsterCards
{
    public class Knights : MonsterCard
    {
        public Knights(int damage) : base(typeof(Knights).ToString(), damage, ElementType.Normal)
        {

        }
    }
}
