## DG_Retail

DG Retail is an ASP.NET Core MVC web application demonstrating the use of Microsoft Azure Storage services in a retail management system.

Features

Orders — Azure Table Storage

Create and view customer orders.

Azure table: CustomerOrders

Products — Azure Blob Storage

Upload and view product images.

Blob container: product-images

Transactions — Azure Queue Storage

Create and process transaction messages.

Queue: order-processing

Files — Azure File Storage

Upload, view, and download business documents.

File share: abc-retail-files

Architecture

ABCRetail
│
├── Controllers
│   ├── HomeController.cs
│   ├── OrdersController.cs
│   ├── ProductsController.cs
│   ├── TransactionsController.cs
│   └── FilesController.cs
│
├── Models
│   ├── Order.cs
│   ├── ProductImage.cs
│   ├── Transaction.cs
│   └── StoredFile.cs
│
├── Services
│   ├── AzureTableService.cs
│   ├── AzureBlobService.cs
│   ├── AzureQueueService.cs
│   └── AzureFileService.cs
│
├── Views
│   ├── Home
│   ├── Orders
│   ├── Products
│   ├── Transactions
│   ├── Files
│   └── Shared
│
├── wwwroot
├── Program.cs
├── appsettings.json
└── ABCRetail.csproj

Azure Storage Architecture

Orders        → Azure Table Storage → CustomerOrders
Products      → Azure Blob Storage  → product-images
Transactions  → Azure Queue Storage → order-processing
Files         → Azure File Storage  → abc-retail-files

Technologies

ASP.NET Core MVC

C#

.NET

Razor Views

Bootstrap

Azure Table Storage

Azure Blob Storage

Azure Queue Storage

Azure File Storage

Azure Storage SDKs

Git / GitHub

Azure Packages

Azure.Data.Tables
Azure.Storage.Blobs
Azure.Storage.Queues
Azure.Storage.Files.Shares

Configuration

The application requires an Azure Storage connection string.

Do not commit the real connection string to GitHub.

For local development, use ASP.NET Core User Secrets:

{
  "ConnectionStrings": {
    "AzureStorage": "YOUR_AZURE_STORAGE_CONNECTION_STRING"
  }
}

Keep the real value in User Secrets or another secure configuration mechanism.

Running the Application

1. Clone the repository

git clone <repository-url>

2. Open the project

Open ABCRetail.sln in Visual Studio.

3. Configure Azure Storage

Configure the AzureStorage connection string using ASP.NET Core User Secrets.

4. Restore dependencies

dotnet restore

5. Run the application

dotnet run

You can also run the application directly from Visual Studio.

Application Routes

Feature

Route

Dashboard

/Home

Orders

/Orders

Create Order

/Orders/Create

Products

/Products

Upload Product Image

/Products/Upload

Transactions

/Transactions

Create Transaction

/Transactions/Create

Files

/Files

Upload File

/Files/Upload

Security

The project uses basic security practices including:

Azure connection strings are kept out of source control.

.gitignore helps prevent sensitive development files from being committed.

ASP.NET Core dependency injection is used for Azure services.

Anti-forgery validation is enabled on POST forms.

File uploads are validated.

Azure Storage is accessed through the official Azure SDKs.

Project Purpose

DG Retail demonstrates how different Azure Storage services can be selected according to the type of information being stored:

Structured Data      → Azure Table Storage
Multimedia           → Azure Blob Storage
Transactions         → Azure Queue Storage
Documents / Files    → Azure File Storage

The application follows a clean MVC structure with separate controllers, models, views, and Azure service classes.

Author
Lucky Mkhatshwa

##DG Retail Project

Developed using ASP.NET Core MVC and Microsoft Azure Storage
