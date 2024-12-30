using Microsoft.VisualBasic;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using System.Data;


namespace MTCG.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public string Create(User user)
        {
            var commandText = """
                INSERT INTO users(username, password, coins, elo, role)
                VALUES (@username, @password, @coins, @elo, @role)
                RETURNING id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@username", DbType.String, user.Username);
            DataLayer.AddParameterWithValue(command, "@password", DbType.String, user.Password);
            DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, user.Coins);
            DataLayer.AddParameterWithValue(command, "@elo", DbType.Int32, user.Elo);
            DataLayer.AddParameterWithValue(command, "@role", DbType.String, user.Role);

            return command.ExecuteScalar() as string ?? "";
        }

        public List<User> GetAll()
        {
            var commandText = """
            SELECT u.username, u.password, u.role, u.coins, u.elo, u.id, s.id as stack_id, d.id as deck_id, st.id as statistic_id
            FROM users u
            JOIN stacks s ON u.id = s.user_id
            JOIN decks d ON u.id = d.user_id
            JOIN statistics st ON u.id = st.user_id
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);

            List<User> result = [];
            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var user = new User(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                user.SetCoins(reader.GetInt32(3));
                user.SetElo(reader.GetInt32(4));
                user.SetId(reader.GetString(5));
                var stack_id = reader.GetString(6);
                user.Stack.SetId(stack_id);
                user.Deck.SetId(reader.GetString(7));
                user.Statistic.SetId(reader.GetString(8));
                result.Add(user);
            }

            return result;
        }

        public User? GetByName(string username)
        {
            var commandText = """
            SELECT u.username, u.password, u.role, u.coins, u.elo, u.id, s.id as stack_id, d.id as deck_id, st.id as statistic_id
            FROM users u
            JOIN stacks s ON u.id = s.user_id
            JOIN decks d ON u.id = d.user_id
            JOIN statistics st ON u.id = st.user_id
            WHERE u.username = @username
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@username", DbType.String, username.Trim());

            using IDataReader reader = command.ExecuteReader();
            User? user = null;
            if (reader.Read())
            {
                user = new User(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                user.SetCoins(reader.GetInt32(3));
                user.SetElo(reader.GetInt32(4));
                user.SetId(reader.GetString(5));
                string stack_id = reader.GetString(6);
                user.Stack.SetId(stack_id);
                user.Deck.SetId(reader.GetString(7));
                user.Statistic.SetId(reader.GetString(8));
            }
            return user;
        }

        public User? GetById(string userId)
        {
            var commandText = """
            SELECT u.username, u.password, u.role, u.coins, u.elo, u.id, s.id as stack_id, d.id as deck_id, st.id as statistic_id
            FROM users u
            JOIN stacks s ON u.id = s.user_id
            JOIN decks d ON u.id = d.user_id
            JOIN statistics st ON u.id = st.user_id
            WHERE u.id = @id
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, userId.Trim());

            using IDataReader reader = command.ExecuteReader();
            User? user = null;
            if (reader.Read())
            {
                user = new User(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                user.SetCoins(reader.GetInt32(3));
                user.SetElo(reader.GetInt32(4));
                user.SetId(reader.GetString(5));
                string stack_id = reader.GetString(6);
                user.Stack.SetId(stack_id);
                user.Deck.SetId(reader.GetString(7));
                user.Statistic.SetId(reader.GetString(8));
            }
            return user;
        }

        public void UpdateUserCreds(string username, User user)
        {
            var commandText = """
            UPDATE users 
            SET username = @newUsername, elo = @elo, coins = @coins, password = @password, role = @role
            WHERE username = @username
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@elo", DbType.Int32, user.Elo);
            DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, user.Coins);
            DataLayer.AddParameterWithValue(command, "@password", DbType.String, user.Password);
            DataLayer.AddParameterWithValue(command, "@newUsername", DbType.String, user.Username);
            DataLayer.AddParameterWithValue(command, "@username", DbType.String, username);
            DataLayer.AddParameterWithValue(command, "@role", DbType.String, user.Role);
            command.ExecuteNonQuery();

        }


        public int Delete(string username)
        {
            var commandText = """DELETE from users where username = @username""";
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@username", DbType.String, username.Trim());
            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected;
        }


    }
}
