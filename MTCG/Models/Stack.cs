using MTCG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public class Stack
    {
        public List<Card> Cards { get; private set; }

        public void addCard(Card card)
        {
            Cards.Add(card);
        }

        public void Set(List<Card> cards)
        {
            for (int card = 0; card < cards.Count; card++)
                addCard(cards[card]);
        }

        public Stack()
        {
            Cards = new List<Card>();
        }

        public void removeCard(Card card)
        {
            Cards.Remove(card);
        }

        public Card? popRandomCard()
        {
            Random rnd = new Random();
            return Cards[rnd.Next(Cards.Count)];
            
        }

    }
}
