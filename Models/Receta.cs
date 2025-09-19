namespace RecetApp.Models
{
    public class Receta
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; } = new();
        public string Titulo { get; set; }
        public string Descripcion { get; set; }//= ha pasos

        public List<RecetaFavorita> RecetaFavoritas { get; set; } = new();
        public List<CategoriaReceta> CategoriaRecetas { get; set; } = new();
        public List<RecetaIngrediente> RecetaIngredientes { get; set; } = new();
    }
}
