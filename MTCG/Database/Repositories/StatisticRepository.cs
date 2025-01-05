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

        public void Update(Statistic statistic)
        {
            var commandText = """
            UPDATE statistics 
            SET wins = @wins, losses = @losses, draws = @draws
            WHERE id = @statistic_id
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@wins", DbType.Int32, statistic.Wins);
            DataLayer.AddParameterWithValue(command, "@losses", DbType.Int32, statistic.Losses);
            DataLayer.AddParameterWithValue(command, "@draws", DbType.Int32, statistic.Draws);
            DataLayer.AddParameterWithValue(command, "@statistic_id", DbType.String, statistic.Id);
            command.ExecuteNonQuery();
        }

        public Statistic? GetByUserId(string userId)
        {
            var commandText = """
            SELECT id, wins, losses, draws
            FROM statistics
            WHERE user_id = @id
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, userId.Trim());

            using IDataReader reader = command.ExecuteReader();
            Statistic? statistic = null;
            if (reader.Read())
            {
                statistic = new Statistic(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
            }
            return statistic;
        }
    }
}
