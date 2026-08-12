namespace ABCRetail.Models
{
    public class Transaction
    {
        public string TransactionId { get; set; } = string.Empty;

        public string OrderId { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public string TransactionType { get; set; } = "Order";

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }
    }
}