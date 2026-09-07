using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace Data;

/// <summary>
/// Design-time factory for creating <see cref="HistoricalMapDbContext"/> instances.
/// This is used by EF Core tools during migrations and other design-time operations.
/// </summary>
public class HistoricalMapDbContextFactory : IDesignTimeDbContextFactory<HistoricalMapDbContext>
{
    /// <summary>
    /// Creates a new instance of <see cref="HistoricalMapDbContext"/>.
    /// </summary>
    /// <param name="args">Command-line arguments (unused).</param>
    /// <returns>A new configured instance of <see cref="HistoricalMapDbContext"/>.</returns>
    public HistoricalMapDbContext CreateDbContext(string[] args)
    {
        // Build configuration from appsettings.json in the DataImporter project
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "DataImporter"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var connectionString = configuration.GetConnectionString("HistoricalMapDatabase");

        var optionsBuilder = new DbContextOptionsBuilder<HistoricalMapDbContext>();
        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.CommandTimeout(null);
        });

        return new HistoricalMapDbContext(optionsBuilder.Options);
    }
}
