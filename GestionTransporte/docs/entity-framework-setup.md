# Entity Framework Core Setup and Configuration

## Explicación

### ¿Qué es Entity Framework Core?
Entity Framework Core (EF Core) es un mapeador objeto-relacional (ORM) que permite a los desarrolladores trabajar con una base de datos usando objetos .NET. En términos simples, te permite interactuar con tu base de datos usando código C# en lugar de escribir consultas SQL directamente.

### Estructura del Proyecto
La estructura del proyecto está organizada siguiendo los principios de arquitectura limpia:

1. **DAL (Data Access Layer)**: 
   - Contiene todo lo relacionado con la base de datos
   - Aquí va el `GestionTransporteDbContext.cs`
   - Es como el "intermediario" entre tu código y la base de datos

2. **Models**:
   - `DbEntities`: Clases que representan las tablas de tu base de datos
   - `ViewModels`: Clases que representan los datos que mostrarás en tus vistas

### ¿Cómo Funciona?
1. **Generación de Entidades**:
   - El script `generate-entities.ps1` crea automáticamente las clases C# basadas en tu base de datos
   - Estas clases se guardan en `Models/DbEntities`
   - El contexto de la base de datos se guarda en `DAL`

2. **Configuración**:
   - El archivo `efpt.config.json` le dice a EF Core:
     - Dónde guardar los archivos
     - Cómo nombrar las clases
     - Qué configuración usar

3. **Uso**:
   - Para generar las clases: ejecuta el script PowerShell
   - Para actualizar la base de datos: usa los comandos de migración

### Migraciones en Entity Framework Core

#### ¿Qué son las Migraciones?
Las migraciones son como un sistema de control de versiones para tu base de datos. Te permiten:
- Crear la base de datos inicial
- Actualizar el esquema de la base de datos
- Revertir cambios si es necesario

#### Comandos de Migración
1. **Crear una nueva migración**:
   ```powershell
   dotnet ef migrations add NombreDeLaMigracion
   ```
   Este comando:
   - Crea una nueva carpeta `Migrations` si no existe
   - Genera archivos con el código necesario para actualizar la base de datos
   - Guarda un "snapshot" del estado actual de tu modelo

2. **Actualizar la base de datos**:
   ```powershell
   dotnet ef database update
   ```
   Este comando:
   - Aplica todas las migraciones pendientes
   - Actualiza la base de datos al último estado

3. **Revertir la última migración**:
   ```powershell
   dotnet ef database update NombreDeLaMigracionAnterior
   ```
   O para eliminar la última migración:
   ```powershell
   dotnet ef migrations remove
   ```

4. **Ver el estado de las migraciones**:
   ```powershell
   dotnet ef migrations list
   ```

#### Buenas Prácticas con Migraciones
1. **Antes de crear una migración**:
   - Asegúrate de que tu código compila
   - Haz un respaldo de la base de datos
   - Revisa los cambios que vas a aplicar

2. **Al nombrar migraciones**:
   - Usa nombres descriptivos
   - Incluye la fecha si es relevante
   - Ejemplo: `AddTipoIdentificacionTable_20240511`

3. **Después de aplicar migraciones**:
   - Verifica que la base de datos se actualizó correctamente
   - Prueba las nuevas funcionalidades
   - Documenta los cambios importantes

### Ejemplo Práctico
Si tienes una tabla `Tipo_Identificacion` en tu base de datos:
1. EF Core creará una clase `TipoIdentificacion` en `Models/DbEntities`
2. Podrás usar esta clase en tu código así:
```csharp
// Crear un nuevo tipo de identificación
var nuevoTipo = new TipoIdentificacion 
{ 
    NombreTipoIdentificacion = "DNI" 
};

// Guardar en la base de datos
dbContext.TipoIdentificacions.Add(nuevoTipo);
dbContext.SaveChanges();
```

### Consejos Importantes
1. **Seguridad**:
   - Nunca guardes la cadena de conexión en el código
   - Usa el archivo `appsettings.json` para las configuraciones
   - Considera usar Azure Key Vault o similar para producción

2. **Mantenimiento**:
   - Revisa las clases generadas antes de usarlas
   - Haz respaldo de la base de datos antes de ejecutar migraciones
   - Mantén un registro de los cambios en la base de datos

3. **Buenas Prácticas**:
   - Mantén la estructura de carpetas organizada
   - Usa nombres descriptivos para las clases y propiedades
   - Documenta los cambios importantes
   - Sigue el principio de responsabilidad única
   - Mantén las entidades lo más simples posible

4. **Optimización**:
   - Usa índices apropiadamente
   - Considera el rendimiento al diseñar las relaciones
   - Evalúa el uso de consultas compiladas para operaciones frecuentes

### Solución de Problemas Comunes
1. **Error de conexión**:
   - Verifica la cadena de conexión
   - Asegúrate de que el servidor está accesible
   - Comprueba los permisos de la base de datos

2. **Errores de migración**:
   - Revisa el historial de migraciones
   - Verifica que no hay conflictos
   - Considera hacer un respaldo antes de revertir cambios

3. **Problemas de rendimiento**:
   - Revisa las consultas generadas
   - Considera usar `AsNoTracking()` cuando sea apropiado
   - Evalúa el uso de índices

### Comandos para Database-First con la Nueva Estructura

#### Usando PowerShell
```powershell
# Generar entidades y contexto desde la base de datos
dotnet ef dbcontext scaffold "Server=tu_servidor;Database=tu_base_datos;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer `
    -o Models/DbEntities `
    -c GestionTransporteDbContext `
    --context-dir DAL `
    --namespace GestionTransporte.Models.DbEntities `
    --context-namespace GestionTransporte.DAL `
    --force

# Crear una nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Actualizar la base de datos
dotnet ef database update
```

#### Usando el Script PowerShell (Recomendado)
```powershell
# Ejecutar el script de generación
.\scripts\generate-entities.ps1 -ConnectionString "Server=tu_servidor;Database=tu_base_datos;Trusted_Connection=True;TrustServerCertificate=True;" -MigrationName "NombreDeLaMigracion"
```

#### Explicación de los Parámetros
- `-o Models/DbEntities`: Genera las entidades en la carpeta Models/DbEntities
- `-c GestionTransporteDbContext`: Nombre de la clase del contexto
- `--context-dir DAL`: Coloca el DbContext en la carpeta DAL
- `--namespace GestionTransporte.Models.DbEntities`: Namespace para las entidades
- `--context-namespace GestionTransporte.DAL`: Namespace para el DbContext
- `--force`: Sobrescribe los archivos existentes

#### Ejemplo de Uso
1. **Primera vez que generas las entidades**:
   ```powershell
   dotnet ef dbcontext scaffold "Server=tu_servidor;Database=tu_base_datos;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer `
       -o Models/DbEntities `
       -c GestionTransporteDbContext `
       --context-dir DAL `
       --namespace GestionTransporte.Models.DbEntities `
       --context-namespace GestionTransporte.DAL `
       --force
   ```

2. **Cuando haces cambios en la base de datos**:
   ```powershell
   # Primero, elimina las migraciones existentes si es necesario
   dotnet ef migrations remove

   # Luego, regenera las entidades
   dotnet ef dbcontext scaffold "Server=tu_servidor;Database=tu_base_datos;Trusted_Connection=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer `
       -o Models/DbEntities `
       -c GestionTransporteDbContext `
       --context-dir DAL `
       --namespace GestionTransporte.Models.DbEntities `
       --context-namespace GestionTransporte.DAL `
       --force

   # Crea una nueva migración
   dotnet ef migrations add ActualizacionTablas
   ```

#### Notas Importantes
1. **Antes de ejecutar los comandos**:
   - Asegúrate de estar en el directorio raíz del proyecto
   - Verifica que tienes los paquetes NuGet necesarios instalados
   - Haz un respaldo de la base de datos

2. **Después de ejecutar los comandos**:
   - Revisa las clases generadas en `Models/DbEntities`
   - Verifica el DbContext generado en `DAL`
   - Comprueba que los namespaces son correctos

3. **Solución de problemas comunes**:
   - Si hay errores de compilación, verifica los namespaces
   - Si hay problemas de conexión, verifica la cadena de conexión
   - Si hay conflictos, considera eliminar las migraciones existentes

## Project Structure
```
GestionTransporte/
├── Controllers/
├── DAL/                    # Data Access Layer
│   └── GestionTransporteDbContext.cs
├── Models/                 # Domain Models
│   ├── DbEntities/        # Database Entities
│   └── ViewModels/        # View Models
├── Views/
└── wwwroot/
```

## Configuration Files

### 1. efpt.config.json
Este archivo es la configuración para las Entity Framework Power Tools (EFPT). Las EFPT son herramientas que ayudan a:
- Generar código a partir de una base de datos existente
- Realizar ingeniería inversa de la base de datos
- Configurar cómo se generarán las clases y el contexto

El nombre "efpt" viene de:
- **EF**: Entity Framework
- **PT**: Power Tools

Este archivo de configuración es importante porque:
1. Define la estructura de carpetas para las clases generadas
2. Establece los namespaces que se usarán
3. Configura cómo se generarán las clases (usando Fluent API o atributos)
4. Determina el comportamiento de la generación de código
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Name=DefaultConnection"
  },
  "ContextClassName": "GestionTransporteDbContext",
  "ContextNamespace": "GestionTransporte.DAL",
  "ContextDir": "DAL",
  "DefaultDacpacSchema": null,
  "DoNotCombineNamespace": false,
  "IdReplace": false,
  "IncludeConnectionString": true,
  "ModelNamespace": "GestionTransporte.Models.DbEntities",
  "OutputPath": "Models/DbEntities",
  "ProjectRootNamespace": "GestionTransporte",
  "SelectedHandlebarsLanguage": 0,
  "SelectedToBeGenerated": 0,
  "Tables": [],
  "UseDatabaseNames": false,
  "UseFluentApiOnly": true,
  "UseHandleBars": false,
  "UseInflector": false,
  "UseLegacyPluralizer": false,
  "UseSpatial": false
}
```

### 2. Entity Classes

#### GestionTransporteDbContext.cs
```csharp
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using GestionTransporte.Models.DbEntities;

namespace GestionTransporte.DAL;

public partial class GestionTransporteDbContext : DbContext
{
    public GestionTransporteDbContext()
    {
    }

    public GestionTransporteDbContext(DbContextOptions<GestionTransporteDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TipoIdentificacion> TipoIdentificacions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoIdentificacion>(entity =>
        {
            entity.HasKey(e => e.IdTipoIdentificacion);

            entity.ToTable("Tipo_Identificacion");

            entity.Property(e => e.NombreTipoIdentificacion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
```

#### TipoIdentificacion.cs
```csharp
using System;
using System.Collections.Generic;

namespace GestionTransporte.Models.DbEntities;

public partial class TipoIdentificacion
{
    public int IdTipoIdentificacion { get; set; }

    public string NombreTipoIdentificacion { get; set; } = null!;
}
```

## PowerShell Script for Entity Generation

### generate-entities.ps1
```powershell
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
```

## Usage Instructions

1. **Generate Entities and Run Migrations**
   ```powershell
   .\scripts\generate-entities.ps1 -ConnectionString "Your_Connection_String" -MigrationName "YourMigrationName"
   ```

2. **Manual Commands**
   ```powershell
   # Create a new migration
   dotnet ef migrations add MigrationName

   # Update the database
   dotnet ef database update
   ```

## Important Notes

1. The project follows clean architecture principles:
   - DAL (Data Access Layer) contains database-related code
   - Models contains domain models and view models
   - Clear separation of concerns

2. Entity Framework Configuration:
   - Uses Fluent API for configuration
   - Entities are generated in Models/DbEntities
   - DbContext is placed in DAL folder
   - Proper namespacing is maintained

3. Best Practices:
   - Keep connection strings secure
   - Review generated entities before applying migrations
   - Back up database before running migrations in production 