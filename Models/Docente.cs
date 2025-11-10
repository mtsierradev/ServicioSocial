namespace ServicioSocial.Models
{
    public class Docente
    {
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string Departamento { get; set; }
    }

}
