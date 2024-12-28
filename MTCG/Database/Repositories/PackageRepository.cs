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
                string card_id = card.Id;
                if (!CardExists(card.Id))
                    card_id = AddCard(card);
                AddJointTable(card_id, package_id);
            }
        }

        public List<Package> GetAll()
        {
            var commandText = """
            SELECT c.name as card_name, c.damage as damage, c.element_type as element_type, c.card_type as card_type, c.id as card_id, p.id as package_id 
            FROM cards as c 
            JOIN packages_cards as pc ON c.id = pc.card_id 
            JOIN packages as p ON p.id = pc.package_id
            """;

            using IDbCommand command = _dal.CreateCommand(commandText);
            var packages = new Dictionary<string, Package>();

            using IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var card = new Card(
                    reader.GetString(0), // card_name
                    reader.GetInt32(1),  // damage
                    Enum.Parse<ElementType>(reader.GetString(2)), // element_type
                    Enum.Parse<CardType>(reader.GetString(3)),    // card_type
                    reader.GetString(4)  // card_id
                );

                string packageId = reader.GetString(5); // package_id (korrekter Index)

                if (!packages.ContainsKey(packageId))
                {
                    var package = new Package(packageId);
                    packages[packageId] = package;
                }
                packages[packageId].Cards.Add(card);
            }

            return packages.Values.ToList();
        }


        public Card? Get(string id)
        {
            var commandText = """
                SELECT c.name as card_name, c.damage as damage, c.element_type as element_type, c.card_type as card_type, c.id as card_id, p.id as package_id 
                from cards as c 
                JOIN packages_cards as pc ON c.id = pc.card_id 
                JOIN packages as p ON p.id = pc.package_id 
                WHERE c.id = @id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, id.Trim());

            using IDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                var card = new Card(
                    reader.GetString(0),
                    reader.GetInt32(1),
                    Enum.Parse<ElementType>(reader.GetString(2)),
                    Enum.Parse<CardType>(reader.GetString(3)),
                    reader.GetString(4)
                );
                return card;
            }

            return null;
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
            Console.WriteLine(card.ElementType.ToString());
            Console.WriteLine(card.CardType.ToString());
            var commandText = """
                INSERT INTO cards(id, name, damage, element_type, card_type)
                VALUES (@id, @name, @damage, @element_type, @card_type)
                RETURNING id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, card.Id);
            DataLayer.AddParameterWithValue(command, "@name", DbType.String, card.Name);
            DataLayer.AddParameterWithValue(command, "@damage", DbType.Int32, card.Damage);
            DataLayer.AddParameterWithValue(command, "@element_type", DbType.String, card.ElementType.ToString());
            DataLayer.AddParameterWithValue(command, "@card_type", DbType.String, card.CardType.ToString());
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

        private bool CardExists(string id)
        {
            var commandText = """
                SELECT COUNT(*) 
                FROM cards 
                WHERE id = @id
            """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, id);
            var result = command.ExecuteScalar();
            return Convert.ToInt32(result) > 0;
        }

        public void Delete(string id)
        {
            //var deletePackageCardsCommandText = """
            //    DELETE from packages_cards WHERE package_id = @id
            //    """;
            //using IDbCommand deletePackageCardsCommand = _dal.CreateCommand(deletePackageCardsCommandText);
            //DataLayer.AddParameterWithValue(deletePackageCardsCommand, "@id", DbType.String, id);
            //deletePackageCardsCommand.ExecuteNonQuery();

            var deletePackageCommandText = """
                DELETE from packages WHERE id = @id
                """;
            using IDbCommand deletePackageCommand = _dal.CreateCommand(deletePackageCommandText);
            DataLayer.AddParameterWithValue(deletePackageCommand, "@id", DbType.String, id);
            deletePackageCommand.ExecuteNonQuery();
        }
    }
}
