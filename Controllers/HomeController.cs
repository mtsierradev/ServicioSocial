using Microsoft.AspNetCore.Mvc;
using ServicioSocial.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace ServicioSocial.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Redirección forzada - nadie ve la página de inicio
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Estadisticas", "Docentes");
            }
            else if (User.IsInRole("Docente"))
            {
                return RedirectToAction("ActividadesPendientes", "Docentes");
            }
            else if (User.IsInRole("User"))
            {
                return RedirectToAction("Index", "Actividades");
            }

            // Por si acaso, redirigir al login si no tiene rol
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        public IActionResult AccessDenied()
        {
            // ✅ REDIRECCIÓN DESDE ACCESS DENIED
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Estadisticas", "Docentes");
            }
            else if (User.IsInRole("Docente"))
            {
                return RedirectToAction("ActividadesPendientes", "Docentes");
            }
            else if (User.IsInRole("User"))
            {
                return RedirectToAction("Index", "Actividades");
            }

            // Si llega aquí, mostrar la página de acceso denegado
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}