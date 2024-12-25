﻿namespace MTCG.Models.Cards.MonsterCards
{
    public class Goblin : MonsterCard
    {
        public static readonly ElementType ElementType = ElementType.Normal;

        public Goblin(int damage, string id) : base(typeof(Goblin).ToString(), damage, ElementType, id)
        {

        }
    }
}
