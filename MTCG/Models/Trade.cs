using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MTCG.Models
{
    public enum TradeStatus
    {
        Listed,
        Traded
    }
    public class Trade
    {
        public string Id { get; private set; }

        public string Created_By_Id { get; private set; }

        [JsonInclude]
        public string Card_Id { get; private set; }

        [JsonInclude]
        public CardType Required_Card_Type { get; private set; }

        [JsonInclude]
        public int Min_Damage { get; private set; }

        public TradeStatus Status { get; private set; }

        [JsonConstructor]
        private Trade() { }

        public Trade(string id, string card_id, string created_by_id, CardType required_card_type, int min_damage, TradeStatus status)
        {
            Id = id;
            Status = status;
            Created_By_Id = created_by_id;
            Card_Id = card_id;
            Min_Damage = min_damage;
            Required_Card_Type = required_card_type;
        }

        public Trade(string card_id, CardType required_card_type, int min_damage, string created_by_id)
        {
            Id = Guid.NewGuid().ToString();
            Status = TradeStatus.Listed;
            Created_By_Id = created_by_id;
            Card_Id = card_id;
            Min_Damage = min_damage;
            Required_Card_Type = required_card_type;
        }

    }
}
