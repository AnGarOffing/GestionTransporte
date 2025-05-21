# Implementación de Gestión de Tipos de Identificación

Este documento detalla la implementación del módulo de gestión de tipos de identificación en el sistema de Gestión de Transporte.

## Estructura de Archivos

La implementación consta de los siguientes archivos:

1. `Controllers/TipoIdentificacionController.cs`
2. `Models/ViewModels/TipoIdentificacionViewModel.cs`
3. `Views/TipoIdentificacion/Index.cshtml`
4. `Views/TipoIdentificacion/Edit.cshtml`

## Modelo de Datos

### Entidad TipoIdentificacion
```csharp
public class TipoIdentificacion
{
    public int IdTipoIdentificacion { get; set; }
    public string NombreTipoIdentificacion { get; set; } = string.Empty;
}
```

### Contexto de Base de Datos
En el `GestionTransporteDbContext`, la entidad se define como:
```csharp
public virtual DbSet<TipoIdentificacion> TipoIdentificacion { get; set; }
```

La propiedad es `virtual` para permitir:
- Sobrescritura en clases derivadas
- Funcionamiento correcto de los proxies de Entity Framework
- Facilidad en pruebas unitarias
- Compatibilidad con características como lazy loading

## Modelo de Vista (ViewModel)

El modelo de vista `TipoIdentificacionViewModel` se utiliza para transferir datos entre el controlador y las vistas:

```csharp
public class TipoIdentificacionViewModel
{
    public int IdTipoIdentificacion { get; set; }

    [DisplayName("Tipo de Identificacion")]
    [Required(ErrorMessage = "El nombre del tipo de identificación es requerido")]
    public string NombreTipoIdentificacion { get; set; } = string.Empty;
}
```

Características del ViewModel:
- `IdTipoIdentificacion`: Identificador único del tipo de identificación
- `NombreTipoIdentificacion`: Nombre del tipo de identificación (requerido)
- Incluye validación de datos y mensajes de error en español
- Utiliza Data Annotations para validación y presentación

## Controlador

El controlador `TipoIdentificacionController` implementa el patrón Repository y maneja todas las operaciones CRUD:

### Constructor y Dependencias
```csharp
private readonly GestionTransporteDbContext _context;

public TipoIdentificacionController(GestionTransporteDbContext context)
{
    _context = context;
}
```

### Acciones Principales

1. **Index (Listar)**
```csharp
public async Task<IActionResult> Index()
{
    var tiposIdentificacion = await _context.TipoIdentificacion
        .Select(t => new TipoIdentificacionViewModel
        {
            IdTipoIdentificacion = t.IdTipoIdentificacion,
            NombreTipoIdentificacion = t.NombreTipoIdentificacion
        })
        .ToListAsync();

    return View(tiposIdentificacion);
}
```

2. **Edit (Editar)**
```csharp
public async Task<IActionResult> Edit(int id)
{
    var tipoIdentificacion = await _context.TipoIdentificacion.FindAsync(id);
    if (tipoIdentificacion == null)
    {
        return NotFound();
    }

    var viewModel = new TipoIdentificacionViewModel
    {
        IdTipoIdentificacion = tipoIdentificacion.IdTipoIdentificacion,
        NombreTipoIdentificacion = tipoIdentificacion.NombreTipoIdentificacion
    };

    return View(viewModel);
}
```

3. **Edit POST (Guardar Cambios)**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, TipoIdentificacionViewModel viewModel)
{
    if (id != viewModel.IdTipoIdentificacion)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        try
        {
            var tipoIdentificacion = await _context.TipoIdentificacion.FindAsync(id);
            if (tipoIdentificacion == null)
            {
                return NotFound();
            }

            tipoIdentificacion.NombreTipoIdentificacion = viewModel.NombreTipoIdentificacion;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TipoIdentificacionExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }
    return View(viewModel);
}
```

4. **Delete (Eliminar)**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
{
    var tipoIdentificacion = await _context.TipoIdentificacion.FindAsync(id);
    if (tipoIdentificacion == null)
    {
        return NotFound();
    }

    _context.TipoIdentificacion.Remove(tipoIdentificacion);
    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(Index));
}
```

## Vistas

### Index.cshtml (Lista de Tipos de Identificación)

La vista principal muestra una tabla con todos los tipos de identificación y sus acciones:

```html
@model IEnumerable<GestionTransporte.Models.ViewModels.TipoIdentificacionViewModel>

<div class="container mt-4">
    <div class="row">
        <div class="col-md-12">
            <h2 class="mb-4">Tipos de Identificación</h2>
            
            <div class="table-responsive">
                <table class="table table-hover">
                    <thead class="thead-dark">
                        <tr>
                            <th>@Html.DisplayNameFor(model => model.NombreTipoIdentificacion)</th>
                            <th>Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var item in Model)
                        {
                            <tr>
                                <td>@Html.DisplayFor(modelItem => item.NombreTipoIdentificacion)</td>
                                <td>
                                    <a asp-action="Edit" asp-route-id="@item.IdTipoIdentificacion" class="btn btn-primary btn-sm">
                                        <i class="fas fa-edit"></i> Editar
                                    </a>
                                    <button type="button" class="btn btn-danger btn-sm" 
                                            onclick="confirmarEliminacion(@item.IdTipoIdentificacion)">
                                        <i class="fas fa-trash"></i> Eliminar
                                    </button>
                                </td>
                            </tr>
                        }
                    </tbody>
                </table>
            </div>

            <div class="text-end mt-3">
                <a asp-action="Create" class="btn btn-success">
                    <i class="fas fa-plus"></i> Nuevo Tipo
                </a>
            </div>
        </div>
    </div>
</div>
```

### Edit.cshtml (Formulario de Edición)

Formulario para editar un tipo de identificación existente:

```html
@model GestionTransporte.Models.ViewModels.TipoIdentificacionViewModel

<div class="container mt-4">
    <div class="row">
        <div class="col-md-8 offset-md-2">
            <h2 class="mb-4">Editar Tipo de Identificación</h2>

            <form asp-action="Edit">
                <div asp-validation-summary="ModelOnly" class="text-danger"></div>
                <input type="hidden" asp-for="IdTipoIdentificacion" />

                <div class="form-group mb-3">
                    <label asp-for="NombreTipoIdentificacion" class="control-label"></label>
                    <input asp-for="NombreTipoIdentificacion" class="form-control" />
                    <span asp-validation-for="NombreTipoIdentificacion" class="text-danger"></span>
                </div>

                <div class="form-group">
                    <button type="submit" class="btn btn-primary">
                        <i class="fas fa-save"></i> Guardar
                    </button>
                    <a asp-action="Index" class="btn btn-secondary">
                        <i class="fas fa-arrow-left"></i> Volver
                    </a>
                </div>
            </form>
        </div>
    </div>
</div>
```

## Características Implementadas

1. **Operaciones CRUD Completas**
   - Listar tipos de identificación
   - Editar tipos existentes
   - Eliminar tipos (con confirmación)
   - Validación de datos

2. **Interfaz de Usuario**
   - Diseño responsive con Bootstrap
   - Iconos de Font Awesome
   - Tabla con acciones por fila
   - Formularios con validación

3. **Seguridad**
   - Tokens anti-falsificación
   - Validación de datos del lado del servidor
   - Confirmación antes de eliminar

4. **Validación**
   - Campos requeridos
   - Mensajes de error en español
   - Validación del lado del cliente y servidor

## Requisitos Técnicos

Para que esta implementación funcione correctamente, se necesitan:

1. **Paquetes NuGet**
   - Microsoft.EntityFrameworkCore
   - Microsoft.EntityFrameworkCore.SqlServer
   - Bootstrap
   - Font Awesome

2. **Configuración de Base de Datos**
   - Cadena de conexión configurada en `appsettings.json`
   - Contexto de base de datos registrado en `Program.cs`

3. **Layout**
   - Referencias a Bootstrap y Font Awesome en el layout principal
   - Sección de scripts para validación del lado del cliente

## Consideraciones Adicionales

1. **Manejo de Errores**
   - Se implementa manejo de errores para casos de concurrencia
   - Se verifica la existencia de registros antes de operaciones
   - Se muestran mensajes de error apropiados

2. **Rendimiento**
   - Uso de operaciones asíncronas
   - Consultas optimizadas con LINQ
   - Carga eficiente de datos

3. **Mantenibilidad**
   - Código organizado y comentado
   - Separación clara de responsabilidades
   - Uso de ViewModels para transferencia de datos

4. **Buenas Prácticas**
   - Uso de async/await para operaciones de base de datos
   - Implementación de validación en múltiples capas
   - Separación de responsabilidades (SRP)
   - Código limpio y mantenible 