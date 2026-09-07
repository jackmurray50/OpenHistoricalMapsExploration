using Data;
using Data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

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
    optionsBuilder.UseNpgsql(connectionString, options =>
    {
        options.CommandTimeout(null);
    });

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

        Console.WriteLine("Optimizing PostgreSQL for bulk import...");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER SYSTEM SET work_mem = '256MB'");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER SYSTEM SET effective_cache_size = '2GB'");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER SYSTEM SET wal_level = minimal");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER SYSTEM SET fsync = off");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER SYSTEM SET synchronous_commit = off");
        await dbContext.Database.ExecuteSqlRawAsync("SELECT pg_reload_conf()");

        // Disable foreign key constraints for bulk import
        Console.WriteLine("Disabling foreign key constraints for faster import...");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmTags\" DISABLE TRIGGER ALL");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"WayNodes\" DISABLE TRIGGER ALL");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"RelationMembers\" DISABLE TRIGGER ALL");

        // Disable indexes for faster import
        Console.WriteLine("Disabling indexes for faster import...");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmNode\" DISABLE TRIGGER ALL");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmWay\" DISABLE TRIGGER ALL");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmRelation\" DISABLE TRIGGER ALL");

        // Disabling all constraints...
        Console.WriteLine("Disabling all constraints...");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"WayNodes\" DROP CONSTRAINT IF EXISTS \"FK_WayNodes_OsmWay_WayId\"");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"WayNodes\" DROP CONSTRAINT IF EXISTS \"FK_WayNodes_OsmNode_NodeId\"");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"RelationMembers\" DROP CONSTRAINT IF EXISTS \"FK_RelationMembers_OsmRelation_RelationId\"");

        // Import data from PBF
        Console.WriteLine();
        Console.WriteLine("Reading PBF file...");

        // Subscribe to the OnProgress event
        importer.OnProgress += (itemsProcessed, nodesInBatch, waysInBatch, relationsInBatch, tagsInBatch) =>
        {
            Console.WriteLine($"Progress: {itemsProcessed:N0} items processed");
            Console.WriteLine($"  Nodes: {nodesInBatch:N0} | Ways: {waysInBatch:N0} | Relations: {relationsInBatch:N0} | Tags: {tagsInBatch:N0}");
            Console.Out.Flush();
        };

        await importer.ImportFromPbfStreamingAsync(pbfFilePath, databaseOps);

        // Re-enable indexes after import
        Console.WriteLine();
        Console.WriteLine("Rebuilding indexes...");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmNode\" ENABLE TRIGGER ALL");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmWay\" ENABLE TRIGGER ALL");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmRelation\" ENABLE TRIGGER ALL");

        // Re-enable write-ahead logging
        Console.WriteLine("Restoring PostgreSQL settings...");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER SYSTEM SET wal_level = replica");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER SYSTEM SET fsync = on");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER SYSTEM SET synchronous_commit = on");
        await dbContext.Database.ExecuteSqlRawAsync("SELECT pg_reload_conf()");

        // Re-enable foreign key constraints
        Console.WriteLine("Re-enabling foreign key constraints...");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"OsmTags\" ENABLE TRIGGER ALL");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"WayNodes\" ENABLE TRIGGER ALL");
        await dbContext.Database.ExecuteSqlRawAsync("ALTER TABLE \"RelationMembers\" ENABLE TRIGGER ALL");

        // Rebuilding constraints...
        Console.WriteLine("Rebuilding constraints...");
        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""WayNodes"" ADD CONSTRAINT ""FK_WayNodes_OsmWay_WayId"" 
            FOREIGN KEY (""WayId"") REFERENCES ""OsmWay"" (""Id"") ON DELETE CASCADE;
            ALTER TABLE ""WayNodes"" ADD CONSTRAINT ""FK_WayNodes_OsmNode_NodeId"" 
            FOREIGN KEY (""NodeId"") REFERENCES ""OsmNode"" (""Id"") ON DELETE CASCADE;
            ALTER TABLE ""RelationMembers"" ADD CONSTRAINT ""FK_RelationMembers_OsmRelation_RelationId"" 
            FOREIGN KEY (""RelationId"") REFERENCES ""OsmRelation"" (""Id"") ON DELETE CASCADE;
        ");

        // Verify referential integrity (optional but recommended)
        Console.WriteLine("Verifying referential integrity...");
        await dbContext.Database.ExecuteSqlRawAsync("REINDEX DATABASE historicalmap");

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
    var downloadsPath = Directory.GetCurrentDirectory();

    var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
    var logFilePath = Path.Combine(downloadsPath, $"DataImporter_Error_{timestamp}.txt");

    try
    {
        using (var writer = new StreamWriter(logFilePath, append: false))
        {
            writer.WriteLine("=== DataImporter Error Log ===");
            writer.WriteLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine($"Command: {command} {pbfFilePath}");
            writer.WriteLine();

            // Exception details
            writer.WriteLine("=== Exception ===");
            writer.WriteLine($"Message: {ex.Message}");
            writer.WriteLine($"Type: {ex.GetType().FullName}");
            writer.WriteLine();

            // Stack trace
            writer.WriteLine("=== Stack Trace ===");
            writer.WriteLine(ex.StackTrace);
            writer.WriteLine();

            // Inner exception
            if (ex.InnerException != null)
            {
                writer.WriteLine("=== Inner Exception ===");
                writer.WriteLine($"Message: {ex.InnerException.Message}");
                writer.WriteLine($"Type: {ex.InnerException.GetType().FullName}");
                writer.WriteLine($"Stack Trace: {ex.InnerException.StackTrace}");
                writer.WriteLine();
            }

            // System information
            writer.WriteLine("=== System Information ===");
            writer.WriteLine($"OS: {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
            writer.WriteLine($".NET Runtime: {System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription}");
            writer.WriteLine($"Machine: {Environment.MachineName}");
            writer.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
            writer.WriteLine();

            // Database info
            writer.WriteLine("=== Database Info ===");
            writer.WriteLine($"Connection String (masked): User ID=***, Host={GetHostFromConnectionString(connectionString)}");
            writer.WriteLine();

            // PBF file info
            writer.WriteLine("=== PBF File Info ===");
            if (File.Exists(pbfFilePath))
            {
                var fileInfo = new FileInfo(pbfFilePath);
                writer.WriteLine($"Path: {pbfFilePath}");
                writer.WriteLine($"Size: {fileInfo.Length:N0} bytes ({(fileInfo.Length / 1024.0 / 1024.0 / 1024.0):F2} GB)");
                writer.WriteLine($"Last Modified: {fileInfo.LastWriteTime:yyyy-MM-dd HH:mm:ss}");
            }
            else
            {
                writer.WriteLine($"Path: {pbfFilePath}");
                writer.WriteLine("Status: FILE NOT FOUND");
            }

            writer.Flush();
        }

        Console.WriteLine($"✓ Error logged to: {logFilePath}");
    }
    catch (Exception logEx)
    {
        Console.WriteLine($"Failed to write error log: {logEx.Message}");
    }

    Console.WriteLine($"Error: {ex.Message}");
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Details: {ex.InnerException.Message}");
    }

    Environment.Exit(1);
}

// Helper method to extract host from connection string
static string GetHostFromConnectionString(string connectionString)
{
    var hostMatch = System.Text.RegularExpressions.Regex.Match(connectionString, @"Host=([^;]+)");
    return hostMatch.Success ? hostMatch.Groups[1].Value : "Unknown";
}
