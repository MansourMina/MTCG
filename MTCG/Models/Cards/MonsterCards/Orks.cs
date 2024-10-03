using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.Models.Cards.MonsterCards
{
    public class Orks : MonsterCard
    {
        public Orks(int damage) : base(typeof(Orks).ToString(), damage, ElementType.Normal)
        {

        }
    }
}
