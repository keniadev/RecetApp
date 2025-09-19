namespace RecetApp.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Clave { get; set; }
        public int RolId { get; set; } 
        public Rol? Rol { get; set; }

        public List<Receta> Recetas { get; set; } = new();
        public List<RecetaFavorita> RecetaFavoritas { get; set; } = new();

    }
}
