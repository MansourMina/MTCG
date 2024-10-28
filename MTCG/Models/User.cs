using System.Text.Json.Serialization;

namespace MTCG.Models
{

    public class User
    {
        [JsonInclude]
        public string Username { get; private set; }
        [JsonInclude]
        public string Password { get; private set; }
        public int Coins { get; private set; } = 20;
        public Stack Stack { get; private set; } = new Stack();
        public Deck Deck { get; private set; } = new Deck();
        public int Elo { get; private set; } = 100;
        public string Token { get; private set; }
        public Statistic statistic { get; private set; } = new Statistic();

        [JsonConstructor]
        private User() { }
        public User(string name, string password)
        {
            _setCredentials(name, password);
        }

        public User(string name, string password, Stack stack)
        {
            _setCredentials(name, password);
            Stack = stack;
        }

        private void _setCredentials(string name, string password)
        {
            Username = name;
            Password = password;
        }

        public void addWin(int points)
        {
            Elo += points;
            statistic.addWin();
        }

        public void addLosses(int points)
        {
            Elo -= Elo - points <= 0 ? 0 : points;
            statistic.addLosses();
        }

        public void addDraw()
        {
            statistic.addDraw();
        }

        public bool noCardsLeft()
        {
            return cardsCount() == 0;
        }

        public int cardsCount()
        {
            return Stack.Cards.Count + Deck.Cards.Count;
        }

        public void setToken(string token)
        {
            if (Guid.TryParse(token, out Guid parsedGuid)) Token = token;
        }

        public void setCoins(int coins)
        {
            Coins = coins;
        }

        public void setElo(int elo)
        {
            Elo = elo;
        }

    }
}
