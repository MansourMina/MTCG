using MTCG.Services;

namespace MTCG.Models
{
    public class Deck
    {
        public List<Card> Cards { get; private set; }
        public Deck(int deck_size = BattleService.DeckSize)
        {
            Cards = new List<Card>(deck_size);
        }

        public void addCard(Card card)
        {
            if (Cards.Count < Cards.Capacity)
            {
                Cards.Add(card);
                return;
            }
            for (int i = 0; i < Cards.Count; i++)
            {
                if (card.Damage > Cards[i].Damage)
                {
                    Cards[i] = card;
                    return;
                }
            }
        }

        public void Set(List<Card> cards)
        {
            int deckSpace = Cards.Capacity - Cards.Count;
            for (int card = 0; card < deckSpace && card < cards.Count; card++)
                addCard(cards[card]);
        }
        public void removeCard(Card card)
        {
            Cards.Remove(card);
        }

    }
}
