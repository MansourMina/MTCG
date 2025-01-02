using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;

namespace MTCG.Database.Repositories
{
    public class DeckRepository : IDeckRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public string Create(string deck_id, string user_id)
        {
            var commandText = """
                INSERT INTO decks(id, user_id)
                VALUES (@stack_id,@user_id)
                RETURNING id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@stack_id", DbType.String, deck_id);
            DataLayer.AddParameterWithValue(command, "@user_id", DbType.String, user_id);
            return command.ExecuteScalar() as string ?? "";
        }

        public void AddCard(string card_id, string deck_id)
        {
            var commandText = """
                INSERT INTO deck_cards(id, deck_id, card_id)
                VALUES (@id, @deck_id, @card_id)
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, Guid.NewGuid().ToString());
            DataLayer.AddParameterWithValue(command, "@deck_id", DbType.String, deck_id);
            DataLayer.AddParameterWithValue(command, "@card_id", DbType.String, card_id);
            command.ExecuteNonQuery();
        }

        public int Remove(string card_id, string deck_id)
        {
            var commandText = """DELETE from decks_cards where card_id = @card_id AND deck_id = @deck_id""";
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@card_id", DbType.String, card_id);
            DataLayer.AddParameterWithValue(command, "@deck_id", DbType.String, deck_id);
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected;
        }

    }
}
