using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecetApp.Data;
using RecetApp.Dto.RecetaFavorita;
using RecetApp.Models;

namespace RecetApp.Endpoints
{
    public static class RecetaFavoritaEndpoint
    {
        public static void AddRecetaFavoritaEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/recetas-favoritas").WithTags("recetas-favoritas");

            // Agregar receta a favoritos 
            group.MapPost("/", async (RecetAppDb db, RecetaFavoritaDTO dto) =>
            {
                var errores = new Dictionary<string, string[]>();

                // Validaciones 
                var usuarioExiste = await db.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
                if (!usuarioExiste)
                    errores["UsuarioId"] = new[] { "El usuario no existe" };

                var recetaExiste = await db.Recetas.AnyAsync(r => r.Id == dto.RecetaId);
                if (!recetaExiste)
                    errores["RecetaId"] = new[] { "La receta no existe" };

                // Verificar si ya es favorita
                var yaExiste = await db.RecetaFavoritas
                    .AnyAsync(rf => rf.UsuarioId == dto.UsuarioId && rf.RecetaId == dto.RecetaId);

                if (yaExiste)
                    errores["Relacion"] = new[] { "La receta ya está en favoritos" };

                if (errores.Count > 0)
                    return Results.ValidationProblem(errores);

                var recetaFavorita = new RecetaFavorita
                {
                    UsuarioId = dto.UsuarioId,
                    RecetaId = dto.RecetaId
                };

                db.RecetaFavoritas.Add(recetaFavorita);
                await db.SaveChangesAsync();

                var dtoSalida = new RecetaFavoritaDTO(
                    recetaFavorita.Id,
                    recetaFavorita.UsuarioId,
                    recetaFavorita.RecetaId
                );

                return Results.Created($"/api/recetas-favoritas/{recetaFavorita.Id}", dtoSalida);
            });

            // Verificar si una receta es favorita
            group.MapGet("/usuario/{usuarioId:int}/receta/{recetaId:int}", async (RecetAppDb db, int usuarioId, int recetaId) =>
            {
                var esFavorita = await db.RecetaFavoritas
                    .AnyAsync(rf => rf.UsuarioId == usuarioId && rf.RecetaId == recetaId);

                return Results.Ok(new { EsFavorita = esFavorita });
            });

            // Quitar de favoritos 
            group.MapDelete("/usuario/{usuarioId:int}/receta/{recetaId:int}", async (RecetAppDb db, int usuarioId, int recetaId) =>
            {
                var recetaFavorita = await db.RecetaFavoritas
                    .FirstOrDefaultAsync(rf => rf.UsuarioId == usuarioId && rf.RecetaId == recetaId);

                if (recetaFavorita == null)
                    return Results.NotFound();

                db.RecetaFavoritas.Remove(recetaFavorita);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });


            // Modificar receta favorita
            group.MapPut("/{id:int}", async (RecetAppDb db, int id, RecetaFavoritaDTO dto) =>
            {
                var recetaFavorita = await db.RecetaFavoritas.FindAsync(id);
                if (recetaFavorita == null)
                    return Results.NotFound();

                var errores = new Dictionary<string, string[]>();

                // Validar que el nuevo usuario existe
                var usuarioExiste = await db.Usuarios.AnyAsync(u => u.Id == dto.UsuarioId);
                if (!usuarioExiste)
                    errores["UsuarioId"] = new[] { "El usuario no existe" };

                // Validar que la nueva receta existe
                var recetaExiste = await db.Recetas.AnyAsync(r => r.Id == dto.RecetaId);
                if (!recetaExiste)
                    errores["RecetaId"] = new[] { "La receta no existe" };

                // Validar que no existe
                var yaExiste = await db.RecetaFavoritas
                    .AnyAsync(rf => rf.Id != id && rf.UsuarioId == dto.UsuarioId && rf.RecetaId == dto.RecetaId);

                if (yaExiste)
                    errores["Relacion"] = new[] { "Ya existe esta relación de favoritos" };

                if (errores.Count > 0)
                    return Results.ValidationProblem(errores);

                // Actualizar los valores
                recetaFavorita.UsuarioId = dto.UsuarioId;
                recetaFavorita.RecetaId = dto.RecetaId;

                await db.SaveChangesAsync();

                var dtoSalida = new RecetaFavoritaDTO(
                    recetaFavorita.Id,
                    recetaFavorita.UsuarioId,
                    recetaFavorita.RecetaId
                );

                return Results.Ok(dtoSalida);
            });
        }
    }
}
        
    
