using MongoDB.Bson;
using Realms;
using System;

namespace UWP.Shared.Models {
    /// <summary>
    /// Model for portfolio purchase/sell transactions
    /// </summary>
    public class TransactionModel : RealmObject {

        [PrimaryKey]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [Required]
        public string PortfolioId { get; set; } = "";

        [Required]
        public string Asset { get; set; }

        [Required]
        public double Quantity { get; set; } = 0;

        [Required]
        public TransactionType Type { get; set; }


        public bool AddDeductFromHoldings { get; set; } = false;

        public double TransactionFee { get; set; } = 0;

        public DateTimeOffset Date { get; set; }
        
        public string Exchange { get; set; } = "";
        
        public string Notes { get; set; } = "";
    }

    public enum TransactionType {
        Purchase = 0,
        Sale = 1
    }
}
