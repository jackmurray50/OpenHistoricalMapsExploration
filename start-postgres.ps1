# Check if container exists
if (docker ps -a --format "{{.Names}}" | Select-String "historical-map-db") {
    Write-Host "Container exists. Starting..."
    docker start historical-map-db
} else {
    Write-Host "Creating new container..."
    docker run --name historical-map-db `
      -e POSTGRES_PASSWORD=postgres `
      -e POSTGRES_DB=historicalmap `
      -e POSTGRES_INITDB_ARGS="-c wal_level=replica" `
      -v "F:/postgres-historical-map:/var/lib/postgresql" `
      -p 5432:5432 `
      --memory="8g" `
      --cpus="6" `
      -d postgres:latest

    if ($LASTEXITCODE -eq 0) {
        Write-Host "PostgreSQL container created successfully"
    } else {
        Write-Host "Failed to create container"
    }
}

Write-Host "PostgreSQL is running at localhost:5432"
docker ps --filter name=historical-map-db
