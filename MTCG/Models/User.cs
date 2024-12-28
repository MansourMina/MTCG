using System.Data;
using System.Text.Json.Serialization;
using MTCG.Database.Repositories;
using MTCG.Services;

namespace MTCG.Models
{

    public class User
    {
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

        public StatisticService Statistic { get; private set; } = new StatisticService();

        private readonly UserRepository _userRepository;
        private readonly StackRepository _stackRepository;
        private readonly DeckRepository _deckRepository;

        [JsonConstructor]
        private User() { }

        public User(string name, string password)
        {
            _setProperties(name, password);
            _userRepository = new UserRepository();
            _deckRepository = new DeckRepository();
            _stackRepository = new StackRepository();

        }
        public User(string name, string password, string role)
        {
            _setProperties(name, password);
            Role = role;
            _userRepository = new UserRepository();
            _deckRepository = new DeckRepository();
            _stackRepository = new StackRepository();
        }

        public User(string name, string password, string stack_id, string deck_id, string statistic_id)
        {
            _setProperties(name, password);
            Stack.SetId(stack_id);
            Deck.SetId(deck_id);
            Statistic.SetId(statistic_id);
            _userRepository = new UserRepository();
            _deckRepository = new DeckRepository();
            _stackRepository = new StackRepository();
        }

        public User(string name, string password, UserRepository userRepository, StackRepository stackRepository, DeckRepository deckRepository)
        {
            _setProperties(name, password);
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _stackRepository = stackRepository ?? throw new ArgumentNullException(nameof(stackRepository));
            _deckRepository = deckRepository ?? throw new ArgumentNullException(nameof(deckRepository));
        }

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
            Statistic.AddWin();
        }

        public void AddLosses(int points)
        {
            Elo = (Elo - points < 0) ? 0 : Elo - points;
            Statistic.AddLosses();
        }

        public void AddDraw()
        {
            Statistic.AddDraw();
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

        public void AcquirePackage(int costs, List<Card> cards)
        {
            Coins -= costs;
            Stack.Set(cards);
            _userRepository.UpdateUserCreds(Username, this);
        }

        

    }
}
