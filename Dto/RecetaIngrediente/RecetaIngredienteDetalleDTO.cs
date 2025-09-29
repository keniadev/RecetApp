namespace RecetApp.Dto.RecetaIngrediente
{
    public record RecetaIngredienteDetalleDTO(
       int Id,
       string Nombre,
       decimal Cantidad,
       string UnidadMedida
   );
}
