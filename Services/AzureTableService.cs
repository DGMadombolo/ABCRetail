using Azure.Data.Tables;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class AzureTableService
    {
        private readonly TableClient _tableClient;

        public AzureTableService(IConfiguration configuration)
        {
            string? connectionString =
                configuration.GetConnectionString("AzureStorage");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Storage connection string is not configured.");
            }

            _tableClient = new TableClient(
                connectionString,
                "CustomerOrders");

            _tableClient.CreateIfNotExists();
        }

        // Get all orders
        public async Task<List<Order>> GetOrdersAsync()
        {
            List<Order> orders = new();

            await foreach (var entity in _tableClient.QueryAsync<TableEntity>())
            {
                orders.Add(new Order
                {
                    OrderId = entity.RowKey,
                    CustomerName = entity.GetString("CustomerName") ?? "",
                    ProductName = entity.GetString("ProductName") ?? "",
                    Quantity = entity.GetInt32("Quantity") ?? 0,
                    OrderDate = entity.GetDateTime("OrderDate") ?? DateTime.MinValue,
                    Status = entity.GetString("Status") ?? "Pending"
                });
            }

            return orders;
        }

        // Add a new order
        public async Task AddOrderAsync(Order order)
        {
            TableEntity entity = new TableEntity
            {
                PartitionKey = "Orders",
                RowKey = order.OrderId,

                ["CustomerName"] = order.CustomerName,
                ["ProductName"] = order.ProductName,
                ["Quantity"] = order.Quantity,
                ["OrderDate"] = order.OrderDate.ToUniversalTime(),
                ["Status"] = order.Status
            };

            await _tableClient.AddEntityAsync(entity);
        }
    }
}