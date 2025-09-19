namespace RecetApp.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public List<CategoriaReceta> CategoriaRecetas { get; set; } = new();
    }
}
