using Azure.Data.Tables;

namespace ABCRetail.Models
{
    public class Order
    {
        public string OrderId { get; set; } = string.Empty;

        public string CustomerName { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = "Pending";
    }
}