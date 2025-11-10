using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicioSocial.Models
{
    public class Actividad
    {
        public int Id { get; set; }

        // Eliminar EstudianteId y usar el usuario autenticado
        public string? UserId { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        [Display(Name = "Descripción de la actividad")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de la actividad")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Las horas dedicadas son requeridas")]
        [Range(0.5, 24, ErrorMessage = "Las horas deben estar entre 0.5 y 24")]
        [Display(Name = "Horas dedicadas")]
        [Column(TypeName = "decimal(4,2)")]
        public decimal HorasDedicadas { get; set; }

        // Propiedad calculada para compatibilidad (opcional)
        [NotMapped]
        public int MinutosDedicados => (int)(HorasDedicadas * 60);

        // Estos campos serán llenados después por el docente
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente";

        [Display(Name = "Comentario del docente")]
        [StringLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
        public string? ComentarioDocente { get; set; }

        [Display(Name = "Docente aprobador")]
        public string? DocenteAprobadorId { get; set; }

        // Propiedad de navegación
        [ForeignKey("UserId")]
        public virtual ApplicationUser? Usuario { get; set; }
    }
}