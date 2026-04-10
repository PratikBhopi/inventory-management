# Shop Billing System

A comprehensive shop billing and inventory management system built with ASP.NET Core MVC and Entity Framework Core.

## Features

- Product inventory management with full CRUD operations
- User authentication and authorization using ASP.NET Core Identity
- Order and billing management
- SQL Server database with Entity Framework Core

## Prerequisites

Before you begin, ensure you have the following installed:

- **Visual Studio 2022 or later** (Community, Professional, or Enterprise edition)
- **.NET 10 SDK** - [Download here](https://dotnet.microsoft.com/download)
- **SQL Server** or **SQL Server LocalDB** (included with Visual Studio)
  - LocalDB is automatically installed with Visual Studio's "ASP.NET and web development" workload

## Getting Started

### 1. Clone or Download the Project

Download the project to your local machine and open the solution file (`ShopBillingSystem.sln`) in Visual Studio.

### 2. Configure the Database Connection

The application uses SQL Server LocalDB by default. The connection string is located in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ShopBillingSystemDb;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

If you're using a different SQL Server instance, update this connection string accordingly.

### 3. Set Up the Database

You need to create the database and apply migrations. Choose one of the following methods:

#### Option A: Using Package Manager Console (PMC) in Visual Studio

1. Open Visual Studio
2. Go to **Tools** → **NuGet Package Manager** → **Package Manager Console**
3. Run the following command:
   ```powershell
   Update-Database
   ```

#### Option B: Using .NET CLI

1. Open a terminal in the project directory
2. Run the following command:
   ```bash
   dotnet ef database update
   ```

This will create the database and all necessary tables (Users, Products, Orders, etc.).

### 4. Run the Application

#### Option A: Using Visual Studio

1. Press **F5** or click the **Play button** (green arrow) at the top of Visual Studio
2. The application will build and launch in your default browser

#### Option B: Using .NET CLI

1. Open a terminal in the project directory
2. Run:
   ```bash
   dotnet run
   ```
3. Open your browser and navigate to the URL shown in the terminal (typically `https://localhost:5001`)

### 5. Using the Application

- **Home Page**: The landing page with navigation
- **Products** (`/Products`): Main entry point for inventory management
  - View all products
  - Add new products (requires login)
  - Edit existing products
  - Delete products (requires login)
- **Register**: Create a new user account
- **Login**: Sign in to access protected features

## Project Structure

```
ShopBillingSystem/
├── Controllers/        # MVC Controllers
├── Models/            # Data models (Product, Order, etc.)
├── Views/             # Razor views
├── Data/              # Database context and migrations
├── Areas/Identity/    # Identity UI pages
└── wwwroot/           # Static files (CSS, JS, images)
```

## Technologies Used

- ASP.NET Core 10 MVC
- Entity Framework Core 10
- ASP.NET Core Identity (Authentication & Authorization)
- SQL Server / LocalDB
- Bootstrap 5 (UI Framework)
- Razor Views

## Troubleshooting

### Database Connection Issues

If you encounter database connection errors:
- Ensure SQL Server LocalDB is installed
- Verify the connection string in `appsettings.json`
- Try running `Update-Database` again

### Migration Issues

If migrations fail:
- Delete the `Migrations` folder in the `Data` directory
- Run `Add-Migration InitialCreate` in PMC
- Run `Update-Database`

## Next Steps

- Explore the Products page to manage inventory
- Register an account to access protected features
- Start creating orders and managing billing

## License

This project is for educational purposes.
