namespace RecetApp.Models
{
    public class Imagen
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public Receta? Receta { get; set; }
        public string Url { get; set; } = string.Empty;
     
    }
}
