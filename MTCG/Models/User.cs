using System.Text.Json.Serialization;
using MTCG.Services;

namespace MTCG.Models
{

    public class User
    {
        [JsonInclude]
        public string Role { get; private set; }
        [JsonInclude]
        public string Username { get; private set; }
        [JsonInclude]
        public string Password { get; private set; }
        [JsonInclude]
        public int? Coins { get; private set; }
        public Stack Stack { get; private set; } = new Stack();
        public Deck Deck { get; private set; } = new Deck();
        [JsonInclude]
        public int? Elo { get; private set; }
        public string Token { get; private set; }

        public string Id { get; private set; }

        public StatisticService statistic { get; private set; } = new StatisticService();

        [JsonConstructor]
        private User() { }

        public User(string name, string password)
        {
            _setProperties(name, password);
        }
        public User(string name, string password, string role)
        {
            _setProperties(name, password);
            Role = role;
        }

        //public User(string name, string password, Stack stack)
        //{
        //    _setProperties(name, password);
        //    Stack = stack;
        //}

        private void _setProperties(string name, string password)
        {
            Username = name;
            Password = password;
            Coins = 20;
            Elo = 100;
        }

        public void SetId(string id)
        {
            Id = id;
        }
        public void AddWin(int points)
        {
            Elo += points;
            statistic.AddWin();
        }

        public void AddLosses(int points)
        {
            Elo = (Elo - points < 0) ? 0 : Elo - points;
            statistic.AddLosses();
        }

        public void AddDraw()
        {
            statistic.AddDraw();
        }

        public bool NoCardsLeft()
        {
            return CardsCount() == 0;
        }

        public int CardsCount()
        {
            return Stack.Cards.Count + Deck.Cards.Count;
        }

        public void SetToken(string token)
        {
            if (Guid.TryParse(token, out Guid parsedGuid)) Token = token;
        }

        public void SetCoins(int coins)
        {
            Coins = coins;
        }

        public void SetElo(int elo)
        {
            Elo = elo;
        }

        public void ChangePassword(string password)
        {
            Password = password;
        }

        public void ChangeUsername(string username)
        {
            Username = username;
        }

        public void AddCardToDeck(Card card)
        {
            Deck.Cards.Add(card);
        }

        public void SetDeck()
        {
            Stack.Cards.Sort((x, y) => y.Damage.CompareTo(x.Damage));
            int length = Deck.Cards.Capacity - Deck.Cards.Count;
            for (int i = 0; i < length && i < Stack.Cards.Count; i++)
            {
                Deck.Cards.Add(Stack.Cards[i]);
                Stack.Cards.Remove(Stack.Cards[i]);
            }
        }
    }
}
