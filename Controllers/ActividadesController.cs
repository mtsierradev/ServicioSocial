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
    [Authorize]
    public class ActividadesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActividadesController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Actividades
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var actividades = await _context.Actividades
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();

            var totalHoras = actividades.Where(a => a.Estado == "Aprobado").Sum(a => a.HorasDedicadas);
            ViewBag.TotalHorasAprobadas = totalHoras;
            ViewBag.MaximoHoras = 80;

            return View(actividades);
        }

        // GET: Actividades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var actividad = await _context.Actividades.FirstOrDefaultAsync(m => m.Id == id);
            if (actividad == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (actividad.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Docente"))
            {
                return Forbid();
            }

            return View(actividad);
        }

        // GET: Actividades/Create
        [Authorize(Roles = "User")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actividades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create([Bind("Descripcion,Fecha,HorasDedicadas")] Actividad actividad)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);

                // Validación de 80 horas
                var horasTotalesAprobadas = await _context.Actividades
                    .Where(a => a.UserId == userId && a.Estado == "Aprobado")
                    .SumAsync(a => a.HorasDedicadas);

                var horasPendientes = await _context.Actividades
                    .Where(a => a.UserId == userId && a.Estado == "Pendiente")
                    .SumAsync(a => a.HorasDedicadas);

                var totalHorasActual = horasTotalesAprobadas + horasPendientes;
                var totalHorasConNueva = totalHorasActual + actividad.HorasDedicadas;

                if (totalHorasConNueva > 80)
                {
                    ModelState.AddModelError("HorasDedicadas",
                        $"No puedes registrar más de 80 horas en total. " +
                        $"Actualmente tienes {horasTotalesAprobadas} horas aprobadas " +
                        $"y {horasPendientes} horas pendientes. " +
                        $"Te quedan {80 - totalHorasActual} horas disponibles.");
                    return View(actividad);
                }

                actividad.UserId = userId;
                actividad.Estado = "Pendiente";

                _context.Add(actividad);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Actividad registrada exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            return View(actividad);
        }

        // GET: Actividades/Edit/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (actividad.UserId != userId)
            {
                return Forbid();
            }

            return View(actividad);
        }

        // POST: Actividades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Fecha,HorasDedicadas")] Actividad actividad)
        {
            if (id != actividad.Id)
            {
                return NotFound();
            }

            var actividadExistente = await _context.Actividades.FindAsync(id);
            if (actividadExistente == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (actividadExistente.UserId != userId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validación de 80 horas en edición
                    var horasTotalesAprobadas = await _context.Actividades
                        .Where(a => a.UserId == userId && a.Estado == "Aprobado" && a.Id != id)
                        .SumAsync(a => a.HorasDedicadas);

                    var horasPendientes = await _context.Actividades
                        .Where(a => a.UserId == userId && a.Estado == "Pendiente" && a.Id != id)
                        .SumAsync(a => a.HorasDedicadas);

                    var totalHorasActual = horasTotalesAprobadas + horasPendientes;
                    var totalHorasConEditada = totalHorasActual + actividad.HorasDedicadas;

                    if (totalHorasConEditada > 80)
                    {
                        ModelState.AddModelError("HorasDedicadas",
                            $"No puedes tener más de 80 horas en total. " +
                            $"Con esta edición tendrías {totalHorasConEditada} horas. " +
                            $"Máximo permitido: 80 horas.");
                        return View(actividad);
                    }

                    actividadExistente.Descripcion = actividad.Descripcion;
                    actividadExistente.Fecha = actividad.Fecha;
                    actividadExistente.HorasDedicadas = actividad.HorasDedicadas;

                    _context.Update(actividadExistente);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Actividad actualizada exitosamente!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActividadExists(actividad.Id))
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
            return View(actividad);
        }

        // GET: Actividades/Delete/5
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actividad = await _context.Actividades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actividad == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (actividad.UserId != userId)
            {
                return Forbid();
            }

            return View(actividad);
        }

        // POST: Actividades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actividad = await _context.Actividades.FindAsync(id);
            if (actividad != null)
            {
                var userId = _userManager.GetUserId(User);
                if (actividad.UserId != userId)
                {
                    return Forbid();
                }

                _context.Actividades.Remove(actividad);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Actividad eliminada exitosamente!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ActividadExists(int id)
        {
            return _context.Actividades.Any(e => e.Id == id);
        }
    }
}