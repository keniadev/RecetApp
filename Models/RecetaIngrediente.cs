namespace RecetApp.Models
{
    public class RecetaIngrediente
    {
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public Receta? Receta { get; set; }
        public int IngredienteId { get; set; }
        public Ingrediente? Ingrediente { get; set; }
        public Decimal Cantidad { get; set; }
    }
}
