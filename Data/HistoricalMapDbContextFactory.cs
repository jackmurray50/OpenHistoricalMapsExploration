using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
        var optionsBuilder = new DbContextOptionsBuilder<HistoricalMapDbContext>();
        optionsBuilder.UseSqlite("Data Source=HistoricalMap.db");

        return new HistoricalMapDbContext(optionsBuilder.Options);
    }
}
