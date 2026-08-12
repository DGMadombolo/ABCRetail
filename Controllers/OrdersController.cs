using ABCRetail.Models;
using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AzureTableService _tableService;

        public OrdersController(AzureTableService tableService)
        {
            _tableService = tableService;
        }

        // Display all orders
        public async Task<IActionResult> Index()
        {
            var orders = await _tableService.GetOrdersAsync();

            return View(orders);
        }

        // Display Create Order page
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Process Create Order form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }

            // Generate a unique order ID using UTC time
            order.OrderId = $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}";

            // Azure Table Storage requires UTC DateTime
            order.OrderDate = DateTime.UtcNow;

            order.Status = "Pending";

            await _tableService.AddOrderAsync(order);

            return RedirectToAction(nameof(Index));
        }
    }
}