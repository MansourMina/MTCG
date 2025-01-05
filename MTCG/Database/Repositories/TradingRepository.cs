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
            SELECT id, card_to_trade_id, created_by_id, required_card_type, min_damage, status from trading_deals WHERE status = 'Listed';
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

        public Trade? GetTradeFromCardId(string cardID)
        {
            var commandText = """
            SELECT id, card_to_trade_id, created_by_id, required_card_type, min_damage, status from trading_deals where card_to_trade_id = @cardID AND status = 'Listed';
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@cardID", DbType.String, cardID.Trim());

            using IDataReader reader = command.ExecuteReader();
            Trade? trade = null;
            if (reader.Read())
            {
                trade = new Trade(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    Enum.Parse<CardType>(reader.GetString(3)),
                    reader.GetInt32(4),
                    Enum.Parse<TradeStatus>(reader.GetString(5))
                );
            }
            return trade;
        }

        public Trade? GetTradeFromCardIdOfUser(string cardID, string userId)
        {
            var commandText = """
            SELECT id, card_to_trade_id, created_by_id, required_card_type, min_damage, status from trading_deals where card_to_trade_id = @cardID AND created_by_id=@userId AND status = 'Listed';
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@cardID", DbType.String, cardID.Trim());
            DataLayer.AddParameterWithValue(command, "@userId", DbType.String, userId.Trim());

            using IDataReader reader = command.ExecuteReader();
            Trade? trade = null;
            if (reader.Read())
            {
                trade = new Trade(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    Enum.Parse<CardType>(reader.GetString(3)),
                    reader.GetInt32(4),
                    Enum.Parse<TradeStatus>(reader.GetString(5))
                );
            }
            return trade;
        }

        public void Traded(string tradeID, TradeStatus status)
        {
            var commandText = """
            UPDATE trading_deals 
            SET status = @status
            WHERE id = @tradeID
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@status", DbType.String, status.ToString());
            DataLayer.AddParameterWithValue(command, "@tradeID", DbType.String, tradeID);
            command.ExecuteNonQuery();
        }


        public int Delete(string tradeId, string userId)
        {
            var commandText = """DELETE from trading_deals where card_to_trade_id= @tradeId AND created_by_id=@userId""";
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@tradeId", DbType.String, tradeId.Trim());
            DataLayer.AddParameterWithValue(command, "@userId", DbType.String, userId.Trim());
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected;
        }
    }
}
