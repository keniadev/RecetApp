using RecetApp.Dto.Categoria;
using RecetApp.Dto.Imagen;
using RecetApp.Dto.RecetaIngrediente;

namespace RecetApp.Dto.Receta
{
    public record RecetaDetalleDto(
    int Id,
     string Titulo,
    string Descripcion,
     string? UsuarioNombre,
     List<CategoriaDTO> Categorias,
     List<RecetaIngredienteDetalleDTO> Ingredientes,
     List<ImagenDto> Imagenes
 );
}
