using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Database.Repositories
{
    public class SessionRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;
        public void Add(string userId, string token, DateTime expires_at)
        {
            var commandText = """
                INSERT INTO user_sessions (user_id, session_token, expires_at) VALUES (@user_id, @token, @expires_at)
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@user_id", DbType.String, userId);
            DataLayer.AddParameterWithValue(command, "@token", DbType.String, token);
            DataLayer.AddParameterWithValue(command, "@expires_at", DbType.DateTime2, expires_at);
            command.ExecuteNonQuery();
        }

        public bool VerifyToken(string token)
        {
            var commandText = """
                SELECT COUNT(*) 
                FROM user_sessions 
                WHERE session_token = @token AND expires_at > NOW()
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@token", DbType.String, token);
            var result = command.ExecuteScalar();
            return Convert.ToInt32(result) > 0;
        }

        public void Delete(string token)
        {
            var commandText = """DELETE FROM user_sessions WHERE session_token = @token""";
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@token", DbType.String, token.Trim());
            int rowsAffected = command.ExecuteNonQuery();
        }

        public string? GetUserToken(string userId)
        {
            var commandText = """
                SELECT session_token
                FROM user_sessions 
                WHERE user_id = @userId AND expires_at > NOW()
                LIMIT 1
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@userId", DbType.String, userId);

            using IDataReader reader = command.ExecuteReader();
            if (reader.Read())
                return reader.GetString(0);
            return null;
        }

        public string? GetUserId(string token)
        {
            var commandText = """
                SELECT user_id
                FROM user_sessions 
                WHERE session_token = @token AND expires_at > NOW()
                LIMIT 1
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@token", DbType.String, token);

            using IDataReader reader = command.ExecuteReader();
            if (reader.Read())
                return reader.GetString(0);
            return null;
        }

        internal bool TokenExists(string token)
        {
            var commandText = """
                SELECT COUNT(*) 
                FROM user_sessions 
                WHERE session_token = @token
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@token", DbType.String, token);
            var result = command.ExecuteScalar();
            return Convert.ToInt32(result) > 0;
        }
        public void DeleteByUserId(string userId)
        {
            var commandText = """DELETE FROM user_sessions WHERE user_id = @userId""";
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@token", DbType.String, userId.Trim());
            int rowsAffected = command.ExecuteNonQuery();
        }
    }
}
