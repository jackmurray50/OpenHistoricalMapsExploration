using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// Build configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get connection string
var connectionString = configuration.GetConnectionString("HistoricalMapDatabase");

// Configure DbContext options (ready for dependency injection or direct instantiation)
var optionsBuilder = new DbContextOptionsBuilder<HistoricalMapDbContext>();
optionsBuilder.UseSqlite(connectionString);

// Example: Create DbContext instance (uncomment when needed for data import)
// using var dbContext = new HistoricalMapDbContext(optionsBuilder.Options);
// await dbContext.Database.EnsureCreatedAsync();

Console.WriteLine("DataImporter is ready. DbContext configuration completed.");
Console.WriteLine($"Database: {connectionString}");
