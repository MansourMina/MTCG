using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services.Interfaces;

namespace MTCG.Services
{
    public class UserManager : IUserManager
    {
        private readonly ILoginService _loginService;
        private readonly IUserRepository _userRepository;
        private readonly IDeckRepository _deckRepository;
        private readonly IStackRepository _stackRepository;
        private readonly IStatisticRepository _statisticRepository;
        private readonly ICardRepository _cardRepository;

        public UserManager(ILoginService loginService, IUserRepository userRepository, IDeckRepository deckRepository, IStackRepository stackRepository, IStatisticRepository statisticRepository, ICardRepository cardRepository)
        {
            _loginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _deckRepository = deckRepository ?? throw new ArgumentNullException(nameof(deckRepository));
            _stackRepository = stackRepository ?? throw new ArgumentNullException(nameof(stackRepository));
            _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
            _statisticRepository = statisticRepository ?? throw new ArgumentNullException(nameof(statisticRepository));
        }

        public UserManager()
        {
            _loginService = new LoginService();
            _userRepository = new UserRepository();
            _cardRepository = new CardRepository();
            _deckRepository = new DeckRepository();
            _stackRepository = new StackRepository();
            _statisticRepository = new StatisticRepository();
        }

        public void Register(string name, string password)
        {
            var user = _userRepository?.GetByName(name.Trim());
            if (user != null) throw new InvalidOperationException("User already exists");
            if (string.IsNullOrWhiteSpace(name.Trim()) || string.IsNullOrWhiteSpace(password.Trim()))
                throw new ArgumentException("Username or password cannot be empty.");

            var newUser = new User(name, BCrypt.Net.BCrypt.EnhancedHashPassword(password));
            string new_user_id = _userRepository?.Create(newUser);

            // Create Stack for User
            string new_stack_id = Guid.NewGuid().ToString();
            _stackRepository.Create(new_stack_id, new_user_id);

            // Create Deck for User
            string new_deck_id = Guid.NewGuid().ToString();
            _deckRepository.Create(new_deck_id, new_user_id);

            // Create Statistic for User
            string new_statistic_id = Guid.NewGuid().ToString();
            _statisticRepository.Create(new_statistic_id, new_user_id);

        }

        public ILoginService GetLoginService() => _loginService;
        public IUserRepository GetUserRepository() => _userRepository;

        public User GetUserByName(string username)
        {
            User user = _userRepository.GetByName(username.Trim());
            List<Card> cards = _cardRepository.GetStackCards(user.Stack.Id);
            List<Card> deck = _cardRepository.GetDeckCards(user.Deck.Id);
            user.Stack.Set(cards);
            user.Deck.Set(deck);
            return user;
        }

        public void ConfigureUserDeck(List<string> card_ids, User user)
        {
            if (card_ids == null || card_ids.Count == 0)
                return;
            var cardIdSet = new HashSet<string>(card_ids);
            var foundCards = user.Stack.Cards.Where(card => card_ids.Contains(card.Id)).ToList();

            if (foundCards.Count == 0) throw new KeyNotFoundException("No cards were found in the stack");

            int cards_added = 0;
            foreach (var card in foundCards)
            {
                if (user.Deck.Cards.Count >= user.Deck.Cards.Capacity)
                    throw new ArgumentException($"Deck capacity reached. {cards_added} new cards added.");
                _deckRepository.AddCard(Guid.NewGuid().ToString(), card.Id, user.Deck.Id);
                cards_added++;
            }
        }
    }
}
