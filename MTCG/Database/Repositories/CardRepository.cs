using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Database.Repositories
{
    public class CardRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public List<Card> GetCards(string stack_id)
        {
            var commandText = """
            SELECT c.name, c.damage, c.element_type, c.card_type, c.id
            FROM users u
            JOIN stacks s ON u.stack_id = s.id
            JOIN stack_cards sc ON s.id = sc.stack_id
            JOIN cards c ON sc.card_id = c.id
            WHERE u.stack_id = @stack_id;
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
    }
}
