using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MTCG.Database.Repositories
{
    public class UserRepository
    {
        private static readonly DataLayer _dal= DataLayer.Instance;

        public void Add(User user)
        {
            using IDbCommand command = _dal.CreateCommand("""
                INSERT INTO users(username, password, coins, elo, stack_id, deck_id, statistic_id)
                VALUES (@username, @password, @coins, @elo, NULL, NULL, NULL)
                """);
            DataLayer.AddParameterWithValue(command, "@username", DbType.String, user.Username);
            DataLayer.AddParameterWithValue(command, "@password", DbType.String, user.Password);
            DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, user.Coins);
            DataLayer.AddParameterWithValue(command, "@elo", DbType.Int32, user.Elo);
            //person.Id = (int)(dbCommand.ExecuteScalar() ?? 0);
            int rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException("User konnte nicht hinzugefügt werden.");
            }
        }

        public List<User> GetAll()
        {
            using IDbCommand command = _dal.CreateCommand("""
            SELECT username, password, coins, elo 
            FROM users
            """);

            List<User> result = [];
            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var user = new User(reader.GetString(0), reader.GetString(1));
                user.setCoins(reader.GetInt32(2));
                user.setElo(reader.GetInt32(3));
                result.Add(user);
            }
            return result;
        }

        public User? GetByUsername(string username)
        {
            var commandText = """SELECT username, password, coins, elo from users where username = @username""";
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@username", DbType.String, username);

            using IDataReader reader = command.ExecuteReader();
            if (!reader.Read()) return null;

            var user = new User(reader.GetString(0), reader.GetString(1));
            user.setCoins(reader.GetInt32(2));
            user.setElo(reader.GetInt32(3));
            return user;
        }
    }
}
