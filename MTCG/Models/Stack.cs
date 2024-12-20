﻿namespace MTCG.Models
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
            //if (Cards.Count == 0) return null;
            //Card? highestCard = null;
            //foreach (Card card in Cards)
            //{
            //    if (highestCard == null || card.Damage > highestCard.Damage)
            //        highestCard = card;
            //}
            //removeCard(highestCard);
            //return highestCard;
            if(Cards.Count == 0) return null;

            Random rnd = new Random();
            var card = Cards[rnd.Next(Cards.Count)];
            Cards.Remove(card);
            return card;
        }

    }
}
