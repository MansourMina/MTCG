using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Database.Repositories
{
    public class TradingRepository : ITradingRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public string Create(string id, string card_to_trade, string status, string created_by_id, string required_card_type, int min_damage)
        {
            var commandText = """
                INSERT INTO trading_deals(id, card_to_trade_id, created_by_id, required_card_type, min_damage, status)
                VALUES (@id, @card_to_trade, @created_by_id, @required_card_type, @min_damage, @status)
                RETURNING id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, id);
            DataLayer.AddParameterWithValue(command, "@card_to_trade", DbType.String, card_to_trade);
            DataLayer.AddParameterWithValue(command, "@created_by_id", DbType.String, created_by_id);
            DataLayer.AddParameterWithValue(command, "@required_card_type", DbType.String, required_card_type);
            DataLayer.AddParameterWithValue(command, "@min_damage", DbType.Int32, min_damage);
            DataLayer.AddParameterWithValue(command, "@status", DbType.String, status);
            return command.ExecuteScalar() as string ?? "";
        }

        public List<Trade> GetAll()
        {
            var commandText = """
            SELECT id, card_to_trade_id, created_by_id, required_card_type, min_damage, status from trading_deals;
            """;

            using IDbCommand command = _dal.CreateCommand(commandText);

            var trades = new List<Trade>();

            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var trade = new Trade(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    Enum.Parse<CardType>(reader.GetString(3)),
                    reader.GetInt32(4),
                    Enum.Parse<TradeStatus>(reader.GetString(5))
                );
                trades.Add(trade);
            }
            return trades;
        }
    }
}
