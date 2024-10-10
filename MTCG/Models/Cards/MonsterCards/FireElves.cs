﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;

namespace MTCG.Models.Cards.MonsterCards
{
    public class FireElves : MonsterCard
    {
        public FireElves(int damage) : base(typeof(FireElves).ToString(), damage, ElementType.Fire)
        {

        }
    }
}