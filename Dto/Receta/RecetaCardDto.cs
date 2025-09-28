namespace RecetApp.Dto.Receta
{
    public record RecetaCardDto(
        int Id,
        string Titulo,
        string? ImagenPrincipal,
        string? UsuarioNombre
    );
}