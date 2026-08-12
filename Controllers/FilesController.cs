using ABCRetail.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetail.Controllers
{
    public class FilesController : Controller
    {
        private readonly AzureFileService _fileService;

        public FilesController(AzureFileService fileService)
        {
            _fileService = fileService;
        }

        // Display all stored files
        public async Task<IActionResult> Index()
        {
            var files = await _fileService.GetFilesAsync();

            return View(files);
        }

        // Display upload page
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        // Process file upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(
                    "file",
                    "Please select a file.");

                return View();
            }

            await _fileService.UploadFileAsync(file);

            return RedirectToAction(nameof(Index));
        }

        // Download a stored file
        public async Task<IActionResult> Download(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest();
            }

            var stream =
                await _fileService.DownloadFileAsync(fileName);

            return File(
                stream,
                "application/octet-stream",
                fileName);
        }
    }
}