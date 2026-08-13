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
                Console.WriteLine("===== LOADING PRODUCT IMAGES =====");

                var images = await _blobService.GetImagesAsync();

                Console.WriteLine(
                    $"Images loaded successfully: {images.Count}");

                return View(images);
            }
            catch (Exception ex)
            {
                Console.WriteLine("===== ERROR LOADING IMAGES =====");
                Console.WriteLine(ex.ToString());

                ViewBag.ErrorMessage = ex.Message;

                return View(
                    new List<ABCRetail.Models.ProductImage>());
            }
        }

        // Display upload page
        [HttpGet]
        public IActionResult Upload()
        {
            Console.WriteLine("===== UPLOAD PAGE OPENED =====");

            return View();
        }

        // Process image upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // IMPORTANT DEBUG MESSAGE
            Console.WriteLine(
                "🔥🔥🔥 UPLOAD METHOD WAS CALLED 🔥🔥🔥");

            try
            {
                // Check file
                if (file == null || file.Length == 0)
                {
                    Console.WriteLine(
                        "ERROR: No file was selected.");

                    ModelState.AddModelError(
                        "file",
                        "Please select an image.");

                    return View();
                }

                Console.WriteLine(
                    $"File name: {file.FileName}");

                Console.WriteLine(
                    $"File size: {file.Length} bytes");

                Console.WriteLine(
                    $"Content type: {file.ContentType}");

                // Allowed extensions
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

                Console.WriteLine(
                    $"File extension: {extension}");

                // Validate extension
                if (!allowedExtensions.Contains(extension))
                {
                    Console.WriteLine(
                        "ERROR: File extension is not allowed.");

                    ModelState.AddModelError(
                        "file",
                        "Only JPG, JPEG, PNG, GIF and WEBP images are allowed.");

                    return View();
                }

                Console.WriteLine(
                    "File validation successful.");

                // Upload to Azure
                Console.WriteLine(
                    "===== STARTING AZURE BLOB UPLOAD =====");

                var uploadedImage =
                    await _blobService.UploadImageAsync(file);

                Console.WriteLine(
                    "===== AZURE BLOB UPLOAD COMPLETED =====");

                Console.WriteLine(
                    $"Blob name: {uploadedImage.BlobName}");

                Console.WriteLine(
                    $"Image URL: {uploadedImage.ImageUrl}");

                Console.WriteLine(
                    "===== REDIRECTING TO PRODUCTS =====");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "🔥🔥🔥 IMAGE UPLOAD ERROR 🔥🔥🔥");

                Console.WriteLine(
                    ex.ToString());

                Console.WriteLine(
                    "🔥🔥🔥 END IMAGE UPLOAD ERROR 🔥🔥🔥");

                ModelState.AddModelError(
                    "file",
                    $"Upload failed: {ex.Message}");

                return View();
            }
        }
    }
}