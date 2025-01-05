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
            _tradingRepository.Create(trade.Id, trade.Card_Id, trade.Status.ToString(), trade.Created_By_Id, trade.Required_Card_Type, trade.Min_Damage);
        }

        public void Trade(string tradeCardId, User offeringUser, string offeredCardId)
        {
            Trade? trade = _tradingRepository.GetTradeFromCardId(tradeCardId);
            if (trade == null )
                throw new KeyNotFoundException("Trade not found");
            if(trade.Created_By_Id == offeringUser.Id)
                throw new DuplicateNameException("Cannot trade with yourself");

            if (!_userManager.HasCardInInventory(offeringUser, offeredCardId))
                throw new KeyNotFoundException("Offered card not found in user's inventory");

            User trader = _userManager.GetUserById(trade.Created_By_Id) ?? throw new KeyNotFoundException("The user who created the trade was not found"); ;
            Card? offeredCard = _userManager.GetCard(offeringUser, offeredCardId) ?? throw new KeyNotFoundException("Offered Card not found");
            Card? tradeCard = _userManager.GetCard(trader, tradeCardId) ?? throw new KeyNotFoundException("Trade Card not found");

            if (trade.Required_Card_Type != offeredCard.CardType.ToString() || offeredCard.Damage < trade.Min_Damage)
                throw new ArgumentException("The offered card does not meet the trade requirements");


            ProcessCardTransfer(offeringUser, offeredCard, tradeCard);
            ProcessCardTransfer(trader, tradeCard, offeredCard);
            _tradingRepository.Traded(trade.Id, TradeStatus.Traded);
        }

        private void ProcessCardTransfer(User user, Card removedCard, Card addedCard)
        {
            _userManager.RemoveCardFromUser(user, removedCard);
            if (_userManager.HasCardInStack(user, removedCard.Id))
                _userManager.AddCardToStack(user, addedCard);
            else if (_userManager.HasCardInDeck(user, removedCard.Id))
                _userManager.AddCardToDeck(user, addedCard);
        }

        public void Delete(string cardId, User user)
        {
            Trade? trade = _tradingRepository.GetTradeFromCardId(cardId);
            if (trade == null) throw new KeyNotFoundException("Trade not found");
            if (trade.Created_By_Id != user.Id) throw new UnauthorizedAccessException("Unauthorized");
            int deleted = _tradingRepository.Delete(cardId, user.Id);
        }
    }
}
