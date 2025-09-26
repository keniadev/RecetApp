namespace RecetApp.Dto.Receta

{
    public record RecetaDto
    (
      int Id,
        int UsuarioId,
        string Titulo,
        string Descripcion,
        string? UsuarioNombre,
        List<RecetApp.Dto.Imagen.ImagenDto> Imagenes
        );
    
}
