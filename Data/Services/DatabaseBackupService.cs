namespace Data.Services;

/// <summary>
/// Service for backing up and managing the SQLite database file.
/// </summary>
public class DatabaseBackupService
{
    private readonly string _databasePath;
    private readonly string _backupDirectory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseBackupService"/> class.
    /// </summary>
    /// <param name="connectionString">The SQLite connection string.</param>
    /// <param name="backupDirectory">Directory to store backups. Defaults to "Backups" folder in current directory.</param>
    public DatabaseBackupService(string connectionString, string backupDirectory = "Backups")
    {
        // Extract the database path from the connection string
        // Format: "Data Source=HistoricalMap.db"
        var parts = connectionString.Split('=');
        _databasePath = parts.Length > 1 ? parts[1].Trim() : "HistoricalMap.db";

        _backupDirectory = backupDirectory;

        // Create backup directory if it doesn't exist
        if (!Directory.Exists(_backupDirectory))
        {
            Directory.CreateDirectory(_backupDirectory);
        }
    }

    /// <summary>
    /// Creates a backup of the current database.
    /// </summary>
    /// <returns>The path to the created backup file, or null if no database exists to backup.</returns>
    public string? CreateBackup()
    {
        if (!File.Exists(_databasePath))
        {
            return null;
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupFileName = $"HistoricalMap_{timestamp}.db.backup";
        var backupPath = Path.Combine(_backupDirectory, backupFileName);

        // Copy the database file
        File.Copy(_databasePath, backupPath, overwrite: true);

        return backupPath;
    }

    /// <summary>
    /// Gets all available backups ordered by creation date (newest first).
    /// </summary>
    /// <returns>A list of backup file paths.</returns>
    public List<string> ListBackups()
    {
        if (!Directory.Exists(_backupDirectory))
        {
            return new List<string>();
        }

        return Directory.GetFiles(_backupDirectory, "HistoricalMap_*.db.backup")
            .OrderByDescending(f => File.GetLastWriteTime(f))
            .ToList();
    }

    /// <summary>
    /// Restores the database from a backup file.
    /// </summary>
    /// <param name="backupPath">Path to the backup file to restore.</param>
    /// <returns>True if restoration was successful, false otherwise.</returns>
    public bool RestoreFromBackup(string backupPath)
    {
        if (!File.Exists(backupPath))
        {
            return false;
        }

        // Close any existing connections by deleting the current database
        if (File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }

        // Copy backup to current database
        File.Copy(backupPath, _databasePath);

        return true;
    }

    /// <summary>
    /// Deletes a backup file.
    /// </summary>
    /// <param name="backupPath">Path to the backup file to delete.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    public bool DeleteBackup(string backupPath)
    {
        if (!File.Exists(backupPath))
        {
            return false;
        }

        File.Delete(backupPath);
        return true;
    }
}
