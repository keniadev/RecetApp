namespace RecetApp.Dto.Usuario
{
    public record UsuarioDTO
   (
        int Id,
        string Nombre,
        string Email,
        string? FotoPerfilUrl
   );
}
