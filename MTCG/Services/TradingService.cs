using MTCG.Database.Repositories.Interfaces;
using MTCG.Database.Repositories;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Services.Interfaces;
using System.Data;

namespace MTCG.Services
{
    public class TradingService
    {
        private readonly ITradingRepository _tradingRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly UserManager _userManager;
        public TradingService(ITradingRepository tradingRepository, IPackageRepository packageRepository)
        {
            _tradingRepository = tradingRepository ?? throw new ArgumentNullException(nameof(tradingRepository));
            _packageRepository = packageRepository ?? throw new ArgumentNullException(nameof(packageRepository));
        }

        public TradingService()
        {
            _tradingRepository = new TradingRepository();
            _packageRepository = new PackageRepository();
            _userManager = new();
        }

        public List<Trade> GetAll()
        {
            return _tradingRepository.GetAll();
        }

        public void CreateTrade(Trade trade, User user)
        {
            
            if (!_userManager.HasCardInInventory(user, trade.Card_Id)) throw new KeyNotFoundException("Card not found in inventory");
            _tradingRepository.Create(trade.Id, trade.Card_Id, trade.Status.ToString(), trade.Created_By_Id, trade.Required_Card_Type.ToString(), trade.Min_Damage);
        }

        public void Trade(string tradeId, User tradingUser, string offeredCardId)
        {
            Trade? trade = _tradingRepository.Get(tradeId);
            if (trade == null )
                throw new KeyNotFoundException("Trade not found");
            if(trade.Created_By_Id == tradingUser.Id)
                throw new DuplicateNameException("Cannot trade with yourself");

            if (!_userManager.HasCardInInventory(tradingUser, offeredCardId))
                throw new KeyNotFoundException("Offered card not found in user's inventory");

            Card offeredCard = _packageRepository.Get(offeredCardId)!;
            Card tradeCard = _packageRepository.Get(trade.Card_Id)!;

            if (trade.Required_Card_Type != offeredCard.CardType || offeredCard.Damage < trade.Min_Damage)
                throw new ArgumentException("The offered card does not meet the trade requirements");

            User createdTrade = _userManager.GetUserById(trade.Created_By_Id) ?? throw new KeyNotFoundException("The user who created the trade was not found"); ;

            ProcessCardTransfer(tradingUser, offeredCard, tradeCard);
            ProcessCardTransfer(createdTrade, tradeCard, offeredCard);
        }

        private void ProcessCardTransfer(User user, Card removedCard, Card addedCard)
        {
            if (_userManager.HasCardInStack(user, removedCard.Id))
                _userManager.AddCardToStack(user, addedCard);
            else if (_userManager.HasCardInDeck(user, removedCard.Id))
                _userManager.AddCardToDeck(user, addedCard);
            _userManager.RemoveCardFromUser(user, removedCard);
        }
    }
}
