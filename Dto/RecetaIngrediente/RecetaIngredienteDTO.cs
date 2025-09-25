namespace RecetApp.Dto.RecetaIngrediente
{
    public record RecetaIngredienteDTO
    (
    int Id,
    int RecetaId,
    int IngredienteId,
    decimal Cantidad
    );
}
