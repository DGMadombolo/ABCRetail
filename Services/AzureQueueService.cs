using Azure.Storage.Queues;
using System.Text.Json;
using ABCRetail.Models;

namespace ABCRetail.Services
{
    public class AzureQueueService
    {
        private readonly QueueClient _queueClient;

        public AzureQueueService(IConfiguration configuration)
        {
            string? connectionString =
                configuration.GetConnectionString("AzureStorage");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Storage connection string is not configured.");
            }

            _queueClient = new QueueClient(
                connectionString,
                "order-processing");

            _queueClient.CreateIfNotExists();
        }

        // Add a transaction to the queue
        public async Task AddTransactionAsync(Transaction transaction)
        {
            string message = JsonSerializer.Serialize(transaction);

            await _queueClient.SendMessageAsync(message);
        }

        // Get messages from the queue
        public async Task<List<string>> GetMessagesAsync()
        {
            List<string> messages = new();

            var response =
                await _queueClient.ReceiveMessagesAsync(
                    maxMessages: 32);

            foreach (var message in response.Value)
            {
                messages.Add(message.MessageText);
            }

            return messages;
        }
    }
}