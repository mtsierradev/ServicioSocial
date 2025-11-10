// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ServicioSocial.Models;

namespace ServicioSocial.Areas.Identity.Pages.Account
{
    // ✅ SOLO ADMINS PUEDEN ACCEDER O USUARIOS NO AUTENTICADOS (REGISTRO PÚBLICO)
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // Lista de roles disponibles para el dropdown (solo para admins)
        public List<SelectListItem> Roles { get; set; }

        // Propiedad para saber si el usuario actual es admin
        public bool IsAdmin { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Display(Name = "Role")]
            public string Role { get; set; } = "User"; // Valor por defecto

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            await InitializePage(returnUrl);
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            await InitializePage(returnUrl);

            // ✅ VERIFICACIÓN DE SEGURIDAD: Si es usuario autenticado pero no Admin, redirigir
            if (User.Identity.IsAuthenticated && !IsAdmin)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            if (ModelState.IsValid)
            {
                // Validar permisos para asignar roles
                var currentUser = await _userManager.GetUserAsync(User);
                var isCurrentUserAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

                // ✅ RESTRICCIONES MEJORADAS:
                // - Solo Admin puede crear Admins y Docentes
                // - Usuarios no autenticados solo pueden crear Users
                // - Usuarios autenticados no-admin no deberían llegar aquí (por la verificación arriba)
                if (!isCurrentUserAdmin)
                {
                    Input.Role = "User"; // Fuerza rol User para no-admins
                }

                // ✅ VALIDACIÓN CRÍTICA: Si el rol es null o vacío, asignar "User"
                if (string.IsNullOrEmpty(Input.Role))
                {
                    Input.Role = "User";
                }

                // Validar que el rol existe
                var roleExists = await _roleManager.RoleExistsAsync(Input.Role);
                if (!roleExists)
                {
                    Input.Role = "User";
                }

                var user = CreateUser();

                // Asignar todas las propiedades requeridas
                user.FullName = Input.FullName;
                user.Role = Input.Role;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Asignar el rol al usuario
                    var roleResult = await _userManager.AddToRoleAsync(user, Input.Role);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogWarning("Failed to assign role to user: {Errors}",
                            string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // Solo hacer sign-in si el usuario que registra no está loggeado (es decir, es un usuario anónimo registrándose)
                        if (!User.Identity.IsAuthenticated)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                        }
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }

        private async Task InitializePage(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Determinar si el usuario actual es admin
            var currentUser = await _userManager.GetUserAsync(User);
            IsAdmin = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Configurar la lista de roles según los permisos
            if (IsAdmin)
            {
                Roles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "User", Text = "Usuario" },
                    new SelectListItem { Value = "Docente", Text = "Docente" },
                    new SelectListItem { Value = "Admin", Text = "Administrador" }
                };
            }
            else if (User.Identity.IsAuthenticated)
            {
                // ✅ Usuarios logueados no-admin no deberían ver esta página
                // Pero por si acaso, configuramos solo rol User
                Roles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "User", Text = "Usuario", Selected = true }
                };
            }
            else
            {
                // Usuarios anónimos solo pueden crear Users (registro público)
                Roles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "User", Text = "Usuario", Selected = true }
                };
            }
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}