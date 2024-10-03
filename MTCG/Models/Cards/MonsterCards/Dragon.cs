using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.Models.Cards.MonsterCards
{
    public class Dragon : MonsterCard
    {
        public Dragon(int damage) : base(typeof(Dragon).ToString(), damage, ElementType.Fire)
        {

        }
    }
}
