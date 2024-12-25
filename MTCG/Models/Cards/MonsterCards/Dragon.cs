﻿namespace MTCG.Models.Cards.MonsterCards
{
    public class Dragon : MonsterCard
    {
        public static readonly ElementType ElementType = ElementType.Fire;
        public Dragon(int damage, string id) : base(typeof(Dragon).ToString(), damage, ElementType, id)
        {

        }
    }
}
