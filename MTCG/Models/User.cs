using System.Reflection.Metadata;
using System.Text.Json.Serialization;

namespace MTCG.Models
{

    public class User
    {
       
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
            Coins = 20;
            Elo = 100;
        }

        private void _setCredentials(string name, string password)
        {
            Username = name;
            Password = password;
        }

        public void AddWin(int points)
        {
            Elo += points;
            statistic.addWin();
        }

        public void AddLosses(int points)
        {
            Elo -= Elo - points <= 0 ? 0 : points;
            statistic.addLosses();
        }

        public void AddDraw()
        {
            statistic.addDraw();
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
            if (!IsPasswordHashed(password)) throw new UnauthorizedAccessException("Failed to change Password");
            Password = password;
        }

        private bool IsPasswordHashed(string password)
        {
            return password.StartsWith("$2a$") || password.StartsWith("$2b$") || password.StartsWith("$2y$");
        }

        internal void ChangeUsername(string username)
        {
            Username = username;
        }
    }
}
