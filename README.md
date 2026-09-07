# Historical Map Web App

A .NET 10 Blazor web application for exploring and managing historical map data with integrated data import and database management capabilities.

## 🎯 Project Overview

This project combines a modern Blazor web interface with powerful data migration utilities for managing historical map information. It includes:

- **Blazor Server Application** - Interactive web UI for viewing and managing map data
- **Data Importer** - Console utility for importing OpenStreetMap data (PBF format) into the database
- **Data Services** - Comprehensive database backup, validation, and management services
- **Entity Framework Core** - ORM layer supporting PostgreSQL and SQLite databases

## 🏗️ Architecture

### Projects

| Project | Purpose |
|---------|---------|
| **HistoricalMapWebApp** | Blazor Server web application (UI/frontend) |
| **DataImporter** | Console application for importing PBF map data |
| **Data** | Entity Framework Core context, models, and services |
| **DataTypes** | Shared data type definitions |

### Technology Stack

- **.NET 10** - Modern .NET platform
- **Blazor Server** - Interactive web UI framework
- **Entity Framework Core** - ORM and data access
- **PostgreSQL / SQLite** - Database support
- **Bootstrap** - UI styling and responsiveness

## 🚀 Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL or SQLite database
- Visual Studio Community 2026 (or compatible IDE)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/jackmurray50/OpenHistoricalMapsExploration.git
   cd HistoricalMapWebApp
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Configure your database connection in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "HistoricalMapDatabase": "Server=localhost;Database=historical_maps;..."
     }
   }
   ```

4. Run database migrations (if applicable):
   ```bash
   dotnet ef database update --project Data
   ```

## 📝 Usage

### Running the Web Application

```bash
cd HistoricalMapWebApp
dotnet run
```

The application will be available at `https://localhost:7000` (or as configured in `launchSettings.json`).

### Using the Data Importer

The DataImporter console utility imports OpenStreetMap data in PBF (Protocol Buffer Format) into your database.

#### Update Mode (Upsert)
Merges new data with existing records:
```bash
dotnet run --project DataImporter update path/to/map-data.pbf
```

#### Overwrite Mode
Replaces existing data completely:
```bash
dotnet run --project DataImporter overwrite path/to/map-data.pbf
```

## 🛠️ Development

### Project Structure

```
HistoricalMapWebApp/
├── Components/          # Blazor components and pages
│   ├── Pages/          # Routable pages (Home, Counter, Weather, etc.)
│   ├── Layout/         # Main layout and navigation components
│   └── App.razor       # Root component
├── wwwroot/            # Static assets (CSS, JavaScript, Bootstrap)
├── appsettings.json    # Configuration
└── Program.cs          # Application startup

Data/
├── Services/           # DatabaseBackupService and other utilities
├── Entities/           # Entity Framework models
└── DatabaseContext.cs  # EF Core DbContext

DataImporter/
├── Program.cs          # Import logic and CLI parsing
└── appsettings.json    # Configuration
```

### Key Services

**DatabaseBackupService** - Manages database backups and recovery
- Create automated backups
- Validate database integrity
- Handle database migrations

## 📊 Database Design

The application supports both PostgreSQL and SQLite:

- **PostgreSQL**: Recommended for production environments
- **SQLite**: Suitable for development and smaller deployments

Configure your connection string in `appsettings.json` to switch between databases.

## 🔒 Security

- HSTS enabled for HTTPS enforcement
- Antiforgery protection on requests
- Entity Framework Core parameterized queries prevent SQL injection
- Status code page handling for error management

## 📦 Dependencies

Key NuGet packages:
- `Microsoft.EntityFrameworkCore`
- `Npgsql.EntityFrameworkCore.PostgreSQL`
- `Microsoft.AspNetCore.Components`

Full dependency list can be found in project files (`.csproj`).

## 🧪 Testing & Validation

The project includes data validation services to ensure data integrity during imports and backups. See `Data/Services/` for validation utilities.

## 🌳 Branch Structure

This repository is currently on the `DataImporter` branch, indicating active development on data import features.

## 📝 Configuration

Key configuration files:
- `appsettings.json` - Database connections, logging settings
- `appsettings.Development.json` - Development-specific overrides
- `launchSettings.json` - IIS Express and console runner settings

## 🤝 Contributing

For contributions, ensure:
- Code follows existing conventions
- Database changes include migration scripts
- New features include appropriate tests
- PBF import logic is thoroughly validated

## 📄 License

See LICENSE file for details.

## 📞 Contact

For questions or issues, refer to the GitHub repository: https://github.com/jackmurray50/OpenHistoricalMapsExploration
