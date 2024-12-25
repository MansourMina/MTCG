﻿using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Database.Repositories
{
    public class StackRepository
    {
        private static readonly DataLayer _dal = DataLayer.Instance;

        public string Add(string stack_id, string user_id)
        {
            var commandText = """
                INSERT INTO stacks(id, user_id)
                VALUES (@stack_id, @user_id)
                RETURNING id
                """;
            using IDbCommand command = _dal.CreateCommand(commandText);
            DataLayer.AddParameterWithValue(command, "@stack_id", DbType.String, stack_id);
            DataLayer.AddParameterWithValue(command, "@user_id", DbType.String, user_id);
            return command.ExecuteScalar() as string ?? "";
        }

        public void AddCards(string stack_id, List<Card> cards)
        {
            foreach(var card in cards)
            {
                var commandText = """
                INSERT INTO stacks_cards(stack_id,card_id)
                VALUES (@stack_id, @card_id)
                RETURNING id
                """;
                using IDbCommand command = _dal.CreateCommand(commandText);
                DataLayer.AddParameterWithValue(command, "@stack_id", DbType.String, stack_id);
                DataLayer.AddParameterWithValue(command, "@card_id", DbType.String, card.Id);
                command.ExecuteNonQuery();
            }
            
        }

    }
}