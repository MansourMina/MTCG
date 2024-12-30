using MTCG.Database.Repositories.Interfaces;
using MTCG.Database.Repositories;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Services.Interfaces;

namespace MTCG.Services
{
    public class TradingService
    {
        private readonly ITradingRepository _tradingRepository;
        private readonly UserManager _userManager;
        public TradingService(ITradingRepository tradingRepository)
        {
            _tradingRepository = tradingRepository ?? throw new ArgumentNullException(nameof(tradingRepository));
        }

        public TradingService()
        {
            _tradingRepository = new TradingRepository();
        }

        public List<Trade> GetAll()
        {
            return _tradingRepository.GetAll();
        }

        public void CreateTrade(Trade trade, User user)
        {
            UserManager userManager = new();
            if (!userManager.HasCardInInventory(user, trade.Card_Id)) throw new KeyNotFoundException("Card not found in inventory");
            _tradingRepository.Create(trade.Id, trade.Card_Id, trade.Status.ToString(), trade.Created_By_Id, trade.Required_Card_Type.ToString(), trade.Min_Damage);
        }
    }
}
