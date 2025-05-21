param(
    [Parameter(Mandatory=$true)]
    [string]$ConnectionString,
    
    [Parameter(Mandatory=$false)]
    [string]$MigrationName = "InitialCreate"
)

# Ensure the output directories exist
New-Item -ItemType Directory -Force -Path "Models/DbEntities"
New-Item -ItemType Directory -Force -Path "DAL"

# Generate the entities
dotnet ef dbcontext scaffold $ConnectionString Microsoft.EntityFrameworkCore.SqlServer `
    -o Models/DbEntities `
    -c GestionTransporteDbContext `
    --context-dir DAL `
    --namespace GestionTransporte.Models.DbEntities `
    --context-namespace GestionTransporte.DAL `
    --force

# Create and apply the migration
dotnet ef migrations add $MigrationName
dotnet ef database update

Write-Host "Entity generation and migration completed successfully!" 