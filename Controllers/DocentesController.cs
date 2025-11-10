using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServicioSocial.Data;
using ServicioSocial.Models;
using Microsoft.AspNetCore.Identity;

namespace ServicioSocial.Controllers
{
    [Authorize(Roles = "Docente,Admin")]
    public class DocentesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DocentesController> _logger;

        public DocentesController(AppDbContext context, UserManager<ApplicationUser> userManager, ILogger<DocentesController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: Docentes/ActividadesPendientes - Vista principal para docentes
        public async Task<IActionResult> ActividadesPendientes()
        {
            var actividades = await _context.Actividades
                .Include(a => a.Usuario)
                .Where(a => a.Estado == "Pendiente")
                .OrderBy(a => a.Fecha)
                .ToListAsync();

            return View(actividades);
        }

        // GET: Docentes/DetallesActividad/5
        public async Task<IActionResult> DetallesActividad(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actividad = await _context.Actividades
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (actividad == null)
            {
                return NotFound();
            }

            return View(actividad);
        }

        // POST: Docentes/AprobarActividad/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AprobarActividad(int id, string comentario)
        {
            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            actividad.Estado = "Aprobado";
            actividad.ComentarioDocente = comentario;
            actividad.DocenteAprobadorId = _userManager.GetUserId(User);

            _context.Update(actividad);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Actividad aprobada exitosamente!";
            return RedirectToAction(nameof(ActividadesPendientes));
        }

        // POST: Docentes/RechazarActividad/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RechazarActividad(int id, string comentario)
        {
            if (string.IsNullOrEmpty(comentario))
            {
                TempData["Error"] = "El comentario es requerido al rechazar una actividad.";
                return RedirectToAction(nameof(DetallesActividad), new { id });
            }

            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            actividad.Estado = "Rechazado";
            actividad.ComentarioDocente = comentario;
            actividad.DocenteAprobadorId = _userManager.GetUserId(User);

            _context.Update(actividad);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Actividad rechazada.";
            return RedirectToAction(nameof(ActividadesPendientes));
        }

        // GET: Docentes/HistorialActividades
        public async Task<IActionResult> HistorialActividades()
        {
            var actividades = await _context.Actividades
                .Include(a => a.Usuario)
                .Where(a => a.Estado != "Pendiente")
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();

            return View(actividades);
        }

        // GET: Docentes/Estadisticas
        // GET: Docentes/Estadisticas
        public async Task<IActionResult> Estadisticas()
        {
            try
            {
                // Obtener estadísticas de la base de datos
                var totalActividades = await _context.Actividades.CountAsync();
                var actividadesPendientes = await _context.Actividades.CountAsync(a => a.Estado == "Pendiente");
                var actividadesAprobadas = await _context.Actividades.CountAsync(a => a.Estado == "Aprobado");
                var actividadesRechazadas = await _context.Actividades.CountAsync(a => a.Estado == "Rechazado");
                var totalHorasAprobadas = await _context.Actividades
                    .Where(a => a.Estado == "Aprobado")
                    .SumAsync(a => a.HorasDedicadas);

                // Crear objeto anónimo con las estadísticas
                var estadisticas = new
                {
                    TotalActividades = totalActividades,
                    ActividadesPendientes = actividadesPendientes,
                    ActividadesAprobadas = actividadesAprobadas,
                    ActividadesRechazadas = actividadesRechazadas,
                    TotalHorasAprobadas = totalHorasAprobadas
                };

                // Pasar las estadísticas a la vista usando ViewBag o ViewData
                ViewBag.TotalActividades = totalActividades;
                ViewBag.ActividadesPendientes = actividadesPendientes;
                ViewBag.ActividadesAprobadas = actividadesAprobadas;
                ViewBag.ActividadesRechazadas = actividadesRechazadas;
                ViewBag.TotalHorasAprobadas = totalHorasAprobadas;

                return View(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar las estadísticas");
                TempData["Error"] = "Error al cargar las estadísticas";
                return RedirectToAction("ActividadesPendientes");
            }
        }

        // Métodos originales para gestión de docentes (solo Admin)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Docentes.Include(d => d.ApplicationUser);
            return View(await appDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docente = await _context.Docentes
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docente == null)
            {
                return NotFound();
            }

            return View(docente);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Mostrar solo usuarios que tienen rol de Docente pero no están en la tabla Docentes
            var usuariosDocentes = _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Docente")))
                .Where(u => !_context.Docentes.Any(d => d.ApplicationUserId == u.Id))
                .Select(u => new { u.Id, NombreCompleto = u.FullName });

            ViewData["ApplicationUserId"] = new SelectList(usuariosDocentes, "Id", "NombreCompleto");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,Departamento")] Docente docente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(docente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var usuariosDocentes = _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Docente")))
                .Select(u => new { u.Id, NombreCompleto = u.FullName });

            ViewData["ApplicationUserId"] = new SelectList(usuariosDocentes, "Id", "NombreCompleto", docente.ApplicationUserId);
            return View(docente);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docente = await _context.Docentes.FindAsync(id);
            if (docente == null)
            {
                return NotFound();
            }

            var usuariosDocentes = _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Docente")))
                .Select(u => new { u.Id, NombreCompleto = u.FullName });

            ViewData["ApplicationUserId"] = new SelectList(usuariosDocentes, "Id", "NombreCompleto", docente.ApplicationUserId);
            return View(docente);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,Departamento")] Docente docente)
        {
            if (id != docente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(docente);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocenteExists(docente.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var usuariosDocentes = _context.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                    _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Docente")))
                .Select(u => new { u.Id, NombreCompleto = u.FullName });

            ViewData["ApplicationUserId"] = new SelectList(usuariosDocentes, "Id", "NombreCompleto", docente.ApplicationUserId);
            return View(docente);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docente = await _context.Docentes
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docente == null)
            {
                return NotFound();
            }

            return View(docente);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);
            if (docente != null)
            {
                _context.Docentes.Remove(docente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocenteExists(int id)
        {
            return _context.Docentes.Any(e => e.Id == id);
        }
    }
}