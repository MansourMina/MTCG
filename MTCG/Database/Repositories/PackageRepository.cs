using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Database.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public void Add(Package package)
        {
            string package_id = AddPackage(package);
            foreach (var card in package.Cards)
            {
                string card_id = AddCard(card);
                AddJointTable(card_id, package_id);
            }
        }

        private string AddPackage(Package package)
        {
            var commandText = """
            INSERT INTO packages(id)
            VALUES (@id)
            RETURNING id;
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, package.Id);
            return command.ExecuteScalar() as string ?? "";
        }

        private string AddCard(Card card)
        {
            var commandText = """
                INSERT INTO cards(id, name, damage)
                VALUES (@id, @name, @damage)
                RETURNING id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, card.Id);
            DataLayer.AddParameterWithValue(command, "@name", DbType.String, card.Name);
            DataLayer.AddParameterWithValue(command, "@damage", DbType.Int32, card.Damage);
            return command.ExecuteScalar() as string ?? "";
        }

        private void AddJointTable(string card_id, string package_id)
        {
            var commandText = """
                INSERT INTO packages_cards(package_id, card_id)
                VALUES (@package_id, @card_id)
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@package_id", DbType.String, package_id);
            DataLayer.AddParameterWithValue(command, "@card_id", DbType.String, card_id);
            command.ExecuteNonQuery();
        }
    }
}
