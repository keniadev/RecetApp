using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Dto.Imagen;
using RecetApp.Models;

namespace RecetApp.Endpoints
{
    public static class ImagenEndpoint
    {
        public static void AddImagenEndpoint(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/imagenes").WithTags("imagenes");

            // Crear imagen
            group.MapPost("/", async (RecetAppDb db, CrearImagenDto dto) =>
            {
                var errores = new Dictionary<string, string[]>();

                if (string.IsNullOrWhiteSpace(dto.Url))
                    errores["Url"] = new[] { "La URL es requerida" };

                // Validar que la receta exista
                if (!await db.Recetas.AnyAsync(r => r.Id == dto.RecetaId))
                    errores["RecetaId"] = new[] { "La receta no existe" };

                if (errores.Count > 0)
                    return Results.ValidationProblem(errores);

                var imagen = new Imagen
                {
                    RecetaId = dto.RecetaId,
                    Url = dto.Url
                };

                db.Imagenes.Add(imagen);
                await db.SaveChangesAsync();

                var dtoSalida = new ImagenDto(imagen.Id, imagen.RecetaId, imagen.Url);

                return Results.Created($"/api/imagenes/{imagen.Id}", dtoSalida);
            });

            // Listar imágenes
            group.MapGet("/", async (RecetAppDb db) =>
            {
                var imagenes = await db.Imagenes
                    .Select(i => new ImagenDto(i.Id, i.RecetaId, i.Url))
                    .ToListAsync();

                return Results.Ok(imagenes);
            });

            // Obtener imagen por Id
            group.MapGet("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var imagen = await db.Imagenes
                    .Where(i => i.Id == id)
                    .Select(i => new ImagenDto(i.Id, i.RecetaId, i.Url))
                    .FirstOrDefaultAsync();

                return imagen != null ? Results.Ok(imagen) : Results.NotFound();
            });
        }
    }
}
