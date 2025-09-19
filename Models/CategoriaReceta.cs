namespace RecetApp.Models
{
    public class CategoriaReceta
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public Receta? Receta { get; set; }
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
    }
}
