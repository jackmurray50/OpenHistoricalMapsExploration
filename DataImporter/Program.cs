using Data;
using Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// Build configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get connection string
var connectionString = configuration.GetConnectionString("HistoricalMapDatabase");

// Parse command-line arguments
if (args.Length < 2)
{
    Console.WriteLine("Usage: DataImporter <update|overwrite> <path-to-pbf-file>");
    Console.WriteLine("  update <path>    - Update database with data from PBF file (upsert)");
    Console.WriteLine("  overwrite <path> - Overwrite database with data from PBF file (creates backup first)");
    Environment.Exit(1);
}

var command = args[0].ToLowerInvariant();
var pbfFilePath = args[1];

if (command != "update" && command != "overwrite")
{
    Console.WriteLine($"Error: Unknown command '{command}'. Use 'update' or 'overwrite'.");
    Environment.Exit(1);
}

if (!File.Exists(pbfFilePath))
{
    Console.WriteLine($"Error: PBF file not found: {pbfFilePath}");
    Environment.Exit(1);
}

try
{
    // Configure DbContext
    var optionsBuilder = new DbContextOptionsBuilder<HistoricalMapDbContext>();
    optionsBuilder.UseSqlite(connectionString);

    using (var dbContext = new HistoricalMapDbContext(optionsBuilder.Options))
    {
        // Ensure database exists
        await dbContext.Database.EnsureCreatedAsync();

        var databaseOps = new DatabaseOperations(dbContext);
        var backupService = new DatabaseBackupService(connectionString!);
        var importer = new OsmDataImporter();

        Console.WriteLine($"Starting {command} import from: {pbfFilePath}");
        Console.WriteLine();

        if (command == "overwrite")
        {
            // Create backup of current database before overwriting
            Console.WriteLine("Creating database backup...");
            var backupPath = backupService.CreateBackup();

            if (backupPath != null)
            {
                Console.WriteLine($"✓ Backup created: {Path.GetFileName(backupPath)}");
                Console.WriteLine($"  Location: {backupPath}");
            }
            else
            {
                Console.WriteLine("✓ No existing database to backup");
            }

            // Clear all data
            Console.WriteLine();
            Console.WriteLine("Clearing existing database...");
            await databaseOps.ClearAllDataAsync();
            Console.WriteLine("✓ Database cleared");
        }

        // Import data from PBF
        Console.WriteLine();
        Console.WriteLine("Reading PBF file...");
        var (entities, tags, wayNodes, relationMembers) = importer.ImportFromPbf(pbfFilePath);

        Console.WriteLine($"✓ PBF file parsed:");
        Console.WriteLine($"  Entities: {entities.Count:N0}");
        Console.WriteLine($"  Tags: {tags.Count:N0}");
        Console.WriteLine($"  Way nodes: {wayNodes.Count:N0}");
        Console.WriteLine($"  Relation members: {relationMembers.Count:N0}");

        // Save to database
        Console.WriteLine();
        Console.WriteLine("Importing to database...");

        Console.WriteLine("  Upserting entities...");
        await databaseOps.UpsertEntitiesAsync(entities);

        Console.WriteLine("  Upserting tags...");
        await databaseOps.UpsertTagsAsync(tags);

        Console.WriteLine("  Upserting way nodes...");
        await databaseOps.UpsertWayNodesAsync(wayNodes);

        Console.WriteLine("  Upserting relation members...");
        await databaseOps.UpsertRelationMembersAsync(relationMembers);

        // Final count
        var (finalEntityCount, finalTagCount, finalWayNodeCount, finalRelationMemberCount) =
            await databaseOps.GetCountsAsync();

        Console.WriteLine();
        Console.WriteLine("✓ Import completed successfully!");
        Console.WriteLine($"  Database now contains:");
        Console.WriteLine($"    Entities: {finalEntityCount:N0}");
        Console.WriteLine($"    Tags: {finalTagCount:N0}");
        Console.WriteLine($"    Way nodes: {finalWayNodeCount:N0}");
        Console.WriteLine($"    Relation members: {finalRelationMemberCount:N0}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Details: {ex.InnerException.Message}");
    }
    Environment.Exit(1);
}
