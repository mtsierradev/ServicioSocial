using System.ComponentModel.DataAnnotations;

namespace ServicioSocial.Models
{
    public class Estudiante
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Nombre { get; set; }

        [Required, EmailAddress]
        public string Correo { get; set; }

        [StringLength(15)]
        public string Telefono { get; set; }

        [Required]
        public string Programa { get; set; }

        // Relación con el usuario Identity
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        // Relación 1:N con Actividades
        public ICollection<Actividad>? Actividades { get; set; }
    }

}
