﻿using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services.Interfaces;
using System.Drawing;

namespace MTCG.Services
{
    public class UserManager : IUserManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IDeckRepository _deckRepository;
        private readonly IStackRepository _stackRepository;
        private readonly IStatisticRepository _statisticRepository;
        private readonly ICardRepository _cardRepository;
        private readonly ISessionRepository _sessionRepository;

        public UserManager(IUserRepository userRepository, IDeckRepository deckRepository, IStackRepository stackRepository, IStatisticRepository statisticRepository, ICardRepository cardRepository, ISessionRepository sessionRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _deckRepository = deckRepository ?? throw new ArgumentNullException(nameof(deckRepository));
            _stackRepository = stackRepository ?? throw new ArgumentNullException(nameof(stackRepository));
            _cardRepository = cardRepository ?? throw new ArgumentNullException(nameof(cardRepository));
            _statisticRepository = statisticRepository ?? throw new ArgumentNullException(nameof(statisticRepository));
            _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        }

        public UserManager()
        {
            _userRepository = new UserRepository();
            _cardRepository = new CardRepository();
            _deckRepository = new DeckRepository();
            _stackRepository = new StackRepository();
            _statisticRepository = new StatisticRepository();
            _sessionRepository = new SessionRepository();
        }

        public void Register(string name, string password, string role)
        {
            var user = _userRepository?.GetByName(name.Trim());
            if (user != null) throw new InvalidOperationException("User already exists");
            if (string.IsNullOrWhiteSpace(name.Trim()) || string.IsNullOrWhiteSpace(password.Trim()))
                throw new ArgumentException("Username or password cannot be empty.");

            var newUser = new User(name, BCrypt.Net.BCrypt.EnhancedHashPassword(password), role);
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

        public IUserRepository GetUserRepository() => _userRepository;

        public User GetUserByName(string username)
        {
            User user = _userRepository.GetByName(username.Trim());
            if (user == null) return null;
            SetUserData(user);
            return user;
        }

        public User? GetUserById(string id)
        {
            User? user = _userRepository.GetById(id.Trim());
            if(user == null) return null;
            SetUserData(user);
            return user;
        }

        public List<User> GetAllUser()
        {
            var users = _userRepository.GetAll();
            foreach(var user in users) SetUserData(user);
            return users;
        }

        public int DeleteUser(string username)
        {
            return _userRepository.Delete(username);
        }

        public void AddCardsToStack(string stackId, List<Card> cards)
        {
            _stackRepository.AddCards(stackId, cards);
        }

        public void AddCardToStack(User user, Card card)
        {
            _stackRepository.AddCard(user.Stack.Id, card);
        }

        public void AddCardToDeck(User user, Card card)
        {
            _deckRepository.AddCard(card.Id, user.Deck.Id);
        }
        public void AddCardToUser(User user, Card card)
        {
            _stackRepository.AddCard(user.Stack.Id, card);
        }

        public void RemoveCardFromUser(User user, Card card)
        {
            if (HasCardInStack(user, card.Id))
                _stackRepository.Remove(user.Stack.Id, card.Id);
            else if (HasCardInDeck(user, card.Id))
                _deckRepository.Remove(user.Deck.Id, card.Id);
        }


        private void SetUserData(User user)
        {
            List<Card> cards = _cardRepository.GetStackCards(user.Stack.Id);
            List<Card> deck = _cardRepository.GetDeckCards(user.Deck.Id);
            string? token = _sessionRepository.GetUserToken(user.Id);
            user.Stack.Set(cards);
            user.Deck.Set(deck);
            if(token != null)
                user.SetToken(token);
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
                _deckRepository.AddCard(card.Id, user.Deck.Id);
                _stackRepository.Remove(card.Id, user.Stack.Id);
                cards_added++;
            }
        }

        public void AcquirePackage(User user, int costs, List<Card> cards)
        {
            if (user.Coins < Package.Costs) throw new NotSupportedException("Not enough money");
            user.DecCoins(costs);
            user.Stack.Set(cards);
            _userRepository.UpdateUserCreds(user.Username, user);
        }

        public void UpdateUserCreds(string username, User user)
        {
            _userRepository.UpdateUserCreds(username, user);
        }

        public bool HasCardInInventory(User user, string cardId)
        {
            return HasCardInStack(user, cardId) || HasCardInDeck(user, cardId);
        }

        public bool HasCardInStack(User user, string cardId)
        {
            List<Card> stack = _cardRepository.GetStackCards(user.Stack.Id);
            return stack.Any(card => card.Id == cardId);
        }

        public bool HasCardInDeck(User user, string cardId)
        {
            List<Card> deck = _cardRepository.GetDeckCards(user.Deck.Id);
            return deck.Any(card => card.Id == cardId);
        }

    }
}
