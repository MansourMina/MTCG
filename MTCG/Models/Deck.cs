﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Deck
    {
        public List<Card> Cards { get; private set; }
        private const int DefaultSize = 4;

        public Deck(int deck_size = DefaultSize)
        {
            Cards = new List<Card>(deck_size);
        }

        public void AddCard(Card card)
        {
            if (Cards.Count >= Cards.Capacity) return;
            Cards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
        }

    }
}
