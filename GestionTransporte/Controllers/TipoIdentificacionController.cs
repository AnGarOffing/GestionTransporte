using Microsoft.AspNetCore.Mvc;
using GestionTransporte.DAL;
using GestionTransporte.Models.DbEntities;
using GestionTransporte.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace GestionTransporte.Controllers
{
    public class TipoIdentificacionController : Controller
    {
        private readonly GestionTransporteDbContext _context;
        private readonly ILogger<TipoIdentificacionController> _logger;

        public TipoIdentificacionController(GestionTransporteDbContext context, ILogger<TipoIdentificacionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Mensaje desde el logger");
                var tiposIdentificacion = await _context.TipoIdentificacion
                    .Select(t => new TipoIdentificacionViewModel
                    {
                        IdTipoIdentificacion = t.IdTipoIdentificacion,
                        NombreTipoIdentificacion = t.NombreTipoIdentificacion
                    })
                    .ToListAsync();

                return View(tiposIdentificacion);
            }
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
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
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
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
            catch (Exception ex)
            {
                // Log the exception
                return View("Error");
            }
        }

        private bool TipoIdentificacionExists(int id)
        {
            return _context.TipoIdentificacion.Any(e => e.IdTipoIdentificacion == id);
        }
    }
}
