using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.Models.Cards.MonsterCards
{
    public class Kraken : MonsterCard
    {
        public Kraken(int damage) : base(typeof(Kraken).ToString(), damage, ElementType.Water)
        {

        }
    }
}
