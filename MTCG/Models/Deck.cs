using MTCG.Database.Repositories;
using MTCG.Services;

namespace MTCG.Models
{
    public class Deck
    {
        public List<Card> Cards { get; private set; }
        public string Id { get; private set; }
        private readonly DeckRepository _deckRepository;

        public Deck(int deck_size = BattleService.DeckSize)
        {
            Cards = new List<Card>(deck_size);
        }

        public void AddCard(Card card)
        {
                Cards.Add(card);
        }

        public void Set(List<Card> cards)
        {
            for (int card = 0; card < cards.Count; card++)
                AddCard(cards[card]);
        }

        public void SetId(string id)
        {
            Id = id;
        }
        public void removeCard(Card card)
        {
            if (card == null) 
                return;
            Cards.Remove(card);
        }

        public void Configure(List<string> cards_ids)
        {

        }
    }
}
