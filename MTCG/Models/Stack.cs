using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Stack
    {
        private List<Card> _cards;

        public void addCard(Card card)
        {
            _cards.Add(card);
        }

        public void removeCard(Card card)
        {
            _cards.Remove(card);
        }
    }
}
