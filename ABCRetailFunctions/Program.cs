using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ABCRetail.Services;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// Register the existing ABCRetail Azure services
builder.Services.AddScoped<AzureTableService>();
builder.Services.AddScoped<AzureBlobService>();
builder.Services.AddScoped<AzureQueueService>();
builder.Services.AddScoped<AzureFileService>();

builder.Build().Run();