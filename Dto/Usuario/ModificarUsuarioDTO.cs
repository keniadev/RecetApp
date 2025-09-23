namespace RecetApp.Dto.Usuario
{
    public record ModificarUsuarioDTO
    (
        string Nombre,
        string? Email,
        string? Clave,
        string? FotoPerfilUrl
    );
}
