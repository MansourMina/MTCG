using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Database.Repositories.Interfaces;

namespace MTCG.Database.Repositories
{
    public class StatisticRepository : IStatisticRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public string Create(string id, string user_id)
        {
            var commandText = """
                INSERT INTO statistics(id, user_id)
                VALUES (@id, @user_id)
                RETURNING id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, id);
            DataLayer.AddParameterWithValue(command, "@user_id", DbType.String, user_id);
            return command.ExecuteScalar() as string ?? "";
        }
    }
}
