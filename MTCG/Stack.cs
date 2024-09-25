using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG
{
    public class Stack
    {
        private List<Card> cards;

        public void addCard(Card card)
        {
            cards.Add(card);
        }

        public void removeCard(Card card)
        {
            cards.Remove(card);
        }
    }
}
