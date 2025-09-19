namespace RecetApp.Models
{
    public class RecetaFavorita
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int RecetaId { get; set; }
        public Receta? Receta { get; set; }
    }
}
