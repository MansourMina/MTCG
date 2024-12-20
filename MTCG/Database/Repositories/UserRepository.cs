﻿using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using System.Data;


namespace MTCG.Database.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public void Add(User user)
        {
            var commandText = """
                INSERT INTO users(username, password, coins, elo, stack_id, deck_id, statistic_id)
                VALUES (@username, @password, @coins, @elo, NULL, NULL, NULL)
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@username", DbType.String, user.Username);
            DataLayer.AddParameterWithValue(command, "@password", DbType.String, user.Password);
            DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, user.Coins);
            DataLayer.AddParameterWithValue(command, "@elo", DbType.Int32, user.Elo);
            //person.Id = (int)(dbCommand.ExecuteScalar() ?? 0);
            command.ExecuteNonQuery();
        }

        public List<User> GetAll()
        {
            var commandText = """
            SELECT username, password, coins, elo 
            FROM users
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);

            List<User> result = [];
            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var user = new User(reader.GetString(0), reader.GetString(1));
                user.SetCoins(reader.GetInt32(2));
                user.SetElo(reader.GetInt32(3));
                result.Add(user);
            }
            return result;
        }

        public User? Get(string username)
        {
            var commandText = """SELECT username, password, coins, elo from users where username = @username""";
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@username", DbType.String, username.Trim());

            using IDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                var user = new User(reader.GetString(0), reader.GetString(1));
                user.SetCoins(reader.GetInt32(2));
                user.SetElo(reader.GetInt32(3));
                return user;
            }

            return null;
        }

        public void Update(string username, User user)
        {

            var commandText = """
            UPDATE users 
            SET username = @newUsername, elo = @elo, coins = @coins, password = @password
            WHERE username = @username
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@elo", DbType.Int32, user.Elo);
            DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, user.Coins);
            DataLayer.AddParameterWithValue(command, "@password", DbType.String, user.Password);
            DataLayer.AddParameterWithValue(command, "@newUsername", DbType.String, user.Username);
            DataLayer.AddParameterWithValue(command, "@username", DbType.String, username);
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
