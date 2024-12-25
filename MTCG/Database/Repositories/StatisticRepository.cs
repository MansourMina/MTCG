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

        public string Add(string id)
        {
            var commandText = """
                INSERT INTO statistics(id)
                VALUES (@id)
                RETURNING id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, id);
            return command.ExecuteScalar() as string ?? "";
        }
    }
}
