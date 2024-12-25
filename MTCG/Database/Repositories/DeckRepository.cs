using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Database.Repositories.Interfaces;

namespace MTCG.Database.Repositories
{
    public class DeckRepository : IDeckRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public string Add(string deck_id)
        {
            var commandText = """
                INSERT INTO decks(id)
                VALUES (@stack_id)
                RETURNING id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@stack_id", DbType.String, deck_id);
            return command.ExecuteScalar() as string ?? "";
        }
    }
}
