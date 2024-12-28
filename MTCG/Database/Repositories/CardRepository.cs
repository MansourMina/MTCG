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
    public class CardRepository : ICardRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public List<Card> GetStackCards(string stack_id)
        {
            var commandText = """
            SELECT c.name, c.damage, c.element_type, c.card_type, c.id
            FROM cards c
            JOIN stack_cards sc ON c.id = sc.card_id
            WHERE sc.stack_id = @stack_id;
            """;

            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@stack_id", DbType.String, stack_id.Trim());

            var cards = new List<Card>();

            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var card = new Card(
                    reader.GetString(0),
                    reader.GetInt32(1),
                    Enum.Parse<ElementType>(reader.GetString(2)),
                    Enum.Parse<CardType>(reader.GetString(3)),
                    reader.GetString(4)
                );
                cards.Add(card);
            }
            return cards;
        }

        public List<Card> GetDeckCards(string deck_id)
        {
            var commandText = """
            SELECT c.name, c.damage, c.element_type, c.card_type, c.id
            FROM cards c
            JOIN deck_cards dc ON c.id = dc.card_id
            WHERE dc.deck_id = @deck_id;
            """;

            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@deck_id", DbType.String, deck_id.Trim());

            var cards = new List<Card>();

            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var card = new Card(
                    reader.GetString(0),
                    reader.GetInt32(1),
                    Enum.Parse<ElementType>(reader.GetString(2)),
                    Enum.Parse<CardType>(reader.GetString(3)),
                    reader.GetString(4)
                );
                cards.Add(card);
            }
            return cards;
        }
    }
}
