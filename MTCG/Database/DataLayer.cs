using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MTCG.Database
{
    public class DataLayer : IDisposable
    {
        private static DataLayer? _instance;
        public static DataLayer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataLayer("Host=localhost;Database=mtcg;Username=postgres;Password=postgres;Persist Security Info=True");
                return _instance;
            }
        }

        private readonly string _connString;
        private IDbConnection _connection;

        public DataLayer(string connString)
        {
            this._connString = connString;
            _connection = new NpgsqlConnection(connString);
            _connection.Open();
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }

        public IDbCommand CreateCommand(string commandText)
        {
            IDbCommand command = _connection.CreateCommand();
            command.CommandText = commandText;
            return command;
        }

        public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object? value)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.DbType = type;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}
