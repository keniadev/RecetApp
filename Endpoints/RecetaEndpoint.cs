using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Dto.Categoria;
using RecetApp.Dto.Imagen;
using RecetApp.Dto.Receta;
using RecetApp.Dto.RecetaIngrediente;
using RecetApp.Models;
using System.Linq;

namespace RecetApp.Endpoints
{
    public static class RecetaEndpoint
    {
        public static void AddRecetaEndpoint(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/recetas").WithTags("recetas");

            group.MapPost("/", async (RecetAppDb db, CrearRecetaDto dto) => { 
                var errores = new Dictionary<string, string[]>();

                if (string.IsNullOrWhiteSpace(dto.Titulo))
                    errores["Titulo"] = ["El título es requerido"]; 

                if (string.IsNullOrWhiteSpace(dto.Descripcion))
                    errores["Descripcion"] = ["La descripción es requerida"];

                if (errores.Count > 0) return Results.ValidationProblem(errores);

                var receta = new Receta
                {
                    UsuarioId = dto.UsuarioId,
                    Titulo = dto.Titulo,
                    Descripcion = dto.Descripcion
                };


                db.Recetas.Add(receta);
                await db.SaveChangesAsync();

                var dtoSalida = new RecetaDto(
                    receta.Id,
                    receta.UsuarioId,
                    receta.Titulo,
                    receta.Descripcion,
                    receta.Usuario?.Nombre,
                    new List<ImagenDto>()
                );

                return Results.Created($"/api/recetas/{receta.Id}", dtoSalida);
            });

            // Listar recetas
            group.MapGet("/", async (RecetAppDb db) =>
            {
                var recetas = await db.Recetas
                    .Include(r => r.Usuario)
                    .Include(r => r.Imagenes)

                    .Select(r => new RecetaCardDto(

                        r.Id,
                        r.Titulo,
                        r.Imagenes.FirstOrDefault() != null ? r.Imagenes.First().Url : null,
                        r.Usuario != null ? r.Usuario.Nombre : null
                    )) 
                    .ToListAsync();

                return Results.Ok(recetas);
            });

            group.MapGet("/{id:int}", async (RecetAppDb db, int id) =>
            {
                // Detalle completo de receta
                var receta = await db.Recetas
                    .Include(r => r.Usuario)
                    .Include(r => r.Imagenes)
                    .Include(r => r.CategoriaRecetas)
                        .ThenInclude(cr => cr.Categoria)
                    .Include(r => r.RecetaIngredientes)
                        .ThenInclude(ri => ri.Ingrediente)
                    .Where(r => r.Id == id)
                    .Select(r => new RecetaDetalleDto(
                        r.Id,
                        r.Titulo,
                        r.Descripcion,
                    r.Usuario != null ? r.Usuario.Nombre : null,
                        r.CategoriaRecetas.Select(cr => new CategoriaDTO(cr.Categoria.Id, cr.Categoria.Nombre)).ToList(),
                        r.RecetaIngredientes.Select(ri => new RecetaIngredienteDetalleDTO(
                            ri.Ingrediente.Id,
                            ri.Ingrediente.Nombre,
                            ri.Cantidad,
                            ri.Ingrediente.UnidadMedida
                        )).ToList(),
                        r.Imagenes.Select(i => new ImagenDto(i.Id, i.RecetaId, i.Url)).ToList()
                    ))
                    .FirstOrDefaultAsync();

                return receta != null ? Results.Ok(receta) : Results.NotFound();
            });

            // Modificar receta
            group.MapPut("/{id:int}", async (RecetAppDb db, int id, ModificarRecetaDto dto) =>
            {
                var receta = await db.Recetas.FindAsync(id);
                if (receta == null) return Results.NotFound();

                if (!string.IsNullOrWhiteSpace(dto.Titulo))
                    receta.Titulo = dto.Titulo;

                if (!string.IsNullOrWhiteSpace(dto.Descripcion))
                    receta.Descripcion = dto.Descripcion;

                await db.SaveChangesAsync();

                var dtoSalida = new RecetaDto(
                    receta.Id,
                    receta.UsuarioId,
                    receta.Titulo,
                    receta.Descripcion,
                    receta.Usuario?.Nombre,
                    receta.Imagenes.Select(i => new ImagenDto(i.Id, i.RecetaId, i.Url)).ToList()
                );

                return Results.Ok(dtoSalida);
            });

            // Eliminar receta
            group.MapDelete("/{id:int}", async (RecetAppDb db, int id) =>
            {
                var receta = await db.Recetas.FindAsync(id);
                if (receta == null) return Results.NotFound();

                db.Recetas.Remove(receta);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

        }
    }
}
