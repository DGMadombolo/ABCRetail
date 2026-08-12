using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly AzureQueueService _queueService;

        public TransactionsController(AzureQueueService queueService)
        {
            _queueService = queueService;
        }

        // Display queue transactions
        public async Task<IActionResult> Index()
        {
            var messages = await _queueService.GetMessagesAsync();

            return View(messages);
        }

        // Display transaction form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Process transaction form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                return View(transaction);
            }

            transaction.TransactionId =
                $"TXN{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            transaction.CreatedAt = DateTime.UtcNow;
            transaction.Status = "Pending";

            await _queueService.AddTransactionAsync(transaction);

            return RedirectToAction(nameof(Index));
        }
    }
}