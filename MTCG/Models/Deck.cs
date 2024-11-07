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

        

        public void removeCard(Card card)
        {
            if (card == null) 
                return;
            Cards.Remove(card);
        }

    }
}
