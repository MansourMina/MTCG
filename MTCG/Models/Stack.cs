using MTCG.Database.Repositories;

namespace MTCG.Models
{
    public class Stack
    {
        public List<Card> Cards { get; private set; }
        public string Id { get; private set; }

        private readonly StackRepository _stackRepository;

        public void addCard(Card card)
        {
            Cards.Add(card);
        }

        public void Set(List<Card> cards)
        {
            for (int card = 0; card < cards.Count; card++)
                addCard(cards[card]);
        }

        public Stack(string id)
        {

            Cards = new List<Card>();
            Id = id;
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
            if(Cards.Count == 0) return null;

            Random rnd = new Random();
            var card = Cards[rnd.Next(Cards.Count)];
            Cards.Remove(card);
            return card;
        }

        public void SetId(string id)
        {
            Id = id;
        }


    }
}
