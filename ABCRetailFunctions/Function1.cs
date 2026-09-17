using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using ABCRetail.Models;
using ABCRetail.Services;

namespace ABCRetailFunctions;

public class Function1
{
    private readonly AzureTableService _tableService;
    private readonly AzureBlobService _blobService;
    private readonly AzureQueueService _queueService;
    private readonly AzureFileService _fileService;
    private readonly ILogger<Function1> _logger;

    public Function1(
        AzureTableService tableService,
        AzureBlobService blobService,
        AzureQueueService queueService,
        AzureFileService fileService,
        ILogger<Function1> logger)
    {
        _tableService = tableService;
        _blobService = blobService;
        _queueService = queueService;
        _fileService = fileService;
        _logger = logger;
    }

    // =========================================================
    // FUNCTION 1 - AZURE TABLE STORAGE
    // =========================================================

    [Function("StoreOrderFunction")]
    public async Task<IActionResult> StoreOrder(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = "orders")]
        HttpRequest req)
    {
        _logger.LogInformation("StoreOrderFunction triggered.");

        try
        {
            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();

            var order = JsonSerializer.Deserialize<Order>(
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (order == null)
            {
                return new BadRequestObjectResult(
                    "Invalid order data.");
            }

            await _tableService.AddOrderAsync(order);

            return new OkObjectResult(new
            {
                message = "Order successfully stored in Azure Table Storage.",
                order = order
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error storing order in Azure Table Storage.");

            return new ObjectResult(new
            {
                error = ex.Message
            })
            {
                StatusCode = 500
            };
        }
    }


    // =========================================================
    // FUNCTION 2 - AZURE BLOB STORAGE
    // =========================================================

    [Function("UploadProductImageFunction")]
    public async Task<IActionResult> UploadProductImage(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = "products/images")]
        HttpRequest req)
    {
        _logger.LogInformation(
            "UploadProductImageFunction triggered.");

        try
        {
            var form = await req.ReadFormAsync();

            var file = form.Files.GetFile("file");

            if (file == null)
            {
                return new BadRequestObjectResult(
                    "Please upload a file using the form field named 'file'.");
            }

            var uploadedImage =
                await _blobService.UploadImageAsync(file);

            return new OkObjectResult(new
            {
                message =
                    "Image successfully uploaded to Azure Blob Storage.",
                image = uploadedImage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error uploading image to Azure Blob Storage.");

            return new ObjectResult(new
            {
                error = ex.Message
            })
            {
                StatusCode = 500
            };
        }
    }


    // =========================================================
    // FUNCTION 3 - AZURE QUEUE STORAGE
    // =========================================================

    [Function("TransactionQueueFunction")]
    public async Task<IActionResult> TransactionQueue(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            "post",
            Route = "transactions")]
        HttpRequest req)
    {
        _logger.LogInformation(
            "TransactionQueueFunction triggered.");

        try
        {
            // -------------------------------------------------
            // POST = Write transaction to Azure Queue Storage
            // -------------------------------------------------

            if (HttpMethods.IsPost(req.Method))
            {
                using var reader = new StreamReader(req.Body);
                var body = await reader.ReadToEndAsync();

                var transaction =
                    JsonSerializer.Deserialize<Transaction>(
                        body,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (transaction == null)
                {
                    return new BadRequestObjectResult(
                        "Invalid transaction data.");
                }

                await _queueService.AddTransactionAsync(
                    transaction);

                return new OkObjectResult(new
                {
                    message =
                        "Transaction successfully written to Azure Queue Storage.",
                    transaction = transaction
                });
            }

            // -------------------------------------------------
            // GET = Read messages from Azure Queue Storage
            // -------------------------------------------------

            if (HttpMethods.IsGet(req.Method))
            {
                var messages =
                    await _queueService.GetMessagesAsync();

                return new OkObjectResult(new
                {
                    message =
                        "Messages retrieved from Azure Queue Storage.",
                    messages = messages
                });
            }

            return new BadRequestObjectResult(
                "Unsupported HTTP method.");
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing Azure Queue Storage.");

            return new ObjectResult(new
            {
                error = ex.Message
            })
            {
                StatusCode = 500
            };
        }
    }


    // =========================================================
    // FUNCTION 4 - AZURE FILE STORAGE
    // =========================================================

    [Function("UploadFileFunction")]
    public async Task<IActionResult> UploadFile(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "post",
            Route = "files")]
        HttpRequest req)
    {
        _logger.LogInformation(
            "UploadFileFunction triggered.");

        try
        {
            var form = await req.ReadFormAsync();

            var file = form.Files.GetFile("file");

            if (file == null)
            {
                return new BadRequestObjectResult(
                    "Please upload a file using the form field named 'file'.");
            }

            var uploadedFile =
                await _fileService.UploadFileAsync(file);

            return new OkObjectResult(new
            {
                message =
                    "File successfully uploaded to Azure Files.",
                fileName = uploadedFile.FileName,
                fileSize = uploadedFile.FileSize
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error uploading file to Azure Files.");

            return new ObjectResult(new
            {
                error = ex.Message
            })
            {
                StatusCode = 500
            };
        }
    }
}