namespace PortfolioApi.Models
{
    public class Transaction
    {
        public required long TxnId { get; set; } //transaction id   
        public  string TrdId { get; set; } //trade id (portfolio + invnum + version)  
        public string GsSecId { get; set; } //asset id  
        public DateTime Date { get; set; } //transaction date  
        public int Quantity { get; set; } //quantity of asset bought or sold  
        public decimal Price { get; set; } //price per asset at the time of transaction  
        public decimal Fees { get; set; } // transaction fees, if any
        public TransactionType Type { get; set; } // Buy or Sell transaction  
        public string? CreatedBy { get; set; } // user who created the transaction  
        public string? ModifiedBy { get; set; } // user who modified the transaction  
        public string? CreatedAt { get; set; } // timestamp when the transaction was created  
        public string? ModifiedAt { get; set; } // timestamp when the transaction was last modified  
        public decimal NetAmount => Quantity * Price; // Net amount of the transaction (Quantity * Price)  
    }

    public enum TransactionType
    {
        Buy,
        Sell
    }
}
