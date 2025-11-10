using Microsoft.AspNetCore.Identity.UI.Services;

namespace ServicioSocial.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Aquí no hacemos nada. Es un "mock" para desarrollo.
            return Task.CompletedTask;
        }
    }
}
