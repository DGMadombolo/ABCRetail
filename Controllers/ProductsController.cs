using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AzureBlobService _blobService;

        public ProductsController(AzureBlobService blobService)
        {
            _blobService = blobService;
        }

        // Display all product images
        public async Task<IActionResult> Index()
        {
            try
            {
                var images = await _blobService.GetImagesAsync();

                return View(images);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<ABCRetail.Models.ProductImage>());
            }
        }

        // Display upload page
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // Process image upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(
                    "file",
                    "Please select an image.");

                return View();
            }

            string[] allowedExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".webp"
            };

            string extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(
                    "file",
                    "Only JPG, JPEG, PNG, GIF and WEBP images are allowed.");

                return View();
            }

            try
            {
                await _blobService.UploadImageAsync(file);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    "file",
                    $"Upload failed: {ex.Message}");

                return View();
            }
        }
    }
}