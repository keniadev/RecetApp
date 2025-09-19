namespace RecetApp.Models
{
    public class Ingrediente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }

        public List<RecetaIngrediente> RecetaIngredientes { get; set; } = new();
    }
}
